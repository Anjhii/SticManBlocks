using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private float transitionTime = 2f;

    private void Start()
    {
        // Mostramos el nivel actual leyendo el dato persistente
        levelText.text = "LEVEL " + GameManager.Instance.CurrentLevel;
        StartCoroutine(TransitionRoutine());
    }

    private IEnumerator TransitionRoutine()
    {
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene("Gameplay");
    }
}
