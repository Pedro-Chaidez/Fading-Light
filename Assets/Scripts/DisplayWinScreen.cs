using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class DisplayWinScreen : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject youWinScreenPrefab;

    [Header("Level Progression")]
    [SerializeField] private float levelCompleteDelay = 3f; // Seconds to wait before loading next level
    [SerializeField] private bool enableAutoProgression = true;

    private GameObject winScreenInstance;
    private bool isLevelCleared;
    private int initialGhostCount = 0;
    private bool hasSeenGhosts = false;
    private bool hasTransitioned = false;
    private bool hasStartedChecking = false; // Flag to prevent checking before ghosts spawn

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
        hasStartedChecking = false;

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
        
        // Wait for ghosts to spawn before counting them
        StartCoroutine(DelayedCountInitialGhosts());
    }
    
    private System.Collections.IEnumerator DelayedCountInitialGhosts()
    {
        // Wait for ghosts to spawn (they might spawn late via PrefabSpawner)
        yield return new WaitForSeconds(1f);
        
        // Count ghosts after delay
        GameObject[] initialGhosts = GameObject.FindGameObjectsWithTag("Ghost");
        initialGhostCount = initialGhosts.Length;
        hasSeenGhosts = initialGhostCount > 0;
        hasStartedChecking = true;
        
        Debug.Log($"DisplayWinScreen: After spawn delay, found {initialGhostCount} ghost(s).");
        
        if (initialGhostCount == 0)
        {
            Debug.LogWarning("DisplayWinScreen: No ghosts found after spawn delay. Win screen will not appear unless ghosts spawn later.");
        }
    }

    void LateUpdate()
    {
        // Don't check until we've waited for ghosts to spawn
        if (!hasStartedChecking) return;
        
        // If we already won, stop checking
        if (isLevelCleared) return;

        // CRITICAL: Only check if we actually found ghosts initially
        // If initialGhostCount is 0, keep checking in case ghosts spawn late
        if (initialGhostCount == 0)
        {
            // Re-check for ghosts that might have spawned late
            GameObject[] lateGhosts = GameObject.FindGameObjectsWithTag("Ghost");
            if (lateGhosts.Length > 0)
            {
                initialGhostCount = lateGhosts.Length;
                hasSeenGhosts = true;
                Debug.Log($"DisplayWinScreen: Found {initialGhostCount} ghost(s) that spawned late.");
            }
            return; // Don't show win screen if we never found any ghosts
        }

        // If there were no ghosts at the start, don't show win screen
        if (!hasSeenGhosts) return;

        // Dynamically check for ghosts each frame (more reliable)
        GameObject[] currentGhosts = GameObject.FindGameObjectsWithTag("Ghost");
        int currentGhostCount = currentGhosts.Length;

        // CRITICAL: Only show win screen if we had ghosts initially AND they're all gone now
        if (currentGhostCount == 0 && initialGhostCount > 0)
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
                
                // Transition to Level1 after delay
                if (enableAutoProgression && !hasTransitioned)
                {
                    string currentSceneName = SceneManager.GetActiveScene().name;
                    if (currentSceneName == "TutorialLevel")
                    {
                        hasTransitioned = true;
                        StartCoroutine(TransitionToLevel1());
                    }
                }
            }
            else
            {
                Debug.LogError("DisplayWinScreen: Win screen instance is null! Cannot display win screen.");
            }
        }
    }

    private System.Collections.IEnumerator TransitionToLevel1()
    {
        Debug.Log($"DisplayWinScreen: Transitioning to Level1 in {levelCompleteDelay} seconds...");
        
        // Wait for delay so players can see win screen
        yield return new WaitForSeconds(levelCompleteDelay);
        
        // Load Level1
        Debug.Log("DisplayWinScreen: Loading Level1...");
        SceneManager.LoadScene("Level1");
    }
}