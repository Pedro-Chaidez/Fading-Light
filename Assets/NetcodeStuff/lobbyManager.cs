using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Collections; 
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; 
using Unity.Netcode.Transports.UTP; 

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [Header("Lobby Settings")]
    [SerializeField] private string gameSceneName = "Main Game";
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

    private NetworkList<FixedString32Bytes> connectedPlayers;
    private HashSet<string> foundServers = new HashSet<string>(); 
    private string selectedServerIP; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        connectedPlayers = new NetworkList<FixedString32Bytes>();
    }

    private void Start()
    {
        if (lobbyPanel != null) lobbyPanel.SetActive(false);
        if (joinPanel != null) joinPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);

        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        if (LanDiscovery.Instance != null)
        {
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
        base.OnDestroy();
    }

    public void CreateLobby()
    {
        if (NetworkManager.Singleton.IsListening) NetworkManager.Singleton.Shutdown();

        NetworkManager.Singleton.StartHost();
        
        if (LanDiscovery.Instance != null) LanDiscovery.Instance.StartBroadcasting();

        ShowLobbyUI(true);
        
        if (IsServer) 
        {
            connectedPlayers.Clear();
            connectedPlayers.Add($"Player {NetworkManager.Singleton.LocalClientId} (Host)");
        }
    }

    public void JoinLobby()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (joinPanel != null) joinPanel.SetActive(true);
        
        selectedServerIP = "";
        
        if (joinSelectionButton != null)
        {
            joinSelectionButton.SetActive(false);
            UnityEngine.UI.Button btn = joinSelectionButton.GetComponent<UnityEngine.UI.Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => {
                    Debug.Log($"[LobbyManager] Join button clicked! Selected IP: {selectedServerIP}");
                    if (!string.IsNullOrEmpty(selectedServerIP))
                    {
                        ConnectToIP(selectedServerIP);
                    }
                });
                Debug.Log("[LobbyManager] Join button listener added");
            }
        }

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
                Debug.Log($"Selected Server: {ipAddress}");
                if (joinSelectionButton != null) joinSelectionButton.SetActive(true);
            });
        }
    }

    private void ConnectToIP(string ip)
    {
        Debug.Log($"[LobbyManager] Connecting to IP: {ip}");
        
        if (LanDiscovery.Instance != null) LanDiscovery.Instance.StopListening();

        if (NetworkManager.Singleton.IsListening)
        {
            Debug.Log("[LobbyManager] Shutting down existing connection before joining");
            NetworkManager.Singleton.Shutdown();
            StartCoroutine(ConnectAfterShutdown(ip));
            return;
        }

        AttemptConnection(ip);
    }

    private System.Collections.IEnumerator ConnectAfterShutdown(string ip)
    {
        yield return new WaitForSeconds(0.5f);
        AttemptConnection(ip);
    }

    private void AttemptConnection(string ip)
    {
        Debug.Log($"[LobbyManager] Attempting connection to {ip}:7777");
        
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (transport != null)
        {
            transport.SetConnectionData(ip, 7777);
            Debug.Log($"[LobbyManager] Transport configured for {ip}:7777");
        }
        else
        {
            Debug.LogError("[LobbyManager] UnityTransport component not found on NetworkManager!");
            return;
        }

        bool started = NetworkManager.Singleton.StartClient();
        Debug.Log($"[LobbyManager] StartClient() returned: {started}");
        
        if (!started)
        {
            Debug.LogError("[LobbyManager] Failed to start client!");
        }
    }

    public void BackToMain()
    {
        if (LanDiscovery.Instance != null) LanDiscovery.Instance.StopListening();
        if (joinPanel != null) joinPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    private void OnClientConnected(ulong clientId)
    {
        Debug.Log($"[LobbyManager] Client connected: {clientId}, LocalClientId: {NetworkManager.Singleton.LocalClientId}, IsServer: {IsServer}");
        
        if (IsServer)
        {
            if (!IsPlayerInList(clientId))
            {
                connectedPlayers.Add($"Player {clientId}");
                Debug.Log($"[LobbyManager] Added player {clientId} to list. Total players: {connectedPlayers.Count}");
            }
        }
        
        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            Debug.Log($"[LobbyManager] THIS IS ME! Local client connected. IsHost: {IsHost}, showing lobby UI...");
            ShowLobbyUI(IsHost);
            
            if (lobbyPanel != null)
            {
                Debug.Log($"[LobbyManager] Lobby panel state after ShowLobbyUI: {lobbyPanel.activeSelf}");
            }
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
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            connectedPlayers.OnListChanged -= OnPlayerListChanged;
        }
    }

    private void OnPlayerListChanged(NetworkListEvent<FixedString32Bytes> changeEvent)
    {
        UpdatePlayerListUI();
    }

    private void UpdatePlayerListUI()
    {
        if (playerListContainer == null) return;

        VerticalLayoutGroup layout = playerListContainer.GetComponent<VerticalLayoutGroup>();
        if (layout != null)
        {
            layout.padding.top = listTopPadding;
            LayoutRebuilder.ForceRebuildLayoutImmediate(playerListContainer as RectTransform);
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
                
                if (NetworkManager.Singleton != null && pName.Contains($"Player {NetworkManager.Singleton.LocalClientId}"))
                {
                    textComp.color = localPlayerColor;
                }
                else
                {
                    textComp.color = otherPlayerColor;
                }
            }
        }
    }

    private void ShowLobbyUI(bool isHost)
    {
        Debug.Log($"[LobbyManager] ShowLobbyUI called. IsHost: {isHost}");
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (joinPanel != null) joinPanel.SetActive(false);
        if (lobbyPanel != null) lobbyPanel.SetActive(true);
        if (startGameButton != null) startGameButton.SetActive(isHost);
        Debug.Log($"[LobbyManager] Lobby UI shown. Panel active: {lobbyPanel != null && lobbyPanel.activeSelf}");
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

        if (NetworkManager.Singleton != null) NetworkManager.Singleton.Shutdown();
        
        if (lobbyPanel != null) lobbyPanel.SetActive(false);
        if (joinPanel != null) joinPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        
        if (playerListContainer != null)
        {
            for (int i = playerListContainer.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(playerListContainer.GetChild(i).gameObject);
            }
        }
    }
}