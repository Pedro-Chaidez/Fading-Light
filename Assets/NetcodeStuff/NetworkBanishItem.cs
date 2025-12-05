using Unity.Netcode;
using UnityEngine;

public class NetworkBanishItem : Interactable
{
    protected override void Interact()
    {
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        if (ghosts.Length > 0)
        {
            GameObject ghostToDestroy = ghosts[0];
            
            // Check if we're in a networked game
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                // Use network manager to destroy
                if (NetworkGhostManager.Instance != null)
                {
                    NetworkGhostManager.Instance.DestroyGhost(ghostToDestroy);
                }
                else
                {
                    // Fallback: destroy on server
                    if (NetworkManager.Singleton.IsServer)
                    {
                        NetworkObject ghostNetworkObj = ghostToDestroy.GetComponent<NetworkObject>();
                        if (ghostNetworkObj != null)
                        {
                            ghostNetworkObj.Despawn();
                        }
                        else
                        {
                            Destroy(ghostToDestroy);
                        }
                    }
                }
            }
            else
            {
                // Single player mode
                Destroy(ghostToDestroy);
            }
        }
    }
}
