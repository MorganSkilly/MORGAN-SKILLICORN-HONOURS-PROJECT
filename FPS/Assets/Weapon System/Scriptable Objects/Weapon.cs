using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon System/Create New")]
public class Weapon : ScriptableObject
{
    [Tooltip("Actual descriptive name of the weapon.")]
    public new string name;

    [Tooltip("Description of the weapon that can be referenced in the game UI.")]
    public string description;

    [Tooltip("The base damage from a single round of the weapon before any multipliers are applied.")]
    public int baseDamage;

    [Tooltip("If true the trigger can be held for automatic firing. For single shot firing like DMRs and snipers select false.")]
    public bool automatic;

    [Tooltip("How many rounds are contained in a single magazine. For single fire weapons like RPGs and shell loaded shotguns select 1.")]
    public int magazineSize;
}
