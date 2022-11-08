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

    // Start is called before the first frame update
    void Start()
    {
        UpdateWeaponModel(primary);

        primaryAmmo = primary.startAmmo - primary.magazineSize;
        secondaryAmmo = secondary.startAmmo - secondary.magazineSize;

        primaryCurrentMag = primary.magazineSize;
        secondaryCurrentMag = secondary.magazineSize;
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

    }

    private void SwitchToPrimary(InputAction.CallbackContext context)
    {
        UpdateWeaponModel(primary);
        isPrimary = true;
    }
    private void SwitchToSecondary(InputAction.CallbackContext context)
    {
        UpdateWeaponModel(secondary);
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

    void UpdateWeaponModel(Weapon weapon)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        weaponModel = Instantiate(weapon.model, new Vector3(transform.position.x, transform.position.y, transform.position.z), transform.rotation);
        if (!weapon.flipY)
            weaponModel.transform.Rotate(new Vector3(0, 180, 0));
        weaponModel.transform.parent = gameObject.transform;
    }

    public Weapon GetCurrentWeapon()
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
