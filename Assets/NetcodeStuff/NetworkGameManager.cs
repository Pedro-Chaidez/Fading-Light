using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkGameManager : NetworkBehaviour
{
    public static NetworkGameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private string lobbySceneName = "mainMenu";
    [SerializeField] private bool autoSpawnWinScreenManager = true;
    [SerializeField] private GameObject winScreenManagerPrefab;
    
    [Header("Scene Progression")]
    [SerializeField] private bool enableAutoProgression = true;
    [SerializeField] private float levelCompleteDelay = 3f; // Seconds to wait before loading next level
    private string currentSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Update current scene name when scene loads
        currentSceneName = scene.name;
        hasTransitioned = false; // Reset transition flag
        Debug.Log($"NetworkGameManager: Scene loaded: {currentSceneName}");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        // Store current scene name
        currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        hasTransitioned = false; // Reset transition flag for new scene
        Debug.Log($"NetworkGameManager: Spawned in scene: {currentSceneName}. Auto-progression: {enableAutoProgression}");
        
        if (IsServer && autoSpawnWinScreenManager)
        {
            SpawnWinScreenManager();
        }
        
        if (IsServer)
        {
            SpawnGhostManager();
        }
    }

    private void SpawnWinScreenManager()
    {
        if (winScreenManagerPrefab != null)
        {
            GameObject winManager = Instantiate(winScreenManagerPrefab);
            NetworkObject winManagerNetworkObj = winManager.GetComponent<NetworkObject>();
            if (winManagerNetworkObj != null)
            {
                winManagerNetworkObj.Spawn();
            }
        }
        else
        {
            // Try to find existing one or create basic one
            if (NetworkWinScreenManager.Instance == null)
            {
                GameObject winManagerObj = new GameObject("NetworkWinScreenManager");
                winManagerObj.AddComponent<NetworkWinScreenManager>();
                NetworkObject netObj = winManagerObj.AddComponent<NetworkObject>();
                netObj.Spawn();
            }
        }
    }

    private void SpawnGhostManager()
    {
        if (NetworkGhostManager.Instance == null)
        {
            GameObject ghostManagerObj = new GameObject("NetworkGhostManager");
            ghostManagerObj.AddComponent<NetworkGhostManager>();
            NetworkObject netObj = ghostManagerObj.AddComponent<NetworkObject>();
            netObj.Spawn();
        }
    }

    public void ReturnToLobby()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(lobbySceneName, LoadSceneMode.Single);
        }
    }

    public void DisconnectAll()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
    
    private bool hasTransitioned = false;
    
    private System.Collections.IEnumerator TransitionToNextLevel()
    {
        // Prevent multiple transitions
        enableAutoProgression = false;
        
        Debug.Log("NetworkGameManager: Tutorial complete! Transitioning to Level1 in " + levelCompleteDelay + " seconds...");
        
        // Wait for delay (so players can see win screen)
        yield return new WaitForSeconds(levelCompleteDelay);
        
        // Load next level
        if (IsServer)
        {
            Debug.Log("NetworkGameManager: Loading Level1...");
            NetworkManager.Singleton.SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        }
    }
    
    public void OnLevelComplete()
    {
        if (!IsServer) return;
        if (!enableAutoProgression) return;
        if (hasTransitioned) return; // Prevent multiple transitions
        
        // Check if we should progress to next level
        if (currentSceneName == "TutorialLevel")
        {
            hasTransitioned = true;
            StartCoroutine(TransitionToNextLevel());
        }
    }
}
