using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    public GameObject TitleScreenUI;
    public GameObject inGameUI;

    // We no longer need playerArmature or cameraLookScript!

    [SerializeField] private Button serverBtn;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button clientBtn;

    private void Awake()
    {
        // No more disabling scripts, the player doesn't exist yet!

        if (serverBtn != null)
        {
            serverBtn.onClick.AddListener(() =>
            {
                TitleScreenUI.SetActive(false);
                inGameUI.SetActive(true);
                // The player spawn is handled by NetworkManager
                NetworkManager.Singleton.StartServer();
            });
        }

        if (hostBtn != null)
        {
            hostBtn.onClick.AddListener(() =>
            {
                TitleScreenUI.SetActive(false);
                inGameUI.SetActive(true);
                // The player spawn is handled by NetworkManager
                NetworkManager.Singleton.StartHost();
            });
        }

        if (clientBtn != null)
        {
            clientBtn.onClick.AddListener(() =>
            {
                TitleScreenUI.SetActive(false);
                inGameUI.SetActive(true);
                // The player spawn is handled by NetworkManager
                NetworkManager.Singleton.StartClient();
            });
        }
    }
}