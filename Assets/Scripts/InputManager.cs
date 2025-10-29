using UnityEngine;
using UnityEngine.InputSystem;
public class InputManager : MonoBehaviour
{
    private InputSystem_Actions playerInput;
    public InputSystem_Actions.PlayerActions onFoot;
    private PlayerMotor motor;
    private PlayerLook look;
    private void Awake()
    {
        playerInput = new InputSystem_Actions();
        onFoot = playerInput.Player;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        onFoot.Jump.performed += ctx => motor.Jump();
        onFoot.Crouch.performed += ctx => motor.Crouch();
        onFoot.Sprint.performed += ctx => motor.Sprint();
    }
    private void FixedUpdate()
    {
        motor.ProcessMove(onFoot.Move.ReadValue<Vector2>());
    }
    private void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }
    private void OnEnable()
    {
        onFoot.Enable();
    }
    private void OnDisable()
    {
        onFoot.Disable();
    }
}