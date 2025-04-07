using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Collections;

public class MatchmakingController : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private string _mainMenuScene = "MainMenuScene";
    [SerializeField] private string _gameScene = "GameSceneMultiplayer";

    [Header("Matchmaking Settings")]
    [SerializeField] private float _matchTimeout = 10f; // Tempo massimo per cercare altri giocatori
    [SerializeField] private float _countdownTime = 3f; // Countdown prima di iniziare la partita

    // Riferimenti alla UI
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
        // Inizializza riferimenti UI
        VisualElement root = _uiDocument.rootVisualElement;

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

        // Inizializza UI
        ShowSearchPanel();
    }

    private void Start()
    {
        // Registra callback dei bottoni
        _cancelButton.clicked += CancelSearch;
        _playAiButton.clicked += PlayAgainstAI;
        _retryButton.clicked += RetrySearch;
        _backButton.clicked += ReturnToMainMenu;

        // Avvia la ricerca automaticamente
        StartSearch();

        // Avvia l'animazione del spinner
        _rotationCoroutine = StartCoroutine(AnimateSpinner());
    }

    private void OnDestroy()
    {
        // Ferma le coroutine quando la scena viene distrutta
        if (_searchCoroutine != null)
            StopCoroutine(_searchCoroutine);

        if (_rotationCoroutine != null)
            StopCoroutine(_rotationCoroutine);
    }

    private void StartSearch()
    {
        if (_isSearching)
            return;

        _isSearching = true;
        _searchTime = 0f;
        _statusText.text = "In attesa di un avversario...";

        // Avvia la coroutine di ricerca
        _searchCoroutine = StartCoroutine(SearchForMatch());

        // Determina se avviare come host o client
        // Per semplicità, avviamo sempre come host in questo esempio
        // In un'implementazione reale, ci sarebbe una logica più complessa
        if (NetworkBootstrap.Instance != null)
        {
            // Se non siamo già connessi, avvia come host
            if (!NetworkBootstrap.Instance.IsClient() && !NetworkBootstrap.Instance.IsServer())
            {
                NetworkBootstrap.Instance.StartHost();
            }
        }
    }

    private void CancelSearch()
    {
        if (!_isSearching)
            return;

        _isSearching = false;

        if (_searchCoroutine != null)
            StopCoroutine(_searchCoroutine);

        // Ferma la rete se siamo connessi
        if (NetworkBootstrap.Instance != null)
        {
            NetworkBootstrap.Instance.StopNetwork();
        }

        ReturnToMainMenu();
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
        ShowSearchPanel();
        StartSearch();
    }

    private void ReturnToMainMenu()
    {
        SceneManager.LoadScene(_mainMenuScene);
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
            _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Simula un match trovato dopo un certo tempo
            // In un caso reale, controlleremmo se un client si è connesso
            if (_searchTime > 3f && NetworkBootstrap.Instance != null && NetworkBootstrap.Instance.IsServer())
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
            _countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1f);
            countdown--;
        }

        // Avvia la partita
        SceneManager.LoadScene(_gameScene);
    }

    private IEnumerator AnimateSpinner()
    {
        float spinnerRotation = 0f;
        float spinnerInnerRotation = 0f;

        while (true)
        {
            // Ruota lo spinner esterno in senso orario
            spinnerRotation += Time.deltaTime * 50f;
            if (spinnerRotation >= 360f) spinnerRotation -= 360f;
            _loadingSpinner.style.rotate = new StyleRotate(new Rotate(spinnerRotation));

            // Ruota lo spinner interno in senso antiorario
            spinnerInnerRotation -= Time.deltaTime * 30f;
            if (spinnerInnerRotation <= -360f) spinnerInnerRotation += 360f;
            _spinnerInner.style.rotate = new StyleRotate(new Rotate(spinnerInnerRotation));

            yield return null;
        }
    }

    // Metodi helper per mostrare i pannelli
    private void ShowSearchPanel()
    {
        _searchPanel.RemoveFromClassList("panel-hidden");
        _matchFoundPanel.AddToClassList("panel-hidden");
        _noMatchPanel.AddToClassList("panel-hidden");
    }

    private void ShowMatchFoundPanel()
    {
        _searchPanel.AddToClassList("panel-hidden");
        _matchFoundPanel.RemoveFromClassList("panel-hidden");
        _noMatchPanel.AddToClassList("panel-hidden");
    }

    private void ShowNoMatchPanel()
    {
        _searchPanel.AddToClassList("panel-hidden");
        _matchFoundPanel.AddToClassList("panel-hidden");
        _noMatchPanel.RemoveFromClassList("panel-hidden");
    }
}