using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Timeline;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
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

    [Header("Audio Settings")]
    public AudioClip walkingClip;
    private AudioSource audioSource;

    [Header("Step Intervals")]
    public float walkInterval = 0.7f;   // Time between steps when walking
    public float sprintInterval = 0.4f; // Time between steps when sprinting
    private float footstepTimer = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
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
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }

    public void Sprint()
    {
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
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);

        // --- FOOTSTEP LOGIC START ---

        // 1. Check if we are grounded
        // 2. Check if input is strong enough (Fixes "Machine Gun" bug caused by stick drift)
        if (isGrounded && input.sqrMagnitude > 0.1f)
        {
            // Count down the timer
            footstepTimer -= Time.deltaTime;

            // If timer reaches 0, play sound
            if (footstepTimer <= 0)
            {
                PlayFootstep();

                // RESET TIMER
                // Select interval based on sprinting state
                footstepTimer = sprinting ? sprintInterval : walkInterval;
            }
        }
        else
        {
            // When stopped, reset timer to 0. 
            // This ensures the NEXT time you move, the step plays instantly.
            footstepTimer = 0;
        }
        // --- FOOTSTEP LOGIC END ---
    }

    private void PlayFootstep()
    {
        if (walkingClip == null) return;

        // Randomize pitch and volume slightly for natural feel
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.volume = Random.Range(0.85f, 1.0f);

        audioSource.PlayOneShot(walkingClip);
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }
}