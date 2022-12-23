using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageReciever : MonoBehaviour
{
    public DamageManager damageObject;

    public void RecieveHit(float damage)
    {
        damageObject.ModifyHealth(-damage);
    }
}
