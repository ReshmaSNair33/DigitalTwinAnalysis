using System;
using System.IO;
using System.Globalization;
using UnityEngine;
using System.Collections.Generic;
#if INPUT_SYSTEM_ENABLED
using Input = XCharts.Runtime.InputHelper;
#endif
using XCharts.Runtime;

namespace XCharts.ExampleChart
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class SessionDurationChart : MonoBehaviour
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
            xAxis.data.Clear();  // Clear existing data

            // Configure the Y-axis for average speed
            var yAxis = chart.EnsureChartComponent<YAxis>();
            yAxis.show = true;
            yAxis.type = Axis.AxisType.Value;
            yAxis.interval = 0.1f; // Adjust as needed

            // Remove existing data
            chart.RemoveData();

            // Add bar series for User1 and User2
            var user1Serie = chart.AddSerie<Bar>("User1");
            user1Serie.itemStyle.color = Color.red; // Set bar color

            var user2Serie = chart.AddSerie<Bar>("User2");
            user2Serie.itemStyle.color = Color.blue; // Set bar color

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    reader.ReadLine(); // Skip the header line

                    while (!reader.EndOfStream)
                    {
                        var lineUser1 = reader.ReadLine();
                        var valuesUser1 = lineUser1.Split(',');

                        if (!reader.EndOfStream)
                        {
                            var lineUser2 = reader.ReadLine();
                            var valuesUser2 = lineUser2.Split(',');

                            if (valuesUser1[0].Trim() == "User1" && valuesUser2[0].Trim() == "User2")
                            {
                                string sessionLabel = $"Session {xAxis.data.Count + 1}";

                                double avgSpeedUser1 = double.Parse(valuesUser1[1].Trim(), CultureInfo.InvariantCulture);
                                double avgSpeedUser2 = double.Parse(valuesUser2[1].Trim(), CultureInfo.InvariantCulture);

                                xAxis.data.Add(sessionLabel);

                                // Add data to the chart
                                chart.AddData(0, avgSpeedUser1); // Series index 0 for User1
                                chart.AddData(1, avgSpeedUser2); // Series index 1 for User2
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading or processing file: {ex.Message}");
            }

            chart.RefreshChart();
        }





    }
}