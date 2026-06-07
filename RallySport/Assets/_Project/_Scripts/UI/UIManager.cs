using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Templates")]
    [SerializeField] private VisualTreeAsset mainMenuTemplate;
    [SerializeField] private VisualTreeAsset levelSelectionTemplate;
    [SerializeField] private VisualTreeAsset settingsTemplate;
    [SerializeField] private VisualTreeAsset creditsTemplate;
    [SerializeField] private VisualTreeAsset controlsTemplate;

    [Header("Level Scene Configuration")]
    [Tooltip("List of scene names corresponding to each level button in order (1 to 5).")]
    [SerializeField] private string[] levelSceneNames = new string[] { "Level1", "LevelSnow", "Level3", "Level4", "Level5" };

    private UIDocument uiDocument;
    private VisualElement root;

    private VisualElement notificationOverlay;
    private IVisualElementScheduledItem notificationTimer;
    private System.Action returnToScreenAction;

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component is missing on UIManager!");
            return;
        }

        root = uiDocument.rootVisualElement;

        AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        
        if (PlayerPrefs.GetInt("ShowLevelSelection", 0) == 1)
        {
            PlayerPrefs.SetInt("ShowLevelSelection", 0);
            PlayerPrefs.Save();
            ShowLevelSelection();
        }
        else
        {
            ShowMainMenu();
        }
    }

    private void SwitchView(VisualTreeAsset template)
    {
        if (template == null)
        {
            Debug.LogError("Trying to switch to a null UI Template!");
            return;
        }

        root.Clear();

        var templateContainer = template.Instantiate();
        templateContainer.style.flexGrow = 1f;
        root.Add(templateContainer);
    }

    public void ShowMainMenu()
    {
        HideNotification();

        if (mainMenuTemplate == null)
        {
            Debug.LogError("MainMenuTemplate is not assigned in UIManager!");
            return;
        }

        SwitchView(mainMenuTemplate);

        var playButton = root.Q<Button>("PlayButton");
        var settingsButton = root.Q<Button>("SettingsButton");
        var creditsButton = root.Q<Button>("CreditsButton");
        var exitButton = root.Q<Button>("ExitButton");

        if (playButton != null)
        {
            playButton.clicked -= OnPlayClicked;
            playButton.clicked += OnPlayClicked;
        }
        if (settingsButton != null)
        {
            settingsButton.clicked -= OnSettingsClickedFromMainMenu;
            settingsButton.clicked += OnSettingsClickedFromMainMenu;
        }
        if (creditsButton != null)
        {
            creditsButton.clicked -= OnCreditsClicked;
            creditsButton.clicked += OnCreditsClicked;
        }
        if (exitButton != null)
        {
            exitButton.clicked -= OnExitClicked;
            exitButton.clicked += OnExitClicked;
        }
    }

    public void ShowLevelSelection()
    {
        if (levelSelectionTemplate == null)
        {
            Debug.LogError("LevelSelectionTemplate is not assigned in UIManager!");
            return;
        }

        SwitchView(levelSelectionTemplate);

        var goBackMenuButton = root.Q<Button>("GoBackMenuButton");
        var settingsButton = root.Q<Button>("SettingsButton");
        var exitGameButton = root.Q<Button>("ExitGameButton");

        if (goBackMenuButton != null)
        {
            goBackMenuButton.clicked -= ShowMainMenu;
            goBackMenuButton.clicked += () => {
                AudioManager.Source?.SelectButton();
                ShowMainMenu();
            };
        }
        if (settingsButton != null)
        {
            settingsButton.clicked -= OnSettingsClickedFromLevelSelection;
            settingsButton.clicked += OnSettingsClickedFromLevelSelection;
        }
        if (exitGameButton != null)
        {
            exitGameButton.clicked -= OnExitClicked;
            exitGameButton.clicked += OnExitClicked;
        }

        notificationOverlay = root.Q<VisualElement>("NotificationOverlay");
        var closeButton = root.Q<Button>("NotificationCloseButton");

        if (closeButton != null && notificationOverlay != null)
        {
            closeButton.clicked += () => {
                AudioManager.Source?.SelectButton();
                HideNotification();
            };
        }

        for (int i = 1; i <= 5; i++)
        {
            int levelIndex = i;
            var levelContainer = root.Q<Button>($"LevelContainer{levelIndex}");
            if (levelContainer != null)
            {
                if (levelContainer.ClassListContains("is-locked"))
                {
                    levelContainer.clicked += () => {
                        AudioManager.Source?.SelectButton();
                        OnLevelLockedClicked(levelIndex);
                    };
                }
                else
                {
                    levelContainer.clicked += () => {
                        AudioManager.Source?.SelectButton();
                        LoadLevel(levelIndex);
                    };
                }
            }
        }
    }

    public void ShowSettings()
    {
        if (settingsTemplate == null)
        {
            Debug.LogError("SettingsTemplate is not assigned in UIManager!");
            return;
        }

        SwitchView(settingsTemplate);

        var settingsButtonsContainer = root.Q<VisualElement>("SettingsButtonsContainer");
        var audioPanelContainer = root.Q<VisualElement>("AudioPanelContainer");

        var audioButton = root.Q<Button>("AudioButton");
        var controlsButton = root.Q<Button>("ControlsButton");
        var closeSettingsButton = root.Q<Button>("CloseSettingsButton");

        var audioBackButton = root.Q<Button>("AudioBackButton");
        var masterVolumeSlider = root.Q<Slider>("MasterVolumeSlider");
        var musicVolumeSlider = root.Q<Slider>("MusicVolumeSlider");
        var sfxVolumeSlider = root.Q<Slider>("SFXVolumeSlider");

       
        if (masterVolumeSlider != null) masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        if (musicVolumeSlider != null) musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.9f);

       
        if (audioButton != null && settingsButtonsContainer != null && audioPanelContainer != null)
        {
            audioButton.clicked += () =>
            {
                AudioManager.Source?.SelectButton();
                settingsButtonsContainer.AddToClassList("is-hidden");
                audioPanelContainer.RemoveFromClassList("is-hidden");
            };
        }

        if (controlsButton != null)
        {
            controlsButton.clicked += () => {
                AudioManager.Source?.SelectButton();
                ShowControls(ShowSettings);
            };
        }

        if (audioBackButton != null && settingsButtonsContainer != null && audioPanelContainer != null)
        {
            audioBackButton.clicked += () =>
            {
                AudioManager.Source?.SelectButton();
                audioPanelContainer.AddToClassList("is-hidden");
                settingsButtonsContainer.RemoveFromClassList("is-hidden");
            };
        }

        if (closeSettingsButton != null)
        {
            closeSettingsButton.clicked += () =>
            {
                AudioManager.Source?.SelectButton();
                if (returnToScreenAction != null)
                {
                    returnToScreenAction.Invoke();
                }
                else
                {
                    ShowMainMenu();
                }
            };
        }

       
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                PlayerPrefs.SetFloat("MasterVolume", evt.newValue);
                PlayerPrefs.Save();
                AudioListener.volume = evt.newValue; // Apply real-time change to Unity global volume
                Debug.Log($"Master Volume Changed: {evt.newValue}");
            });
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                PlayerPrefs.SetFloat("MusicVolume", evt.newValue);
                PlayerPrefs.Save();
                Debug.Log($"Music Volume Changed: {evt.newValue}");
            });
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                PlayerPrefs.SetFloat("SFXVolume", evt.newValue);
                PlayerPrefs.Save();
                Debug.Log($"SFX Volume Changed: {evt.newValue}");
            });
        }
    }

    private void OnPlayClicked()
    {
        AudioManager.Source?.PlayButton();
        ShowLevelSelection();
    }

    private void OnSettingsClickedFromMainMenu()
    {
        AudioManager.Source?.SelectButton();
        returnToScreenAction = ShowMainMenu;
        ShowSettings();
    }

    private void OnSettingsClickedFromLevelSelection()
    {
        AudioManager.Source?.SelectButton();
        returnToScreenAction = ShowLevelSelection;
        ShowSettings();
    }

    private void OnCreditsClicked()
    {
        AudioManager.Source?.SelectButton();
        ShowCredits();
    }

    public void ShowCredits()
    {
        if (creditsTemplate == null)
        {
            Debug.LogError("CreditsTemplate is not assigned in UIManager!");
            return;
        }

        SwitchView(creditsTemplate);

        var closeCreditsButton = root.Q<Button>("CloseCreditsButton");
        if (closeCreditsButton != null)
        {
            closeCreditsButton.clicked += () => {
                AudioManager.Source?.SelectButton();
                ShowMainMenu();
            };
        }
    }

    public void ShowControls(System.Action onReturn)
    {
        if (controlsTemplate == null)
        {
            Debug.LogError("ControlsTemplate is not assigned in UIManager!");
            return;
        }

        SwitchView(controlsTemplate);

        var closeControlsButton = root.Q<Button>("CloseControlsButton");
        if (closeControlsButton != null)
        {
            closeControlsButton.clicked += () => {
                AudioManager.Source?.SelectButton();
                if (onReturn != null)
                {
                    onReturn.Invoke();
                }
                else
                {
                    ShowMainMenu();
                }
            };
        }
    }

    private void OnExitClicked()
    {
        AudioManager.Source?.ExitButton();
        Debug.Log("Exit Game Clicked");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void OnLevelLockedClicked(int levelIndex)
    {
        Debug.LogWarning($"Level {levelIndex} is locked! Complete previous levels first.");

        if (notificationOverlay != null)
        {
            if (notificationTimer != null)
            {
                notificationTimer.Pause();
                notificationTimer = null;
            }

            notificationOverlay.AddToClassList("is-visible");

            notificationTimer = notificationOverlay.schedule.Execute(HideNotification).StartingIn(2500);
        }
    }

    private void HideNotification()
    {
        if (notificationOverlay != null)
        {
            notificationOverlay.RemoveFromClassList("is-visible");
        }
        if (notificationTimer != null)
        {
            notificationTimer.Pause();
            notificationTimer = null;
        }
    }

    private void LoadLevel(int levelIndex)
    {
        int arrayIndex = levelIndex - 1;
        string sceneName = "";

        if (levelSceneNames != null && arrayIndex >= 0 && arrayIndex < levelSceneNames.Length)
        {
            sceneName = levelSceneNames[arrayIndex];
        }

        if (string.IsNullOrEmpty(sceneName))
        {
            sceneName = "Level" + levelIndex;
            Debug.LogWarning($"Level scene name not configured for index {levelIndex}. Falling back to default: {sceneName}");
        }
        else
        {
            Debug.Log($"Loading Level {levelIndex} ({sceneName})...");
        }

        SceneManager.LoadScene(sceneName);
    }
}
