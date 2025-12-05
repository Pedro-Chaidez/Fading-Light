using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class DisplayWinScreen : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject youWinScreenPrefab;

    private GameObject winScreenInstance;
    private bool isLevelCleared;
    private int initialGhostCount = 0;
    private bool hasSeenGhosts = false;

    void Awake()
    {
        // Check if we're in a networked game - if so, let NetworkWinScreenManager handle it
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            // In multiplayer, NetworkWinScreenManager handles this
            // This script can be disabled or used as fallback
            if (NetworkWinScreenManager.Instance != null)
            {
                Debug.Log("DisplayWinScreen: NetworkWinScreenManager detected. Disabling local win screen manager.");
                enabled = false;
                return;
            }
        }
        
        isLevelCleared = false;
        
        // Count ghosts at the start of the scene
        GameObject[] initialGhosts = GameObject.FindGameObjectsWithTag("Ghost");
        initialGhostCount = initialGhosts.Length;
        hasSeenGhosts = initialGhostCount > 0;
        
        Debug.Log($"DisplayWinScreen: Scene started with {initialGhostCount} ghost(s).");

        // Handle the UI instantiation safely
        if (youWinScreenPrefab != null)
        {
            // Find Canvas to parent the win screen (important for builds)
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = GameObject.Find("Canvas");
                if (canvasObj != null)
                {
                    canvas = canvasObj.GetComponent<Canvas>();
                }
            }

            if (canvas != null)
            {
                winScreenInstance = Instantiate(youWinScreenPrefab, canvas.transform);
            }
            else
            {
                winScreenInstance = Instantiate(youWinScreenPrefab);
                Debug.LogWarning("DisplayWinScreen: Canvas not found! Win screen instantiated without parent.");
            }
            
            winScreenInstance.SetActive(false);
        }
        else
        {
            Debug.LogError("DisplayWinScreen: YouWinScreen Prefab is not assigned in the Inspector!");
        }
    }

    void LateUpdate()
    {
        // If we already won, stop checking
        if (isLevelCleared) return;

        // If there were no ghosts at the start, don't show win screen
        if (!hasSeenGhosts) return;

        // Dynamically check for ghosts each frame (more reliable)
        GameObject[] currentGhosts = GameObject.FindGameObjectsWithTag("Ghost");
        int currentGhostCount = currentGhosts.Length;

        // Check if all ghosts are gone
        if (currentGhostCount == 0)
        {
            // All ghosts are dead! Show win screen
            if (winScreenInstance != null)
            {
                winScreenInstance.SetActive(true);
                
                // Bring to front
                if (winScreenInstance.transform.parent != null)
                {
                    winScreenInstance.transform.SetAsLastSibling();
                }
                
                // Ensure visibility
                CanvasGroup canvasGroup = winScreenInstance.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
                
                Debug.Log($"DisplayWinScreen: All {initialGhostCount} ghost(s) eliminated! Win screen displayed.");
                isLevelCleared = true;
            }
            else
            {
                Debug.LogError("DisplayWinScreen: Win screen instance is null! Cannot display win screen.");
            }
        }
    }
}