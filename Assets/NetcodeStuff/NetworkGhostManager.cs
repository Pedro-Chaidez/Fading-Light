using Unity.Netcode;
using UnityEngine;

public class NetworkGhostManager : NetworkBehaviour
{
    public static NetworkGhostManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    public void DestroyGhostServerRpc(ulong ghostNetworkId)
    {
        NetworkObject ghostObj = null;
        
        // Try to find the network object
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(ghostNetworkId, out ghostObj))
        {
            if (ghostObj != null && ghostObj.gameObject.CompareTag("Ghost"))
            {
                ghostObj.Despawn();
                Debug.Log($"NetworkGhostManager: Ghost {ghostNetworkId} destroyed by server.");
                
                // Notify win screen manager
                if (NetworkWinScreenManager.Instance != null)
                {
                    NetworkWinScreenManager.Instance.NotifyGhostDestroyedServerRpc();
                }
            }
        }
    }

    public void DestroyGhost(GameObject ghost)
    {
        if (!IsClient) return;
        
        NetworkObject ghostNetworkObj = ghost.GetComponent<NetworkObject>();
        if (ghostNetworkObj != null)
        {
            DestroyGhostServerRpc(ghostNetworkObj.NetworkObjectId);
        }
        else
        {
            // Fallback for non-networked ghosts (single player compatibility)
            if (IsServer)
            {
                Destroy(ghost);
            }
        }
    }
}
