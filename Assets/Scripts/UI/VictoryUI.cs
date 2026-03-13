using TMPro;
using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Personaje")]
    [SerializeField] private Transform previewContainer;
    [SerializeField] private float previewScale = 2.5f;

    private void Start()
    {
        scoreText.text = "Score: " + GameManager.Instance.Score;
        MostrarPersonaje();
    }

    private void MostrarPersonaje()
    {
        GameObject skinPrefab = GameManager.Instance.GetSelectedSkinPrefab();
        if (skinPrefab == null || previewContainer == null) return;

        GameObject personaje = Instantiate(skinPrefab, previewContainer);
        personaje.transform.localPosition = Vector3.zero;
        personaje.transform.localScale = Vector3.one * previewScale;

        // ✅ Idle: en suelo y sin movimiento
        Animator anim = personaje.GetComponentInChildren<Animator>();
        if (anim != null)
        {
            anim.SetBool("EnSuelo", true);
            anim.SetFloat("Movimiento", 0f);
        }
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