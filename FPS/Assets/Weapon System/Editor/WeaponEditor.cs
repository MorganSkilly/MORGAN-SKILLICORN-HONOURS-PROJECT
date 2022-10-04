using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WeaponEditor : EditorWindow
{
    [Tooltip("visit www.Morgan.Games for more!")]
    public Texture aTexture;

    [MenuItem("Window/Weapon System/Weapon Editor")]
    public static void ShowWindow()
    {
        GetWindow<WeaponEditor>("Weapon Parameter Editor");
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(10, 10, 320, 64), aTexture, ScaleMode.StretchToFill, true, 10.0F);
        GUILayout.Label("\n\n\n\n\n\nThis is the weapon editor window. Here you can edit the parameters of weapon objects to manually balance them.", EditorStyles.wordWrappedLabel);
    
        
    }
}
