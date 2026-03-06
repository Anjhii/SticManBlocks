using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI reasonText;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        // Leemos la razón de la derrota y el puntaje final desde el GameManager persistente
        reasonText.text = GameManager.Instance.LastGameOverReason;
        scoreText.text = "Final Score: " + GameManager.Instance.Score;
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
