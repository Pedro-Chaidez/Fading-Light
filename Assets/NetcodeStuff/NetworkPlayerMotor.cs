using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NetworkObject))]
public class NetworkPlayerMotor : NetworkBehaviour
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
    public float walkInterval = 0.7f;
    public float sprintInterval = 0.4f;
    private float footstepTimer = 0f;

    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<float> networkRotationY = new NetworkVariable<float>();

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

        // Smooth position interpolation for non-owner clients
        if (!IsOwner)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition.Value, Time.deltaTime * 10f);
            Vector3 euler = transform.eulerAngles;
            euler.y = Mathf.LerpAngle(euler.y, networkRotationY.Value, Time.deltaTime * 10f);
            transform.eulerAngles = euler;
        }
    }

    public void Crouch()
    {
        if (!IsOwner) return;
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }

    public void Sprint()
    {
        if (!IsOwner) return;
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
        if (!IsOwner) return;

        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);

        // Update network position
        UpdateNetworkPositionServerRpc(transform.position, transform.eulerAngles.y);

        // Footstep logic
        if (isGrounded && input.sqrMagnitude > 0.1f)
        {
            footstepTimer -= Time.deltaTime;
            if (footstepTimer <= 0)
            {
                PlayFootstep();
                footstepTimer = sprinting ? sprintInterval : walkInterval;
            }
        }
        else
        {
            footstepTimer = 0;
        }
    }

    [ServerRpc]
    private void UpdateNetworkPositionServerRpc(Vector3 position, float rotationY)
    {
        networkPosition.Value = position;
        networkRotationY.Value = rotationY;
    }

    private void PlayFootstep()
    {
        if (walkingClip == null) return;
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.volume = Random.Range(0.85f, 1.0f);
        audioSource.PlayOneShot(walkingClip);
    }

    public void Jump()
    {
        if (!IsOwner) return;
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }
}
