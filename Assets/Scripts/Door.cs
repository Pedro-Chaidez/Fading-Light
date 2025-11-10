using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool requiresKey = false;
    [SerializeField] private string requiredKeyID = "";
    
    [Header("Animation")]
    [SerializeField] private float openSpeed = 2f;
    [SerializeField] private float doorOpenAngle = 90f;
    [SerializeField] private Transform doorTransform;
    
    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip lockedSound;
    
    private AudioSource audioSource;
    private bool isOpen = false;
    private bool isAnimating = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    
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
        
        closedRotation = doorTransform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, doorOpenAngle, 0);
    }
    
    void Update()
    {
        if (isAnimating)
        {
            AnimateDoor();
        }
    }
    
    public void Interact(GameObject interactor = null)
    {
        if (isAnimating) return;
        
        if (isLocked)
        {
            if (requiresKey && interactor != null)
            {
                // Check if the interactor has the required key
                if (HasRequiredKey(interactor))
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
        Quaternion targetRotation = isOpen ? openRotation : closedRotation;
        doorTransform.localRotation = Quaternion.Slerp(
            doorTransform.localRotation,
            targetRotation,
            Time.deltaTime * openSpeed
        );
        
        if (Quaternion.Angle(doorTransform.localRotation, targetRotation) < 0.1f)
        {
            doorTransform.localRotation = targetRotation;
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
        // This is a placeholder - implement your own inventory system check
        // For example:
        // Inventory inventory = interactor.GetComponent<Inventory>();
        // return inventory != null && inventory.HasKey(requiredKeyID);
        
        return false; // Default to false
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
    
    public bool IsOpen()
    {
        return isOpen;
    }
    
    public bool IsLocked()
    {
        return isLocked;
    }
    
    // For simple collision-based interaction
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isLocked)
        {
            Interact(other.gameObject);
        }
    }
}