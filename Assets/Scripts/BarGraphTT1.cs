using System;
using System.IO;
using System.Globalization;
using UnityEngine;
#if INPUT_SYSTEM_ENABLED
using Input = XCharts.Runtime.InputHelper;
#endif
using XCharts.Runtime;

namespace XCharts.User2ExampleBG1
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class BarGraphTT1 : MonoBehaviour
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

            var xAxis = chart.EnsureChartComponent<XAxis>();
            xAxis.show = true;
            xAxis.type = Axis.AxisType.Category;
            xAxis.boundaryGap = true;

            var yAxis = chart.EnsureChartComponent<YAxis>();
            yAxis.show = true;
            yAxis.type = Axis.AxisType.Value;
            yAxis.interval = 1; // Adjust as needed

            chart.RemoveData();

            // Add bar series for User2
            var user2Serie = chart.AddSerie<Bar>("User2");
            user2Serie.itemStyle.color = Color.blue; // Set bar color

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
                            chart.AddData(0, totalTime); // Series index 0 for User2
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
