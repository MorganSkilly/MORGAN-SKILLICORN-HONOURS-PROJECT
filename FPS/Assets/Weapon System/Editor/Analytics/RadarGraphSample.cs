using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RadarGraphSample : EditorWindow
{
    GraphsPlus.RadarGraph radarGraph;
    public List<Weapon> weapons = new List<Weapon>();
    Weapon weaponAddField = null;

    [MenuItem("Graphs Plus/Custom/Radar Graph")]
    static void ShowWindow()
    {
        RadarGraphSample window = CreateInstance<RadarGraphSample>();
        window.titleContent.text = "Radar Graph Instance";
        window.Show();
    }

    private void OnEnable()
    {
        weapons = PullWeapons();

        RenderEditor();
    }

    private void RenderEditor()
    {
        List<GraphsPlus.DataSet> dataSets = new List<GraphsPlus.DataSet>();

        List<Weapon> weapons = new List<Weapon>();

        string[] assetNames = AssetDatabase.FindAssets("t:Weapon");

        weapons.Clear();

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var character = AssetDatabase.LoadAssetAtPath<Weapon>(SOpath);
            weapons.Add(character);

            Debug.Log(character.name);
        }

        weapons = this.weapons;

        foreach (Weapon weapon in weapons)
        {
            List<GraphsPlus.DataPoint> dataSet1 = new List<GraphsPlus.DataPoint>();
            dataSet1.Add(new GraphsPlus.DataPoint("base damage", weapon.parameters.baseDamage, 0f, 100f));
            dataSet1.Add(new GraphsPlus.DataPoint("effective range", weapon.parameters.effectiveRange, 0f, 100f));
            dataSet1.Add(new GraphsPlus.DataPoint("fire rate", weapon.parameters.fireRate, 0f, 100));
            dataSet1.Add(new GraphsPlus.DataPoint("magazine size", weapon.parameters.magazineSize, 0f, 100f));
            dataSet1.Add(new GraphsPlus.DataPoint("range", weapon.parameters.range, 0f, 100));
            GraphsPlus.DataSet dataset = new GraphsPlus.DataSet(dataSet1, weapon.name);
            dataSets.Add(dataset);
        }

        GraphsPlus.RadarGraphAxis radarGraphAxis = new GraphsPlus.RadarGraphAxis(
            new Vector2(position.width / 2, position.height / 2),
            Mathf.Min(position.width, position.height) / 3,
            10, dataSets[0].dataPoints.Count);

        radarGraph = new GraphsPlus.RadarGraph(radarGraphAxis, dataSets);
    }

    public void OnGUI()
    {
        try
        {
            RenderEditor();
            radarGraph.Render(position);

            weaponAddField = WeaponField("Compare", weaponAddField);

            if (GUILayout.Button("Add/Remove from weapon comparison graph"))
            {
                if (weaponAddField != null && !weapons.Contains(weaponAddField))
                {
                    weapons.Add(weaponAddField);
                }
                else if (weaponAddField != null && weapons.Contains(weaponAddField) && weapons.Count > 1)
                {
                    weapons.Remove(weaponAddField);
                }
            }
        }
        catch
        {
            Debug.LogWarning("A weapon object was deleted with active graph windows open, attempting to restore graphs");
            weapons = PullWeapons();
            RenderEditor();
        }
    }

    void OnInspectorUpdate()
    {
        radarGraph.Resize(position);
    }

    private List<Weapon> PullWeapons()
    {
        List<Weapon> weapons = new List<Weapon>();


        weapons.Clear();

        string[] assetNames = AssetDatabase.FindAssets("t:Weapon");

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var character = AssetDatabase.LoadAssetAtPath<Weapon>(SOpath);
            weapons.Add(character);
        }

        return weapons;
    }

    private static Weapon WeaponField(string name, Weapon weaponObject)
    {
        GUILayout.BeginHorizontal();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.fixedWidth = 148;
        GUILayout.Label(name, style);
        var result = (Weapon)EditorGUILayout.ObjectField(weaponObject, typeof(Weapon), false);
        GUILayout.EndVertical();
        return result;
    }
}
