using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUIExample : MonoBehaviour
{
    [SerializeField] private Transform m_ContentContainer;
    [SerializeField] private GameObject m_ItemPrefab;
    [SerializeField] private int m_ItemsToGenerate;
    public WeaponSystem_LogicHandler weaponSystem;

    void Start()
    {
        List<Weapon> weapons = new List<Weapon>();

        string[] assetNames = AssetDatabase.FindAssets("t:Weapon");

        foreach (string SOName in assetNames)
        {
            var SOpath = AssetDatabase.GUIDToAssetPath(SOName);
            var character = AssetDatabase.LoadAssetAtPath<Weapon>(SOpath);

            var button = Instantiate(m_ItemPrefab);
            // do something with the instantiated item -- for instance
            //item_go.GetComponentInChildren<Text>().text = "Item #" + i;
            //item_go.GetComponent<Image>().color = i % 2 == 0 ? Color.yellow : Color.cyan;
            //parent the item to the content container
            button.transform.SetParent(m_ContentContainer);
            //reset the item's scale -- this can get munged with UI prefabs
            button.transform.localScale = Vector2.one;

            button.GetComponent<changeWeapon>().weaponSystem = weaponSystem;
            button.GetComponent<changeWeapon>().weapon = character;
            button.GetComponentInChildren<TMP_Text>().text = character.parameters.name;
        }
    }
}
