using System;
using System.Collections;
using System.IO;
using System.Globalization;
using UnityEngine;
#if INPUT_SYSTEM_ENABLED
using Input = XCharts.Runtime.InputHelper;
#endif
using XCharts.Runtime;

namespace XCharts.Example
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]


    public class SessionDurationChartUser1 : MonoBehaviour
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
            var chart = gameObject.GetComponent<LineChart>();
            if (chart == null)
            {
                chart = gameObject.AddComponent<LineChart>();
                chart.Init();
                chart.SetSize(580, 300);
            }

            var xAxis = chart.EnsureChartComponent<XAxis>();
            xAxis.show = true;
            xAxis.type = Axis.AxisType.Category;
            xAxis.boundaryGap = true;
            // xAxis.axisName.show = true;
            // xAxis.axisName.name = "Sessions";

            var yAxis = chart.EnsureChartComponent<YAxis>();
            yAxis.show = true;
            yAxis.type = Axis.AxisType.Value;
            yAxis.interval = 1; // Adjust as needed
            //yAxis.axisName.show = true;
            //yAxis.axisName.name = "Total Time";
            //yAxis.axisName.color = Color.red;




            chart.RemoveData();
            chart.AddSerie<Line>();

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
                            chart.AddData(0, totalTime);
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