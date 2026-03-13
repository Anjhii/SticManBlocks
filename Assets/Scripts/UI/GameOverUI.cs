using TMPro;
using UnityEngine;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI reasonText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Personaje")]
    [SerializeField] private Transform previewContainer;
    [SerializeField] private float previewScale = 2.5f;

    private void Start()
    {
        reasonText.text = GameManager.Instance.LastGameOverReason;
        scoreText.text = "Final Score: " + GameManager.Instance.Score;

        StartCoroutine(MostrarPersonajeDerotado());
    }

    private IEnumerator MostrarPersonajeDerotado()
    {
        GameObject skinPrefab = GameManager.Instance.GetSelectedSkinPrefab();
        if (skinPrefab == null || previewContainer == null) yield break;

        // Instanciamos la skin directamente (no el Player completo)
        GameObject personaje = Instantiate(skinPrefab, previewContainer);
        personaje.transform.localPosition = Vector3.zero;
        personaje.transform.localScale = Vector3.one * previewScale;

        // ✅ Esperamos un frame para que el Animator inicialice
        yield return null;

        Animator anim = personaje.GetComponentInChildren<Animator>();
        if (anim != null)
            anim.SetBool("Derrotado", true);
        else
            Debug.Log("Animator no encontrado");
    }

    public void OnRetryClicked()
    {
        GameManager.Instance.StartNewGame();
    }

    public void OnMenuClicked()
    {
        GameManager.Instance.GoToMainMenu();
    }
}