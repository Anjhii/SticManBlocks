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
    [SerializeField] private FloatingAnimation floatingScript;
    [SerializeField] private GameObject floatingGroup;

    [Header("Animation Settings")]
    [SerializeField] private float uiExitDuration = 0.4f;
    [SerializeField] private float fallGravity = 20f; // ✅ Ajustado para Transform normal

    private void Start()
    {
        if (GameManager.Instance == null && gameManagerPrefab != null)
            Instantiate(gameManagerPrefab);
       
        AudioManager.Instance.PlayMusic(AudioManager.Instance.inicioVF);
    }
    

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

    IEnumerator PlayExitSequence()
    {
        SetButtonsInteractable(false);

        StartCoroutine(SlideOut(titleText,   Vector2.up    * 1200f, uiExitDuration));
        StartCoroutine(SlideOut(playButton,  Vector2.right * 1200f, uiExitDuration));
        StartCoroutine(SlideOut(skinsButton, Vector2.left  * 1200f, uiExitDuration));
        StartCoroutine(SlideOut(exitButton,  Vector2.right * 1200f, uiExitDuration));

        yield return new WaitForSeconds(uiExitDuration * 0.6f);

        yield return StartCoroutine(CharacterFallAndExit());

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
            floatingScript.enabled = false;

        if (floatingGroup == null) yield break;

        // ✅ Usamos Transform normal en lugar de RectTransform
        Transform floatingTransform = floatingGroup.transform;
        Vector3 velocity = Vector3.zero;
        float elapsed = 0f;
        float maxTime = 3f;

        while (elapsed < maxTime)
        {
            elapsed += Time.deltaTime;
            velocity.y -= fallGravity * Time.deltaTime;
            floatingTransform.position += velocity * Time.deltaTime;

            // Sale de pantalla hacia abajo
            if (floatingTransform.position.y < -20f)
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