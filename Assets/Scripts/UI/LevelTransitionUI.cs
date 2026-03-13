using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI tipText;

    [Header("Plataforma")]
    [SerializeField] private MonoBehaviour platformScript;

    [Header("Tiempos")]
    [SerializeField] private float transitionTime = 8f;
    [SerializeField] private float tipChangeInterval = 2f;

    [Header("Tips")]
    [SerializeField] private string[] tips = {
        "💡 Usa A y D para moverte",
        "💡 Presiona SPACE para saltar",
        "💡 Evita los obstáculos",
        "💡 Recoge las monedas para más puntos"
    };

    private int tipIndex = 0;

    private void Start()
    {
        levelText.text = "LEVEL " + GameManager.Instance.CurrentLevel;
        tipText.text = tips[0];

        if (platformScript != null)
            platformScript.enabled = false;

        StartCoroutine(TransitionRoutine());
        StartCoroutine(CycleTips());
        AudioManager.Instance.FadeOutMusic(2f);
    }

    private IEnumerator TransitionRoutine()
    {
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("Gameplay");
    }

    private IEnumerator CycleTips()
    {
        while (true)
        {
            yield return new WaitForSeconds(tipChangeInterval);
            tipIndex = (tipIndex + 1) % tips.Length;
            tipText.text = tips[tipIndex];
        }
    }
}