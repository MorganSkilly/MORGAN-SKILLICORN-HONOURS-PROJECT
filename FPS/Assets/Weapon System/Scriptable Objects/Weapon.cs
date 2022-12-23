using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon System/Create New Profile")]
public class Weapon : ScriptableObject
{
    [Tooltip("Actual descriptive name of the weapon.")]
    public new string name = "new gun";

    [Tooltip("Description of the weapon that can be referenced in the game UI.")]
    public string description = "this is a new gun and has not yet been assigned a description";

    [Tooltip("The weapon icon that can be used in areas of the UI.")]
    public Texture thumbnail;

    [Tooltip("The weapon model to be used in game.")]
    public GameObject model;

    [Tooltip("Flip the Y rotation of the gun model by 180 degrees.")]
    public bool flipY;

    [Tooltip("The base damage from a single round of the weapon before any multipliers are applied.")]
    public int baseDamage = 10;

    [Tooltip("Multiplies the damage of base damage when a shot hits an area on a target tagged as critical.")]
    public int criticalMultiplier = 10;

    [Tooltip("If true the trigger can be held for automatic firing. For single shot firing like DMRs and snipers select false.")]
    public bool automatic = true;

    [Tooltip("The ammount of ammo to initialise this gun with.")]
    public int startAmmo = 100;

    [Tooltip("How many rounds are contained in a single magazine. For single fire weapons like RPGs and shell loaded shotguns select 1.")]
    public int magazineSize = 30;

    [Tooltip("(Milliseconds) How long it takes to reload, the reload animation is minipulated automatically to account for this parameter.")] //do this in animator speed to figure out speed
    public int reloadSpeed = 5000;
    
    [Tooltip("(Milliseconds) Delay after firing a shot before the next shot can be triggered.")]
    public int FireRate = 100;

    [Tooltip("Distance from raycast until damage starts to fall off.")]
    public int EffectiveRange = 10;

    [Tooltip("Distance from raycast where damage reaches 0, damage will be reduced linearly from the last effective range down to this point.")]
    public int Range = 100;

    [Tooltip("Speed of aiming down sights.")]
    public float AdsSpeed = 0.5f;

    [Tooltip("Sound clips for weapon.")]
    public AudioClip shot, emptyShot, reload;

    [Tooltip("Position of the end of the barrel.")]
    public Vector3 barrelExit;

    [Tooltip("Particle effects for shots.")]
    public GameObject muzzleFlash, impactShot;
}

