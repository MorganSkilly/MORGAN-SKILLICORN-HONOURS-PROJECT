using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem_SampleMovementController : MonoBehaviour
{
    public PlayerActions playerActions;
    public Vector3 moveInput;
    public Vector2 mouseInput;
    public GameObject headObject;
    public Transform groundCheck;
    public float groundDist;
    public LayerMask groundMask;
    public float mouseSens = 2f;
    public float walkSpeed = 10f;
    public float runSpeed = 20f;
    private float moveSpeed = 10f;
    public float gravity = -9.81f;

    private float xRot = 0f;
    private Vector3 velocity;
    private CharacterController controller;
    private bool isGrounded;
    private bool isCrouched = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        controller = GetComponent<CharacterController>();

    }

    private void Awake()
    {       
        playerActions = new PlayerActions();
        playerActions.PlayerMap.Crouch.started += Crouch;
        playerActions.PlayerMap.Crouch.canceled += CrouchCancel;
        playerActions.PlayerMap.Run.started += Run;
        playerActions.PlayerMap.Run.canceled += Walk;
    }

    void Crouch(InputAction.CallbackContext context)
    {
        isCrouched = true;
    }

    void CrouchCancel(InputAction.CallbackContext context)
    {
        isCrouched = false;
    }

    void Run(InputAction.CallbackContext context)
    {
        moveSpeed = runSpeed;
    }

    void Walk(InputAction.CallbackContext context)
    {
        moveSpeed = walkSpeed;
    }

    void Update()
    {
        if (isCrouched)
        {
            headObject.transform.localPosition = Vector3.Slerp(headObject.transform.localPosition, new Vector3(0, -1.5f, 0), 5f * Time.deltaTime);
        }
        else
        {
            headObject.transform.localPosition = Vector3.Slerp(headObject.transform.localPosition, new Vector3(0, 0, 0), 5f * Time.deltaTime);
        }

        try
        {
            MouseLookUpdate();
        }
        catch (ArgumentException ex)
        {
            Debug.LogError("Mouse look error caught: " + ex);
        }

        try
        {
            MovementUpdate();
        }
        catch (ArgumentException ex)
        {
            Debug.LogError("Movement error caught: " + ex);
        }

        try
        {
            PhysicsUpdate();
        }
        catch (ArgumentException ex)
        {
            Debug.LogError("Physics error caught: " + ex);
        }
    }

    private void MouseLookUpdate()
    {
        float mouseX = playerActions.PlayerMap.Look.ReadValue<Vector2>().x * mouseSens * Time.deltaTime;
        float mouseY = playerActions.PlayerMap.Look.ReadValue<Vector2>().y * mouseSens * Time.deltaTime;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -75, 75);

        transform.Rotate(Vector3.up * mouseX);
        headObject.transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
    }

    private void MovementUpdate()
    {
        float moveX = playerActions.PlayerMap.Movement.ReadValue<Vector3>().x;
        float moveZ = playerActions.PlayerMap.Movement.ReadValue<Vector3>().z;

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        controller.Move(move * moveSpeed * Time.deltaTime);
    }

    private void PhysicsUpdate()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDist, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void OnEnable()
    {
        playerActions.PlayerMap.Enable();
    }
    private void OnDisable()
    {
        playerActions.PlayerMap.Disable();
    }
}
