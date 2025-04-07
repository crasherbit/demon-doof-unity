using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private string _gameScene = "GameSceneMultiplayer";

    [Header("Matchmaking Settings")]
    [SerializeField] private float _matchTimeout = 10f;
    [SerializeField] private float _countdownTime = 3f;

    // Pannelli principali
    private VisualElement _mainPanel;
    private VisualElement _playPanel;
    private VisualElement _settingsPanel;
    private VisualElement _creditsPanel;
    private VisualElement _matchmakingPanel;

    // Bottoni del menu principale
    private Button _playButton;
    private Button _settingsButton;
    private Button _creditsButton;
    private Button _quitButton;

    // Bottoni del pannello play
    private Button _singlePlayerButton;
    private Button _multiplayerButton;
    private Button _playBackButton;

    // Componenti del pannello impostazioni
    private SliderInt _musicSlider;
    private SliderInt _sfxSlider;
    private DropdownField _qualityDropdown;
    private Toggle _fullscreenToggle;
    private Button _settingsBackButton;

    // Bottoni del pannello crediti
    private Button _creditsBackButton;

    // Componenti del matchmaking
    private VisualElement _searchPanel;
    private VisualElement _matchFoundPanel;
    private VisualElement _noMatchPanel;
    private Label _statusText;
    private Label _timerText;
    private Label _countdownText;
    private Button _cancelButton;
    private Button _playAiButton;
    private Button _retryButton;
    private Button _backButton;
    private VisualElement _loadingSpinner;
    private VisualElement _spinnerInner;

    // Stato del matchmaking
    private float _searchTime = 0f;
    private bool _isSearching = false;
    private bool _matchFound = false;
    private Coroutine _searchCoroutine;
    private Coroutine _rotationCoroutine;

    private void Awake()
    {
        if (_uiDocument == null)
        {
            Debug.LogError("UIDocument is missing! Please assign it in the inspector.");
            return;
        }

        // Ottiene l'elemento radice
        VisualElement root = _uiDocument.rootVisualElement;

        // Inizializza tutti i riferimenti UI
        InitializePanels(root);
        InitializeMainMenuButtons(root);
        InitializePlayPanelButtons(root);
        InitializeSettingsPanelElements(root);
        InitializeCreditsPanelElements(root);
        InitializeMatchmakingElements(root);
    }

    private void Start()
    {
        // Registra i callback dei bottoni
        RegisterButtonCallbacks();

        // Inizializza i valori delle impostazioni
        InitializeSettingsValues();

        // Mostra il pannello principale
        ShowMainPanel();
    }

    private void OnDestroy()
    {
        // Ferma tutte le coroutine attive
        if (_searchCoroutine != null)
            StopCoroutine(_searchCoroutine);

        if (_rotationCoroutine != null)
            StopCoroutine(_rotationCoroutine);
    }

    #region Initialization

    private void InitializePanels(VisualElement root)
    {
        _mainPanel = root.Q<VisualElement>("main-panel");
        _playPanel = root.Q<VisualElement>("play-panel");
        _settingsPanel = root.Q<VisualElement>("settings-panel");
        _creditsPanel = root.Q<VisualElement>("credits-panel");
        _matchmakingPanel = root.Q<VisualElement>("matchmaking-panel");
    }

    private void InitializeMainMenuButtons(VisualElement root)
    {
        _playButton = root.Q<Button>("play-button");
        _settingsButton = root.Q<Button>("settings-button");
        _creditsButton = root.Q<Button>("credits-button");
        _quitButton = root.Q<Button>("quit-button");
    }

    private void InitializePlayPanelButtons(VisualElement root)
    {
        _singlePlayerButton = root.Q<Button>("single-player-button");
        _multiplayerButton = root.Q<Button>("multiplayer-button");
        _playBackButton = root.Q<Button>("play-back-button");
    }

    private void InitializeSettingsPanelElements(VisualElement root)
    {
        _musicSlider = root.Q<SliderInt>("music-slider");
        _sfxSlider = root.Q<SliderInt>("sfx-slider");
        _qualityDropdown = root.Q<DropdownField>("quality-dropdown");
        _fullscreenToggle = root.Q<Toggle>("fullscreen-toggle");
        _settingsBackButton = root.Q<Button>("settings-back-button");
    }

    private void InitializeCreditsPanelElements(VisualElement root)
    {
        _creditsBackButton = root.Q<Button>("credits-back-button");
    }

    private void InitializeMatchmakingElements(VisualElement root)
    {
        _searchPanel = root.Q<VisualElement>("search-panel");
        _matchFoundPanel = root.Q<VisualElement>("match-found-panel");
        _noMatchPanel = root.Q<VisualElement>("no-match-panel");
        _statusText = root.Q<Label>("status-text");
        _timerText = root.Q<Label>("timer-text");
        _countdownText = root.Q<Label>("countdown-text");
        _cancelButton = root.Q<Button>("cancel-button");
        _playAiButton = root.Q<Button>("play-ai-button");
        _retryButton = root.Q<Button>("retry-button");
        _backButton = root.Q<Button>("back-button");
        _loadingSpinner = root.Q<VisualElement>("loading-spinner");
        _spinnerInner = root.Q<VisualElement>("spinner-inner");
    }

    private void RegisterButtonCallbacks()
    {
        // Menu principale
        if (_playButton != null) _playButton.clicked += OnPlayButtonClicked;
        if (_settingsButton != null) _settingsButton.clicked += OnSettingsButtonClicked;
        if (_creditsButton != null) _creditsButton.clicked += OnCreditsButtonClicked;
        if (_quitButton != null) _quitButton.clicked += OnQuitButtonClicked;

        // Play panel
        if (_singlePlayerButton != null) _singlePlayerButton.clicked += OnSinglePlayerButtonClicked;
        if (_multiplayerButton != null) _multiplayerButton.clicked += OnMultiplayerButtonClicked;
        if (_playBackButton != null) _playBackButton.clicked += OnPlayBackButtonClicked;

        // Settings panel
        if (_settingsBackButton != null) _settingsBackButton.clicked += OnSettingsBackButtonClicked;

        // Credits panel
        if (_creditsBackButton != null) _creditsBackButton.clicked += OnCreditsBackButtonClicked;

        // Matchmaking
        if (_cancelButton != null) _cancelButton.clicked += OnCancelButtonClicked;
        if (_playAiButton != null) _playAiButton.clicked += OnPlayAiButtonClicked;
        if (_retryButton != null) _retryButton.clicked += OnRetryButtonClicked;
        if (_backButton != null) _backButton.clicked += OnBackButtonClicked;
    }

    private void InitializeSettingsValues()
    {
        // Imposta i valori del dropdown della qualità
        if (_qualityDropdown != null)
        {
            _qualityDropdown.choices = new System.Collections.Generic.List<string>
            {
                "Low", "Medium", "High", "Ultra"
            };
            _qualityDropdown.index = 2; // High di default
        }

        // Imposta lo stato del fullscreen
        if (_fullscreenToggle != null)
        {
            _fullscreenToggle.value = Screen.fullScreen;
        }
    }

    #endregion

    #region Panel Navigation

    private void ShowMainPanel()
    {
        HideAllPanels();
        ShowPanel(_mainPanel);
    }

    private void ShowPlayPanel()
    {
        HideAllPanels();
        ShowPanel(_playPanel);
    }

    private void ShowSettingsPanel()
    {
        HideAllPanels();
        ShowPanel(_settingsPanel);
    }

    private void ShowCreditsPanel()
    {
        HideAllPanels();
        ShowPanel(_creditsPanel);
    }

    private void ShowMatchmakingPanel()
    {
        HideAllPanels();
        ShowPanel(_matchmakingPanel);
        ShowSearchPanel();
    }

    private void HideAllPanels()
    {
        HidePanel(_mainPanel);
        HidePanel(_playPanel);
        HidePanel(_settingsPanel);
        HidePanel(_creditsPanel);
        HidePanel(_matchmakingPanel);
    }

    private void ShowPanel(VisualElement panel)
    {
        if (panel == null) return;

        panel.RemoveFromClassList("panel-hidden");

        // Se è un pannello fullscreen, aggiungi anche la classe visible
        if (panel.ClassListContains("fullscreen-panel"))
        {
            panel.AddToClassList("fullscreen-panel-visible");
        }
    }
    private void HidePanel(VisualElement panel)
    {
        if (panel == null) return;

        panel.AddToClassList("panel-hidden");

        // Se è un pannello fullscreen, rimuovi anche la classe visible
        if (panel.ClassListContains("fullscreen-panel"))
        {
            panel.RemoveFromClassList("fullscreen-panel-visible");
        }
    }

    private void ShowSearchPanel()
    {
        if (_searchPanel == null)
        {
            Debug.LogError("Search panel is null! Check your UXML.");
            return;
        }

        if (_matchFoundPanel == null)
        {
            Debug.LogError("Match found panel is null! Check your UXML.");
        }
        else
        {
            _matchFoundPanel.AddToClassList("panel-hidden");
        }

        if (_noMatchPanel == null)
        {
            Debug.LogError("No match panel is null! Check your UXML.");
        }
        else
        {
            _noMatchPanel.AddToClassList("panel-hidden");
        }

        // Mostra il pannello di ricerca
        _searchPanel.RemoveFromClassList("panel-hidden");

        Debug.Log("Search panel should be visible now");
    }

    private void ShowMatchFoundPanel()
    {
        if (_searchPanel == null || _matchFoundPanel == null || _noMatchPanel == null) return;

        _searchPanel.AddToClassList("panel-hidden");
        _matchFoundPanel.RemoveFromClassList("panel-hidden");
        _noMatchPanel.AddToClassList("panel-hidden");
    }

    private void ShowNoMatchPanel()
    {
        if (_searchPanel == null || _matchFoundPanel == null || _noMatchPanel == null) return;

        _searchPanel.AddToClassList("panel-hidden");
        _matchFoundPanel.AddToClassList("panel-hidden");
        _noMatchPanel.RemoveFromClassList("panel-hidden");
    }

    #endregion

    #region Button Callbacks

    // Menu principale
    private void OnPlayButtonClicked()
    {
        ShowPlayPanel();
    }

    private void OnSettingsButtonClicked()
    {
        ShowSettingsPanel();
    }

    private void OnCreditsButtonClicked()
    {
        ShowCreditsPanel();
    }

    private void OnQuitButtonClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // Play panel
    private void OnSinglePlayerButtonClicked()
    {
        // Imposta la modalità single player con IA
        PlayerPrefs.SetInt("PlayingAgainstAI", 1);
        SceneManager.LoadScene(_gameScene);
    }

    private void OnMultiplayerButtonClicked()
    {
        Debug.Log("Multiplayer button clicked - Showing matchmaking panel");

        ShowMatchmakingPanel();

        // Avvia la ricerca
        StartSearch();
    }

    private void OnPlayBackButtonClicked()
    {
        ShowMainPanel();
    }

    // Settings panel
    private void OnSettingsBackButtonClicked()
    {
        ShowMainPanel();
    }

    // Credits panel
    private void OnCreditsBackButtonClicked()
    {
        ShowMainPanel();
    }

    // Matchmaking
    private void OnCancelButtonClicked()
    {
        CancelSearch();
    }

    private void OnPlayAiButtonClicked()
    {
        PlayAgainstAI();
    }

    private void OnRetryButtonClicked()
    {
        RetrySearch();
    }

    private void OnBackButtonClicked()
    {
        ShowPlayPanel();
    }

    #endregion

    #region Matchmaking

    private void StartSearch()
    {
        if (_isSearching)
            return;

        _isSearching = true;
        _searchTime = 0f;

        if (_statusText != null)
            _statusText.text = "In attesa di un avversario...";

        // Mostra il pannello di ricerca
        ShowSearchPanel();

        // Avvia la coroutine di ricerca
        _searchCoroutine = StartCoroutine(SearchForMatch());

        // Avvia l'animazione dello spinner
        _rotationCoroutine = StartCoroutine(AnimateSpinner());

        // Se abbiamo un NetworkBootstrap, avvia il networking
        if (NetworkBootstrap.Instance != null)
        {
            NetworkBootstrap.Instance.StartHost();
        }
    }

    private void CancelSearch()
    {
        if (!_isSearching)
            return;

        _isSearching = false;

        if (_searchCoroutine != null)
            StopCoroutine(_searchCoroutine);

        if (_rotationCoroutine != null)
            StopCoroutine(_rotationCoroutine);

        // Ferma il networking se necessario
        if (NetworkBootstrap.Instance != null)
        {
            NetworkBootstrap.Instance.StopNetwork();
        }

        // Torna al pannello di gioco
        ShowPlayPanel();
    }

    private void PlayAgainstAI()
    {
        // Imposta una flag per indicare che si sta giocando contro l'IA
        PlayerPrefs.SetInt("PlayingAgainstAI", 1);

        // Carica la scena di gioco
        SceneManager.LoadScene(_gameScene);
    }

    private void RetrySearch()
    {
        StartSearch();
    }

    private IEnumerator SearchForMatch()
    {
        while (_isSearching && _searchTime < _matchTimeout)
        {
            _searchTime += Time.deltaTime;

            // Aggiorna il timer
            int seconds = Mathf.FloorToInt(_searchTime);
            int minutes = seconds / 60;
            seconds = seconds % 60;

            if (_timerText != null)
                _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Simula un match trovato dopo X secondi (per test)
            if (_searchTime > 3f)
            {
                _matchFound = true;
                OnMatchFound();
                yield break;
            }

            yield return null;
        }

        // Se il timeout è scaduto e non è stato trovato un match
        if (_isSearching && !_matchFound)
        {
            OnNoMatchFound();
        }
    }

    private void OnMatchFound()
    {
        _isSearching = false;
        ShowMatchFoundPanel();

        // Avvia il countdown
        StartCoroutine(StartCountdown());
    }

    private void OnNoMatchFound()
    {
        _isSearching = false;
        ShowNoMatchPanel();
    }

    private IEnumerator StartCountdown()
    {
        int countdown = Mathf.CeilToInt(_countdownTime);

        while (countdown > 0)
        {
            if (_countdownText != null)
                _countdownText.text = countdown.ToString();

            yield return new WaitForSeconds(1f);
            countdown--;
        }

        // Avvia la partita
        PlayerPrefs.SetInt("PlayingAgainstAI", 0); // Assicurati che non stai giocando contro l'IA
        SceneManager.LoadScene(_gameScene);
    }

    private IEnumerator AnimateSpinner()
    {
        if (_loadingSpinner == null || _spinnerInner == null)
            yield break;

        float spinnerRotation = 0f;
        float spinnerInnerRotation = 0f;

        while (true)
        {
            // Ruota lo spinner esterno in senso orario
            spinnerRotation += Time.deltaTime * 50f;
            if (spinnerRotation >= 360f)
                spinnerRotation -= 360f;

            _loadingSpinner.style.rotate = new StyleRotate(new Rotate(spinnerRotation));

            // Ruota lo spinner interno in senso antiorario
            spinnerInnerRotation -= Time.deltaTime * 30f;
            if (spinnerInnerRotation <= -360f)
                spinnerInnerRotation += 360f;

            _spinnerInner.style.rotate = new StyleRotate(new Rotate(spinnerInnerRotation));

            yield return null;
        }
    }

    #endregion
}