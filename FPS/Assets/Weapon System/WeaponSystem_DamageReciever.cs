using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// VERY RUDIMENTARY TO BE WORKED ON FURTHER

public class WeaponSystem_DamageReciever : MonoBehaviour
{
    public WeaponSystem_DamageManager damageObject;

    public void RecieveHit(float damage)
    {
        damageObject.ModifyHealth(-damage);
    }
}
