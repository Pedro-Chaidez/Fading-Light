using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Collections; 
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [Header("Lobby Settings")]
    [SerializeField] private string gameSceneName = "Main Game";
    [SerializeField] private int minPlayersToStart = 1;

    [Header("Visual Customization")]
    [SerializeField] private Color localPlayerColor = Color.green; 
    [SerializeField] private Color otherPlayerColor = Color.white;

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

    private NetworkList<FixedString32Bytes> connectedPlayers;

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
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        base.OnDestroy();
    }

    public void CreateLobby()
    {
        NetworkManager.Singleton.StartHost();
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
        //RefreshServerList();
    }

    /*public void RefreshServerList()
    {
        if (serverListContainer == null || serverButtonPrefab == null) return;

        for (int i = serverListContainer.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(serverListContainer.GetChild(i).gameObject);
        }

        GameObject btn = Instantiate(serverButtonPrefab, serverListContainer);
        TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
        if (btnText != null) btnText.text = "Local Game (Click to Join)";

        btn.GetComponent<Button>().onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    } */

    public void BackToMain()
    {
        if (joinPanel != null) joinPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    private void OnClientConnected(ulong clientId)
    {
        if (IsServer)
        {
            if (!IsPlayerInList(clientId)) connectedPlayers.Add($"Player {clientId}");
        }
        if (clientId == NetworkManager.Singleton.LocalClientId) ShowLobbyUI(IsHost);
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

        // 1. REVERSE LOOP (The only way to delete correctly)
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
                
                // --- VISUAL FIXES ---
                textComp.enableAutoSizing = false; // Turn off auto-size so we can control it
                textComp.fontSize = 24;            // Force font size to 24 (Make this smaller if needed)
                
                if (NetworkManager.Singleton != null && pName.Contains($"Player {NetworkManager.Singleton.LocalClientId}"))
                {
                    textComp.color = localPlayerColor;
                    textComp.fontStyle = FontStyles.Normal; 
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