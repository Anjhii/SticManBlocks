using TMPro;
using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Start()
    {
        scoreText.text = "Score: " + GameManager.Instance.Score;
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
