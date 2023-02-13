using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class WeaponSystem_InputDetection : MonoBehaviour
{
    private WeaponSystem_LogicHandler logicHandler;
    private WeaponActionsMap weaponActionsMap;

    private void Start()
    {
    }

    private void Awake()
    {
        logicHandler = GetComponent<WeaponSystem_LogicHandler>();
        weaponActionsMap = new WeaponActionsMap();

        weaponActionsMap.WeaponSystem.Primary.started += SwitchToPrimary;
        weaponActionsMap.WeaponSystem.Primary.canceled += StopSwitchToPrimary;

        weaponActionsMap.WeaponSystem.Secondary.started += SwitchToSecondary;
        weaponActionsMap.WeaponSystem.Secondary.canceled += StopSwitchToSecondary;

        weaponActionsMap.WeaponSystem.Swap.started += SwapWeapon;
        weaponActionsMap.WeaponSystem.Swap.canceled += StopSwapWeapon;

        weaponActionsMap.WeaponSystem.Fire.started += Fire;
        weaponActionsMap.WeaponSystem.Fire.canceled += StopFire;

        weaponActionsMap.WeaponSystem.Aim.started += Aim;
        weaponActionsMap.WeaponSystem.Aim.canceled += StopAim;

        weaponActionsMap.WeaponSystem.Reload.started += Reload;
        weaponActionsMap.WeaponSystem.Reload.canceled += StopReload;
    }

    void Update()
    {

    }

    #region callback contexts

    private void SwitchToPrimary(InputAction.CallbackContext context)
    {
        logicHandler.SetWeapon(true, WeaponSystem_LogicHandler.WeaponSlots.primary);
    }

    private void StopSwitchToPrimary(InputAction.CallbackContext context)
    {
        logicHandler.SetWeapon(false, WeaponSystem_LogicHandler.WeaponSlots.primary);
    }

    private void SwitchToSecondary(InputAction.CallbackContext context)
    {
        logicHandler.SetWeapon(true, WeaponSystem_LogicHandler.WeaponSlots.secondary);
    }

    private void StopSwitchToSecondary(InputAction.CallbackContext context)
    {
        logicHandler.SetWeapon(false, WeaponSystem_LogicHandler.WeaponSlots.secondary);
    }

    private void SwapWeapon(InputAction.CallbackContext context)
    {
        logicHandler.SetWeapon(true);
    }

    private void StopSwapWeapon(InputAction.CallbackContext context)
    {

    }

    private void Aim(InputAction.CallbackContext context)
    {
        logicHandler.Aim(true);
    }

    private void StopAim(InputAction.CallbackContext context)
    {
        logicHandler.Aim(false);
    }

    private void Fire(InputAction.CallbackContext context)
    {
        logicHandler.Fire(true);
    }

    private void StopFire(InputAction.CallbackContext context)
    {
        logicHandler.Fire(false);
    }

    private void Reload(InputAction.CallbackContext context)
    {
        logicHandler.Reload(true);
    }

    private void StopReload(InputAction.CallbackContext context)
    {
        logicHandler.Reload(false);
    }

    #endregion

    private void OnEnable()
    {
        weaponActionsMap.WeaponSystem.Enable();
    }

    private void OnDisable()
    {
        weaponActionsMap.WeaponSystem.Disable();
    }
}
