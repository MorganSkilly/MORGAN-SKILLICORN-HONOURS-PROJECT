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
        dashboardConsole.Log("CONSOLE TEST", LogType.info);
        dashboardConsole.Log("CONSOLE TEST", LogType.warning);
        dashboardConsole.Log("CONSOLE TEST", LogType.error);

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

                    ListWeapons();
                }
                else
                {
                    GUILayout.Label("Weapon Objects In This Project", titleStyle);
                    GUILayout.Label("(click to add to comparison)", titleStyle);
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                    ListWeapons();
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

    private void ListWeapons()
    {
        GUI.skin.button.normal.textColor = Color.cyan;
        if (GUILayout.Button("Compare all assault rifles"))
        {
            weapons.Clear();
            foreach (Weapon weapon in weaponsAll)
            {
                if (weapon.parameters.description.Contains("assault rifle"))
                {
                    if (!weapons.Contains(weapon))
                    {
                        weapons.Add(weapon);
                        RefreshGraph(graphDisplay);
                    }
                }
            }
        }

        GUI.skin.button.normal.textColor = Color.white;
        foreach (Weapon weapon in weaponsAll)
        {
            if (weapon.parameters.description.Contains("assault rifle"))
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

        GUI.skin.button.normal.textColor = Color.cyan;
        if (GUILayout.Button("Compare all light machine guns"))
        {
            weapons.Clear();
            foreach (Weapon weapon in weaponsAll)
            {
                if (weapon.parameters.description.Contains("light machine gun"))
                {
                    if (!weapons.Contains(weapon))
                    {
                        weapons.Add(weapon);
                        RefreshGraph(graphDisplay);
                    }
                }
            }
        }

        GUI.skin.button.normal.textColor = Color.white;
        foreach (Weapon weapon in weaponsAll)
        {
            if (weapon.parameters.description.Contains("light machine gun"))
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

        GUI.skin.button.normal.textColor = Color.cyan;
        if (GUILayout.Button("Compare all marksman rifles"))
        {
            weapons.Clear();
            foreach (Weapon weapon in weaponsAll)
            {
                if (weapon.parameters.description.Contains("marksman rifle"))
                {
                    if (!weapons.Contains(weapon))
                    {
                        weapons.Add(weapon);
                        RefreshGraph(graphDisplay);
                    }
                }
            }
        }

        GUI.skin.button.normal.textColor = Color.white;
        foreach (Weapon weapon in weaponsAll)
        {
            if (weapon.parameters.description.Contains("marksman rifle"))
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

        GUI.skin.button.normal.textColor = Color.cyan;
        if (GUILayout.Button("Compare all pistols"))
        {
            weapons.Clear();
            foreach (Weapon weapon in weaponsAll)
            {
                if (weapon.parameters.description.Contains("pistol"))
                {
                    if (!weapons.Contains(weapon))
                    {
                        weapons.Add(weapon);
                        RefreshGraph(graphDisplay);
                    }
                }
            }
        }

        GUI.skin.button.normal.textColor = Color.white;
        foreach (Weapon weapon in weaponsAll)
        {
            if (weapon.parameters.description.Contains("pistol"))
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

        GUI.skin.button.normal.textColor = Color.cyan;
        if (GUILayout.Button("Compare all shotguns"))
        {
            weapons.Clear();
            foreach (Weapon weapon in weaponsAll)
            {
                if (weapon.parameters.description.Contains("shotgun"))
                {
                    if (!weapons.Contains(weapon))
                    {
                        weapons.Add(weapon);
                        RefreshGraph(graphDisplay);
                    }
                }
            }
        }

        GUI.skin.button.normal.textColor = Color.white;
        foreach (Weapon weapon in weaponsAll)
        {
            if (weapon.parameters.description.Contains("shotgun"))
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

        GUI.skin.button.normal.textColor = Color.cyan;
        if (GUILayout.Button("Compare all sniper rifles"))
        {
            weapons.Clear();
            foreach (Weapon weapon in weaponsAll)
            {
                if (weapon.parameters.description.Contains("sniper rifle"))
                {
                    if (!weapons.Contains(weapon))
                    {
                        weapons.Add(weapon);
                        RefreshGraph(graphDisplay);
                    }
                }
            }
        }

        GUI.skin.button.normal.textColor = Color.white;
        foreach (Weapon weapon in weaponsAll)
        {
            if (weapon.parameters.description.Contains("sniper rifle"))
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

        GUI.skin.button.normal.textColor = Color.cyan;
        if (GUILayout.Button("Compare all submachine guns"))
        {
            weapons.Clear();
            foreach (Weapon weapon in weaponsAll)
            {
                if (weapon.parameters.description.Contains("submachine gun"))
                {
                    if (!weapons.Contains(weapon))
                    {
                        weapons.Add(weapon);
                        RefreshGraph(graphDisplay);
                    }
                }
            }
        }

        GUI.skin.button.normal.textColor = Color.white;
        foreach (Weapon weapon in weaponsAll)
        {
            if (weapon.parameters.description.Contains("submachine gun"))
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
    }

    void DashTopBar()
    {
        GUIStyle styleFullWidth = new GUIStyle();
            styleFullWidth.fixedWidth = position.width;

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fixedHeight = 25;

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
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("ads recoil x", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.adsRecoilX))
                {
                    advancedParams.Add(weaponParamType.adsRecoilX);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.adsRecoilX);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("ads recoil y", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.adsRecoilY))
                {
                    advancedParams.Add(weaponParamType.adsRecoilY);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.adsRecoilY);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("ads recoil randomness x", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.adsRecoilRandomnessX))
                {
                    advancedParams.Add(weaponParamType.adsRecoilRandomnessX);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.adsRecoilRandomnessX);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("ads recoil randomness y", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.adsRecoilRandomnessY))
                {
                    advancedParams.Add(weaponParamType.adsRecoilRandomnessY);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.adsRecoilRandomnessY);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("ads recoil snap", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.adsRecoilSnap))
                {
                    advancedParams.Add(weaponParamType.adsRecoilSnap);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.adsRecoilSnap);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("ads recoil return", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.adsRecoilReturn))
                {
                    advancedParams.Add(weaponParamType.adsRecoilReturn);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.adsRecoilReturn);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("hip recoil x", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.hipRecoilX))
                {
                    advancedParams.Add(weaponParamType.hipRecoilX);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.hipRecoilX);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("hip recoil y", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.hipRecoilY))
                {
                    advancedParams.Add(weaponParamType.hipRecoilY);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.hipRecoilY);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("hip recoil randomness x", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.hipRecoilRandomnessX))
                {
                    advancedParams.Add(weaponParamType.hipRecoilRandomnessX);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.hipRecoilRandomnessX);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("hip recoil randomness y", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.hipRecoilRandomnessY))
                {
                    advancedParams.Add(weaponParamType.hipRecoilRandomnessY);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.hipRecoilRandomnessY);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("hip recoil snap", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.hipRecoilSnap))
                {
                    advancedParams.Add(weaponParamType.hipRecoilSnap);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.hipRecoilSnap);
                }
                RefreshGraph(graphDisplay);
            }
            if (GUILayout.Button("hip recoil return", buttonStyle))
            {
                if (!advancedParams.Contains(weaponParamType.hipRecoilReturn))
                {
                    advancedParams.Add(weaponParamType.hipRecoilReturn);
                }
                else
                {
                    advancedParams.Remove(weaponParamType.hipRecoilReturn);
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

        public void Clear()
        {
            logs.Clear();
        }

        public void Draw()
        {                        
            //scrollPosConsole = EditorGUILayout.BeginScrollView(scrollPosConsole);
            
            for (var i = logs.Count - 1; i >= 0; i--)
            {
                GUIStyle style = new GUIStyle();
                style.wordWrap = true;


                //loading multiple textures slows unity UI down
                /*
                if (i % 2 == 0)
                {
                    style.normal.background = GetSolidTexture(new Color(0.2f, 0.2f, 0.2f));
                }
                else
                {
                    style.normal.background = GetSolidTexture(new Color(0.25f, 0.25f, 0.25f));
                }*/

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

                            adsRecoilData.Add(new GraphsPlus.DataPoint("recoil x", Mathf.Abs(weapon.parameters.adsRecoil.x), 0f, GetUpper(weapons, weaponParamType.recoilX), true));
                            adsRecoilData.Add(new GraphsPlus.DataPoint("recoil y", Mathf.Abs(weapon.parameters.adsRecoil.y), 0f, GetUpper(weapons, weaponParamType.recoilY), true));
                            adsRecoilData.Add(new GraphsPlus.DataPoint("recoil randomness x", Mathf.Abs(weapon.parameters.adsRecoilRandomness.x), 0f, GetUpper(weapons, weaponParamType.recoilRandomnessX), true));
                            adsRecoilData.Add(new GraphsPlus.DataPoint("recoil randomness y", Mathf.Abs(weapon.parameters.adsRecoilRandomness.y), 0f, GetUpper(weapons, weaponParamType.recoilRandomnessY), true));
                            adsRecoilData.Add(new GraphsPlus.DataPoint("recoil snap", weapon.parameters.adsRecoilSnap, 0f, GetUpper(weapons, weaponParamType.recoilSnap), false));
                            adsRecoilData.Add(new GraphsPlus.DataPoint("recoil return", weapon.parameters.adsRecoilReturn, 0f, GetUpper(weapons, weaponParamType.recoilReturn), false));
                            GraphsPlus.DataSet dataset1 = new GraphsPlus.DataSet(adsRecoilData, weapon.name + " - ADS");

                            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil x", Mathf.Abs(weapon.parameters.hipRecoil.x), 0f, GetUpper(weapons, weaponParamType.hipRecoilX), true));
                            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil y", Mathf.Abs(weapon.parameters.hipRecoil.y), 0f, GetUpper(weapons, weaponParamType.hipRecoilY), true));
                            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil randomness x", Mathf.Abs(weapon.parameters.hipRecoilRandomness.x), 0f, GetUpper(weapons, weaponParamType.hipRecoilRandomnessX), true));
                            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil randomness y", Mathf.Abs(weapon.parameters.hipRecoilRandomness.y), 0f, GetUpper(weapons, weaponParamType.hipRecoilRandomnessY), true));
                            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil snap", weapon.parameters.hipRecoilSnap, 0f, GetUpper(weapons, weaponParamType.hipRecoilSnap), false));
                            hipRecoilData.Add(new GraphsPlus.DataPoint("hip recoil return", weapon.parameters.hipRecoilReturn, 0f, GetUpper(weapons, weaponParamType.hipRecoilReturn), false));
                            GraphsPlus.DataSet dataset2 = new GraphsPlus.DataSet(hipRecoilData, weapon.name + " - HIP");

                            dataSets.Add(dataset1);
                            dataSets.Add(dataset2);
                        }
                        break;

                    case GraphDisplay.damage:

                        foreach (Weapon weapon in weapons)
                        {
                            List<GraphsPlus.DataPoint> damageData = new List<GraphsPlus.DataPoint>();

                            damageData.Add(new GraphsPlus.DataPoint("base damage", weapon.parameters.baseDamage, 0f, GetUpper(weapons, weaponParamType.baseDamage), false));
                            damageData.Add(new GraphsPlus.DataPoint("critical multiplier", weapon.parameters.criticalMultiplier, 0f, GetUpper(weapons, weaponParamType.criticalMultiplier), false));
                            damageData.Add(new GraphsPlus.DataPoint("effective range", weapon.parameters.effectiveRange, 0f, GetUpper(weapons, weaponParamType.effectiveRange), false));
                            damageData.Add(new GraphsPlus.DataPoint("range", weapon.parameters.range, 0f, GetUpper(weapons, weaponParamType.range), false));
                            damageData.Add(new GraphsPlus.DataPoint("fire rate", weapon.parameters.fireRate, 0f, GetUpper(weapons, weaponParamType.fireRate), false));
                            //dataSet1.Add(new GraphsPlus.DataPoint("ads speed", weapon.parameters.adsSpeed, 0f, 100f));
                            GraphsPlus.DataSet dataset1 = new GraphsPlus.DataSet(damageData, weapon.name);

                            dataSets.Add(dataset1);
                        }
                        break;

                    case GraphDisplay.ammo:
                        foreach (Weapon weapon in weapons)
                        {
                            List<GraphsPlus.DataPoint> ammoData = new List<GraphsPlus.DataPoint>();

                            ammoData.Add(new GraphsPlus.DataPoint("fire rate", weapon.parameters.fireRate, 0f, GetUpper(weapons, weaponParamType.fireRate), false));
                            ammoData.Add(new GraphsPlus.DataPoint("start ammo", weapon.parameters.startAmmo, 0f, GetUpper(weapons, weaponParamType.startAmmo), false));
                            ammoData.Add(new GraphsPlus.DataPoint("magazine size", weapon.parameters.magazineSize, 0f, GetUpper(weapons, weaponParamType.magazineSize), false));
                            ammoData.Add(new GraphsPlus.DataPoint("reload speed", weapon.parameters.reloadSpeed, 0f, GetUpper(weapons, weaponParamType.reloadSpeed), true));
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
                            allAdvancedData.Add(new GraphsPlus.DataPoint("base damage", weapon.parameters.baseDamage, 0f, GetUpper(weapons, weaponParamType.baseDamage), false));
                        }
                        else if (param == weaponParamType.criticalMultiplier)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("critical multiplier", weapon.parameters.criticalMultiplier, 0f, GetUpper(weapons, weaponParamType.criticalMultiplier), false));
                        }
                        else if (param == weaponParamType.effectiveRange)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("effective range", weapon.parameters.effectiveRange, 0f, GetUpper(weapons, weaponParamType.effectiveRange), false));
                        }
                        else if (param == weaponParamType.range)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("range", weapon.parameters.range, 0f, GetUpper(weapons, weaponParamType.range), false));
                        }
                        else if (param == weaponParamType.fireRate)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("fire rate", weapon.parameters.fireRate, 0f, GetUpper(weapons, weaponParamType.fireRate), false));
                        }
                        else if (param == weaponParamType.startAmmo)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("start ammo", weapon.parameters.startAmmo, 0f, GetUpper(weapons, weaponParamType.startAmmo), false));
                        }
                        else if (param == weaponParamType.magazineSize)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("magazine size", weapon.parameters.magazineSize, 0f, GetUpper(weapons, weaponParamType.magazineSize), false));
                        }
                        else if (param == weaponParamType.reloadSpeed)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("reload speed", weapon.parameters.reloadSpeed, 0f, GetUpper(weapons, weaponParamType.reloadSpeed), true));
                        }
                        else if (param == weaponParamType.adsSpeed)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads speed", weapon.parameters.adsSpeed, 0f, GetUpper(weapons, weaponParamType.adsSpeed), true));
                        }
                        else if (param == weaponParamType.adsRecoilX)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads recoil x", Mathf.Abs(weapon.parameters.adsRecoil.x), 0f, GetUpper(weapons, weaponParamType.adsRecoilX), false));
                        }
                        else if (param == weaponParamType.adsRecoilY)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads recoil y", Mathf.Abs(weapon.parameters.adsRecoil.y), 0f, GetUpper(weapons, weaponParamType.adsRecoilY), true));
                        }
                        else if (param == weaponParamType.adsRecoilRandomnessX)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads recoil randomness x", Mathf.Abs(weapon.parameters.adsRecoilRandomness.x), 0f, GetUpper(weapons, weaponParamType.adsRecoilRandomnessX), true));
                        }
                        else if (param == weaponParamType.adsRecoilRandomnessY)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads recoil randomness y", Mathf.Abs(weapon.parameters.adsRecoilRandomness.y), 0f, GetUpper(weapons, weaponParamType.adsRecoilRandomnessY), true));
                        }
                        else if (param == weaponParamType.adsRecoilSnap)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads recoil snap", weapon.parameters.adsRecoilSnap, 0f, GetUpper(weapons, weaponParamType.adsRecoilSnap), false));
                        }
                        else if (param == weaponParamType.adsRecoilReturn)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("ads recoil return", weapon.parameters.adsRecoilReturn, 0f, GetUpper(weapons, weaponParamType.adsRecoilReturn), false));
                        }
                        else if (param == weaponParamType.hipRecoilX)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("hip recoil x", Mathf.Abs(weapon.parameters.hipRecoil.x), 0f, GetUpper(weapons, weaponParamType.hipRecoilX), false));
                        }
                        else if (param == weaponParamType.hipRecoilY)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("hip recoil y", Mathf.Abs(weapon.parameters.hipRecoil.y), 0f, GetUpper(weapons, weaponParamType.hipRecoilY), true));
                        }
                        else if (param == weaponParamType.hipRecoilRandomnessX)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("hip recoil randomness x", Mathf.Abs(weapon.parameters.hipRecoilRandomness.x), 0f, GetUpper(weapons, weaponParamType.hipRecoilRandomnessX), true));
                        }
                        else if (param == weaponParamType.hipRecoilRandomnessY)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("hip recoil randomness y", Mathf.Abs(weapon.parameters.hipRecoilRandomness.y), 0f, GetUpper(weapons, weaponParamType.hipRecoilRandomnessY), true));
                        }
                        else if (param == weaponParamType.hipRecoilSnap)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("hip recoil snap", weapon.parameters.hipRecoilSnap, 0f, GetUpper(weapons, weaponParamType.hipRecoilSnap), false));
                        }
                        else if (param == weaponParamType.hipRecoilReturn)
                        {
                            allAdvancedData.Add(new GraphsPlus.DataPoint("hip recoil return", weapon.parameters.hipRecoilReturn, 0f, GetUpper(weapons, weaponParamType.hipRecoilReturn), false));
                        }
                    }

                    GraphsPlus.DataSet advancedDataset = new GraphsPlus.DataSet(allAdvancedData, weapon.name);

                    dataSets.Add(advancedDataset);
                }
            }

            GraphsPlus.RadarGraphAxis radarGraphAxis = new GraphsPlus.RadarGraphAxis(
                new Vector2(graphContainer.x, graphContainer.y),
                Mathf.Min(graphContainer.width, graphContainer.height) / 3,
                10, dataSets[0].dataPoints.Count);

            radarGraph = new GraphsPlus.RadarGraph(radarGraphAxis, dataSets);

            dashboardConsole.Clear();

            CheckConvexHulls(dataSets);
        }
        catch
        {

        }
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
                    dashboardConsole.Log("The convex hull of " + dataSetI.name + " fully encompasses that of " + dataSetJ.name, LogType.warning);
                }
            }
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
            else if (param == weaponParamType.recoilRandomnessX)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoilRandomness.x) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoilRandomness.x);
                }
                if (Mathf.Abs(weapon.parameters.hipRecoilRandomness.x) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoilRandomness.x);
                }
            }
            else if (param == weaponParamType.recoilRandomnessY)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoilRandomness.y) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoilRandomness.y);
                }
                if (Mathf.Abs(weapon.parameters.hipRecoilRandomness.y) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoilRandomness.y);
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
            else if (param == weaponParamType.adsRecoilRandomnessX)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoilRandomness.x) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoilRandomness.x);
                }
            }
            else if (param == weaponParamType.adsRecoilRandomnessY)
            {
                if (Mathf.Abs(weapon.parameters.adsRecoilRandomness.y) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.adsRecoilRandomness.y);
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
            else if (param == weaponParamType.hipRecoilRandomnessX)
            {
                if (Mathf.Abs(weapon.parameters.hipRecoilRandomness.x) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoilRandomness.x);
                }
            }
            else if (param == weaponParamType.hipRecoilRandomnessY)
            {
                if (Mathf.Abs(weapon.parameters.hipRecoilRandomness.y) > maxValue)
                {
                    maxValue = Mathf.Abs(weapon.parameters.hipRecoilRandomness.y);
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
        recoilRandomnessX,
        recoilRandomnessY,
        recoilSnap,
        recoilReturn,
        adsRecoilX,
        adsRecoilY,
        adsRecoilRandomnessX,
        adsRecoilRandomnessY,
        adsRecoilSnap,
        adsRecoilReturn,
        hipRecoilX,
        hipRecoilY,
        hipRecoilRandomnessX,
        hipRecoilRandomnessY,
        hipRecoilSnap,
        hipRecoilReturn,
    }
}
