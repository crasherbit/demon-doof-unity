using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private string _singlePlayerSceneName = "GameScene";
    [SerializeField] private string _matchmakingSceneName = "MatchmakingScene";
    [SerializeField] private float _panelTransitionTime = 0.3f;

    // Riferimenti ai pannelli
    private VisualElement _mainPanel;
    private VisualElement _playPanel;
    private VisualElement _settingsPanel;
    private VisualElement _creditsPanel;

    // Riferimenti ai bottoni
    private Button _playButton;
    private Button _settingsButton;
    private Button _creditsButton;
    private Button _quitButton;
    private Button _singlePlayerButton;
    private Button _multiplayerButton;
    private Button _playBackButton;
    private Button _settingsBackButton;
    private Button _creditsBackButton;

    // Riferimenti alle impostazioni
    private SliderInt _musicSlider;
    private SliderInt _sfxSlider;
    private DropdownField _qualityDropdown;
    private Toggle _fullscreenToggle;

    // Pannello correntemente attivo
    private VisualElement _currentPanel;

    // Coroutine di transizione
    private Coroutine _transitionCoroutine;

    private void Awake()
    {
        // Ottenimento della root del UI Document
        VisualElement root = _uiDocument.rootVisualElement;

        // Assegnazione dei riferimenti ai pannelli
        _mainPanel = root.Q<VisualElement>("main-panel");
        _playPanel = root.Q<VisualElement>("play-panel");
        _settingsPanel = root.Q<VisualElement>("settings-panel");
        _creditsPanel = root.Q<VisualElement>("credits-panel");

        // Assegnazione dei riferimenti ai bottoni
        _playButton = root.Q<Button>("play-button");
        _settingsButton = root.Q<Button>("settings-button");
        _creditsButton = root.Q<Button>("credits-button");
        _quitButton = root.Q<Button>("quit-button");
        _singlePlayerButton = root.Q<Button>("single-player-button");
        _multiplayerButton = root.Q<Button>("multiplayer-button");
        _playBackButton = root.Q<Button>("play-back-button");
        _settingsBackButton = root.Q<Button>("settings-back-button");
        _creditsBackButton = root.Q<Button>("credits-back-button");

        // Assegnazione dei riferimenti alle impostazioni
        _musicSlider = root.Q<SliderInt>("music-slider");
        _sfxSlider = root.Q<SliderInt>("sfx-slider");
        _qualityDropdown = root.Q<DropdownField>("quality-dropdown");
        _fullscreenToggle = root.Q<Toggle>("fullscreen-toggle");

        // Inizializzazione della UI
        InitializeUI();

        // Registrazione eventi
        RegisterButtonCallbacks();
    }

    private void InitializeUI()
    {
        // Inizializzazione del dropdown qualità grafica
        _qualityDropdown.choices.Clear();
        string[] qualityNames = QualitySettings.names;
        foreach (string name in qualityNames)
        {
            _qualityDropdown.choices.Add(name);
        }
        _qualityDropdown.value = qualityNames[QualitySettings.GetQualityLevel()];

        // Inizializzazione fullscreen
        _fullscreenToggle.value = Screen.fullScreen;

        // Inizializzazione volume musica
        _musicSlider.value = PlayerPrefs.GetInt("MusicVolume", 80);

        // Inizializzazione volume effetti sonori
        _sfxSlider.value = PlayerPrefs.GetInt("SFXVolume", 80);

        // Mostra solo il pannello principale all'inizio
        _currentPanel = _mainPanel;
        _mainPanel.RemoveFromClassList("panel-hidden");
        _mainPanel.AddToClassList("panel-slide-in");
        _playPanel.AddToClassList("panel-hidden");
        _settingsPanel.AddToClassList("panel-hidden");
        _creditsPanel.AddToClassList("panel-hidden");
    }

    private void RegisterButtonCallbacks()
    {
        // Main Panel
        _playButton.clicked += () => TransitionToPanel(_playPanel);
        _settingsButton.clicked += () => TransitionToPanel(_settingsPanel);
        _creditsButton.clicked += () => TransitionToPanel(_creditsPanel);
        _quitButton.clicked += QuitGame;

        // Play Panel
        _singlePlayerButton.clicked += StartSinglePlayer;
        _multiplayerButton.clicked += StartMultiplayer;
        _playBackButton.clicked += () => TransitionToPanel(_mainPanel);

        // Settings Panel
        _settingsBackButton.clicked += () => TransitionToPanel(_mainPanel);

        // Credits Panel
        _creditsBackButton.clicked += () => TransitionToPanel(_mainPanel);

        // Settings Controls
        _musicSlider.RegisterValueChangedCallback(OnMusicVolumeChanged);
        _sfxSlider.RegisterValueChangedCallback(OnSFXVolumeChanged);
        _qualityDropdown.RegisterValueChangedCallback(OnQualityChanged);
        _fullscreenToggle.RegisterValueChangedCallback(OnFullscreenChanged);
    }

    private void TransitionToPanel(VisualElement newPanel)
    {
        if (newPanel == _currentPanel) return;

        // Stoppa qualsiasi transizione precedente
        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        _transitionCoroutine = StartCoroutine(TransitionPanels(_currentPanel, newPanel));
        _currentPanel = newPanel;
    }

    private IEnumerator TransitionPanels(VisualElement oldPanel, VisualElement newPanel)
    {
        // Animazione di uscita
        oldPanel.RemoveFromClassList("panel-slide-in");
        oldPanel.AddToClassList("panel-hidden");

        // Attesa breve
        yield return new WaitForSeconds(_panelTransitionTime);

        // Animazione di entrata
        newPanel.RemoveFromClassList("panel-hidden");
        newPanel.AddToClassList("panel-slide-in");
    }

    private void StartSinglePlayer()
    {
        SceneManager.LoadScene(_singlePlayerSceneName);
    }

    private void StartMultiplayer()
    {
        // Transizione alla scena di matchmaking
        SceneManager.LoadScene(_matchmakingSceneName);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnMusicVolumeChanged(ChangeEvent<int> evt)
    {
        int volume = evt.newValue;
        PlayerPrefs.SetInt("MusicVolume", volume);
        // Qui dovresti applicare il volume alla musica
        // AudioManager.SetMusicVolume(volume / 100f);
    }

    private void OnSFXVolumeChanged(ChangeEvent<int> evt)
    {
        int volume = evt.newValue;
        PlayerPrefs.SetInt("SFXVolume", volume);
        // Qui dovresti applicare il volume agli effetti sonori
        // AudioManager.SetSFXVolume(volume / 100f);
    }

    private void OnQualityChanged(ChangeEvent<string> evt)
    {
        string qualityLevel = evt.newValue;
        // Trova l'indice corrispondente al nome della qualità
        string[] qualityNames = QualitySettings.names;
        for (int i = 0; i < qualityNames.Length; i++)
        {
            if (qualityNames[i] == qualityLevel)
            {
                QualitySettings.SetQualityLevel(i, true);
                break;
            }
        }
    }

    private void OnFullscreenChanged(ChangeEvent<bool> evt)
    {
        bool isFullscreen = evt.newValue;
        Screen.fullScreen = isFullscreen;
    }
}