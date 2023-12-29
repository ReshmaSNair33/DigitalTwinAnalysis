using System;
using System.IO;
using UnityEngine;
using XCharts.Runtime;

namespace XCharts.ExampleBG2
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class BarGraphTT2 : MonoBehaviour
    {
        public string csvFilePath = "E:/Thesis - Robomaster ep/code from git/RoboMaster-SDK-master/examples/mywork/session_summary.csv";

        void Awake()
        {
            AddAverageSpeedToChart(csvFilePath);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AddAverageSpeedToChart(csvFilePath);
            }
        }

        void AddAverageSpeedToChart(string filePath)
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
            // yAxis.interval = 0.10; // Adjust as needed
            yAxis.axisLabel.formatter = "{value:f2}";

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
                            float averageSpeed = float.Parse(values[1].Trim());

                            // Round the averageSpeed to two decimal places
                            averageSpeed = Mathf.Round(averageSpeed * 100f) / 100f;

                            chart.AddXAxisData($"Session {sessionCount}");
                            chart.AddData(0, averageSpeed); // Series index 0 for User2
                            Debug.Log($"Session {sessionCount}: {averageSpeed} speed");
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
