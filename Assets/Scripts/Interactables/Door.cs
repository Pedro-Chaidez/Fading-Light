using UnityEngine;

public class Door : Interactable
{
    [Header("Door Settings")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool requiresKey = false;
    [SerializeField] private string requiredKeyID = "";

    [Header("Animation")]
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private float openAngle = 90f;

    [Tooltip("Assign the Parent Object here if you want the pivot to be different from the mesh center.")]
    [SerializeField] private Transform doorPivot;

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip lockedSound;

    private AudioSource audioSource;
    private bool isOpen = false;
    private bool isAnimating = false;

    // ROTATION: storing start and target rotations
    private Quaternion closedRotation;
    private Quaternion targetRotation;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // AUTO-DETECT PIVOT:
        // Changed default to 'transform' (self) to prevent accidental rotation of 
        // the entire room/wall if the parent is a container object.
        if (doorPivot == null)
        {
            doorPivot = transform;
        }

        // Initialize rotation based on the pivot's current state
        closedRotation = doorPivot.localRotation;
        targetRotation = closedRotation;
    }

    void Update()
    {
        if (isAnimating)
        {
            AnimateDoor();
        }
    }

    protected override void Interact()
    {
        // Find player to determine which side they are on
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (isLocked)
        {
            if (requiresKey && player != null)
            {
                if (HasRequiredKey(player))
                {
                    Unlock();
                    // Pass the player so we can calculate direction
                    ToggleDoor(player.transform.position);
                }
                else
                {
                    PlayLockedSound();
                    Debug.Log("Door is locked. Required key: " + requiredKeyID);
                }
            }
            else
            {
                PlayLockedSound();
                Debug.Log("Door is locked.");
            }
        }
        else
        {
            // Pass the player position to determine open direction
            Vector3 playerPos = (player != null) ? player.transform.position : transform.position + transform.forward;
            ToggleDoor(playerPos);
        }
    }

    private void ToggleDoor(Vector3 interactorPosition)
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            // --- DIRECTION CALCULATION ---
            // Calculate vector from Pivot to Player
            Vector3 directionToInteractor = interactorPosition - doorPivot.position;

            // Dot Product check:
            // > 0 means player is in front (facing the same way as door forward)
            // < 0 means player is behind
            float dot = Vector3.Dot(doorPivot.forward, directionToInteractor);

            // If player is in front, open angle is negative (swing out).
            // If player is behind, open angle is positive (swing in).
            float angle = (dot >= 0) ? -openAngle : openAngle;

            // Calculate the new target rotation relative to the CLOSED state
            targetRotation = closedRotation * Quaternion.Euler(0, angle, 0);

            PlaySound(openSound);
        }
        else
        {
            // Closing: Always return to original rotation
            targetRotation = closedRotation;
            PlaySound(closeSound);
        }

        isAnimating = true;
    }

    private void AnimateDoor()
    {
        // CHANGED: Use RotateTowards for constant speed. 
        // Slerp with Time.deltaTime creates an asymptotic curve that slows down indefinitely at the end.
        // We multiply openSpeed by 45 to make '2' feel similar to the previous speed but linear.
        float step = openSpeed * 45f * Time.deltaTime;

        doorPivot.localRotation = Quaternion.RotateTowards(
            doorPivot.localRotation,
            targetRotation,
            step
        );

        // Check if we are close enough to stop
        if (Quaternion.Angle(doorPivot.localRotation, targetRotation) < 0.1f)
        {
            doorPivot.localRotation = targetRotation;
            isAnimating = false;
        }
    }

    public void Lock()
    {
        isLocked = true;
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public void SetRequiredKey(string keyID)
    {
        requiresKey = true;
        requiredKeyID = keyID;
    }

    private bool HasRequiredKey(GameObject interactor)
    {
        // Placeholder implementation
        return false;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void PlayLockedSound()
    {
        PlaySound(lockedSound);
    }
}