using System;
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
    public class UserSpeedChart : MonoBehaviour
    {
        public string csvFilePath = "<Your File Path Here>";

        void Awake()
        {
            AddSpeedsToChart(csvFilePath);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AddSpeedsToChart(csvFilePath);
            }
        }

        void AddSpeedsToChart(string filePath)
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

            chart.RemoveData();
            chart.AddSerie<Line>("User1");
            chart.AddSerie<Line>("User2");

            int sessionCount = 0;
            float user1SpeedTotal = 0;
            float user2SpeedTotal = 0;
            int user2Count = 0;

            using (var reader = new StreamReader(filePath))
            {
                string previousTimestamp = string.Empty;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    // If the timestamp changes, it's a new session
                    if (values[2] != previousTimestamp)
                    {
                        if (sessionCount > 0)
                        {
                            // Add previous session's data to chart
                            chart.AddXAxisData($"Session {sessionCount}");
                            chart.AddData(0, user1SpeedTotal); // User1's speed
                            chart.AddData(1, user2Count > 0 ? user2SpeedTotal / user2Count : 0); // Average of User2's speed
                        }
                        user1SpeedTotal = 0;
                        user2SpeedTotal = 0;
                        user2Count = 0;
                        sessionCount++;
                        previousTimestamp = values[2];
                    }

                    float speed = float.Parse(values[1]);
                    if (values[0] == "User1")
                    {
                        user1SpeedTotal += speed;
                    }
                    else if (values[0] == "User2")
                    {
                        user2SpeedTotal += speed;
                        user2Count++;
                    }
                }
            }

            // Add last session's data if any
            if (sessionCount > 0)
            {
                chart.AddXAxisData($"Session {sessionCount}");
                chart.AddData(0, user1SpeedTotal); // User1's speed
                chart.AddData(1, user2Count > 0 ? user2SpeedTotal / user2Count : 0); // Average of User2's speed
            }

            chart.RefreshChart();
        }
    }
}
