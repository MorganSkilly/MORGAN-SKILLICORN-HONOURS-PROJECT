using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorGraphs : EditorWindow
{   
    [MenuItem("Editor Graphs/Radar Graph")]
    static void ShowWindow()
    {
        EditorGraphs window = CreateInstance<EditorGraphs>();
        window.titleContent.text = "Radar Graph Instance";
        window.Show();
    }

    int datapoints, datasets;
    private Material mat;
    public static Color axisCol = new Color(0.5f, 0.5f, 0.5f);
    public static Color gridCol = new Color(0.3f, 0.3f, 0.3f);

    List<List<RadarGraph.dataPoint>> dataSets = new List<List<RadarGraph.dataPoint>>();
    Vector2 center;
    float radius = 100;
    int divisions = 10;

    public RadarGraph radarGraph;

    public class RadarGraph
    {
        public List<List<RadarGraph.dataPoint>> dataSets;

        public graphAxis axis = new graphAxis();

        public RadarGraph(graphAxis axis, List<List<RadarGraph.dataPoint>> dataSets)
        {
            this.axis = axis;
            this.dataSets = dataSets;
        }

        public void RenderFull()
        {
            RenderAxis();
            RenderData();
        }

        public void RenderAxis()
        {
            for (int i = 0; i < axis.segments; i++)
            {
                float angle = i * Mathf.PI * 2 / axis.segments;
                Vector2 pointOnCircle = axis.centre + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * axis.radius;
                DrawLine(axisCol, axis.centre, pointOnCircle);

                for (int j = 0; j <= axis.divisions; j++)
                {
                    Vector2 endOfThisAxis = axis.centre + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ((axis.radius / axis.divisions) * j);
                    float prevAngle = (i - 1) * Mathf.PI * 2 / axis.segments;
                    Vector2 endOfPreviousAxis = axis.centre + new Vector2(Mathf.Cos(prevAngle), Mathf.Sin(prevAngle)) * ((axis.radius / axis.divisions) * j);
                    DrawLine(gridCol, endOfPreviousAxis, endOfThisAxis);
                }
            }
        }


        public void RenderData()
        {
            int colourID = 1;
            Color colour = Color.red;
            foreach (List<dataPoint> dataPoints in dataSets)
            {
                if (colourID == 1)
                    colour = Color.red;
                else if (colourID == 2)
                    colour = Color.blue;
                else if (colourID == 3)
                    colour = Color.yellow;
                else if (colourID == 4)
                    colour = Color.green;
                else if (colourID == 5)
                    colour = Color.cyan;
                else if (colourID == 6)
                    colour = Color.magenta;
                else if (colourID == 7)
                    colour = Color.white;

                colourID++;

                for (int i = 0; i < axis.segments; i++)
                {
                    if (i - 1 < 0)
                    {
                        float angle = i * Mathf.PI * 2 / axis.segments;
                        float normalizedValue = (dataPoints[i].data - dataPoints[i].min) / (dataPoints[i].max - dataPoints[i].min) * 100;
                        Vector2 endOfThisAxis = axis.centre + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ((axis.radius / 100) * normalizedValue);
                        float prevAngle = (i - 1) * Mathf.PI * 2 / axis.segments;
                        float prevNormalizedValue = (dataPoints[dataPoints.Count - 1].data - dataPoints[dataPoints.Count - 1].min) / (dataPoints[dataPoints.Count - 1].max - dataPoints[dataPoints.Count - 1].min) * 100;
                        Vector2 endOfPreviousAxis = axis.centre + new Vector2(Mathf.Cos(prevAngle), Mathf.Sin(prevAngle)) * ((axis.radius / 100) * prevNormalizedValue);
                        DrawLine(colour, endOfPreviousAxis, endOfThisAxis);
                        DrawTri(colour, endOfPreviousAxis, endOfThisAxis, axis.centre);
                    }
                    else
                    {
                        float angle = i * Mathf.PI * 2 / axis.segments;
                        float normalizedValue = (dataPoints[i].data - dataPoints[i].min) / (dataPoints[i].max - dataPoints[i].min) * 100;
                        Vector2 endOfThisAxis = axis.centre + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ((axis.radius / 100) * normalizedValue);
                        float prevAngle = (i - 1) * Mathf.PI * 2 / axis.segments;
                        float prevNormalizedValue = (dataPoints[i - 1].data - dataPoints[i - 1].min) / (dataPoints[i - 1].max - dataPoints[i - 1].min) * 100;
                        Vector2 endOfPreviousAxis = axis.centre + new Vector2(Mathf.Cos(prevAngle), Mathf.Sin(prevAngle)) * ((axis.radius / 100) * prevNormalizedValue);
                        DrawLine(colour, endOfPreviousAxis, endOfThisAxis);
                        DrawTri(colour, endOfPreviousAxis, endOfThisAxis, axis.centre);
                    }

                }
            }
        }

        public struct graphAxis
        {
            public graphAxis(
                Vector2 centre,
                float radius,
                int divisions,
                int segments)
            {
                this.centre = centre;
                this.radius = radius;
                this.divisions = divisions;
                this.segments = segments;
            }

            public Vector2 centre;
            public float radius;
            public int divisions;
            public int segments;
        }

        public struct dataPoint
        {
            public dataPoint(
                string label,
                float data,
                float min,
                float max)
            {
                this.label = label;
                this.data = data;
                this.min = min;
                this.max = max;
            }

            public string label;
            public float data;
            public float min;
            public float max;
        }
    }

    private void OnEnable()
    {
        List<RadarGraph.dataPoint> dataSet1 = new List<RadarGraph.dataPoint>();
        dataSet1.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet1.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet1.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet1.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet1.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet1.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet1.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet1.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSets.Add(dataSet1);
        List<RadarGraph.dataPoint> dataSet2 = new List<RadarGraph.dataPoint>();
        dataSet2.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet2.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet2.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet2.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet2.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet2.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet2.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet2.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSets.Add(dataSet2);
        List<RadarGraph.dataPoint> dataSet3 = new List<RadarGraph.dataPoint>();
        dataSet3.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet3.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet3.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet3.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet3.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet3.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet3.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet3.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSets.Add(dataSet3);
        List<RadarGraph.dataPoint> dataSet4 = new List<RadarGraph.dataPoint>();
        dataSet4.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet4.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet4.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet4.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet4.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet4.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet4.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSet4.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
        dataSets.Add(dataSet4);
    }

    private void OnDisable()
    {
        DestroyImmediate(mat);
    }

    public void OnGUI()
    {
        int smallestDataset = 100;

        foreach (List<RadarGraph.dataPoint> dataPoints in dataSets)
        {
            if (dataPoints.Count < smallestDataset)
            {
                smallestDataset = dataPoints.Count;
            }
        }

        RadarGraph.graphAxis axis = new RadarGraph.graphAxis(
            center,
            radius,
            divisions,
            smallestDataset);

        radarGraph = new RadarGraph(axis, dataSets);

        radarGraph.RenderFull();

        datapoints = (int)EditorGUILayout.Slider("datapoints ", datapoints, 3, 50);
        datasets = (int)EditorGUILayout.Slider("datasets ", datasets, 1, 20);

        if (GUI.Button(new Rect(10, 50, 150, 100), "refresh"))
        {
            dataSets.Clear();

            for (int j = 0; j < datasets; j++)
            {
                List<RadarGraph.dataPoint> dataSet1 = new List<RadarGraph.dataPoint>();
                for (int i = 0; i < datapoints; i++)
                {
                    dataSet1.Add(new RadarGraph.dataPoint("test", Random.Range(0f, 24f), 0f, 24f));
                }
                dataSets.Add(dataSet1);

            }

            radarGraph.RenderFull();
        }
    }
    void OnInspectorUpdate()
    {

        center = new Vector2(position.width / 2, position.height / 2);
        radius = Mathf.Min(position.width, position.height) / 3;
    }

    public static void DrawLine(Color color, Vector2 start, Vector2 end)
    {
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex3(start.x, start.y, 0);
        GL.Vertex3(end.x, end.y, 0);
        GL.End();
    }

    public static void DrawTri(Color color, Vector2 start, Vector2 end, Vector2 centre)
    {
        color.a = 0.1f;

        GL.Begin(GL.TRIANGLES);
        GL.Color(color);
        GL.Vertex3(start.x, start.y, 0);
        GL.Vertex3(end.x, end.y, 0);
        GL.Vertex3(centre.x, centre.y, 0);
        GL.End();
    }
}
