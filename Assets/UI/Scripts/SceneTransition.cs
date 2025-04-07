using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance { get; private set; }

    [SerializeField] private Image _fadePanel;
    [SerializeField] private float _fadeDuration = 0.5f;
    [SerializeField] private bool _dontDestroyOnLoad = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

            if (_dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            // Assicurati che il pannello sia inizialmente trasparente
            if (_fadePanel != null)
            {
                Color initialColor = _fadePanel.color;
                initialColor.a = 0f;
                _fadePanel.color = initialColor;
                _fadePanel.gameObject.SetActive(false);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void FadeToScene(string sceneName, bool isNetworked = false)
    {
        StartCoroutine(FadeAndLoadScene(sceneName, isNetworked));
    }

    private IEnumerator FadeAndLoadScene(string sceneName, bool isNetworked)
    {
        // Se non abbiamo un pannello di fade, carica direttamente la scena
        if (_fadePanel == null)
        {
            LoadScene(sceneName, isNetworked);
            yield break;
        }

        // Mostra il pannello e imposta alpha a 0
        _fadePanel.gameObject.SetActive(true);
        Color panelColor = _fadePanel.color;

        // Fade In
        float elapsedTime = 0f;
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / _fadeDuration);

            panelColor.a = normalizedTime;
            _fadePanel.color = panelColor;

            yield return null;
        }

        // Carica la scena
        LoadScene(sceneName, isNetworked);

        // Attendi il caricamento della scena
        yield return new WaitForSeconds(0.5f);

        // Fade Out
        elapsedTime = 0f;
        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / _fadeDuration);

            panelColor.a = 1f - normalizedTime;
            _fadePanel.color = panelColor;

            yield return null;
        }

        // Nascondi il pannello
        _fadePanel.gameObject.SetActive(false);
    }

    private void LoadScene(string sceneName, bool isNetworked)
    {
        if (isNetworked)
        {
            // Se hai già implementato un NetworkSceneManager, usalo qui
            // NetworkSceneManager.Instance.LoadNetworkScene(sceneName);

            // Altrimenti, carica la scena normalmente
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}