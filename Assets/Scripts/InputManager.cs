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

    private bool CanInput()
    {
        // 1. If Game is Paused BLOCK INPUT
        if (Time.timeScale == 0) 
        {
            return false;
        }

        // 2. Standard Network check
        return !IsSpawned || IsOwner;
    }

    private void Awake()
    {
        playerInput = new InputSystem_Actions();
        onFoot = playerInput.Player;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        inventory = GetComponent<Inventory>();
        flashlight = GetComponent<Flashlight>();

        // --- SAFETY CHECKS ---
        if (motor != null)
        {
            onFoot.Jump.performed += ctx => motor.Jump();
            onFoot.Crouch.performed += ctx => motor.Crouch();
            onFoot.Sprint.performed += ctx => motor.Sprint();
        }
        else Debug.LogError("InputManager: Missing 'PlayerMotor' script on this object!");

        if (inventory != null)
        {
            onFoot.NextItem.performed += ctx => { if(CanInput()) inventory.scrollUp(); };
            onFoot.PreviousItem.performed += ctx => { if(CanInput()) inventory.scrollDown(); };
            onFoot.DropItem.performed += ctx => { if(CanInput()) inventory.DropItem(); };
            
            // This is likely where "UseItem" is bound (Left Click?)
            // Assuming you have an action for "Use" or "Fire" not listed here but connected similarly
        }
        else Debug.LogError("InputManager: Missing 'Inventory' script on this object!");

        if (flashlight != null)
        {
            onFoot.ToggleFlashlight.performed += ctx => { if(CanInput()) flashlight.normLight(); };
            onFoot.ToggleMaxFlash.performed += ctx => { if(CanInput()) flashlight.maxLight(); };
        }
        else Debug.LogError("InputManager: Missing 'Flashlight' script on this object!");
    }
    
    private void FixedUpdate()
    {
        if (motor == null) return;
        motor.ProcessMove(onFoot.Move.ReadValue<Vector2>());
    }
    
    private void LateUpdate()
    {
        if (look == null) return;
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }
    
    private void OnEnable()
    {
        if (playerInput == null) return;
        if (!onFoot.enabled) onFoot.Enable();
    }
    
    private void OnDisable()
    {
        if (playerInput == null) return;
        if (onFoot.enabled) onFoot.Disable();
    }
    
    public override void OnNetworkSpawn()
    {
        if (playerInput == null) return;
        if (!onFoot.enabled) onFoot.Enable();
    }
    
    public override void OnNetworkDespawn()
    {
        if (playerInput == null) return;
        if (onFoot.enabled) onFoot.Disable();
    }
}