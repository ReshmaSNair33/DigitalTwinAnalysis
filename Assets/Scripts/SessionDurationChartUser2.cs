using System;
using System.IO;
using System.Globalization;
using UnityEngine;
#if INPUT_SYSTEM_ENABLED
using Input = XCharts.Runtime.InputHelper;
#endif
using XCharts.Runtime;

namespace XCharts.User2Example
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class SessionDurationChartUser2 : MonoBehaviour
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

            var yAxis = chart.EnsureChartComponent<YAxis>();
            yAxis.show = true;
            yAxis.type = Axis.AxisType.Value;
            yAxis.interval = 1; // Adjust as needed

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

                    if (values[0].Trim() == "User2")
                    {
                        try
                        {
                            sessionCount++;
                            float totalTime = float.Parse(values[2].Trim(), CultureInfo.InvariantCulture);

                            chart.AddXAxisData($"Session {sessionCount}");
                            chart.AddData(0, totalTime);
                            Debug.Log($"User2 Session {sessionCount}: {totalTime} hours");
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