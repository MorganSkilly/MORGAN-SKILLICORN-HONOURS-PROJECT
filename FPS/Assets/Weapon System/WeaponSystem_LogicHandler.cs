using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem_LogicHandler : MonoBehaviour
{
    public bool showDebugUI = true;

    #region get/sets
    public void UpdatePrimaryWeapon(Weapon newWeapon)
    {
        primaryWeapon = newWeapon;
    }

    public void UpdateSecondaryWeapon(Weapon newWeapon)
    {
        secondaryWeapon = newWeapon;
    }
    #endregion

    #region class vars
    private GameObject weaponObject;

    public Weapon primaryWeapon, secondaryWeapon;
    private Weapon currentWeapon;

    public WeaponSlots currentWeaponSlot;
    public enum WeaponSlots
    {
        primary,
        secondary
    }

    private Ammo ammo;

    public struct Ammo
    {
        public int primaryAmmo;
        public int secondaryAmmo;
        public int primaryCurrentMag;
        public int secondaryCurrentMag;
    }

    private Animator animator;
    private AnimatorOverrideController animatorOverrideController;
    private AnimationClipOverrides clipOverrides;

    private Vector3 hipfirePosition, aimingPosition;
    private bool isAds;
    private GameObject lookAt;

    Vector3 weaponLookAt;

    private float shootTimer = 0.0f;
    private float reloadTimer = 0.0f;

    private bool firing;

    private GameObject muzzleFlashContainer;

    private bool triggeredReload = false, stillReloading = false;

    private Camera fpsCamera;

    private Vector3 currentRecoilRotation, targetRecoilRotation;
    private Vector3 currentCameraRotation, targetCameraRotation;

    #endregion

    void Start()
    {
        fpsCamera = Camera.main;
        lookAt = new GameObject("look at");
        lookAt.transform.parent = fpsCamera.transform;
        lookAt.transform.localPosition = new Vector3(0, 0, 20);

        SetWeapon(true, WeaponSlots.primary);

        hipfirePosition = transform.localPosition;
        aimingPosition = hipfirePosition;
        aimingPosition.x = 0;
        aimingPosition.y = 2;
        aimingPosition.z = 0.5f;

        ResetAmmo();

        weaponLookAt = fpsCamera.transform.position;
        weaponLookAt.z += 1000;
        RotateTowards(weaponLookAt);
    }

    void Update()
    {      
        if (isAds)
        {
            targetRecoilRotation = Vector3.Lerp(targetRecoilRotation, Vector3.zero, currentWeapon.parameters.adsRecoilReturn * Time.deltaTime);
            currentRecoilRotation = Vector3.Slerp(currentRecoilRotation, targetRecoilRotation, currentWeapon.parameters.adsRecoilSnap * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Euler(currentRecoilRotation);

            targetCameraRotation = Vector3.Lerp(targetCameraRotation, Vector3.zero, currentWeapon.parameters.adsRecoilReturn * Time.deltaTime);
            currentCameraRotation = Vector3.Slerp(currentCameraRotation, targetCameraRotation, currentWeapon.parameters.adsRecoilSnap * Time.fixedDeltaTime);
            fpsCamera.transform.localRotation = Quaternion.Euler(currentCameraRotation);
        }
        else
        {
            targetRecoilRotation = Vector3.Lerp(targetRecoilRotation, Vector3.zero, currentWeapon.parameters.hipRecoilReturn * Time.deltaTime);
            currentRecoilRotation = Vector3.Slerp(currentRecoilRotation, targetRecoilRotation, currentWeapon.parameters.hipRecoilSnap * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Euler(currentRecoilRotation);

            targetCameraRotation = Vector3.Lerp(targetCameraRotation, Vector3.zero, currentWeapon.parameters.hipRecoilReturn * Time.deltaTime);
            currentCameraRotation = Vector3.Slerp(currentCameraRotation, targetCameraRotation, currentWeapon.parameters.hipRecoilSnap * Time.fixedDeltaTime);
            fpsCamera.transform.localRotation = Quaternion.Euler(currentCameraRotation);
        }

        int layerMask = 1 << 2;
        layerMask = ~layerMask;

        RaycastHit hit;

        if (Physics.Raycast(fpsCamera.transform.position,
            fpsCamera.transform.TransformDirection(Vector3.forward),
            out hit, Mathf.Infinity, layerMask))
        {
            if (!firing)
            {
                Debug.DrawRay(fpsCamera.transform.position,
                    fpsCamera.transform.TransformDirection(Vector3.forward) * hit.distance,
                    Color.green);
            }

            weaponLookAt = hit.point;
        }
        else
        {
            weaponLookAt.z = fpsCamera.transform.position.z + 1000;
        }

        Aim();
        RotateTowards(weaponLookAt);


        #region reloadupdate
        bool reloadingAnim = GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Reload");

        if (triggeredReload && !reloadingAnim)
        {
            stillReloading = true;
            triggeredReload = false;
            GetComponentInChildren<Animator>().SetBool("Reloading", true);
        }
        else if (reloadingAnim)
        {
            stillReloading = true;
            GetComponentInChildren<Animator>().SetBool("Reloading", false);
        }
        else if (!reloadingAnim && stillReloading)
        {
            stillReloading = false;
        }
        else if (stillReloading)
        {
            reloadTimer += Time.deltaTime;
            GetComponentInChildren<Animator>().speed = currentWeapon.parameters.reloadSpeed / reloadTimer;
        }
        #endregion
    }

    private void FixedUpdate()
    {
        int layerMask = 1 << 2;
        layerMask = ~layerMask;

        RaycastHit hit;

        #region firing update
        if (firing)
        {
            if (Physics.Raycast(fpsCamera.transform.position,
                    fpsCamera.transform.TransformDirection(Vector3.forward),
                    out hit, Mathf.Infinity, layerMask))
            {
                shootTimer += Time.deltaTime;

                float fireRate = currentWeapon.parameters.fireRate / 60;
                fireRate = 1 / fireRate;

                if (shootTimer > fireRate)
                {
                    shootTimer = shootTimer - fireRate;

                    if (HasAmmo() && !stillReloading)
                    {
                        if (currentWeapon.parameters.automatic)
                        {
                            Debug.DrawRay(fpsCamera.transform.position,
                                fpsCamera.transform.TransformDirection(Vector3.forward) * hit.distance,
                                Color.red);

                            RegisterShot();
                        }
                        else
                        {
                            Debug.DrawRay(fpsCamera.transform.position,
                                fpsCamera.transform.TransformDirection(Vector3.forward) * hit.distance,
                                Color.red);

                            RegisterShot();

                            firing = false;
                        }

                        if (currentWeaponSlot == WeaponSystem_LogicHandler.WeaponSlots.primary)
                        {
                            ammo.primaryCurrentMag--;
                        }
                        else if (currentWeaponSlot == WeaponSystem_LogicHandler.WeaponSlots.secondary)
                        {
                            ammo.secondaryCurrentMag--;
                        }

                        if (muzzleFlashContainer == null)
                        {
                            muzzleFlashContainer = Instantiate(currentWeapon.parameters.muzzleFlash, transform);
                            muzzleFlashContainer.transform.localPosition = currentWeapon.parameters.barrelExit;
                        }

                        muzzleFlashContainer.GetComponent<ParticleSystem>().Play();
                    }
                    else
                    {
                        if (currentWeapon.parameters.automatic)
                        {
                            Debug.DrawRay(fpsCamera.transform.position,
                                fpsCamera.transform.TransformDirection(Vector3.forward) * hit.distance,
                                Color.yellow);

                            GetComponent<AudioSource>().PlayOneShot(currentWeapon.parameters.emptyShotSound, 0.5f);
                        }
                        else
                        {
                            Debug.DrawRay(fpsCamera.transform.position,
                                fpsCamera.transform.TransformDirection(Vector3.forward) * hit.distance,
                                Color.yellow);

                            GetComponent<AudioSource>().PlayOneShot(currentWeapon.parameters.emptyShotSound, 0.5f);

                            firing = false;
                        }
                    }

                    void RegisterShot()
                    {
                        if (!stillReloading)
                        {
                            GetComponent<AudioSource>().PlayOneShot(currentWeapon.parameters.shotSound, 0.5f);

                            if (hit.transform.gameObject.GetComponent<Rigidbody>())
                            {
                                GameObject impact = Instantiate(currentWeapon.parameters.impactShot);
                                impact.transform.position = hit.point;
                                impact.transform.Rotate(fpsCamera.transform.TransformDirection(Vector3.back));
                            }

                            try
                            {
                                hit.transform.gameObject.GetComponent<WeaponSystem_DamageReciever>().RecieveHit(currentWeapon.parameters.baseDamage);
                                weaponSystemDebugInfo.Add("[HIT: " + hit.transform.name + "]");
                            }
                            catch (Exception)
                            {
                                weaponSystemDebugInfo.Add("[HIT: invalid object]");
                            }

                            if (isAds)
                            {
                                Vector3 gunRecoil = currentWeapon.parameters.adsRecoil;
                                Vector3 generatedRecoil = new Vector3(gunRecoil.x, UnityEngine.Random.Range(gunRecoil.y * -currentWeapon.parameters.adsRecoilRandomness, gunRecoil.y * currentWeapon.parameters.adsRecoilRandomness), UnityEngine.Random.Range(gunRecoil.z * -currentWeapon.parameters.adsRecoilRandomness, gunRecoil.z * currentWeapon.parameters.adsRecoilRandomness));
                                targetRecoilRotation += generatedRecoil;
                                targetCameraRotation += generatedRecoil;
                            }
                            else
                            {
                                Vector3 gunRecoil = currentWeapon.parameters.hipRecoil;
                                Vector3 generatedRecoil = new Vector3(gunRecoil.x, UnityEngine.Random.Range(gunRecoil.y * -currentWeapon.parameters.hipRecoilRandomness, gunRecoil.y * currentWeapon.parameters.hipRecoilRandomness), UnityEngine.Random.Range(gunRecoil.z * -currentWeapon.parameters.hipRecoilRandomness, gunRecoil.z * currentWeapon.parameters.hipRecoilRandomness));
                                targetRecoilRotation += generatedRecoil;
                                targetCameraRotation += generatedRecoil;
                            }
                        }
                    }
                }

                bool HasAmmo()
                {
                    if (currentWeaponSlot == WeaponSlots.primary && ammo.primaryCurrentMag > 0)
                        return true;
                    else if (currentWeaponSlot == WeaponSlots.secondary && ammo.secondaryCurrentMag > 0)
                        return true;
                    else
                        return false;
                }
            }
        }
        #endregion

    }

    private List<string> weaponSystemDebugInfo = new List<string>();

    void OnGUI()
    {
        if (showDebugUI)
        {
            GUIStyle styleWhite = new GUIStyle();
            GUIStyle styleRed = new GUIStyle();
            styleWhite.normal.textColor = Color.white;
            styleRed.normal.textColor = Color.red;

            GUILayout.Label("Primary Ammo", styleWhite);
            GUILayout.Label("MAG: " + ammo.primaryCurrentMag + " | RES: " + ammo.primaryAmmo, styleWhite);
            GUILayout.Label("Secondary Ammo", styleWhite);
            GUILayout.Label("MAG: " + ammo.secondaryCurrentMag + " | RES: " + ammo.secondaryAmmo, styleWhite);

            for (int i = weaponSystemDebugInfo.Count - 1; i >= 0; i--)
            {
                GUILayout.Label(weaponSystemDebugInfo[i], styleRed);
            }

        }
    }

    public void SetWeapon(bool onClick)
    {
        if (currentWeaponSlot == WeaponSlots.primary)
        {
            SetWeapon(onClick, WeaponSlots.secondary);
        }
        else if(currentWeaponSlot == WeaponSlots.secondary)
        {
            SetWeapon(onClick, WeaponSlots.primary);
        }
    }

    public void SetWeapon(bool onClick, WeaponSlots weaponSlot)
    {
        if (onClick)
        {
            currentWeaponSlot = weaponSlot;

            if (currentWeaponSlot == WeaponSlots.primary)
            {
                currentWeapon = primaryWeapon;
            }
            else if (currentWeaponSlot == WeaponSlots.secondary)
            {
                currentWeapon = secondaryWeapon;
            }

            weaponSystemDebugInfo.Add("[SWAP: " + currentWeapon.name + "]");

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            weaponObject = Instantiate(currentWeapon.parameters.model, 
                new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);

            if (!currentWeapon.parameters.flipY)
                weaponObject.transform.Rotate(new Vector3(0, 180, 0));
            weaponObject.transform.parent = gameObject.transform;

            setWeaponSpecificAnimations();
        }
        else if (!onClick)
        {

        }
    }

    public void Aim(bool onClick)
    {
        if (onClick)
        {
            isAds = true;
        }
        else if (!onClick)
        {
            isAds = false;
        }
    }

    public void Aim()
    {
        if (isAds)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, aimingPosition,
                (5f * currentWeapon.parameters.adsSpeed) * Time.deltaTime);
        }
        else if (!isAds)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, hipfirePosition,
                (5f * currentWeapon.parameters.adsSpeed) * Time.deltaTime);
        }

        Quaternion targetRot = Quaternion.LookRotation(lookAt.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10 * Time.deltaTime);
    }

    public void Fire(bool onClick)
    {
        if (onClick)
        {
            if (!stillReloading)
            {
                firing = true;
            }
        }
        else if (!onClick)
        {
            firing = false;
        }

    }

    public void Reload(bool onClick)
    {
        if (onClick)
        {
            if (!triggeredReload && !stillReloading)
            {
                stillReloading = true;
                triggeredReload = true;
                reloadTimer = 0f;

                if (currentWeaponSlot == WeaponSystem_LogicHandler.WeaponSlots.primary)
                {
                    ammo.primaryAmmo += ammo.primaryCurrentMag;
                    ammo.primaryCurrentMag = currentWeapon.parameters.magazineSize;
                    ammo.primaryAmmo -= currentWeapon.parameters.magazineSize;

                    if (ammo.primaryAmmo < 0)
                    {
                        ammo.primaryCurrentMag += ammo.primaryAmmo;
                        ammo.primaryAmmo = 0;
                    }
                }
                else if (currentWeaponSlot == WeaponSystem_LogicHandler.WeaponSlots.secondary)
                {
                    ammo.secondaryAmmo += ammo.secondaryCurrentMag;
                    ammo.secondaryCurrentMag = currentWeapon.parameters.magazineSize;
                    ammo.secondaryAmmo -= currentWeapon.parameters.magazineSize;

                    if (ammo.secondaryAmmo < 0)
                    {
                        ammo.secondaryCurrentMag += ammo.secondaryAmmo;
                        ammo.secondaryAmmo = 0;
                    }
                }
                GetComponent<AudioSource>().PlayOneShot(currentWeapon.parameters.reloadSound, 0.5f);

                weaponSystemDebugInfo.Add("[RELOAD: " + currentWeapon.name + "]");
            }
        }
        else if (!onClick)
        {

        }
    }

    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity) : base(capacity) { }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(this[index].Key, value);
            }
        }
    }

    private void RotateTowards(Vector3 to)
    {
        Quaternion lookRotation = Quaternion.LookRotation((to - transform.position).normalized);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10);
    }

    private void setWeaponSpecificAnimations()
    {
        animator = weaponObject.GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);

        clipOverrides["Reload"] = currentWeapon.parameters.reloadAnim;
        animatorOverrideController.ApplyOverrides(clipOverrides);
    }

    public void ResetAmmo()
    {
        ammo.primaryAmmo = primaryWeapon.parameters.startAmmo - primaryWeapon.parameters.magazineSize;
        ammo.secondaryAmmo = secondaryWeapon.parameters.startAmmo - secondaryWeapon.parameters.magazineSize;
        ammo.primaryCurrentMag = primaryWeapon.parameters.magazineSize;
        ammo.secondaryCurrentMag = secondaryWeapon.parameters.magazineSize;
    }
}
