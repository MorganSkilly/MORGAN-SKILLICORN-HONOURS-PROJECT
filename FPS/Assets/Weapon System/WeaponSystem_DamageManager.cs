using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// VERY RUDIMENTARY TO BE WORKED ON FURTHER

public class WeaponSystem_DamageManager : MonoBehaviour
{
    public float startHealth = 100f;
    public float health;

    // Start is called before the first frame update
    void Start()
    {
        health = startHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ModifyHealth(float valueToModify)
    {
        health += valueToModify;

        if (valueToModify < 0)
            Debug.Log("PLAYER DAMAGED " + gameObject.name.ToUpper() + " FOR " + valueToModify + " HP. " + gameObject.name.ToUpper() + " NOW HAS " + health + "/" + startHealth + " HP");
        else
            Debug.Log("PLAYER HEALED " + gameObject.name.ToUpper() + " FOR " + valueToModify + " HP");
    }
}
