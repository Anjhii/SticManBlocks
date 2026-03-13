using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject gameManagerPrefab;

    [Header("UI Elements")]
    [SerializeField] private RectTransform titleText;
    [SerializeField] private RectTransform playButton;
    [SerializeField] private RectTransform skinsButton;
    [SerializeField] private RectTransform exitButton;

    [Header("Floating Character")]
    [SerializeField] private FloatingAnimation floatingScript; // tu script del padre
    [SerializeField] private RectTransform floatingGroup;      // el GameObject padre

    [Header("Animation Settings")]
    [SerializeField] private float uiExitDuration = 0.4f;
    [SerializeField] private float fallGravity = 800f;

    private void Start()
    {
        if (GameManager.Instance == null && gameManagerPrefab != null)
        {
            Instantiate(gameManagerPrefab);
        }
        AudioManager.Instance.PlayMusic(AudioManager.Instance.inicioVF);
    }

    // ---- Botones ----

    public void OnPlayClicked()
    {
        StartCoroutine(PlayExitSequence());
    }

    public void OnSkinsClicked()
    {
        SceneManager.LoadScene("SkinSelection");
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }

    // ---- Animación de salida ----

    IEnumerator PlayExitSequence()
    {
        // 1. Deshabilitar botones para que no se pueda clickear de nuevo
        SetButtonsInteractable(false);

        // 2. UI sale en paralelo
        StartCoroutine(SlideOut(titleText,   Vector2.up   * 1200f, uiExitDuration));
        StartCoroutine(SlideOut(playButton,  Vector2.right * 1200f, uiExitDuration));
        StartCoroutine(SlideOut(skinsButton, Vector2.left * 1200f, uiExitDuration));
        StartCoroutine(SlideOut(exitButton,  Vector2.right * 1200f, uiExitDuration));

        yield return new WaitForSeconds(uiExitDuration * 0.6f);

        // 3. El personaje cae
        yield return StartCoroutine(CharacterFallAndExit());

        // 4. Recién acá llamamos al GameManager
        yield return new WaitForSeconds(0.1f);
        GameManager.Instance.StartNewGame();
    }

    IEnumerator SlideOut(RectTransform element, Vector2 offset, float duration)
    {
        if (element == null) yield break;

        Vector2 startPos = element.anchoredPosition;
        Vector2 endPos   = startPos + offset;
        float elapsed    = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t  = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            element.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        element.anchoredPosition = endPos;
    }

    IEnumerator CharacterFallAndExit()
    {
        if (floatingScript != null)
            floatingScript.enabled = false; // detiene el floating

        Vector2 velocity = Vector2.zero;
        float elapsed    = 0f;
        float maxTime    = 3f;

        while (elapsed < maxTime)
        {
            elapsed    += Time.deltaTime;
            velocity.y -= fallGravity * Time.deltaTime;
            floatingGroup.anchoredPosition += velocity * Time.deltaTime;

            if (floatingGroup.anchoredPosition.y < -Screen.height * 3f)
                break;

            yield return null;
        }
    }

    void SetButtonsInteractable(bool state)
    {
        if (playButton  != null) playButton .GetComponent<UnityEngine.UI.Button>().interactable = state;
        if (skinsButton != null) skinsButton.GetComponent<UnityEngine.UI.Button>().interactable = state;
        if (exitButton  != null) exitButton .GetComponent<UnityEngine.UI.Button>().interactable = state;
    }
}