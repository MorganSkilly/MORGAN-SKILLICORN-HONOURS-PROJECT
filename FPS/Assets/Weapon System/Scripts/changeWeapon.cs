using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class changeWeapon : MonoBehaviour
{
    public WeaponSystem_LogicHandler weaponSystem;
    public Weapon weapon;

    public void ChangeWeapon()
    {
        if (weaponSystem.currentWeaponSlot == WeaponSystem_LogicHandler.WeaponSlots.primary)
        {
            weaponSystem.primaryWeapon = weapon;
        }
        else
        {
            weaponSystem.secondaryWeapon = weapon;
        }

        weaponSystem.SetWeapon(true);
        weaponSystem.SetWeapon(true);

        weaponSystem.ResetAmmo();
    }
}
