using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeaponEditor : EditorWindow
{
    [Tooltip("visit www.Morgan.Games for more!")]
    public Texture aTexture;

    public GameObject baseWeaponPrefab;
    public string weaponsFolder = "Assets/Weapon System/Scriptable Objects/";

    //Weapon parameters
    new string name = "new gun";
    string description = "this is a new gun and has not yet been assigned a description";
    Texture thumbnail;
    GameObject model;
    bool flipY = true;
    int baseDamage = 10;
    int criticalMultiplier = 10;
    bool automatic = true;
    int startAmmo = 100;
    int magazineSize = 30;
    int reloadSpeed = 5000;
    int fireRate = 100;
    int effectiveRange = 10;
    int range = 100;
    float adsSpeed = 0.5f;

    private string UIspacer = "                                                                                                                                                                                                                                                                                                                                                                             ";


    [MenuItem("Window/Weapon System/Weapon Editor")]
    public static void ShowWindow()
    {
        GetWindow<WeaponEditor>("Weapon Parameter Editor");
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(10, 10, 320, 64), aTexture, ScaleMode.StretchToFill, true, 10.0F);
        GUILayout.Label("\n\n\n\n\n\nThis is the weapon editor window. Here you can edit the parameters of weapon objects to manually balance them.", EditorStyles.wordWrappedLabel);

        GenerateNewWeapon();
    }

    void GenerateNewWeapon()
    {
        name = EditorGUILayout.TextField("name", name);
        description = EditorGUILayout.TextField("description", description);
        baseDamage = EditorGUILayout.IntField("base damage", baseDamage);
        criticalMultiplier = EditorGUILayout.IntField("critical multiplier", criticalMultiplier);
        startAmmo = EditorGUILayout.IntField("starting ammo", startAmmo);
        magazineSize = EditorGUILayout.IntField("magazine size", magazineSize);
        reloadSpeed = EditorGUILayout.IntField("reload speed", reloadSpeed);
        fireRate = EditorGUILayout.IntField("fire rate", fireRate);
        effectiveRange = EditorGUILayout.IntField("effective range", effectiveRange);
        range = EditorGUILayout.IntField("range", range);
        adsSpeed = EditorGUILayout.FloatField("ads speed", adsSpeed);
        automatic = EditorGUILayout.Toggle("automatic", automatic);


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(UIspacer, EditorStyles.wordWrappedLabel);
        thumbnail = TextureField("thumbnail", thumbnail);
        model = GameObjectField("model", model);
        flipY = EditorGUILayout.Toggle("flipY", flipY);
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("\n", EditorStyles.wordWrappedLabel);

        if (GUILayout.Button("Generate New Weapon Prefab"))
        {
            Weapon newWeapon = new Weapon();

            if (thumbnail != null)
                newWeapon.thumbnail = thumbnail;
            if (model != null)
                newWeapon.model = model;
            newWeapon.flipY = flipY;
            newWeapon.name = name;
            newWeapon.description = description;
            newWeapon.baseDamage = baseDamage;
            newWeapon.criticalMultiplier = criticalMultiplier;
            newWeapon.automatic = automatic;
            newWeapon.magazineSize = magazineSize;
            newWeapon.reloadSpeed = reloadSpeed;
            newWeapon.FireRate = fireRate;
            newWeapon.EffectiveRange = effectiveRange;
            newWeapon.Range = range;
            newWeapon.AdsSpeed = adsSpeed;

            AssetDatabase.CreateAsset(newWeapon, weaponsFolder + name + ".asset");
            Debug.Log("Generated new weapon");
        }
    }

    private static Texture TextureField(string name, Texture texture)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperCenter;
        style.fixedWidth = 70;
        GUILayout.Label(name, style);
        var result = (Texture)EditorGUILayout.ObjectField(texture, typeof(Texture), false, GUILayout.Width(70), GUILayout.Height(70));
        GUILayout.EndVertical();
        return result;
    }

    private static GameObject GameObjectField(string name, GameObject gameObject)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperCenter;
        style.fixedWidth = 70;
        GUILayout.Label(name, style);
        var result = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), false, GUILayout.Width(70), GUILayout.Height(70));
        GUILayout.EndVertical();
        return result;
    }
}
