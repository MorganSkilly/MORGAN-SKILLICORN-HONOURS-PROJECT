using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class WeaponSystem_Dashboard : EditorWindow
{
    public enum GraphDisplay
    {
        recoil,
        damage,
        ammo
    }

    GraphDisplay graphDisplay = GraphDisplay.recoil;

    Vector2 scrollPos1, scrollPos2, scrollPos3, scrollPos4;
    bool advancedViewActive;

    CustomConsole dashboardConsole;

    GraphsPlus.RadarGraph radarGraph;
    Rect graphContainer;

    Vector2 scrollPos;

    public List<Weapon> weapons = new List<Weapon>();
    public List<Weapon> weaponsAll = new List<Weapon>();

    List<weaponParamType> advancedParams = new List<weaponParamType>();

    [MenuItem("Weapon System/Dashboard")]
    static void ShowWindow()
    {
        WeaponSystem_Dashboard window = CreateInstance<WeaponSystem_Dashboard>();

        window.titleContent.text = "Weapon Balancing Dahsboard";
        window.Show();

        string[] assetNames = AssetDatabase.FindAssets("t:Weapon");
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

        RefreshGraph(graphDisplay);

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

        try
        {
            radarGraph.Render(graphContainer);
        }
        catch
        {

        }
        
        DashTopBar();        

        GUI.skin.scrollView = styleQuarter;                           
        EditorGUILayout.BeginHorizontal();

            scrollPos1 = EditorGUILayout.BeginScrollView(scrollPos1);
                if (!advancedViewActive)
                {
                    GUILayout.Label("Weapon Objects In This Project", titleStyle);
                    GUILayout.Label("(click to add to comparison)", titleStyle);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                    foreach (Weapon weapon in weaponsAll)
                    {
                        if (GUILayout.Button(weapon.name))
                        {
                            if (!weapons.Contains(weapon))
                            {
                                weapons.Add(weapon);
                                RefreshGraph(graphDisplay);

                            }
                        }
                    }
                }
                else
                {
                    GUILayout.Label("Weapon Objects In This Project", titleStyle);
                    GUILayout.Label("(click to add to comparison)", titleStyle);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                    foreach (Weapon weapon in weaponsAll)
                    {
                        if (GUILayout.Button(weapon.name))
                        {
                            if (!weapons.Contains(weapon))
                            {
                                weapons.Add(weapon);
                                RefreshGraph(graphDisplay);

                            }
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
                scrollPos3 = EditorGUILayout.BeginScrollView(scrollPos3);
                    if (!advancedViewActive)
                    {
                        GUILayout.Label("Weapon Objects Active In Graph", titleStyle);
                        GUILayout.Label("(click to remove from comparison)", titleStyle);
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                        List<Weapon> tempWeapons = weapons;

                        for (int i = 0; i < weapons.Count; i++)
                        {
                            if (GUILayout.Button(weapons[i].name))
                            {
                                if (weapons.Contains(weapons[i]))
                                {
                                    if (weapons.Count > 1)
                                    {
                                        weapons.Remove(weapons[i]);
                                        RefreshGraph(graphDisplay);
                                    }

                                }
                            }
                        }                        
                    }
                    else
                    {
                        GUILayout.Label("Weapon Objects Active In Graph", titleStyle);
                        GUILayout.Label("(click to remove from comparison)", titleStyle);
                        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                        List<Weapon> tempWeapons = weapons;

                        for (int i = 0; i < weapons.Count; i++)
                        {
                            if (GUILayout.Button(weapons[i].name))
                            {
                                if (weapons.Contains(weapons[i]))
                                {
                                    if (weapons.Count > 1)
                                    {
                                        weapons.Remove(weapons[i]);
                                        RefreshGraph(graphDisplay);
                                    }

                                }
                            }
                        }
                    }
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
                    RefreshGraph(graphDisplay);
                }
            }
            else
            {
                if (GUILayout.Button("Weapons++ Dashboard (Simplified View Active) - Swap to Advanced View"))
                {
                    advancedViewActive = !advancedViewActive;
                    RefreshGraph(graphDisplay);
                }
            }

            if (GUILayout.Button("Refresh data (do this after editing a weapon object!)"))
            {
                string[] assetNames = AssetDatabase.FindAssets("t:Weapon");
                weaponsAll.Clear();

                foreach (string SOName in assetNames)
                {
                    var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
                    var character = AssetDatabase.LoadAssetAtPath<Weapon>(SOpath);
                    weaponsAll.Add(character);
                }
                RefreshGraph(graphDisplay);
            }

        EditorGUILayout.EndHorizontal();

        if (!advancedViewActive)
        {
            EditorGUILayout.BeginHorizontal(styleFullWidth);

                if (GUILayout.Button("Recoil Analysis", buttonStyle))
                {
                    graphDisplay = GraphDisplay.recoil;

                    RefreshGraph(graphDisplay);
                }
                if (GUILayout.Button("Damage Analysis", buttonStyle))
                {
                    graphDisplay = GraphDisplay.damage;

                    RefreshGraph(graphDisplay);
                }
                if (GUILayout.Button("Ammo Analysis", buttonStyle))
                {
                    graphDisplay = GraphDisplay.ammo;

                    RefreshGraph(graphDisplay);
                }

            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal(styleFullWidth);

            if (GUILayout.Button("base damage", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.baseDamage))
                {
                    advancedParams.Add(weaponParamType.baseDamage);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.baseDamage);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("critical multiplier", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.criticalMultiplier))
                {
                    advancedParams.Add(weaponParamType.criticalMultiplier);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.criticalMultiplier);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("effective range", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.effectiveRange))
                {
                    advancedParams.Add(weaponParamType.effectiveRange);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.effectiveRange);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("range", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.range))
                {
                    advancedParams.Add(weaponParamType.range);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.range);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("fire rate", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.fireRate))
                {
                    advancedParams.Add(weaponParamType.fireRate);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.fireRate);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("start ammo", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.startAmmo))
                {
                    advancedParams.Add(weaponParamType.startAmmo);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.startAmmo);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("magazine size", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.magazineSize))
                {
                    advancedParams.Add(weaponParamType.magazineSize);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.magazineSize);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("reload speed", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.reloadSpeed))
                {
                    advancedParams.Add(weaponParamType.reloadSpeed);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.reloadSpeed);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("ads speed", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.adsSpeed))
                {
                    advancedParams.Add(weaponParamType.adsSpeed);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.adsSpeed);
                }
                RefreshGraph(graphDisplay);
            }
            
            EditorGUILayout.EndHorizontal();
        }
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

    public void RefreshGraph(GraphDisplay graphDisplay)
    {
        List<GraphsPlus.DataSet> dataSets = new List<GraphsPlus.DataSet>();

        try
        {
            if (!advancedViewActive)
            {
                switch (graphDisplay)
                {
                    case GraphDisplay.recoil:

                        foreach (Weapon weapon in weapons)
                        {
                            List<GraphsPlus.DataPoint> adsRecoilData = new List<GraphsPlus.DataPoint>();
                            List<GraphsPlus.DataPoint> hipRecoilData = new List<GraphsPlus.DataPoint>();

                            adsRecoilData.Add(new GraphsPlus.DataPoint("ads recoil x", Mathf.Abs(weapon.parameters.adsRecoil.x), 0f, GetUpper(weapons, weaponParamType.recoilX)));
                            adsRecoilData.Add(new GraphsPlus.DataPoint("ads recoil y", Mathf.Abs(weapon.parameters.adsRecoil.y), 0f, GetUpper(weapons, weaponParamType.recoilY)));
                            adsRecoilData.Add(new GraphsPlus.DataPoint("ads recoil z", Mathf.Abs(weapon.parameters.adsRecoil.z), 0f, GetUpper(weapons, weaponParamType.recoilZ)));
                            adsRecoilData.Add(new GraphsPlus.DataPoint("ads recoil randomness", weapon.parameters.adsRecoilRandomness, 0f, GetUpper(weapons, weaponParamType.recoilRandomness)));
                            adsRecoilData.Add(new GraphsPlus.DataPoint("ads recoil snap", weapon.parameters.adsRecoilSnap, 0f, GetUpper(weapons, weaponParamType.recoilSnap)));
                            adsRecoilData.Add(new GraphsPlus.DataPoint("ads recoil return", weapon.parameters.adsRecoilReturn, 0f, GetUpper(weapons, weaponParamType.recoilReturn)));
                            GraphsPlus.DataSet dataset1 = new GraphsPlus.DataSet(adsRecoilData, weapon.name + " - ADS");

                            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil x", Mathf.Abs(weapon.parameters.hipRecoil.x), 0f, GetUpper(weapons, weaponParamType.recoilX)));
                            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil y", Mathf.Abs(weapon.parameters.hipRecoil.y), 0f, GetUpper(weapons, weaponParamType.recoilY)));
                            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil z", Mathf.Abs(weapon.parameters.hipRecoil.z), 0f, GetUpper(weapons, weaponParamType.recoilZ)));
                            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil randomness", weapon.parameters.hipRecoilRandomness, 0f, GetUpper(weapons, weaponParamType.recoilRandomness)));
                            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil snap", weapon.parameters.hipRecoilSnap, 0f, GetUpper(weapons, weaponParamType.recoilSnap)));
                            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil return", weapon.parameters.hipRecoilReturn, 0f, GetUpper(weapons, weaponParamType.recoilReturn)));
                            GraphsPlus.DataSet dataset2 = new GraphsPlus.DataSet(hipRecoilData, weapon.name + " - HIP");

                            dataSets.Add(dataset1);
                            dataSets.Add(dataset2);
                        }
                        break;

                    case GraphDisplay.damage:

                        foreach (Weapon weapon in weapons)
                        {
                            List<GraphsPlus.DataPoint> damageData = new List<GraphsPlus.DataPoint>();

                            damageData.Add(new GraphsPlus.DataPoint("base damage", weapon.parameters.baseDamage, 0f, GetUpper(weapons, weaponParamType.baseDamage)));
                            damageData.Add(new GraphsPlus.DataPoint("critical multiplier", weapon.parameters.criticalMultiplier, 0f, GetUpper(weapons, weaponParamType.criticalMultiplier)));
                            damageData.Add(new GraphsPlus.DataPoint("effective range", weapon.parameters.effectiveRange, 0f, GetUpper(weapons, weaponParamType.effectiveRange)));
                            damageData.Add(new GraphsPlus.DataPoint("range", weapon.parameters.range, 0f, GetUpper(weapons, weaponParamType.range)));
                            //dataSet1.Add(new GraphsPlus.DataPoint("ads speed", weapon.parameters.adsSpeed, 0f, 100f));
                            GraphsPlus.DataSet dataset1 = new GraphsPlus.DataSet(damageData, weapon.name);

                            dataSets.Add(dataset1);
                        }
                        break;

                    case GraphDisplay.ammo:
                        foreach (Weapon weapon in weapons)
                        {
                            List<GraphsPlus.DataPoint> ammoData = new List<GraphsPlus.DataPoint>();

                            ammoData.Add(new GraphsPlus.DataPoint("fire rate", weapon.parameters.fireRate, 0f, GetUpper(weapons, weaponParamType.fireRate)));
                            ammoData.Add(new GraphsPlus.DataPoint("start ammo", weapon.parameters.startAmmo, 0f, GetUpper(weapons, weaponParamType.startAmmo)));
                            ammoData.Add(new GraphsPlus.DataPoint("magazine size", weapon.parameters.magazineSize, 0f, GetUpper(weapons, weaponParamType.magazineSize)));
                            ammoData.Add(new GraphsPlus.DataPoint("reload speed", weapon.parameters.reloadSpeed, 0f, GetUpper(weapons, weaponParamType.reloadSpeed)));
                            GraphsPlus.DataSet dataset1 = new GraphsPlus.DataSet(ammoData, weapon.name);

                            dataSets.Add(dataset1);
                        }
                        break;
                }
            }
            else
            {
                foreach (Weapon weapon in weapons)
                {
                    List<GraphsPlus.DataPoint> allAdvancedData = new List<GraphsPlus.DataPoint>();

                    foreach (weaponParamType param in advancedParams)
                    {
                        if (param == weaponParamType.baseDamage)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("base damage", weapon.parameters.baseDamage, 0f, GetUpper(weapons, weaponParamType.baseDamage)));
                        }
                        else if (param == weaponParamType.criticalMultiplier)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("critical multiplier", weapon.parameters.criticalMultiplier, 0f, GetUpper(weapons, weaponParamType.criticalMultiplier)));
                        }
                        else if (param == weaponParamType.effectiveRange)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("effective range", weapon.parameters.effectiveRange, 0f, GetUpper(weapons, weaponParamType.effectiveRange)));
                        }
                        else if (param == weaponParamType.range)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("range", weapon.parameters.range, 0f, GetUpper(weapons, weaponParamType.range)));
                        }
                        else if (param == weaponParamType.fireRate)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads speed", weapon.parameters.adsSpeed, 0f, GetUpper(weapons, weaponParamType.adsSpeed)));
                        }
                        else if (param == weaponParamType.startAmmo)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("fire rate", weapon.parameters.fireRate, 0f, GetUpper(weapons, weaponParamType.fireRate)));
                        }
                        else if (param == weaponParamType.magazineSize)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("start ammo", weapon.parameters.startAmmo, 0f, GetUpper(weapons, weaponParamType.startAmmo)));
                        }
                        else if (param == weaponParamType.reloadSpeed)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("magazine size", weapon.parameters.magazineSize, 0f, GetUpper(weapons, weaponParamType.magazineSize)));
                        }
                        else if (param == weaponParamType.adsSpeed)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("reload speed", weapon.parameters.reloadSpeed, 0f, GetUpper(weapons, weaponParamType.reloadSpeed)));
                        }
                        else if (param == weaponParamType.adsRecoilX)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads recoil x", Mathf.Abs(weapon.parameters.adsRecoil.x), 0f, GetUpper(weapons, weaponParamType.adsRecoilX)));
                        }
                        else if (param == weaponParamType.adsRecoilY)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads recoil y", Mathf.Abs(weapon.parameters.adsRecoil.y), 0f, GetUpper(weapons, weaponParamType.adsRecoilY)));
                        }
                        else if (param == weaponParamType.adsRecoilZ)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads recoil z", Mathf.Abs(weapon.parameters.adsRecoil.z), 0f, GetUpper(weapons, weaponParamType.adsRecoilZ)));
                        }
                        else if (param == weaponParamType.adsRecoilRandomness)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads recoil randomness", weapon.parameters.adsRecoilRandomness, 0f, GetUpper(weapons, weaponParamType.adsRecoilRandomness)));
                        }
                        else if (param == weaponParamType.adsRecoilSnap)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads recoil snap", weapon.parameters.adsRecoilSnap, 0f, GetUpper(weapons, weaponParamType.adsRecoilSnap)));
                        }
                        else if (param == weaponParamType.adsRecoilReturn)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads recoil return", weapon.parameters.adsRecoilReturn, 0f, GetUpper(weapons, weaponParamType.adsRecoilReturn)));
                        }
                        else if (param == weaponParamType.hipRecoilX)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("hip recoil x", Mathf.Abs(weapon.parameters.hipRecoil.x), 0f, GetUpper(weapons, weaponParamType.hipRecoilX)));
                        }
                        else if (param == weaponParamType.hipRecoilY)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("hip recoil y", Mathf.Abs(weapon.parameters.hipRecoil.y), 0f, GetUpper(weapons, weaponParamType.hipRecoilY)));
                        }
                        else if (param == weaponParamType.hipRecoilZ)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("hip recoil z", Mathf.Abs(weapon.parameters.hipRecoil.z), 0f, GetUpper(weapons, weaponParamType.hipRecoilZ)));
                        }
                        else if (param == weaponParamType.hipRecoilRandomness)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("hip recoil randomness", weapon.parameters.hipRecoilRandomness, 0f, GetUpper(weapons, weaponParamType.hipRecoilRandomness)));
                        }
                        else if (param == weaponParamType.hipRecoilSnap)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("hip recoil snap", weapon.parameters.hipRecoilSnap, 0f, GetUpper(weapons, weaponParamType.hipRecoilSnap)));
                        }
                        else if (param == weaponParamType.hipRecoilReturn)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("hip recoil return", weapon.parameters.hipRecoilReturn, 0f, GetUpper(weapons, weaponParamType.hipRecoilReturn)));
                        }
                    }

                    GraphsPlus.DataSet advancedDataset = new GraphsPlus.DataSet(allAdvancedData, weapon.name);

                    dataSets.Add(advancedDataset);
                }
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
        catch
        {

        }
    }

    float GetUpper(List<Weapon> weapons, weaponParamType param)
    {
        float maxValue = 0f;

        foreach (Weapon weapon in weapons)
        {
            if (param == weaponParamType.baseDamage)
            {
                if (Mathf.Abs(weapon.parameters.baseDamage) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.baseDamage);
                }
            }
            else if (param == weaponParamType.criticalMultiplier)
            {
                if (Mathf.Abs(weapon.parameters.criticalMultiplier) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.criticalMultiplier);
                }
            }
            else if (param == weaponParamType.effectiveRange)
            {
                if (Mathf.Abs(weapon.parameters.effectiveRange) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.effectiveRange);
                }
            }
            else if (param == weaponParamType.range)
            {
                if (Mathf.Abs(weapon.parameters.range) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.range);
                }
            }
            else if (param == weaponParamType.fireRate)
            {
                if (Mathf.Abs(weapon.parameters.fireRate) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.fireRate);
                }
            }
            else if (param == weaponParamType.startAmmo)
            {
                if (Mathf.Abs(weapon.parameters.startAmmo) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.startAmmo);
                }
            }
            else if (param == weaponParamType.magazineSize)
            {
                if (Mathf.Abs(weapon.parameters.magazineSize) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.magazineSize);
                }
            }
            else if (param == weaponParamType.reloadSpeed)
            {
                if (Mathf.Abs(weapon.parameters.reloadSpeed) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.reloadSpeed);
                }
            }
            else if (param == weaponParamType.adsSpeed)
            {
                if (Mathf.Abs(weapon.parameters.adsSpeed) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsSpeed);
                }
            }
            else if (param == weaponParamType.recoilX)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoil.x) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoil.x);
                }
                if (Mathf.Abs(weapon.parameters.hipRecoil.x) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoil.x);
                }
            }
            else if (param == weaponParamType.recoilY)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoil.y) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoil.y);
                }
                if (Mathf.Abs(weapon.parameters.hipRecoil.y) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoil.y);
                }
            }
            else if (param == weaponParamType.recoilZ)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoil.z) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoil.z);
                }
                if (Mathf.Abs(weapon.parameters.hipRecoil.z) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoil.z);
                }
            }
            else if (param == weaponParamType.recoilRandomness)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoilRandomness) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoilRandomness);
                }
                if (Mathf.Abs(weapon.parameters.hipRecoilRandomness) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoilRandomness);
                }
            }
            else if (param == weaponParamType.recoilSnap)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoilSnap) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoilSnap);
                }
                if (Mathf.Abs(weapon.parameters.hipRecoilSnap) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoilSnap);
                }
            }
            else if (param == weaponParamType.recoilReturn)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoilReturn) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoilReturn);
                }
                if (Mathf.Abs(weapon.parameters.hipRecoilReturn) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoilReturn);
                }
            }
            else if (param == weaponParamType.adsRecoilX)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoil.x) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoil.x);
                }
            }
            else if (param == weaponParamType.adsRecoilY)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoil.y) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoil.y);
                }
            }
            else if (param == weaponParamType.adsRecoilZ)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoil.z) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoil.z);
                }
            }
            else if (param == weaponParamType.adsRecoilRandomness)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoilRandomness) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoilRandomness);
                }
            }
            else if (param == weaponParamType.adsRecoilSnap)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoilSnap) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoilSnap);
                }
            }
            else if (param == weaponParamType.adsRecoilReturn)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoilReturn) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoilReturn);
                }
            }
            else if (param == weaponParamType.hipRecoilX)
            {
                if (Mathf.Abs(weapon.parameters.hipRecoil.x) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoil.x);
                }
            }
            else if (param == weaponParamType.hipRecoilY)
            {
                if (Mathf.Abs(weapon.parameters.hipRecoil.y) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoil.y);
                }
            }
            else if (param == weaponParamType.hipRecoilZ)
            {
                if (Mathf.Abs(weapon.parameters.hipRecoil.z) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoil.z);
                }
            }
            else if (param == weaponParamType.hipRecoilRandomness)
            {
                if (Mathf.Abs(weapon.parameters.hipRecoilRandomness) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoilRandomness);
                }
            }
            else if (param == weaponParamType.hipRecoilSnap)
            {
                if (Mathf.Abs(weapon.parameters.hipRecoilSnap) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoilSnap);
                }
            }
            else if (param == weaponParamType.hipRecoilReturn)
            {
                if (Mathf.Abs(weapon.parameters.hipRecoilReturn) > maxValue)
                {
                    Mathf.Abs(maxValue = Mathf.Abs(weapon.parameters.hipRecoilReturn));
                }
            }

        }

        return Mathf.Abs(maxValue);
    }

    public enum weaponParamType
    {
        baseDamage,
        criticalMultiplier,
        effectiveRange,
        range,
        fireRate,
        startAmmo,
        magazineSize,
        reloadSpeed,
        adsSpeed,
        recoilX,
        recoilY,
        recoilZ,
        recoilRandomness,
        recoilSnap,
        recoilReturn,
        adsRecoilX,
        adsRecoilY,
        adsRecoilZ,
        adsRecoilRandomness,
        adsRecoilSnap,
        adsRecoilReturn,
        hipRecoilX,
        hipRecoilY,
        hipRecoilZ,
        hipRecoilRandomness,
        hipRecoilSnap,
        hipRecoilReturn,
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
