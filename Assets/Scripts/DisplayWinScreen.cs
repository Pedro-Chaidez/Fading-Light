using UnityEngine;

public class DisplayWinScreen : MonoBehaviour
{
    [Header("UI Settings")]
    // Rename this to clearly indicate it's the reference from the Inspector
    [SerializeField] private GameObject youWinScreenPrefab;

    // This will hold the actual object currently in the scene
    private GameObject winScreenInstance;

    // Cache the list of ghosts so we don't have to search the whole world every frame
    private GameObject[] ghosts;
    private bool isLevelCleared;

    void Awake()
    {
        isLevelCleared = false;

        // 1. Find the ghosts ONLY once when the level starts.
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        // 2. Handle the UI instantiation safely
        if (youWinScreenPrefab != null)
        {
            // Create the screen and store it in 'winScreenInstance', keeping the Prefab reference safe
            winScreenInstance = Instantiate(youWinScreenPrefab);
            winScreenInstance.SetActive(false);
        }
        else
        {
            Debug.LogError("YouWinScreen Prefab is not assigned in the Inspector!");
        }
    }

    void LateUpdate()
    {
        // If we already won, stop doing math!
        if (isLevelCleared) return;

        // 3. THE FIX:
        // Instead of searching the world with FindGameObjectsWithTag,
        // we check the list we already made.

        bool allGhostsDead = true;

        foreach (GameObject ghost in ghosts)
        {
            // In Unity, if an object is Destroyed, it equals 'null'.
            // So we check: Is this ghost still alive (not null) AND active?
            if (ghost != null && ghost.activeInHierarchy)
            {
                // Found a living ghost, so we haven't won yet.
                allGhostsDead = false;
                break;
            }
        }

        if (allGhostsDead)
        {
            if (winScreenInstance != null)
            {
                winScreenInstance.SetActive(true);
            }
            isLevelCleared = true;
        }
    }
}