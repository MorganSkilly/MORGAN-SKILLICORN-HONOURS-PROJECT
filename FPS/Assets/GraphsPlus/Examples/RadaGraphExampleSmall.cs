using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RadaGraphExampleSmall : EditorWindow
{
    GraphsPlus.RadarGraph radarGraph;
    Rect graphContainer;

    Vector2 scrollPos;

    [MenuItem("Graphs Plus/Examples/Radar Graph Small")]
    static void ShowWindow()
    {
        RadaGraphExampleSmall window = CreateInstance<RadaGraphExampleSmall>();

        window.titleContent.text = "Radar Graph Example";
        window.Show();
    }

    private void OnEnable()
    {
        Generate();
    }

    public void OnGUI()
    {
        radarGraph.Render(graphContainer);

        if (GUILayout.Button("Generate Random Graph"))
        {
            Generate();
        }

        GUILayout.BeginArea(new Rect(0, 300, position.width, position.height - 300));
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");
        GUILayout.Label("test");

        EditorGUILayout.EndScrollView();

        GUILayout.EndArea();
    }

    void OnInspectorUpdate()
    {
        if (Mathf.Min(position.width, position.height) < 300)
            graphContainer = new Rect(position.width / 2, 300 / 2, position.width, position.height);
        else
            graphContainer = new Rect(position.width / 2, 300 / 2, 300, 300);

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

        if (Mathf.Min(position.width, position.height) < 300)
            graphContainer = new Rect(position.width / 2, 300 / 2, position.width, position.height);
        else
            graphContainer = new Rect(position.width / 2, 300 / 2, 300, 300);


        GraphsPlus.RadarGraphAxis radarGraphAxis = new GraphsPlus.RadarGraphAxis(
            new Vector2(graphContainer.x, graphContainer.y),
            Mathf.Min(graphContainer.width, graphContainer.height) / 3,
            10, dataSets[0].dataPoints.Count);

        radarGraph = new GraphsPlus.RadarGraph(radarGraphAxis, dataSets);

        
    }
}
