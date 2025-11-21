using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class InputManager : NetworkBehaviour
{
    private InputSystem_Actions playerInput;
    public InputSystem_Actions.PlayerActions onFoot;
    private PlayerMotor motor;
    private PlayerLook look;
    private Inventory inventory;
    private Flashlight flashlight;
    private void Awake()
    {
        playerInput = new InputSystem_Actions();
        onFoot = playerInput.Player;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        inventory = GetComponent<Inventory>();
        flashlight = GetComponent<Flashlight>();

        onFoot.Jump.performed += ctx => motor.Jump();
        onFoot.Crouch.performed += ctx => motor.Crouch();
        onFoot.Sprint.performed += ctx => motor.Sprint();
        onFoot.NextItem.performed += ctx => inventory.scrollUp();
        onFoot.PreviousItem.performed += ctx => inventory.scrollDown();
        onFoot.DropItem.performed += ctx => inventory.DropItem();
        onFoot.ToggleFlashlight.performed += ctx => flashlight.normLight();
        onFoot.ToggleMaxFlash.performed += ctx => flashlight.maxLight();
        //onFoot.UseItem.performed += ctx => inventory.UseItem();
    }
    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        motor.ProcessMove(onFoot.Move.ReadValue<Vector2>());
    }
    private void LateUpdate()
    {
        if (!IsOwner)
        {
            return;
        }
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        if (playerInput == null)
        {
            return;
        }
        if (!IsOwner)
        {
            return;
        }
        if (!onFoot.enabled)
        {
            onFoot.Enable();
        }
    }
    private void OnDisable()
    {
        if (playerInput == null)
        {
            return;
        }
        if (onFoot.enabled)
        {
            onFoot.Disable();
        }
    }
    public override void OnNetworkSpawn()
    {
        if (playerInput == null)
        {
            return;
        }
        if (!IsOwner)
        {
            return;
        }
        if (!onFoot.enabled)
        {
            onFoot.Enable();
        }
    }
    public override void OnNetworkDespawn()
    {
        if (playerInput == null)
        {
            return;
        }
        if (onFoot.enabled)
        {
            onFoot.Disable();
        }
    }
}