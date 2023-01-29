using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct WeaponParameters
{
    //weapon name
    public string name;

    //weapon description
    public string description;

    //weapon thumbnail (for UI)
    public Texture thumbnail;

    //3D textured weapon model prefab
    public GameObject model;

    //corrects object that is flipped 180 degrees
    public bool flipY;

    //damage and range values
    public int baseDamage;
    public int criticalMultiplier;
    public int effectiveRange;
    public int range;

    //fire rate values
    public bool automatic;
    public int fireRate;

    //ammo system values
    public int startAmmo;
    public int magazineSize;

    //animation speed values
    public float reloadSpeed;
    public float adsSpeed;

    //audio clips
    public AudioClip shotSound, emptyShotSound, reloadSound;

    //particle effect root position (end of barrel)
    public Vector3 barrelExit;

    //particle effect objects
    public GameObject muzzleFlash, impactShot;

    //particle effect objects
    public AnimationClip reloadAnim;

    public WeaponParameters(
        string name,

        string description,

        Texture thumbnail,

        GameObject model,

        bool flipY,

        int baseDamage,
        int criticalMultiplier,
        int effectiveRange,
        int range,

        bool automatic,
        int fireRate,

        int startAmmo,
        int magazineSize,

        float reloadSpeed,
        float adsSpeed,

        AudioClip shotSound,
        AudioClip emptyShotSound,
        AudioClip reloadSound,

        Vector3 barrelExit,

        GameObject muzzleFlash,
        GameObject impactShot,

        AnimationClip reloadAnim
        )
    {
        this.name = name;

        this.description = description;

        this.thumbnail = thumbnail;

        this.model = model;

        this.flipY = flipY;

        this.baseDamage = baseDamage;
        this.criticalMultiplier = criticalMultiplier;
        this.effectiveRange = effectiveRange;
        this.range = range;

        this.automatic = automatic;
        this.fireRate = fireRate;

        this.startAmmo = startAmmo;
        this.magazineSize = magazineSize;

        this.reloadSpeed = reloadSpeed;
        this.adsSpeed = adsSpeed;

        this.shotSound = shotSound;
        this.emptyShotSound = emptyShotSound;
        this.reloadSound = reloadSound;

        this.barrelExit = barrelExit;

        this.muzzleFlash = muzzleFlash;
        this.impactShot = impactShot;

        this.reloadAnim = reloadAnim;
    }
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon System/Create New Profile")]
public class Weapon : ScriptableObject
{
    public WeaponParameters parameters;
}



