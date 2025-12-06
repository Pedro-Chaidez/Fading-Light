using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Collections; 
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Netcode.Transports.UTP; 

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [Header("Lobby Settings")]
    [SerializeField] private string gameSceneName = "TutorialLevel";
    [SerializeField] private int minPlayersToStart = 1;

    [Header("Visual Customization")]
    [SerializeField] private Color localPlayerColor = Color.green; 
    [SerializeField] private Color otherPlayerColor = Color.white;
    [SerializeField] private int listTopPadding = 150;
    [SerializeField] private int fontSize = 24;

    [Header("UI References")]
    [SerializeField] private GameObject lobbyPanel;       
    [SerializeField] private GameObject mainMenuPanel;    
    [SerializeField] private Transform playerListContainer; 
    [SerializeField] private GameObject playerListItemPrefab; 
    [SerializeField] private TMP_Text lobbyCodeText;      
    [SerializeField] private GameObject startGameButton;  

    [Header("Join Screen References")]
    [SerializeField] private GameObject joinPanel;
    [SerializeField] private Transform serverListContainer;
    [SerializeField] private GameObject serverButtonPrefab;
    
    [SerializeField] private GameObject joinSelectionButton; 

    private NetworkList<FixedString32Bytes> connectedPlayers = new NetworkList<FixedString32Bytes>();
    private HashSet<string> foundServers = new HashSet<string>(); 
    private string selectedServerIP;
    
    [Header("Player Name Settings")]
    [SerializeField] private bool useCustomNames = true;
    [SerializeField] private string defaultPlayerNamePrefix = "Player"; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject); 
        }
        Instance = this;
    }

    private void Start()
    {
        if (lobbyPanel != null) lobbyPanel.SetActive(false);
        if (joinPanel != null) joinPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        
        if (joinSelectionButton != null)
        {
            joinSelectionButton.SetActive(false); 
            
            UnityEngine.UI.Button btn = joinSelectionButton.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners(); 
                btn.onClick.AddListener(() => {
                    if (!string.IsNullOrEmpty(selectedServerIP))
                    {
                        ConnectToIP(selectedServerIP);
                    }
                    else
                    {
                        Debug.LogWarning("No server selected!");
                    }
                });
            }
            else
            {
                Debug.LogError("The Join Selection Button you dragged in does not have a Button component!");
            }
        }

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        if (LanDiscovery.Instance != null)
        {
            LanDiscovery.Instance.OnServerFound -= AddServerToList;
            LanDiscovery.Instance.OnServerFound += AddServerToList;
        }
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        if (LanDiscovery.Instance != null)
        {
            LanDiscovery.Instance.OnServerFound -= AddServerToList;
        }
        
        if (LanDiscovery.Instance != null)
        {
            LanDiscovery.Instance.StopBroadcasting();
            LanDiscovery.Instance.StopListening();
        }

        base.OnDestroy();
    }

    public async void CreateLobby()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton is null!");
            return;
        }

        if (NetworkManager.Singleton.IsListening) 
        {
            Debug.Log("NetworkManager was active. Shutting down...");
            NetworkManager.Singleton.Shutdown();
            
            // Wait for shutdown to complete
            await Task.Delay(200);
            
            // Additional check - wait until not listening
            int maxWait = 50; // Max 5 seconds
            while (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening && maxWait > 0)
            {
                await Task.Delay(100);
                maxWait--;
            }
        }

        // Ensure NetworkManager is still valid
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("NetworkManager.Singleton became null after shutdown!");
            return;
        }

        bool started = NetworkManager.Singleton.StartHost();
        if (started)
        {
            if (LanDiscovery.Instance != null) LanDiscovery.Instance.StartBroadcasting();
            ShowLobbyUI(true);
        }
        else
        {
            Debug.LogError($"Failed to start Host! IsListening: {NetworkManager.Singleton.IsListening}, IsClient: {NetworkManager.Singleton.IsClient}, IsHost: {NetworkManager.Singleton.IsHost}, IsServer: {NetworkManager.Singleton.IsServer}");
        }
    }

    public void JoinLobby()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (joinPanel != null) joinPanel.SetActive(true);
        
        if (joinSelectionButton != null) joinSelectionButton.SetActive(false);
        selectedServerIP = "";

        ClearServerList();
        if (LanDiscovery.Instance != null) LanDiscovery.Instance.StartListening();
    }

    private void ClearServerList()
    {
        foundServers.Clear();
        if (serverListContainer != null)
        {
            for (int i = serverListContainer.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(serverListContainer.GetChild(i).gameObject);
            }
        }
    }

    private void AddServerToList(string ipAddress)
    {
        if (foundServers.Contains(ipAddress)) return;
        
        foundServers.Add(ipAddress);

        if (serverListContainer == null || serverButtonPrefab == null) return;

        GameObject btn = Instantiate(serverButtonPrefab, serverListContainer);
        TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
        
        if (btnText != null) 
        {
            btnText.text = $"FOUND: {ipAddress}"; 
            btnText.enableAutoSizing = false;
            btnText.fontSize = fontSize; 
            btnText.fontStyle = FontStyles.Normal; 
            btnText.color = otherPlayerColor; 
        }

        UnityEngine.UI.Button btnComp = btn.GetComponent<UnityEngine.UI.Button>();
        if (btnComp != null)
        {
            btnComp.onClick.AddListener(() => {
                selectedServerIP = ipAddress;
                Debug.Log($"Selected Server IP: {ipAddress}");
                if (joinSelectionButton != null) joinSelectionButton.SetActive(true);
            });
        }
    }

    private async void ConnectToIP(string ip)
    {
        Debug.Log($"[LobbyManager] Connecting to IP: {ip}");
        
        if (LanDiscovery.Instance != null) LanDiscovery.Instance.StopListening();

        if (NetworkManager.Singleton.IsListening)
        {
             Debug.Log("[LobbyManager] Shutting down existing connection...");
             NetworkManager.Singleton.Shutdown();
             await System.Threading.Tasks.Task.Delay(100);
        }

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.SetConnectionData(ip, 7777); 
        }

        bool result = NetworkManager.Singleton.StartClient();
        if (!result) Debug.LogError("StartClient Failed!");
    }

    public void BackToMain()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (LanDiscovery.Instance != null) LanDiscovery.Instance.StopListening();
        
        if (joinPanel != null) joinPanel.SetActive(false);
        if (lobbyPanel != null) lobbyPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            if (!IsPlayerInList(clientId)) 
            {
                // Default name until client sends their name
                string defaultName = $"{defaultPlayerNamePrefix} {clientId}";
                connectedPlayers.Add(defaultName);
            }
        }
        
        if (clientId == NetworkManager.Singleton.LocalClientId) 
        {
            ShowLobbyUI(IsHost);
        }
    }

    private bool IsPlayerInList(ulong clientId)
    {
        foreach (var p in connectedPlayers)
        {
            if (p.ToString().Contains($"Player {clientId}")) return true;
        }
        return false;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            BackToMain();
            return;
        }

        if (IsServer)
        {
            for (int i = 0; i < connectedPlayers.Count; i++)
            {
                if (connectedPlayers[i].ToString().Contains($"Player {clientId}"))
                {
                    connectedPlayers.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            connectedPlayers.OnListChanged += OnPlayerListChanged;
            UpdatePlayerListUI();
            
            // Register this player's name
            if (IsOwner)
            {
                ulong localClientId = NetworkManager.Singleton.LocalClientId;
                RegisterPlayerNameServerRpc(GetPlayerName(), localClientId);
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient && connectedPlayers != null)
        {
            connectedPlayers.OnListChanged -= OnPlayerListChanged;
        }
        base.OnNetworkDespawn();
    }
    
    private string GetPlayerName()
    {
        if (useCustomNames)
        {
            // Try to get saved player name
            string savedName = PlayerPrefs.GetString("PlayerName", "");
            if (!string.IsNullOrEmpty(savedName))
            {
                return savedName;
            }
        }
        return $"{defaultPlayerNamePrefix} {NetworkManager.Singleton.LocalClientId}";
    }
    
    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RegisterPlayerNameServerRpc(FixedString32Bytes playerName, ulong clientId)
    {
        
        // Update the connected players list with the custom name
        bool found = false;
        for (int i = 0; i < connectedPlayers.Count; i++)
        {
            string currentName = connectedPlayers[i].ToString();
            if (currentName.Contains($"Player {clientId}") || currentName == playerName.ToString())
            {
                connectedPlayers[i] = playerName;
                found = true;
                break;
            }
        }
        if (!found)
        {
            connectedPlayers.Add(playerName);
        }
    }

    private void OnPlayerListChanged(NetworkListEvent<FixedString32Bytes> changeEvent)
    {
        UpdatePlayerListUI();
    }

    private void UpdatePlayerListUI()
    {
        if (playerListContainer == null) return;

        UnityEngine.UI.VerticalLayoutGroup layout = playerListContainer.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        if (layout != null)
        {
            layout.padding.top = listTopPadding; 
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(playerListContainer as RectTransform);
        }

        for (int i = playerListContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(playerListContainer.GetChild(i).gameObject);
        }

        foreach (var playerName in connectedPlayers)
        {
            GameObject item = Instantiate(playerListItemPrefab, playerListContainer);
            TMP_Text textComp = item.GetComponentInChildren<TMP_Text>();
            
            if (textComp != null) 
            {
                string pName = playerName.ToString();
                textComp.text = pName;
                
                textComp.enableAutoSizing = false; 
                textComp.fontSize = fontSize; 
                textComp.fontStyle = FontStyles.Normal; 
                
                // Check if this is the local player
                bool isLocalPlayer = false;
                if (NetworkManager.Singleton != null)
                {
                    ulong localId = NetworkManager.Singleton.LocalClientId;
                    string localName = GetPlayerName();
                    if (pName == localName || pName.Contains($"Player {localId}"))
                    {
                        isLocalPlayer = true;
                    }
                }
                
                textComp.color = isLocalPlayer ? localPlayerColor : otherPlayerColor;
            }
        }
    }

    private void ShowLobbyUI(bool isHost)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (joinPanel != null) joinPanel.SetActive(false);
        if (lobbyPanel != null) lobbyPanel.SetActive(true);
        if (startGameButton != null) startGameButton.SetActive(isHost);
    }

    public void OnStartGameClicked()
    {
        if (!IsHost) return;
        if (connectedPlayers.Count < minPlayersToStart) return; 
        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    public void LeaveLobby()
    {
        if (LanDiscovery.Instance != null) 
        {
            LanDiscovery.Instance.StopBroadcasting();
            LanDiscovery.Instance.StopListening();
        }

        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            Debug.Log("LeaveLobby: Shutting down NetworkManager...");
            NetworkManager.Singleton.Shutdown();
        }
        
        BackToMain(); 
        
        if (playerListContainer != null)
        {
            for (int i = playerListContainer.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(playerListContainer.GetChild(i).gameObject);
            }
        }
        
        // Clear connected players list
        if (IsServer && connectedPlayers != null)
        {
            connectedPlayers.Clear();
        }
    }
}