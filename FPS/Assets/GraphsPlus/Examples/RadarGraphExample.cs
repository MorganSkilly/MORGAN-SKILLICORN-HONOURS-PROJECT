using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RadarGraphExample : EditorWindow
{
    GraphsPlus.RadarGraph radarGraph;

    Rect graphContainer;

    [MenuItem("Graphs Plus/Examples/Radar Graph")]
    static void ShowWindow()
    {
        RadarGraphExample window = CreateInstance<RadarGraphExample>();
        window.titleContent.text = "Radar Graph Example";
        window.Show();
    }

    private void OnEnable()
    {
        graphContainer = new Rect(position.width / 2, position.height / 2, position.width, position.height);
        Generate();
    }

    public void OnGUI()
    {
        radarGraph.Render(graphContainer);

        GUILayout.BeginScrollView(new Vector2(0, 0));
        if (GUILayout.Button("Generate Random Graph"))
        {
            graphContainer = new Rect(position.width / 2, position.height / 2, position.width, position.height);
            Generate();
        }
        GUILayout.EndScrollView();
    }

    void OnInspectorUpdate()
    {
        graphContainer = new Rect(position.width / 2, position.height / 2, position.width, position.height);
        radarGraph.Resize(graphContainer);
    }

    public void Generate()
    {
        List<GraphsPlus.DataSet> dataSets = new List<GraphsPlus.DataSet>();

        int randomI = Random.Range(2, 5);
        int randomJ = Random.Range(3, 10);

        for (int i = 0; i < randomI; i++)
        {
            List<GraphsPlus.DataPoint> dataSet1 = new List<GraphsPlus.DataPoint>();
            for (int j = 0; j < randomJ; j++)
            {
                dataSet1.Add(new GraphsPlus.DataPoint("data point " + j, Random.Range(0, 100), 0f, 100f));
            }

            GraphsPlus.DataSet dataset = new GraphsPlus.DataSet(dataSet1, "data set " + i);
            dataSets.Add(dataset);
        }

        GraphsPlus.RadarGraphAxis radarGraphAxis = new GraphsPlus.RadarGraphAxis(
            new Vector2(graphContainer.x, graphContainer.y),
            Mathf.Min(graphContainer.width, graphContainer.height) / 3,
            10, dataSets[0].dataPoints.Count);

        radarGraph = new GraphsPlus.RadarGraph(radarGraphAxis, dataSets);

        CheckConvexHulls(dataSets);
    }

    void CheckConvexHulls(List<GraphsPlus.DataSet> dataSets)
    {
        foreach (GraphsPlus.DataSet dataSetI in dataSets)
        {
            List<GraphsPlus.DataSet> dataSetWithThisRemoved = dataSets;

            foreach (GraphsPlus.DataSet dataSetJ in dataSetWithThisRemoved)
            {
                if (dataSetI.encompasses(dataSetJ) && dataSetI.name != dataSetJ.name)
                {
                    Debug.LogWarning("The convex hull of " + dataSetI.name + " fully encompasses that of " + dataSetJ.name);
                }
            }
        }
    }
}
