using System;
using System.IO;
using UnityEngine;
#if INPUT_SYSTEM_ENABLED
using Input = XCharts.Runtime.InputHelper;
#endif
using XCharts.Runtime;

namespace XCharts.ExampleBar1
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class BarGraphUser1 : MonoBehaviour
    {
        public string csvFilePath = "E:/Thesis - Robomaster ep/code from git/RoboMaster-SDK-master/examples/mywork/session_summary.csv";

        void Awake()
        {
            AddSessionDurationsToChart(csvFilePath);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AddSessionDurationsToChart(csvFilePath);
            }
        }

        void AddSessionDurationsToChart(string filePath)
        {
            var chart = gameObject.GetComponent<BarChart>();
            if (chart == null)
            {
                chart = gameObject.AddComponent<BarChart>();
                chart.Init();
                chart.SetSize(580, 300);
            }

            // Configure the X-axis for sessions
            var xAxis = chart.EnsureChartComponent<XAxis>();
            xAxis.show = true;
            xAxis.type = Axis.AxisType.Category;
            xAxis.boundaryGap = true;

            // Configure the Y-axis for session duration
            var yAxis = chart.EnsureChartComponent<YAxis>();
            yAxis.show = true;
            yAxis.type = Axis.AxisType.Value;
            yAxis.interval = 1; // Adjust as needed

            chart.RemoveData();

            // Add bar series for User1
            var user1Serie = chart.AddSerie<Bar>("User1");
            user1Serie.itemStyle.color = Color.red; // Set bar color

            int sessionCount = 0;
            using (var reader = new StreamReader(filePath))
            {
                reader.ReadLine(); // Skip the header line

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    if (values[0].Trim() == "User1")
                    {
                        try
                        {
                            sessionCount++;
                            float totalTime = float.Parse(values[2].Trim());

                            chart.AddXAxisData($"Session {sessionCount}");
                            chart.AddData(0, totalTime); // Series index 0 for User1
                            Debug.Log($"Session {sessionCount}: {totalTime} hours");
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error processing line: {line}. Exception: {ex.Message}");
                        }
                    }
                }
            }

            chart.RefreshChart();
        }
    }
}
