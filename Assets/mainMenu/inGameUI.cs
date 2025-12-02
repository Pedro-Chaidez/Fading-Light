using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class inGameUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private GameObject confirmationPrompt;
    
    [SerializeField] private GameObject gameHUDPanel; 
    
    [Header("Scene Settings")]
    [SerializeField] private string mainMenuSceneName = "mainMenu";

    [Header("Volume Settings")]
    [SerializeField] private TMP_Text volValue = null;
    [SerializeField] private Slider volSlider = null;
    [SerializeField] private float defaultVol = 1.0f;

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

    public int mainControllerSen = 4; 
    public static bool IsGamePaused = false; 

    private void Awake()
    {
        EventSystem system = FindAnyObjectByType<EventSystem>();
        if (system == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            system = eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();
        }
        else
        {
            if (system.GetComponent<InputSystemUIInputModule>() == null)
            {
                var oldModule = system.GetComponent<StandaloneInputModule>();
                if (oldModule != null) Destroy(oldModule);
                
                system.gameObject.AddComponent<InputSystemUIInputModule>();
            }
        }
    }

    private void Start()
    {
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);
        if (confirmationPrompt != null) confirmationPrompt.SetActive(false);
        
        if (gameHUDPanel == null)
        {
            gameHUDPanel = GameObject.Find("GameHUD");
        }

        if (gameHUDPanel != null) gameHUDPanel.SetActive(true);

        Time.timeScale = 1f;
        IsGamePaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (resolutionDropdown != null)
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
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        LoadSavedSettings();
    }

    private void LoadSavedSettings()
    {
        if (volSlider) 
        {
            float v = PlayerPrefs.GetFloat("masterVolume", 1.0f);
            volSlider.value = v;
            if(volValue) volValue.text = v.ToString("0.0");
        }
        
        if (senSlider) 
        {
            float s = PlayerPrefs.GetFloat("masterSen", 4.0f);
            senSlider.value = s;
            if(controllerSenTextValue) controllerSenTextValue.text = s.ToString("0");
        }

        if (invertY) invertY.isOn = PlayerPrefs.GetInt("masterInvertY", 0) == 1;
        
        if (qualityDropdown) qualityDropdown.value = PlayerPrefs.GetInt("masterQuality", QualitySettings.GetQualityLevel());
        if (fullScreenToggle) fullScreenToggle.isOn = Screen.fullScreen;
        
        if (brightnessSlider) 
        {
            float b = PlayerPrefs.GetFloat("masterBrightness", 1.0f);
            brightnessSlider.value = b;
            if(brightnessTextValue) brightnessTextValue.text = b.ToString("0.0");
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (optionsMenuPanel != null && optionsMenuPanel.activeSelf)
            {
                OnBackFromOptions();
            }
            else
            {
                if (IsGamePaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }
        }
    }

    public void PauseGame()
    {
        IsGamePaused = true;
        Time.timeScale = 0f; 
        
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
        if (gameHUDPanel != null) gameHUDPanel.SetActive(false);
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        IsGamePaused = false;
        Time.timeScale = 1f; 
        
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);
        if (gameHUDPanel != null) gameHUDPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnExitToMenuClicked()
    {
        Time.timeScale = 1f; 
        IsGamePaused = false;
        if (NetworkManager.Singleton != null) 
        {
            NetworkManager.Singleton.Shutdown();
            Destroy(NetworkManager.Singleton.gameObject);
        }
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnOptionsClicked()
    {
        Debug.Log("OnOptionsClicked called");
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(true);
        else Debug.LogError("OptionsMenuPanel is not assigned in the inspector!");
    }

    public void OnBackFromOptions()
    {
        if (optionsMenuPanel != null) optionsMenuPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        if (volValue) volValue.text = volume.ToString("0.0");
    }

    public void VolumeApply()
    {
        PlayerPrefs.SetFloat("masterVolume", AudioListener.volume);
        StartCoroutine(ConfirmationBox());
    }

    public void SetControllerSen(float sensitivity)
    {
        mainControllerSen = Mathf.RoundToInt(sensitivity);
        if (controllerSenTextValue) controllerSenTextValue.text = sensitivity.ToString("0");
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
        if (brightnessTextValue) brightnessTextValue.text = brightness.ToString("0.0");
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
            if(brightnessSlider) brightnessSlider.value = defaultBrightness;
            if(brightnessTextValue) brightnessTextValue.text = defaultBrightness.ToString("0.0");

            if(qualityDropdown) qualityDropdown.value = 1;
            QualitySettings.SetQualityLevel(1);

            if(fullScreenToggle) fullScreenToggle.isOn = false;
            Screen.fullScreen = false;

            Resolution currentResolution = Screen.currentResolution;
            Screen.SetResolution(currentResolution.width, currentResolution.height, Screen.fullScreen);
            if(resolutionDropdown) resolutionDropdown.value = resolutions.Length;

            GraphicsApply();
        }
        if (menuType == "Audio")
        {
            AudioListener.volume = defaultVol;
            if(volSlider) volSlider.value = defaultVol;
            if(volValue) volValue.text = defaultVol.ToString("0.0");
            VolumeApply();
        }

        if (menuType == "Gameplay")
        {
            if(controllerSenTextValue) controllerSenTextValue.text = defaultSen.ToString("0");
            if(senSlider) senSlider.value = defaultSen;
            mainControllerSen = defaultSen;
            if(invertY) invertY.isOn = false;
            GameplayApply();
        }
    }

    public IEnumerator ConfirmationBox()
    {
        if (confirmationPrompt != null)
        {
            confirmationPrompt.SetActive(true);
            yield return new WaitForSecondsRealtime(2);
            confirmationPrompt.SetActive(false);
        }
    }
}