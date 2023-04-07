using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class WeaponSystem_Dashboard : EditorWindow
{
    Vector2 scrollPos1, scrollPos2, scrollPos3, scrollPos4;
    bool advancedViewActive;

    CustomConsole dashboardConsole;

    GraphsPlus.RadarGraph radarGraph;
    Rect graphContainer;

    Vector2 scrollPos;

    public List<Weapon> weapons = new List<Weapon>();
    public List<Weapon> weaponsAll = new List<Weapon>();


    [MenuItem("Weapon System/Dashboard")]
    static void ShowWindow()
    {
        WeaponSystem_Dashboard window = CreateInstance<WeaponSystem_Dashboard>();

        window.titleContent.text = "Weapon Balancing Dahsboard";
        window.Show();
    }

    private void OnEnable()
    {
        string[] assetNames = AssetDatabase.FindAssets("t:Weapon");
        weaponsAll.Clear();

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var character = AssetDatabase.LoadAssetAtPath<Weapon>(SOpath);
            weaponsAll.Add(character);
        }

        advancedViewActive = false;
        dashboardConsole = new CustomConsole(new List<Log>());
        dashboardConsole.Log("The convex hull of x fully encompasses that of y it is reccomended you balance these weapons to ensure they are both useful", LogType.info);
        dashboardConsole.Log("The convex hull of x fully encompasses that of y it is reccomended you balance these weapons to ensure they are both useful", LogType.info);

        RefreshGraph();

    }

    void OnInspectorUpdate()
    {
        if (Mathf.Min(position.width, position.height) < 500)
            graphContainer = new Rect(position.width / 2, 600 / 2, position.width, position.height);
        else
            graphContainer = new Rect(position.width / 2, 600 / 2, 500, 500);

        radarGraph.Resize(graphContainer);
    }

    public void OnGUI()
    {
        //styles
        GUIStyle labelStyle = new GUIStyle();
            labelStyle.wordWrap = true;
                labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
            labelStyle.hover.textColor = new Color(0.8f, 0.8f, 0.8f);

        GUIStyle titleStyle = labelStyle;
            titleStyle.fontSize = titleStyle.fontSize * 2;
            titleStyle.fontStyle = FontStyle.Bold;

        GUIStyle styleHalf = new GUIStyle();
            styleHalf.fixedWidth = position.width / 2;
            styleHalf.fixedHeight = position.height;

        GUIStyle styleQuarter = new GUIStyle();
            styleQuarter.fixedWidth = position.width / 4;
            styleQuarter.fixedHeight = position.height;

        GUIStyle styleConsole = new GUIStyle();
            styleConsole.fixedWidth = position.width / 2;

        //graph and background
        EditorGUI.DrawRect(new Rect((position.width / 4), 89, (position.width / 2), position.height), new Color(0.15f, 0.15f, 0.15f));
        radarGraph.Render(graphContainer);      
        
        DashTopBar();        

        GUI.skin.scrollView = styleQuarter;                           
        EditorGUILayout.BeginHorizontal();

            scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1);
                GUILayout.Label("Weapon Objects In This Project", titleStyle);
                GUILayout.Label("(click to add to comparison)", titleStyle);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                foreach (Weapon weapon in weaponsAll)
                {
                    if(GUILayout.Button(weapon.name))
                    {
                        if (!weapons.Contains(weapon))
                        {
                            weapons.Add(weapon);
                            RefreshGraph();

                        }
                    }
                }
            EditorGUILayout.EndScrollView();


            GUI.skin.scrollView = styleHalf;
            EditorGUILayout.BeginVertical();

                EditorGUILayout.Space(450);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                        
                GUI.skin.scrollView = styleConsole;

                scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2);
                    GUILayout.Label("Smart Suggestions Based On This Data", titleStyle);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    dashboardConsole.Draw();
                EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

            GUI.skin.scrollView = styleQuarter;
            EditorGUILayout.BeginHorizontal();
                scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos3);
                    GUILayout.Label("Weapon Objects Active In Graph", titleStyle);
                    GUILayout.Label("(click to remove from comparison)", titleStyle);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    foreach (Weapon weapon in weapons)
                    {
                        if (GUILayout.Button(weapon.name))
                        {
                            if (weapons.Contains(weapon))
                            {
                                if (weapons.Count > 1)
                                {
                                    weapons.Remove(weapon);
                                    RefreshGraph();
                                }

                            }
                        }
                    }
                    /*
                    GUILayout.Label("name", labelStyle);
                    GUILayout.Label("description", labelStyle);
                    GUILayout.Label("thumbnail", labelStyle);
                    GUILayout.Label("model", labelStyle);
                    GUILayout.Label("flipY", labelStyle);
                    GUILayout.Label("baseDamage", labelStyle);
                    GUILayout.Label("criticalMultiplier", labelStyle);
                    GUILayout.Label("effectiveRange", labelStyle);
                    GUILayout.Label("range", labelStyle);
                    GUILayout.Label("automatic", labelStyle);
                    GUILayout.Label("fireRate", labelStyle);
                    GUILayout.Label("startAmmo", labelStyle);
                    GUILayout.Label("magazineSize", labelStyle);
                    GUILayout.Label("reloadSpeed", labelStyle);
                    GUILayout.Label("adsSpeed", labelStyle);
                    GUILayout.Label("adsRecoil", labelStyle);
                    GUILayout.Label("adsRecoilRandomness", labelStyle);
                    GUILayout.Label("adsRecoilSnap", labelStyle);
                    GUILayout.Label("adsRecoilReturn", labelStyle);
                    GUILayout.Label("hipRecoil", labelStyle);
                    GUILayout.Label("hipRecoilRandomness", labelStyle);
                    GUILayout.Label("hipRecoilSnap", labelStyle);
                    GUILayout.Label("hipRecoilReturn", labelStyle);
                    GUILayout.Label("shotSound", labelStyle);
                    GUILayout.Label("emptyShotSound", labelStyle);
                    GUILayout.Label("reloadSound", labelStyle);
                    GUILayout.Label("barrelExit", labelStyle);
                    GUILayout.Label("muzzleFlash", labelStyle);
                    GUILayout.Label("impactShot", labelStyle);
                    GUILayout.Label("reloadAnim", labelStyle);
                    */
                EditorGUILayout.EndScrollView();

            EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndHorizontal();

        GUI.skin.scrollView = new GUIStyle();
    }

    void DashTopBar()
    {
        GUIStyle styleFullWidth = new GUIStyle();
            styleFullWidth.fixedWidth = position.width;

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fixedHeight = 50;

        EditorGUILayout.BeginHorizontal(styleFullWidth);

            if (advancedViewActive)
            {
                if (GUILayout.Button("Weapons++ Dashboard (Advanced View Active) - Swap to Simplified View"))
                {
                    advancedViewActive = !advancedViewActive;
                }
            }
            else
            {
                if (GUILayout.Button("Weapons++ Dashboard (Simplified View Active) - Swap to Advanced View"))
                {
                    advancedViewActive = !advancedViewActive;
                }
            }            
            
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(styleFullWidth);

            //add switching statement for buttons load ui
            GUILayout.Button("Recoil Analysis", buttonStyle); //both ads and hip (lighter colour for one)
            GUILayout.Button("Damage Analysis", buttonStyle);
            GUILayout.Button("Ammo Analysis", buttonStyle);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

    }

    struct CustomConsole
    {
        List<Log> logs;

        public CustomConsole(List<Log> logs)
        {
            this.logs = logs;
        }

        public void Log(string message, LogType type)
        {
            logs.Add(new WeaponSystem_Dashboard.Log(message, type));
        }

        public void Draw()
        {                        
            //scrollPosConsole = EditorGUILayout.BeginScrollView(scrollPosConsole);
            
            for (var i = logs.Count - 1; i >= 0; i--)
            {
                GUIStyle style = new GUIStyle();
                style.wordWrap = true;


                //loading multiple textures slows unity UI down

                if (i % 2 == 0)
                {
                    style.normal.background = GetSolidTexture(new Color(0.2f, 0.2f, 0.2f));
                }
                else
                {
                    style.normal.background = GetSolidTexture(new Color(0.25f, 0.25f, 0.25f));
                }

                if (logs[i].type == LogType.error)
                {
                    style.normal.textColor = Color.red;
                    style.hover.textColor = Color.red;
                }
                else if (logs[i].type == LogType.warning)
                {
                    style.normal.textColor = Color.yellow;
                    style.hover.textColor = Color.yellow;
                }
                else if (logs[i].type == LogType.info)
                {
                    style.normal.textColor = new Color(0.8f, 0.8f, 0.8f);
                    style.hover.textColor = new Color(0.8f, 0.8f, 0.8f);
                }

                GUI.skin.label = style;
                //GUILayout.Label($"[{DateTime.Now.ToString("HH:mm:ss")}] {logs[i].message}");
                GUILayout.Label($"{logs[i].message}");
                GUILayout.Label("");
                GUI.skin.label = new GUIStyle();
                //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


                //EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }

            //EditorGUILayout.EndScrollView();
        }
    }

    struct Log
    {
        public Log(string message, LogType type)
        {
            this.message = $"[{DateTime.Now.ToString("HH:mm:ss")}] {message}";
            this.type = type;
        }

        public string message;
        public LogType type;
    }

    enum LogType
    {
        info,
        warning,
        error
    }

    public static Texture2D GetSolidTexture(Color colour)
    {
        var texture = new Texture2D(Screen.width, Screen.height);
        Color[] pixels = Enumerable.Repeat(colour, Screen.width * Screen.height).ToArray();
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }

    public void RefreshGraph()
    {
        List<GraphsPlus.DataSet> dataSets = new List<GraphsPlus.DataSet>();

        foreach (Weapon weapon in weapons)
        {
            List<GraphsPlus.DataPoint> adsRecoilData = new List<GraphsPlus.DataPoint>();
            List<GraphsPlus.DataPoint> hipRecoilData = new List<GraphsPlus.DataPoint>();
            //dataSet1.Add(new GraphsPlus.DataPoint("base damage", weapon.parameters.baseDamage, 0f, 100f));
            //dataSet1.Add(new GraphsPlus.DataPoint("critical multiplier", weapon.parameters.criticalMultiplier, 0f, 200f));
            //dataSet1.Add(new GraphsPlus.DataPoint("effective range", weapon.parameters.effectiveRange, 0f, 10f));
            //dataSet1.Add(new GraphsPlus.DataPoint("range", weapon.parameters.range, 0f, 100f));
            //dataSet1.Add(new GraphsPlus.DataPoint("fire rate", weapon.parameters.fireRate, 0f, 1000f));
            //dataSet1.Add(new GraphsPlus.DataPoint("start ammo", weapon.parameters.startAmmo, 0f, 100f));
            //dataSet1.Add(new GraphsPlus.DataPoint("magazine size", weapon.parameters.magazineSize, 0f, 40f));
            //dataSet1.Add(new GraphsPlus.DataPoint("reload speed", weapon.parameters.reloadSpeed, 0f, 30f));
            //dataSet1.Add(new GraphsPlus.DataPoint("ads speed", weapon.parameters.adsSpeed, 0f, 100f));

            adsRecoilData.Add(new GraphsPlus.DataPoint("ads recoil x", Mathf.Abs(weapon.parameters.adsRecoil.x), 0f, 10f));
            adsRecoilData.Add(new GraphsPlus.DataPoint("ads recoil y", Mathf.Abs(weapon.parameters.adsRecoil.y), 0f, 10f));
            adsRecoilData.Add(new GraphsPlus.DataPoint("ads recoil z", Mathf.Abs(weapon.parameters.adsRecoil.z), 0f, 10f));
            adsRecoilData.Add(new GraphsPlus.DataPoint("ads recoil randomness", weapon.parameters.adsRecoilRandomness, 0f, 10f));
            adsRecoilData.Add(new GraphsPlus.DataPoint("ads recoil snap", weapon.parameters.adsRecoilSnap, 0f, 10f));
            adsRecoilData.Add(new GraphsPlus.DataPoint("ads recoil return", weapon.parameters.adsRecoilReturn, 0f, 10f));
            GraphsPlus.DataSet dataset1 = new GraphsPlus.DataSet(adsRecoilData, weapon.name + " - ADS");

            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil x", Mathf.Abs(weapon.parameters.hipRecoil.x), 0f, 10f));
            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil y", Mathf.Abs(weapon.parameters.hipRecoil.y), 0f, 10f));
            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil z", Mathf.Abs(weapon.parameters.hipRecoil.z), 0f, 10f));
            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil randomness", weapon.parameters.hipRecoilRandomness, 0f, 10f));
            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil snap", weapon.parameters.hipRecoilSnap, 0f, 10f));
            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil return", weapon.parameters.hipRecoilReturn, 0f, 10f));
            GraphsPlus.DataSet dataset2 = new GraphsPlus.DataSet(hipRecoilData, weapon.name + " - HIP");

            dataSets.Add(dataset1);
            dataSets.Add(dataset2);
        }

        if (Mathf.Min(position.width, position.height) < 300)
            graphContainer = new Rect(position.width / 3, 300 / 3, position.width, position.height);
        else
            graphContainer = new Rect(position.width / 3, 300 / 3, 300, 300);


        GraphsPlus.RadarGraphAxis radarGraphAxis = new GraphsPlus.RadarGraphAxis(
            new Vector2(graphContainer.x, graphContainer.y),
            Mathf.Min(graphContainer.width, graphContainer.height) / 3,
            10, dataSets[0].dataPoints.Count);

        radarGraph = new GraphsPlus.RadarGraph(radarGraphAxis, dataSets);


    }
}
