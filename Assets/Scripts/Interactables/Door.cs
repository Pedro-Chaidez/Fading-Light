using UnityEngine;

public class Door : Interactable
{
    [Header("Door Settings")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool requiresKey = false;
    [SerializeField] private string requiredKeyID = "";

    [Header("Animation")]
    [SerializeField] private float openSpeed = 2f;
    // SLIDE SETTINGS: Distance to move up
    [SerializeField] private float slideDistance = 2f;
    [SerializeField] private Transform doorTransform;

    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip lockedSound;

    private AudioSource audioSource;
    private bool isOpen = false;
    private bool isAnimating = false;

    // POSITIONS: storing start and end locations
    private Vector3 closedPosition;
    private Vector3 openPosition;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (doorTransform == null)
        {
            doorTransform = transform;
        }

        // Initialize positions relative to the parent
        closedPosition = doorTransform.localPosition;
        openPosition = closedPosition + new Vector3(0, slideDistance, 0);
    }

    void Update()
    {
        if (isAnimating)
        {
            AnimateDoor();
        }
    }

    // CHANGED: Override the base Interact() method with NO arguments
    // This matches the signature called by PlayerInteract.cs
    protected override void Interact()
    {
        if (isAnimating) return;

        if (isLocked)
        {
            // Attempt to find the player to check for keys since checking
            // relies on inventory. 
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (requiresKey && player != null)
            {
                if (HasRequiredKey(player))
                {
                    Unlock();
                    ToggleDoor();
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
            ToggleDoor();
        }
    }

    private void ToggleDoor()
    {
        isOpen = !isOpen;
        isAnimating = true;

        if (isOpen)
        {
            PlaySound(openSound);
        }
        else
        {
            PlaySound(closeSound);
        }
    }

    private void AnimateDoor()
    {
        Vector3 targetPosition = isOpen ? openPosition : closedPosition;

        // Move position instead of rotation
        doorTransform.localPosition = Vector3.Lerp(
            doorTransform.localPosition,
            targetPosition,
            Time.deltaTime * openSpeed
        );

        if (Vector3.Distance(doorTransform.localPosition, targetPosition) < 0.01f)
        {
            doorTransform.localPosition = targetPosition;
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
        // Inventory inventory = interactor.GetComponent<Inventory>();
        // return inventory != null && inventory.HasKey(requiredKeyID);

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