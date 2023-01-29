using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActiveWeaponManager : MonoBehaviour
{
    public WeaponActionsMap weaponActionsMap;

    public Weapon primary;
    public Weapon secondary;

    public int primaryAmmo;
    public int secondaryAmmo;

    public int primaryCurrentMag;
    public int secondaryCurrentMag;

    public bool isPrimary = true;

    private GameObject weaponModel;

    public GameObject lookAt;

    public Animator animator;
    public AnimatorOverrideController animatorOverrideController;

    public AnimationClipOverrides clipOverrides;

    // Start is called before the first frame update
    void Start()
    {
        UpdateWeapon(primary);

        primaryAmmo = primary.parameters.startAmmo - primary.parameters.magazineSize;
        secondaryAmmo = secondary.parameters.startAmmo - secondary.parameters.magazineSize;

        primaryCurrentMag = primary.parameters.magazineSize;
        secondaryCurrentMag = secondary.parameters.magazineSize;
                
    }

    private void Awake()
    {
        weaponActionsMap = new WeaponActionsMap();

        weaponActionsMap.WeaponSystem.Primary.started += SwitchToPrimary;
        weaponActionsMap.WeaponSystem.Primary.canceled += SwitchToPrimary;
        weaponActionsMap.WeaponSystem.Secondary.started += SwitchToSecondary;
        weaponActionsMap.WeaponSystem.Secondary.canceled += SwitchToSecondary;
        weaponActionsMap.WeaponSystem.Swap.started += SwapWeapon;
        weaponActionsMap.WeaponSystem.Swap.canceled += SwapWeapon;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion targetRot = Quaternion.LookRotation(lookAt.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10 * Time.deltaTime);
    }

    private void SwitchToPrimary(InputAction.CallbackContext context)
    {
        UpdateWeapon(primary);
        isPrimary = true;
    }
    private void SwitchToSecondary(InputAction.CallbackContext context)
    {
        UpdateWeapon(secondary);
        isPrimary = false;
    }

    private void SwapWeapon(InputAction.CallbackContext context)
    {
        if (context.ReadValue<Vector2>() == Vector2.zero)
        {
            Debug.Log(context.ReadValue<Vector2>());

            if (isPrimary)
            {
                SwitchToSecondary(context);
            }
            else
            {
                SwitchToPrimary(context);
            }
        }
        
    }

    void UpdateWeapon(Weapon weapon)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
         
        weaponModel = Instantiate(weapon.parameters.model, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
        if (!weapon.parameters.flipY)
            weaponModel.transform.Rotate(new Vector3(0, 180, 0));
        weaponModel.transform.parent = gameObject.transform;

        animator = weaponModel.GetComponent<Animator>();
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;
        clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
        animatorOverrideController.GetOverrides(clipOverrides);

        clipOverrides["Reload"] = weapon.parameters.reloadAnim;
        animatorOverrideController.ApplyOverrides(clipOverrides);
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

    public Weapon GetCurrentWeapon()//returns the active weapon instance depending on whether primary or secondary is active
    {
        if (isPrimary)
            return primary;
        else
            return secondary;
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
