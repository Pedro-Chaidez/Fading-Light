using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Timeline;

[RequireComponent(typeof(CharacterController))]
public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    private bool lerpCrouch;
    public bool crouching;
    public bool sprinting;
    public float speed = 6f;
    public float gravity = -10f;
    public float jumpHeight = 7f;
    public float crouchTimer = 1f;

    private bool CanMove()
    {
        // Allow movement if not spawned (singleplayer) or if this is the owner in multiplayer
        return !IsSpawned || IsOwner;
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    private void Update()
    {
<<<<<<< HEAD
        if (!CanMove())
        {
            return;
        }
=======
>>>>>>> main
        isGrounded = controller.isGrounded;
        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;
        }
    }
    public void Crouch()
    {
<<<<<<< HEAD
        if (!CanMove())
        {
            return;
        }
=======
>>>>>>> main
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;

    }
    public void Sprint()
    {
<<<<<<< HEAD
        if (!CanMove())
        {
            return;
        }
=======
>>>>>>> main
        sprinting = !sprinting;
        if (sprinting)
        {
            speed = 12f;
        }
        else
        {
            speed = 6f;
        }
    }
    public void ProcessMove(Vector2 input)
    {
<<<<<<< HEAD
        if (!CanMove())
        {
            return;
        }
=======
>>>>>>> main
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);

    }
    public void Jump()
    {
<<<<<<< HEAD
        if (!CanMove())
        {
            return;
        }
=======
>>>>>>> main
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }
}