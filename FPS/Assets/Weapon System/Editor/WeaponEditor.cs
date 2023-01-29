using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public enum EditorWindowStates
{
    generateNew,
    editExisting
}

public struct EditorGlobalVariables
{
    public const string weaponsFolder = "Assets/Weapon System/Scriptable Objects/";
}

public class WeaponConfigurator : EditorWindow
{
    [MenuItem("Window/Weapon System/Configure Weapon System")]
    public static void ShowConfigWindow()
    {
        GetWindow<WeaponConfigurator>("Weapon System Configurator");
    }
}

public class WeaponParameterEditor : EditorWindow
{
    public WeaponParameters parameters;
    public Weapon editingWeapon;
    public WeaponParameters editingParameters;

    [MenuItem("Window/Weapon System/Weapon Editor")]
    public static void ShowToolsWindow()
    {
        GetWindow<WeaponParameterEditor>("Weapon Editor");
    }

    EditorWindowStates editorWindowState = EditorWindowStates.editExisting;

    void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.fixedWidth = 148;

        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 50;

        var buttonStyleRed = new GUIStyle(GUI.skin.button);
        buttonStyleRed.fixedHeight = 50;
        buttonStyleRed.normal.textColor = Color.red;
        buttonStyleRed.hover.textColor = Color.red;

        var buttonStyleGreen = new GUIStyle(GUI.skin.button);
        buttonStyleGreen.fixedHeight = 50;
        buttonStyleGreen.normal.textColor = Color.green;
        buttonStyleGreen.hover.textColor = Color.green;

        if (editorWindowState == EditorWindowStates.generateNew)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate New", buttonStyleGreen))
            {
                parameters = new WeaponParameters();
                editorWindowState = EditorWindowStates.generateNew;
            }
            if (GUILayout.Button("Edit Existing", buttonStyle))
            {
                parameters = new WeaponParameters();
                editorWindowState = EditorWindowStates.editExisting;
            }
            if (GUILayout.Button("Re-configure Weapon System", buttonStyleRed))
            {
                GetWindow<WeaponConfigurator>("Weapon System Configurator");
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("\n", style);

            GenerateNewWeapon();
        }
        else if (editorWindowState == EditorWindowStates.editExisting)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate New", buttonStyle))
            {
                parameters = new WeaponParameters();
                editorWindowState = EditorWindowStates.generateNew;
            }
            if (GUILayout.Button("Edit Existing", buttonStyleGreen))
            {
                parameters = new WeaponParameters();
                editorWindowState = EditorWindowStates.editExisting;
            }
            if (GUILayout.Button("Re-configure Weapon System", buttonStyleRed))
            {
                GetWindow<WeaponConfigurator>("Weapon System Configurator");
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("\n", style);

            EditExistingWeapon();
        }

    }
    void GenerateNewWeapon()
    {
        parameters = new WeaponParameters(
            EditorGUILayout.TextField("name", parameters.name),
            EditorGUILayout.TextField("description", parameters.description),
            TextureField("thumbnail", parameters.thumbnail),
            GameObjectField("model", parameters.model),
            EditorGUILayout.Toggle("flip Y", parameters.flipY),
            EditorGUILayout.IntField("base damage", parameters.baseDamage),
            EditorGUILayout.IntField("critical multiplier", parameters.criticalMultiplier),
            EditorGUILayout.IntField("effective range", parameters.effectiveRange),
            EditorGUILayout.IntField("range", parameters.range),
            EditorGUILayout.Toggle("automatic", parameters.automatic),
            EditorGUILayout.IntField("fire rate", parameters.fireRate),
            EditorGUILayout.IntField("starting ammo", parameters.startAmmo),
            EditorGUILayout.IntField("magazine size", parameters.magazineSize),
            EditorGUILayout.FloatField("reload speed", parameters.reloadSpeed),
            EditorGUILayout.FloatField("ads speed", parameters.adsSpeed),
            AudioClipField("shot", parameters.shotSound),
            AudioClipField("empty Shot", parameters.emptyShotSound),
            AudioClipField("reload", parameters.reloadSound),
            EditorGUILayout.Vector3Field("barrel exit position", parameters.barrelExit),
            GameObjectField("muzzle flash effect", parameters.muzzleFlash),
            GameObjectField("impact shot effect", parameters.impactShot),
            AnimationClipField("reload animation", parameters.reloadAnim)
            );

        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.fixedWidth = 148;
        GUILayout.Label("\n", style);
        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 50;

        if (GUILayout.Button("Generate New Weapon Prefab", buttonStyle))
        {
            Debug.Log(parameters.name);

            Weapon newWeapon = CreateInstance<Weapon>();
            newWeapon.parameters = parameters;

            AssetDatabase.CreateAsset(newWeapon, EditorGlobalVariables.weaponsFolder + parameters.name + ".asset");
            EditorUtility.SetDirty(newWeapon);
            Debug.Log("Generated new weapon: " + newWeapon.name);
        }
    }

    void EditExistingWeapon()
    {
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.normal.textColor = Color.red;
        style.hover.textColor = Color.red;

        var buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fixedHeight = 50;

        var buttonStyleRed = new GUIStyle(GUI.skin.button);
        buttonStyleRed.fixedHeight = 50;
        buttonStyleRed.normal.textColor = Color.red;
        buttonStyleRed.hover.textColor = Color.red;

        

        if (editingWeapon != null)
        {
            editingWeapon = WeaponField("WEAPON TO EDIT", editingWeapon);
            GUILayout.Label("IMPORTANT: Do not reset this value to null or the editor will crash!", style);
            GUILayout.Label("\n\n", style);

            EditorGUILayout.TextField("name", editingWeapon.name);
            editingWeapon.parameters.description = EditorGUILayout.TextField("description", editingWeapon.parameters.description);
            editingWeapon.parameters.thumbnail = TextureField("thumbnail", editingWeapon.parameters.thumbnail);
            editingWeapon.parameters.model = GameObjectField("model", editingWeapon.parameters.model);
            editingWeapon.parameters.flipY = EditorGUILayout.Toggle("flip Y", editingWeapon.parameters.flipY);
            editingWeapon.parameters.baseDamage = EditorGUILayout.IntField("base damage", editingWeapon.parameters.baseDamage);
            editingWeapon.parameters.criticalMultiplier = EditorGUILayout.IntField("critical multiplier", editingWeapon.parameters.criticalMultiplier);
            editingWeapon.parameters.effectiveRange = EditorGUILayout.IntField("effective range", editingWeapon.parameters.effectiveRange);
            editingWeapon.parameters.range = EditorGUILayout.IntField("range", editingWeapon.parameters.range);
            editingWeapon.parameters.automatic = EditorGUILayout.Toggle("automatic", editingWeapon.parameters.automatic);
            editingWeapon.parameters.fireRate = EditorGUILayout.IntField("fire rate", editingWeapon.parameters.fireRate);
            editingWeapon.parameters.startAmmo = EditorGUILayout.IntField("starting ammo", editingWeapon.parameters.startAmmo);
            editingWeapon.parameters.magazineSize = EditorGUILayout.IntField("magazine size", editingWeapon.parameters.magazineSize);
            editingWeapon.parameters.reloadSpeed = EditorGUILayout.FloatField("reload speed", editingWeapon.parameters.reloadSpeed);
            editingWeapon.parameters.adsSpeed = EditorGUILayout.FloatField("ads speed", editingWeapon.parameters.adsSpeed);
            editingWeapon.parameters.shotSound = AudioClipField("shot", editingWeapon.parameters.shotSound);
            editingWeapon.parameters.emptyShotSound = AudioClipField("empty Shot", editingWeapon.parameters.emptyShotSound);
            editingWeapon.parameters.reloadSound = AudioClipField("reload", editingWeapon.parameters.reloadSound);
            editingWeapon.parameters.barrelExit = EditorGUILayout.Vector3Field("barrel exit position", editingWeapon.parameters.barrelExit);
            editingWeapon.parameters.muzzleFlash = GameObjectField("muzzle flash effect", editingWeapon.parameters.muzzleFlash);
            editingWeapon.parameters.impactShot = GameObjectField("impact shot effect", editingWeapon.parameters.impactShot);
            editingWeapon.parameters.reloadAnim = AnimationClipField("reload animation", editingWeapon.parameters.reloadAnim);

            GUILayout.Label("\n\n", style);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save Edit", buttonStyle))
            {
                EditorUtility.SetDirty(editingWeapon);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Edits saved");
            }
            if (GUILayout.Button("Delete Weapon", buttonStyleRed))
            {
                if (AssetDatabase.DeleteAsset(EditorGlobalVariables.weaponsFolder + editingWeapon.name + ".asset"))
                {
                    Debug.Log("Deleted asset successfully");
                }
                else
                {
                    Debug.LogWarning("There was a problem deleting the asset");
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("\n", style);
        }
        else if (editingWeapon == null)
        {
            editingWeapon = WeaponField("WEAPON TO EDIT", null);
            GUILayout.Label("IMPORTANT: Do not reset this value to null or the editor will crash!", style);
            GUILayout.Label("\n\n", style);
        }

        GUILayout.Label("\n", style);
    }

    private static AudioClip AudioClipField(string name, AudioClip audioClip)
    {
        GUILayout.BeginHorizontal();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.fixedWidth = 148;
        GUILayout.Label(name, style);
        var result = (AudioClip)EditorGUILayout.ObjectField(audioClip, typeof(AudioClip), false);
        GUILayout.EndHorizontal();
        return result;
    }

    private static Texture TextureField(string name, Texture texture)
    {
        GUILayout.BeginHorizontal();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.fixedWidth = 148;
        GUILayout.Label(name, style);
        var result = (Texture)EditorGUILayout.ObjectField(texture, typeof(Texture), false);
        GUILayout.EndVertical();
        return result;
    }

    private static AnimationClip AnimationClipField(string name, AnimationClip animationClip)
    {
        GUILayout.BeginHorizontal();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.fixedWidth = 148;
        GUILayout.Label(name, style);
        var result = (AnimationClip)EditorGUILayout.ObjectField(animationClip, typeof(AnimationClip), false);
        GUILayout.EndVertical();
        return result;
    }

    private static GameObject GameObjectField(string name, GameObject gameObject)
    {
        GUILayout.BeginHorizontal();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperLeft;
        style.fixedWidth = 148;
        GUILayout.Label(name, style);
        var result = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), false);
        GUILayout.EndVertical();
        return result;
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
