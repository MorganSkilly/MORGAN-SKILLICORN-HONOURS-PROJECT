using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    public GameObject dummyPrefab;
    private List<GameObject> dummyInstances = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i <= 5; i++)
        {
            BoxCollider spawnZone = GetComponent<BoxCollider>();
            Vector3 spawnPos = new Vector3(
                    Random.Range(spawnZone.center.x - (spawnZone.size.x / 2), spawnZone.center.x + (spawnZone.size.x / 2)),
                    0,
                    Random.Range(spawnZone.center.z - (spawnZone.size.z / 2), spawnZone.center.z + (spawnZone.size.z / 2)));
            dummyInstances.Add(Instantiate(dummyPrefab));
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject dummy in dummyInstances)
        {
            if (dummy.GetComponent<WeaponSystem_DamageManager>().destroy)
            {
                BoxCollider spawnZone = GetComponent<BoxCollider>();

                dummy.transform.position = new Vector3(
                    Random.Range(spawnZone.center.x - (spawnZone.size.x / 2), spawnZone.center.x + (spawnZone.size.x / 2)),
                    0,
                    Random.Range(spawnZone.center.z - (spawnZone.size.z / 2), spawnZone.center.z + (spawnZone.size.z / 2)));

                dummy.GetComponent<WeaponSystem_DamageManager>().destroy = false;
                dummy.GetComponent<WeaponSystem_DamageManager>().Start();
            }
        }
    }
}
