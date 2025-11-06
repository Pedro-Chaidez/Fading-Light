using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    public GameObject TitleScreenUI;
    public GameObject inGameUI;


    [SerializeField] private UnityEngine.UI.Button serverBtn;
    [SerializeField] private UnityEngine.UI.Button hostBtn;
    [SerializeField] private UnityEngine.UI.Button clientBtn;

    private void Awake()
    {

        if (serverBtn != null)
        {
            serverBtn.onClick.AddListener(() =>
            {
                TitleScreenUI.SetActive(false);
                inGameUI.SetActive(true);
                NetworkManager.Singleton.StartServer();
            });
        }

        if (hostBtn != null)
        {
            hostBtn.onClick.AddListener(() =>
            {
                TitleScreenUI.SetActive(false);
                inGameUI.SetActive(true);
                NetworkManager.Singleton.StartHost();
            });
        }

        if (clientBtn != null)
        {
            clientBtn.onClick.AddListener(() =>
            {
                TitleScreenUI.SetActive(false);
                inGameUI.SetActive(true);
                NetworkManager.Singleton.StartClient();
            });
        }
    }
}