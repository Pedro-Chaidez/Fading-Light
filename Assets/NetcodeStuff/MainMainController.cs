using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // For loading scenes
using Unity.Netcode;
using UnityEngine.UI;
using Unity.VisualScripting;
using TMPro;     

public class MenuController : MonoBehaviour
{
    [Header("Scene To Load")]
    [SerializeField] private string singlePlayerSceneName = "Main Game";



    [Header("UI Panels")]
    [SerializeField] private GameObject optionsMenuPanel = null;



    [Header("Volume Settings")]
    [SerializeField] private TMP_Text volValue = null;
    [SerializeField] private Slider volSlider = null;
    [SerializeField] private float defaultVol = 1.0f;



    [Header("Confirmation")]
    [SerializeField] private GameObject confirmationPrompt = null;
    public int mainControllerSen = 4;



    [Header("Toggle Settings")]
    [SerializeField] private Toggle invertY = null;



    [Header("GamePlay Setting")]
    [SerializeField] private TMP_Text controllerSenTextValue = null;
    [SerializeField] private Slider senSlider = null;
    [SerializeField] private int defaultSen = 4;


    [Header("Graphic Setting")]
    [SerializeField] private Slider brightnessSlider = null;
    [SerializeField] private TMP_Text brightnessTextValue = null;
    [SerializeField] private float defaultBrightness = 1;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    private int _qualityLevel;
    private bool _isFullscreen;
    private float _brightnessLevel;

    [Header("Resolution Dropdown")]
    public TMP_Dropdown resolutionDropdown;
    private Resolution[] resolutions;

    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    
    public void OnSinglePlayer()
    {
        // Shutdown any active netcode session for pure singleplayer
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }
        SceneManager.LoadScene(singlePlayerSceneName);
    }

    public void OnHostGame()
    {
        // OLD: SceneManager.LoadScene("Main Game");  <-- DELETED
        // OLD: NetworkManager.Singleton.StartHost(); <-- DELETED

        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.CreateLobby();
        }
        else
        {
            Debug.LogError("LobbyManager is missing from the scene!");
        }
    }

    public void OnJoinGame()
    {
      
        if (LobbyManager.Instance != null)
        {
            LobbyManager.Instance.JoinLobby();
        }
        else
        {
            Debug.LogError("LobbyManager is missing from the scene!");
        }
    }
    public void OnOptionsButton()
    {
        if (optionsMenuPanel != null)
        {
            optionsMenuPanel.SetActive(!optionsMenuPanel.activeSelf);
        }
    }

    public void OnExitGame()
    {
        Application.Quit(); 
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        volValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);
        controllerSenTextValue.text = sensitivity.ToString("0");
    }

    public void GameplayApply()
    {
        if (invertY.isOn)
        {
            PlayerPrefs.SetInt("masterInvertY", 1);
        }
        else
        {
            PlayerPrefs.SetInt("masterInvertY", 0);
        }

        PlayerPrefs.SetFloat("masterSen", mainControllerSen);
        StartCoroutine(ConfirmationBox());
    }

    public void SetBrightness(float brightness)
    {
        _brightnessLevel = brightness;
        brightnessTextValue.text = brightness.ToString("0.0");
    }

    public void SetFullscreen(bool isFullscreen)
    {
        _isFullscreen = isFullscreen;
    }

    public void SetQuality(int qualityIndex)
    {

        _qualityLevel = qualityIndex;
    }

    public void GraphicsApply()
    {
        PlayerPrefs.SetFloat("masterBrightness", _brightnessLevel);

        PlayerPrefs.SetInt("masterQuality", _qualityLevel);
        QualitySettings.SetQualityLevel(_qualityLevel);

        PlayerPrefs.SetInt("masterFullscreen", (_isFullscreen ? 1 : 0));
        Screen.fullScreen = _isFullscreen;

        StartCoroutine(ConfirmationBox());
    }
    public void resetButton(string menuType)
    {
        if (menuType == "Graphics")
        {
            brightnessSlider.value = defaultBrightness;
            brightnessTextValue.text = defaultBrightness.ToString("0.0");

            qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            fullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            resolutionDropdown.value = resolutions.Length;

            GraphicsApply();
        }
        if (menuType == "Audio")
        {
            AudioListener.volume = defaultVol;
            volSlider.value = defaultVol;
            volValue.text = defaultVol.ToString("0.0");
            VolumeApply();
        }

        if (menuType == "Gameplay")
        {
            controllerSenTextValue.text = defaultSen.ToString("0");
            senSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            invertY.isOn = false;
            GameplayApply();
        }
    }



    public IEnumerator ConfirmationBox()
    {
        confirmationPrompt.SetActive(true);
        yield return new WaitForSeconds(2);
        confirmationPrompt.SetActive(false);
    }

}