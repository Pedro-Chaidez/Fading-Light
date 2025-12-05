using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkWinScreenManager : NetworkBehaviour
{
    public static NetworkWinScreenManager Instance { get; private set; }

    [Header("UI Settings")]
    [SerializeField] private GameObject youWinScreenPrefab;

    private GameObject winScreenInstance;
    private NetworkVariable<bool> isLevelCleared = new NetworkVariable<bool>(false);
    private NetworkVariable<int> initialGhostCount = new NetworkVariable<int>(0);
    private NetworkVariable<int> currentGhostCount = new NetworkVariable<int>(0);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (IsServer)
        {
            // Delay counting ghosts to ensure they've spawned
            StartCoroutine(DelayedCountInitialGhosts());
        }
        
        if (IsClient)
        {
            isLevelCleared.OnValueChanged += OnLevelClearedChanged;
            currentGhostCount.OnValueChanged += OnGhostCountChanged;
        }
        
        InitializeWinScreen();
    }
    
    private System.Collections.IEnumerator DelayedCountInitialGhosts()
    {
        // Wait a few frames for ghosts to spawn
        yield return new WaitForSeconds(0.5f);
        CountInitialGhosts();
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            if (isLevelCleared != null)
                isLevelCleared.OnValueChanged -= OnLevelClearedChanged;
            if (currentGhostCount != null)
                currentGhostCount.OnValueChanged -= OnGhostCountChanged;
        }
        base.OnNetworkDespawn();
    }

    private void InitializeWinScreen()
    {
        if (youWinScreenPrefab == null)
        {
            Debug.LogError("NetworkWinScreenManager: YouWinScreen Prefab is not assigned!");
            return;
        }

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
            Debug.LogWarning("NetworkWinScreenManager: Canvas not found! Win screen instantiated without parent.");
        }
        
        winScreenInstance.SetActive(false);
    }

    private void CountInitialGhosts()
    {
        GameObject[] initialGhosts = GameObject.FindGameObjectsWithTag("Ghost");
        initialGhostCount.Value = initialGhosts.Length;
        currentGhostCount.Value = initialGhostCount.Value;
        Debug.Log($"NetworkWinScreenManager: Scene started with {initialGhostCount.Value} ghost(s).");
        
        // If no ghosts found, don't show win screen (same logic as single-player)
        if (initialGhostCount.Value == 0)
        {
            Debug.Log("NetworkWinScreenManager: No ghosts found at start. Win screen will not appear.");
        }
    }

    private void Update()
    {
        if (!IsServer) return;
        if (isLevelCleared.Value) return;
        
        // Don't check if we haven't counted initial ghosts yet
        if (initialGhostCount.Value == 0)
        {
            // Re-check for ghosts if we haven't found any yet (they might spawn late)
            GameObject[] lateSpawnGhosts = GameObject.FindGameObjectsWithTag("Ghost");
            if (lateSpawnGhosts.Length > 0)
            {
                // Ghosts found! Update initial count
                initialGhostCount.Value = lateSpawnGhosts.Length;
                currentGhostCount.Value = initialGhostCount.Value;
                Debug.Log($"NetworkWinScreenManager: Found {initialGhostCount.Value} ghost(s) (late spawn).");
            }
            return; // Don't show win screen if there were never any ghosts
        }

        // Server checks ghost count periodically
        GameObject[] currentGhosts = GameObject.FindGameObjectsWithTag("Ghost");
        int count = currentGhosts.Length;
        
        if (count != currentGhostCount.Value)
        {
            currentGhostCount.Value = count;
            
            // Only show win screen if we had ghosts initially AND they're all gone
            if (count == 0 && initialGhostCount.Value > 0)
            {
                isLevelCleared.Value = true;
                Debug.Log($"NetworkWinScreenManager: All {initialGhostCount.Value} ghost(s) eliminated! Level cleared!");
            }
        }
    }

    private void OnGhostCountChanged(int oldCount, int newCount)
    {
        if (newCount == 0 && oldCount > 0 && initialGhostCount.Value > 0)
        {
            Debug.Log($"NetworkWinScreenManager: Ghost count changed to 0. Checking win condition...");
        }
    }

    private void OnLevelClearedChanged(bool oldValue, bool newValue)
    {
        if (newValue && !oldValue)
        {
            // Double-check: only show win screen if we actually had ghosts initially
            if (initialGhostCount.Value > 0)
            {
                ShowWinScreen();
                
                // Notify NetworkGameManager that level is complete
                if (NetworkGameManager.Instance != null)
                {
                    NetworkGameManager.Instance.OnLevelComplete();
                }
            }
            else
            {
                Debug.LogWarning("NetworkWinScreenManager: Win condition triggered but no ghosts were found initially. Not showing win screen.");
                // Reset the flag
                if (IsServer)
                {
                    isLevelCleared.Value = false;
                }
            }
        }
    }
    
    public bool IsLevelCleared()
    {
        return isLevelCleared.Value;
    }

    private void ShowWinScreen()
    {
        if (winScreenInstance != null)
        {
            winScreenInstance.SetActive(true);
            
            if (winScreenInstance.transform.parent != null)
            {
                winScreenInstance.transform.SetAsLastSibling();
            }
            
            CanvasGroup canvasGroup = winScreenInstance.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
            
            Debug.Log("NetworkWinScreenManager: Win screen displayed for all clients!");
        }
        else
        {
            Debug.LogError("NetworkWinScreenManager: Win screen instance is null!");
        }
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void NotifyGhostDestroyedServerRpc()
    {
        // This can be called when a ghost is destroyed to update count immediately
        GameObject[] currentGhosts = GameObject.FindGameObjectsWithTag("Ghost");
        currentGhostCount.Value = currentGhosts.Length;
        
        if (currentGhostCount.Value == 0 && initialGhostCount.Value > 0)
        {
            isLevelCleared.Value = true;
        }
    }
}
