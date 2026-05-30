using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Templates")]
    [SerializeField] private VisualTreeAsset mainMenuTemplate;
    [SerializeField] private VisualTreeAsset levelSelectionTemplate;

    private UIDocument uiDocument;
    private VisualElement root;


    private VisualElement notificationOverlay;
    private IVisualElementScheduledItem notificationTimer;

    private void Start()
    {
        uiDocument = GetComponent<UIDocument>();
        
        if (uiDocument == null)
        {
            Debug.LogError("UIDocument component is missing on UIManager!");
            return;
        }

        root = uiDocument.rootVisualElement;
        
        ShowMainMenu();
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
            settingsButton.clicked -= OnSettingsClicked;
            settingsButton.clicked += OnSettingsClicked;
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
            goBackMenuButton.clicked += ShowMainMenu;
        }
        if (settingsButton != null)
        {
            settingsButton.clicked -= OnSettingsClicked;
            settingsButton.clicked += OnSettingsClicked;
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
            closeButton.clicked -= HideNotification;
            closeButton.clicked += HideNotification;
        }

        for (int i = 1; i <= 5; i++)
        {
            int levelIndex = i;
            var levelContainer = root.Q<Button>($"LevelContainer{levelIndex}");
            if (levelContainer != null)
            {
                if (levelContainer.ClassListContains("is-locked"))
                {
                    levelContainer.clicked += () => OnLevelLockedClicked(levelIndex);
                }
                else
                {
                    levelContainer.clicked += () => LoadLevel(levelIndex);
                }
            }
        }
    }

    private void OnPlayClicked()
    {
        ShowLevelSelection();
    }

    private void OnSettingsClicked()
    {
        Debug.Log("Settings Screen Clicked");
    }

    private void OnCreditsClicked()
    {
        Debug.Log("Credits Clicked");
    }

    private void OnExitClicked()
    {
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
        Debug.Log($"Loading Level {levelIndex}...");
        SceneManager.LoadScene("Level" + levelIndex);
    }
}
