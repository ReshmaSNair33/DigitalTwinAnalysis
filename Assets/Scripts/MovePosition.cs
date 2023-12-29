using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System.Globalization;
using System.Threading;
using UnityEngine.SceneManagement;


public class MovePosition : MonoBehaviour
{
    float distanceTravelled;

    public Vector3 targetPosition;
    public Vector3 position;
    public float speed;
    public float change_speed;
    public float x;
    public float y;
    public float z;
    private string inputText = "";
    private Terrain terrain;
    private Vector3 lastPosition;
    public float rotationSpeed = 5f;
    Rigidbody m_Rigidbody;
    Vector3 m_YAxis;
    private string currentUserAction = "";

    public InputField speedInputField;
    public Button user1Button, user2Button, cancelButton, FeedbackButton;

    public float desiredyDegreeAngle = 23f;


    private string userSpeedLogFilePath = "E:/Thesis - Robomaster ep/code from git/RoboMaster-SDK-master/examples/mywork/user_speed_log.csv";

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        lastPosition = transform.position;

        user1Button.onClick.AddListener(() => UserButtonClicked("User1"));
        user2Button.onClick.AddListener(() => UserButtonClicked("User2"));
        cancelButton.onClick.AddListener(CancelProgram);
        FeedbackButton.onClick.AddListener(OnFeedbackButtonClicked);
        speedInputField.gameObject.SetActive(false);


        // Setup input field listener
        speedInputField.onEndEdit.AddListener(OnEndEditListener);
        if (!File.Exists(userSpeedLogFilePath))
        {
            using (StreamWriter sw = new StreamWriter(userSpeedLogFilePath, false))
            {
                sw.WriteLine("User,Speed,Timestamp");
            }
        }
    }


    void Update()
    {

        using (FileStream fileStream = new FileStream("E:/Thesis - Robomaster ep/code from git/RoboMaster-SDK-master/examples/mywork/current_robot_position.csv", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            using (StreamReader strReader = new StreamReader(fileStream))
            {
                m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;

                Debug.Log("entered");


                bool endofFile = false;
                while (!endofFile)
                {

                    string data_string = strReader.ReadLine();
                    if (data_string == null)
                    {
                        endofFile = true;
                        break;
                    }

                    var data_values = data_string.Split(',');
                    float.TryParse(data_values[0], out z);
                    float.TryParse(data_values[2], out y);
                    float.TryParse(data_values[1], out x);
                }
            }
            targetPosition = new Vector3(x, y, z);
        }


        using (StreamReader strReader = new StreamReader("E:/Thesis - Robomaster ep/code from git/RoboMaster-SDK-master/examples/mywork/speed_input.csv"))
        {
            bool endofFile = false;
            while (!endofFile)
            {
                string data_string = strReader.ReadLine();
                if (data_string == null)
                {
                    endofFile = true;
                    break;
                }

                var data_values = data_string.Split(';');
                float.TryParse(data_values[0], out speed);
            }


            Vector3 movementDirection = (transform.position - lastPosition).normalized;
            if (transform.position == Vector3.zero)
            {
                // Calculate the rotation to face forward (positive Z-direction)
                float angleToAdjust = Vector3.SignedAngle(transform.forward, movementDirection, Vector3.up);


                float adjustedyAngle = angleToAdjust + desiredyDegreeAngle;
                Vector3 targetDirection = Vector3.forward;
                Quaternion targetRotation = Quaternion.Euler(0, 0, 0);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }


            if (movementDirection != Vector3.zero)
            {

                // Calculate the angle between the forward direction and the movement direction.
                float angleToAdjust = Vector3.SignedAngle(transform.forward, movementDirection, Vector3.up);

                // Calculate the adjusted angle based on the desired 20-degree angle.
                float adjustedyAngle = angleToAdjust + desiredyDegreeAngle;



                // Calculate the new target rotation.
                Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + adjustedyAngle, transform.eulerAngles.z);

                // Calculate the rotation to look at the movement direction
                // Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

                // Smoothly rotate towards the target rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }



            lastPosition = transform.position;
            distanceTravelled += speed * Time.deltaTime;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * speed);

        }

    }

    private void OnFeedbackButtonClicked()
    {
        // Replace "YourNewSceneName" with the actual name of the scene you want to load
        SceneManager.LoadScene("visandana");
    }


    private void LogSpeedForUser(string user, float speed)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string csvLine = $"{user},{speed},{timestamp}";

        try
        {
            using (StreamWriter sw = new StreamWriter(userSpeedLogFilePath, true))
            {
                sw.WriteLine(csvLine);
            }
        }
        catch (IOException ex)
        {
            Debug.LogError($"IOException encountered: {ex.Message}");
            // Handle the error or retry as needed
        }
    }

    public void UserButtonClicked(string user)
    {
        currentUserAction = user; // Update this line
        ShowSpeedInputField();
    }
    private void TryWriteToFile(string filePath, Action<StreamWriter> writeFileAction)
    {
        int attempts = 0;
        while (attempts < 5) // Retry up to 5 times
        {
            try
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fileStream))
                    {
                        writeFileAction(sw);
                    }
                }
                break; // Break the loop if successful
            }
            catch (IOException ex)
            {
                Debug.LogError($"IOException encountered: {ex.Message}");
                attempts++;
                Thread.Sleep(100); // Wait for 100ms before retrying
            }
        }
    }



    private void ShowSpeedInputField()
    {
        speedInputField.gameObject.SetActive(true);
        speedInputField.Select();
    }

    private void OnEndEditListener(string inputText)
    {
        SetSpeedAndHideInputField(inputText, currentUserAction);
    }

    private void SetSpeedAndHideInputField(string inputText, string userAction)
    {
        if (float.TryParse(inputText, out float newSpeed))
        {
            speed = newSpeed;
            // Update speed in speed_input.csv
            using (StreamWriter sw = new StreamWriter("E:/Thesis - Robomaster ep/code from git/RoboMaster-SDK-master/examples/mywork/speed_input.csv", false))
            {
                sw.WriteLine("Speed");
                sw.WriteLine(newSpeed.ToString(CultureInfo.InvariantCulture));
            }
            LogSpeedForUser(userAction, newSpeed);
            // Update user_clicks.csv with "User1"
            TryWriteToFile("E:/Thesis - Robomaster ep/code from git/RoboMaster-SDK-master/examples/mywork/user_clicks.csv", writer =>
            {
                writer.WriteLine(userAction + "," + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            });
        }
        speedInputField.gameObject.SetActive(false); // Hide the input field after setting the speed
    }

    private void CancelProgram()
    {
        using (FileStream fileStream = new FileStream("E:/Thesis - Robomaster ep/code from git/RoboMaster-SDK-master/examples/mywork/user_clicks.csv", FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
        {
            using (StreamWriter sw = new StreamWriter(fileStream))
            {
                sw.WriteLine("Cancel," + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            Application.Quit();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }


}