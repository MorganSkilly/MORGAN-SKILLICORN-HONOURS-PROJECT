using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GraphsPlus : MonoBehaviour
{
    static Color axisCol = new Color(0.5f, 0.5f, 0.5f);
    static Color gridCol = new Color(0.3f, 0.3f, 0.3f);

    public struct RadarGraph
    {
        public RadarGraph(RadarGraphAxis axis, List<DataSet> dataSets)
        {
            int smallestDataset = 100;

            this.axis = axis;
            this.dataSets = dataSets;

            foreach (DataSet dataSet in dataSets)
            {
                if (dataSet.dataPoints.Count < smallestDataset)
                {
                    smallestDataset = dataSet.dataPoints.Count;
                }
            }

            axis.segments = smallestDataset;
        }

        public RadarGraphAxis axis;
        public List<DataSet> dataSets;

        public void Render(Rect position)
        {
            axis.Render();

            foreach (DataSet dataSet in dataSets)
            {
                List<Color> colours = new List<Color>();
                colours.Add(Color.red);
                colours.Add(Color.magenta);
                colours.Add(Color.cyan);
                colours.Add(Color.yellow);
                colours.Add(Color.green);
                colours.Add(Color.blue);
                colours.Add(Color.white);
                for (int i = 0; i < 100; i++)
                {
                    colours.Add(new Color(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1)));
                }

                for (int i = 0; i < axis.segments; i++)
                {
                    if (i - 1 < 0)
                    {
                        float angle = i * Mathf.PI * 2 / axis.segments;
                        float normalizedValue = (dataSet.dataPoints[i].data - dataSet.dataPoints[i].min) / (dataSet.dataPoints[i].max - dataSet.dataPoints[i].min) * 100;
                        Vector2 endOfThisAxis = axis.centre + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ((axis.radius / 100) * normalizedValue);
                        float prevAngle = (i - 1) * Mathf.PI * 2 / axis.segments;
                        float prevNormalizedValue = (dataSet.dataPoints[dataSet.dataPoints.Count - 1].data - dataSet.dataPoints[dataSet.dataPoints.Count - 1].min) / (dataSet.dataPoints[dataSet.dataPoints.Count - 1].max - dataSet.dataPoints[dataSet.dataPoints.Count - 1].min) * 100;
                        Vector2 endOfPreviousAxis = axis.centre + new Vector2(Mathf.Cos(prevAngle), Mathf.Sin(prevAngle)) * ((axis.radius / 100) * prevNormalizedValue);
                        DrawLine(colours[dataSets.IndexOf(dataSet)], endOfPreviousAxis, endOfThisAxis);
                        DrawTri(colours[dataSets.IndexOf(dataSet)], endOfPreviousAxis, endOfThisAxis, axis.centre);
                    }
                    else
                    {
                        try
                        {
                            float angle = i * Mathf.PI * 2 / axis.segments;
                            float normalizedValue = (dataSet.dataPoints[i].data - dataSet.dataPoints[i].min) / (dataSet.dataPoints[i].max - dataSet.dataPoints[i].min) * 100;
                            Vector2 endOfThisAxis = axis.centre + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ((axis.radius / 100) * normalizedValue);
                            float prevAngle = (i - 1) * Mathf.PI * 2 / axis.segments;
                            float prevNormalizedValue = (dataSet.dataPoints[i - 1].data - dataSet.dataPoints[i - 1].min) / (dataSet.dataPoints[i - 1].max - dataSet.dataPoints[i - 1].min) * 100;
                            Vector2 endOfPreviousAxis = axis.centre + new Vector2(Mathf.Cos(prevAngle), Mathf.Sin(prevAngle)) * ((axis.radius / 100) * prevNormalizedValue);
                            DrawLine(colours[dataSets.IndexOf(dataSet)], endOfPreviousAxis, endOfThisAxis);
                            DrawTri(colours[dataSets.IndexOf(dataSet)], endOfPreviousAxis, endOfThisAxis, axis.centre);
                        }
                        catch
                        {

                        }
                    }
                }
            }

            for (int i = 0; i < axis.segments; i++)
            {
                float angle = i * Mathf.PI * 2 / axis.segments;
                Vector2 pointOnCircle = axis.centre + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * axis.radius;
                GUI.Label(new Rect(pointOnCircle.x - 10, pointOnCircle.y, 100, 30), dataSets[0].dataPoints[i].label);
            }

            foreach (DataSet dataSet in dataSets)
            {
                List<Color> colours = new List<Color>();
                colours.Add(Color.red);
                colours.Add(Color.magenta);
                colours.Add(Color.cyan);
                colours.Add(Color.yellow);
                colours.Add(Color.green);
                colours.Add(Color.blue);
                colours.Add(Color.white);
                for (int i = 0; i < 100; i++)
                {
                    colours.Add(new Color(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1)));
                }

                GUIStyle style = new GUIStyle();
                style.normal.textColor = colours[dataSets.IndexOf(dataSet)];

                GUILayout.BeginArea(new Rect(10, 15 * dataSets.IndexOf(dataSet) + 30, 100, 100));
                GUILayout.Label(dataSet.name, style);
                GUILayout.EndArea();
            }

        }

        public void Resize(Rect position)
        {
            axis.Resize(position);
        }
    }

    public struct RadarGraphAxis
    {
        public RadarGraphAxis(
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

        public void Render()
        {
            for (int i = 0; i < segments; i++)
            {
                float angle = i * Mathf.PI * 2 / segments;
                Vector2 pointOnCircle = centre + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                DrawLine(axisCol, centre, pointOnCircle);

                for (int j = 0; j <= divisions; j++)
                {
                    Vector2 endOfThisAxis = centre + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ((radius / divisions) * j);
                    float prevAngle = (i - 1) * Mathf.PI * 2 / segments;
                    Vector2 endOfPreviousAxis = centre + new Vector2(Mathf.Cos(prevAngle), Mathf.Sin(prevAngle)) * ((radius / divisions) * j);
                    DrawLine(gridCol, endOfPreviousAxis, endOfThisAxis);
                }
            }
        }

        public void Resize(Rect position)
        {
            centre = new Vector2(position.x, position.y);
            radius = Mathf.Min(position.width, position.height) / 3;
        }
    }

    public struct DataPoint
    {
        public DataPoint(
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

    public struct DataSet
    {
        public DataSet(
            List<DataPoint> dataPoints,
            string name)
        {
            this.dataPoints = dataPoints;
            this.name = name;
        }

        public List<DataPoint> dataPoints;
        public string name;
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
