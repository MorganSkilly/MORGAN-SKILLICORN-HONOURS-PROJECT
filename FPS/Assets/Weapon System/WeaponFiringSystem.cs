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

    public GameObject targetPos;
    private Vector3 startTargetPos;

    public AudioClip shot;
    public AudioClip magEmpty;
    public AudioClip reload;
    AudioSource audioSource;

    public GameObject particleEffect;

    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        currentWeapon = GetComponent<ActiveWeaponManager>();
        audioSource = GetComponent<AudioSource>();

        startPos = transform.localPosition;
        adsPos = startPos;
        adsPos.x = 0;
        adsPos.y = 2;
        adsPos.z = 0.5f;
        startTargetPos = targetPos.transform.localPosition;
    }

    private void Awake()
    {
        weaponActionsMap = new WeaponActionsMap();

        weaponActionsMap.WeaponSystem.Fire.started += Fire;
        weaponActionsMap.WeaponSystem.Fire.canceled += StopFire;
        weaponActionsMap.WeaponSystem.Aim.started += Aim;
        weaponActionsMap.WeaponSystem.Aim.canceled += StopAim;
        weaponActionsMap.WeaponSystem.Reload.started += Reload;
    }

    private void FixedUpdate()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 2;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;

        if (Physics.Raycast(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);

            targetPos.transform.position = hit.point;
        }
        else
        {
            targetPos.transform.localPosition = startTargetPos;
        }

        currentWeapon = GetComponent<ActiveWeaponManager>();

        timer += Time.deltaTime;

        float fireRate = currentWeapon.GetCurrentWeapon().FireRate / 60;
        fireRate = 1 / fireRate;

        if (timer > fireRate)
        {
            timer = timer - fireRate;

            if (!firing)
            {
                firing = false;
            }
            else
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
        }
    }

    private void FireRaycast()
    {
        bool hasAmmo;
        if (currentWeapon.isPrimary && currentWeapon.primaryCurrentMag > 0)
            hasAmmo = true;
        else if (!currentWeapon.isPrimary && currentWeapon.secondaryCurrentMag > 0)
            hasAmmo = true;
        else
            hasAmmo = false;

        if (hasAmmo)
        {
            // Bit shift the index of the layer (8) to get a bit mask
            int layerMask = 1 << 2;

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

                if (hit.transform.gameObject.GetComponent<Rigidbody>())
                {
                    hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(raycastOrigin.transform.TransformDirection(Vector3.forward) * 10f, ForceMode.Impulse);
                }

                GameObject impact = Instantiate(particleEffect);
                impact.transform.position = hit.point;
                impact.transform.Rotate(raycastOrigin.transform.TransformDirection(Vector3.back));
            }
            else
            {
                Debug.DrawRay(raycastOrigin.transform.position, raycastOrigin.transform.TransformDirection(Vector3.forward) * 1000, Color.blue);
            }

            audioSource.PlayOneShot(shot, 0.5f);

            if (currentWeapon.isPrimary)
                currentWeapon.primaryCurrentMag--;
            else if (!currentWeapon.isPrimary)
                currentWeapon.secondaryCurrentMag--;
        }
        else
        {
            audioSource.PlayOneShot(magEmpty, 1f);
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

    private void Reload(InputAction.CallbackContext context)
    {
        Debug.Log("reload");

        if (currentWeapon.isPrimary)
        {
            currentWeapon.primaryAmmo += currentWeapon.primaryCurrentMag;
            currentWeapon.primaryCurrentMag = currentWeapon.primary.magazineSize;
            currentWeapon.primaryAmmo -= currentWeapon.primary.magazineSize;

            if (currentWeapon.primaryAmmo < 0)
            {
                currentWeapon.primaryCurrentMag += currentWeapon.primaryAmmo;
                currentWeapon.primaryAmmo = 0;
            }
        }
        else if (!currentWeapon.isPrimary)
        {
            currentWeapon.secondaryAmmo += currentWeapon.secondaryCurrentMag;
            currentWeapon.secondaryCurrentMag = currentWeapon.secondary.magazineSize;
            currentWeapon.secondaryAmmo -= currentWeapon.secondary.magazineSize;

            if (currentWeapon.secondaryAmmo < 0)
            {
                currentWeapon.secondaryCurrentMag += currentWeapon.secondaryAmmo;
                currentWeapon.secondaryAmmo = 0;
            }
        }


        audioSource.PlayOneShot(reload, 0.5f);
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
