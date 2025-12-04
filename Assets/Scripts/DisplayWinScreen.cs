using UnityEngine;

public class DisplayWinScreen : MonoBehaviour
{
    [Header("UI Settings")]
    // Rename this to clearly indicate it's the reference from the Inspector
    [SerializeField] private GameObject youWinScreenPrefab;
    // Cache the list of ghosts so we don't have to search the whole world every frame
    private GameObject[] ghosts;
    private bool isLevelCleared;
    private float timer = 0f;
    private const float countdownDuration = 5f;
    private bool timerActive = false;

    void Awake()
    {
        isLevelCleared = false;
        timer = 0f; // Reset the timer
        timerActive = true;
        // 1. Find the ghosts ONLY once when the level starts.

        ghosts = GameObject.FindGameObjectsWithTag("Ghost");

        // 2. Handle the UI instantiation safely
        if (youWinScreenPrefab != null)
        {
            // Create the screen and store it in 'winScreenInstance', keeping the Prefab reference safe
            youWinScreenPrefab = Instantiate(youWinScreenPrefab);
            youWinScreenPrefab.SetActive(false);
        }
        else
        {
            Debug.LogError("YouWinScreen Prefab is not assigned in the Inspector!");
        }
    }

    void LateUpdate()
    {
        if (timerActive)
        {
            timer += Time.deltaTime; // Add the time since the last frame

            if (timer >= countdownDuration)
            {
                Debug.Log("5 seconds have passed in LateUpdate!");
                // Perform actions here after 5 seconds
                timerActive = false; // Stop the timer
                return;
            }
        }
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
            if (youWinScreenPrefab != null)
            {
                youWinScreenPrefab.SetActive(true);
            }
            isLevelCleared = true;
        }
    }
}