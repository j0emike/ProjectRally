using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class UIPauseManager : MonoBehaviour
{
    [Header("UI Templates")]
    [SerializeField] private VisualTreeAsset settingsTemplate;
    [SerializeField] private VisualTreeAsset controlsTemplate;

    private UIDocument uiDocument;
    private VisualElement root;
    private VisualElement pauseButtonsContainer;
    private VisualElement subPanelContainer;

    private bool isPaused = false;

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component is missing on UIPauseManager!");
            return;
        }

        root = uiDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("Root VisualElement is null on UIPauseManager!");
            return;
        }

        pauseButtonsContainer = root.Q<VisualElement>("PauseButtonsContainer");
        subPanelContainer = root.Q<VisualElement>("SubPanelContainer");

        var resumeButton = root.Q<Button>("ResumeButton");
        var settingsButton = root.Q<Button>("SettingsButton");
        var mainMenuButton = root.Q<Button>("MainMenuButton");

        if (resumeButton != null) resumeButton.clicked += () => ResumeGame();
        if (settingsButton != null) settingsButton.clicked += () => ShowSettingsInPause();
        if (mainMenuButton != null) mainMenuButton.clicked += () => ReturnToMainMenu();

        root.style.display = DisplayStyle.None;
    }

    private void Update()
    {
        bool escapePressed = false;
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null)
        {
            escapePressed = Keyboard.current.escapeKey.wasPressedThisFrame;
        }
#else
        escapePressed = Input.GetKeyDown(KeyCode.Escape);
#endif

        if (escapePressed)
        {
            if (isPaused)
            {
                if (subPanelContainer != null && !subPanelContainer.ClassListContains("is-hidden"))
                {
                    CloseSubPanel();
                }
                else
                {
                    ResumeGame();
                }
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        if (root == null) return;

        isPaused = true;
        Time.timeScale = 0f;
        
        root.style.display = DisplayStyle.Flex;
        if (pauseButtonsContainer != null) pauseButtonsContainer.RemoveFromClassList("is-hidden");
        if (subPanelContainer != null)
        {
            subPanelContainer.AddToClassList("is-hidden");
            subPanelContainer.Clear();
        }

        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
    }

    public void ResumeGame()
    {
        if (root == null) return;

        isPaused = false;
        Time.timeScale = 1f;
        
        root.style.display = DisplayStyle.None;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void CloseSubPanel()
    {
        if (subPanelContainer != null)
        {
            subPanelContainer.AddToClassList("is-hidden");
            subPanelContainer.Clear();
        }
        if (pauseButtonsContainer != null)
        {
            pauseButtonsContainer.RemoveFromClassList("is-hidden");
        }
    }

    private void ShowSettingsInPause()
    {
        if (settingsTemplate == null)
        {
            Debug.LogError("settingsTemplate is not assigned on UIPauseManager!");
            return;
        }

        if (subPanelContainer == null || pauseButtonsContainer == null) return;

        subPanelContainer.Clear();
        var settingsInstance = settingsTemplate.Instantiate();
        settingsInstance.style.flexGrow = 1f;
        subPanelContainer.Add(settingsInstance);

        pauseButtonsContainer.AddToClassList("is-hidden");
        subPanelContainer.RemoveFromClassList("is-hidden");

        var masterVolumeSlider = settingsInstance.Q<Slider>("MasterVolumeSlider");
        var musicVolumeSlider = settingsInstance.Q<Slider>("MusicVolumeSlider");
        var sfxVolumeSlider = settingsInstance.Q<Slider>("SFXVolumeSlider");

        if (masterVolumeSlider != null) masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 0.8f);
        if (musicVolumeSlider != null) musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.9f);

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                PlayerPrefs.SetFloat("MasterVolume", evt.newValue);
                PlayerPrefs.Save();
                AudioListener.volume = evt.newValue; 
            });
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                PlayerPrefs.SetFloat("MusicVolume", evt.newValue);
                PlayerPrefs.Save();
            });
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.RegisterValueChangedCallback(evt =>
            {
                PlayerPrefs.SetFloat("SFXVolume", evt.newValue);
                PlayerPrefs.Save();
            });
        }

        var settingsButtonsSub = settingsInstance.Q<VisualElement>("SettingsButtonsContainer");
        var audioPanelSub = settingsInstance.Q<VisualElement>("AudioPanelContainer");

        var audioBtn = settingsInstance.Q<Button>("AudioButton");
        var controlsBtn = settingsInstance.Q<Button>("ControlsButton");
        var closeSettingsBtn = settingsInstance.Q<Button>("CloseSettingsButton");
        var audioBackBtn = settingsInstance.Q<Button>("AudioBackButton");

        if (audioBtn != null && settingsButtonsSub != null && audioPanelSub != null)
        {
            audioBtn.clicked += () =>
            {
                settingsButtonsSub.AddToClassList("is-hidden");
                audioPanelSub.RemoveFromClassList("is-hidden");
            };
        }

        if (audioBackBtn != null && settingsButtonsSub != null && audioPanelSub != null)
        {
            audioBackBtn.clicked += () =>
            {
                audioPanelSub.AddToClassList("is-hidden");
                settingsButtonsSub.RemoveFromClassList("is-hidden");
            };
        }

        if (controlsBtn != null)
        {
            controlsBtn.clicked += () => ShowControlsInPause(() => ShowSettingsInPause());
        }

        if (closeSettingsBtn != null)
        {
            closeSettingsBtn.clicked += () => CloseSubPanel();
        }
    }

    private void ShowControlsInPause(System.Action onReturn)
    {
        if (controlsTemplate == null)
        {
            Debug.LogError("controlsTemplate is not assigned on UIPauseManager!");
            return;
        }

        if (subPanelContainer == null) return;

        subPanelContainer.Clear();
        var controlsInstance = controlsTemplate.Instantiate();
        controlsInstance.style.flexGrow = 1f;
        subPanelContainer.Add(controlsInstance);

        var closeControlsBtn = controlsInstance.Q<Button>("CloseControlsButton");
        if (closeControlsBtn != null)
        {
            closeControlsBtn.clicked += () =>
            {
                if (onReturn != null)
                {
                    onReturn.Invoke();
                }
                else
                {
                    CloseSubPanel();
                }
            };
        }
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
