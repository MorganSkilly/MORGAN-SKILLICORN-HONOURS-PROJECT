using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponFiringSystem : MonoBehaviour
{
    public WeaponActionsMap weaponActionsMap;

    public GameObject raycastOrigin;
    public ActiveWeaponManager currentWeapon;

    private Vector3 startPos;
    private Vector3 adsPos;

    private bool ads;
    private bool firing;

    // Start is called before the first frame update
    void Start()
    {
        currentWeapon = GetComponent<ActiveWeaponManager>();

        startPos = transform.localPosition;
        adsPos = startPos;
        adsPos.x = 0;
        adsPos.y = 2;
        adsPos.z = 0.5f;
    }

    private void Awake()
    {
        weaponActionsMap = new WeaponActionsMap();

        weaponActionsMap.WeaponSystem.Fire.started += Fire;
        weaponActionsMap.WeaponSystem.Fire.canceled += StopFire;
        weaponActionsMap.WeaponSystem.Aim.started += Aim;
        weaponActionsMap.WeaponSystem.Aim.canceled += StopAim;
    }

    private void FixedUpdate()
    {
        currentWeapon = GetComponent<ActiveWeaponManager>();

        if (firing)
        {
            if (currentWeapon.GetCurrentWeapon().automatic)
            {
                FireRaycast();
            }
            else
            {
                FireRaycast();

                firing = false;
            }
        }
        else
        {
            firing = false;
        }
    }

    private void FireRaycast()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward) * hit.distance, Color.red);
            Debug.Log("Hit: " + hit.transform.name);

            if (hit.transform.gameObject.GetComponent<WeaponTarget>())
                hit.transform.gameObject.GetComponent<WeaponTarget>().health -= currentWeapon.GetCurrentWeapon().baseDamage;
        }
        else
        {
            Debug.DrawRay(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward) * 1000, Color.blue);
        }
    }

    private void Aim(InputAction.CallbackContext context)
    {
        Debug.Log("AIM" + System.DateTime.Now.Second.ToString() + System.DateTime.Now.Millisecond.ToString());
        ads = true;
    }
    private void StopAim(InputAction.CallbackContext context)
    {
        Debug.Log("STOP AIM" + System.DateTime.Now.Second.ToString() + System.DateTime.Now.Millisecond.ToString());
        ads = false;
    }

    private void Fire(InputAction.CallbackContext context)
    {
        Debug.Log("fire");
        firing = true;
    }

    private void StopFire(InputAction.CallbackContext context)
    {
        Debug.Log("stop fire");
        firing = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (ads)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, adsPos, (5f * currentWeapon.GetCurrentWeapon().AdsSpeed) * Time.deltaTime);
        }
        else
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, startPos, (5f * currentWeapon.GetCurrentWeapon().AdsSpeed) * Time.deltaTime);
        }

        //Debug.DrawRay(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
    }

    private void OnEnable()
    {
        weaponActionsMap.WeaponSystem.Enable();
    }
    private void OnDisable()
    {
        weaponActionsMap.WeaponSystem.Disable();
    }
}
