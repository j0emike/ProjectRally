using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class UILevelEndManager : MonoBehaviour
{
    public static UILevelEndManager Instance { get; private set; }

    [Header("Level Configuration")]
    [Tooltip("Time limit in seconds. Set to 0 or negative for no time limit.")]
    [SerializeField] private float _levelTimeLimit = 60f;

    private UIDocument _uiDocument;
    private VisualElement _root;
    
    // UI Elements
    private VisualElement _gameplayHUD;
    private Label _timerLabel;
    
    private VisualElement _levelEndContainer;
    private VisualElement _winPanel;
    private VisualElement _losePanel;
    private Button _retryButton;
    private Button _levelSelectionButton;

    // State Variables
    private float _remainingTime;
    private bool _isLevelOver = false;
    private bool _hasTimer = false;

    // Cache References
    private KartController _playerKart;
    private CharacterController _playerCharacterController;
    private UIPauseManager _pauseManager;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _uiDocument = GetComponent<UIDocument>();
        if (_uiDocument == null)
        {
            Debug.LogError("UIDocument component is missing on UILevelEndManager!");
            return;
        }

        _root = _uiDocument.rootVisualElement;
        if (_root == null)
        {
            Debug.LogError("Root VisualElement is null on UILevelEndManager!");
            return;
        }

        // Cache elements from UXML
        _gameplayHUD = _root.Q<VisualElement>("GameplayHUD");
        _timerLabel = _root.Q<Label>("TimerLabel");
        _levelEndContainer = _root.Q<VisualElement>("LevelEndContainer");
        _winPanel = _root.Q<VisualElement>("WinPanel");
        _losePanel = _root.Q<VisualElement>("LosePanel");
        _retryButton = _root.Q<Button>("RetryButton");
        _levelSelectionButton = _root.Q<Button>("LevelSelectionButton");

        // Set up button event listeners
        if (_retryButton != null) _retryButton.clicked += RestartLevel;
        if (_levelSelectionButton != null) _levelSelectionButton.clicked += ReturnToLevelSelection;

        // Find Player in scene
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _playerKart = playerObj.GetComponent<KartController>();
            _playerCharacterController = playerObj.GetComponent<CharacterController>();
        }

        // Find Pause Manager to disable it when level ends
        _pauseManager = FindObjectOfType<UIPauseManager>();

        // Setup Timer HUD
        if (_levelTimeLimit > 0f)
        {
            _remainingTime = _levelTimeLimit;
            _hasTimer = true;
            if (_gameplayHUD != null) _gameplayHUD.style.display = DisplayStyle.Flex;
            UpdateTimerText();
        }
        else
        {
            _hasTimer = false;
            if (_gameplayHUD != null) _gameplayHUD.style.display = DisplayStyle.None;
        }

        // Ensure Level End Screen is hidden at start
        if (_levelEndContainer != null) _levelEndContainer.style.display = DisplayStyle.None;
        
        // Ensure standard time scale is running
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (_isLevelOver) return;

        if (_hasTimer)
        {
            _remainingTime -= Time.deltaTime;
            if (_remainingTime <= 0f)
            {
                _remainingTime = 0f;
                UpdateTimerText();
                LoseLevel();
            }
            else
            {
                UpdateTimerText();
            }
        }
    }

    private void UpdateTimerText()
    {
        if (_timerLabel == null) return;

        int minutes = Mathf.FloorToInt(_remainingTime / 60f);
        float seconds = _remainingTime % 60f;
        _timerLabel.text = string.Format("TIEMPO: <mspace=22>{0:00}:{1:00.00}</mspace>", minutes, seconds);
        
        // Change text color to red if under 10 seconds for visual premium alert
        if (_remainingTime < 10f)
        {
            _timerLabel.style.color = new StyleColor(new Color(238f / 255f, 77f / 255f, 77f / 255f));
            _timerLabel.style.borderLeftColor = new StyleColor(new Color(238f / 255f, 77f / 255f, 77f / 255f));
            _timerLabel.style.borderRightColor = new StyleColor(new Color(238f / 255f, 77f / 255f, 77f / 255f));
            _timerLabel.style.borderTopColor = new StyleColor(new Color(238f / 255f, 77f / 255f, 77f / 255f));
            _timerLabel.style.borderBottomColor = new StyleColor(new Color(238f / 255f, 77f / 255f, 77f / 255f));
        }
        else
        {
            _timerLabel.style.color = new StyleColor(new Color(253f / 255f, 202f / 255f, 64f / 255f));
            _timerLabel.style.borderLeftColor = new StyleColor(new Color(253f / 255f, 211f / 255f, 93f / 255f));
            _timerLabel.style.borderRightColor = new StyleColor(new Color(253f / 255f, 211f / 255f, 93f / 255f));
            _timerLabel.style.borderTopColor = new StyleColor(new Color(253f / 255f, 211f / 255f, 93f / 255f));
            _timerLabel.style.borderBottomColor = new StyleColor(new Color(253f / 255f, 211f / 255f, 93f / 255f));
        }
    }

    public void WinLevel()
    {
        if (_isLevelOver) return;
        EndLevel(true);
    }

    public void LoseLevel()
    {
        if (_isLevelOver) return;
        EndLevel(false);
    }

    private void EndLevel(bool didWin)
    {
        _isLevelOver = true;
        Time.timeScale = 0f;

        // Disable pausing
        if (_pauseManager != null) _pauseManager.enabled = false;

        // Disable player controls
        if (_playerKart != null)
        {
            _playerKart.ResetVelocity();
            _playerKart.enabled = false;
        }
        if (_playerCharacterController != null) _playerCharacterController.enabled = false;

        // Setup Cursor
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;

        // Hide Gameplay HUD and Show Level End Screen
        if (_gameplayHUD != null) _gameplayHUD.style.display = DisplayStyle.None;
        
        if (_levelEndContainer != null)
        {
            _levelEndContainer.style.display = DisplayStyle.Flex;

            if (didWin)
            {
                if (_winPanel != null) _winPanel.RemoveFromClassList("is-hidden");
                if (_losePanel != null) _losePanel.AddToClassList("is-hidden");
            }
            else
            {
                if (_losePanel != null) _losePanel.RemoveFromClassList("is-hidden");
                if (_winPanel != null) _winPanel.AddToClassList("is-hidden");
            }
        }
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene activeScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(activeScene.name);
    }

    private void ReturnToLevelSelection()
    {
        Time.timeScale = 1f;
        
        // Save flag to tell UIManager to open level selection screen directly
        PlayerPrefs.SetInt("ShowLevelSelection", 1);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene("MainMenu");
    }
}
