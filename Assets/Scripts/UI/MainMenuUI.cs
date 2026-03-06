using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject gameManagerPrefab; 

    private void Start()
    {
        if (GameManager.Instance == null && gameManagerPrefab != null)
        {
            Instantiate(gameManagerPrefab);
        }
    }

    public void OnPlayClicked()
    {
        GameManager.Instance.StartNewGame();
    }

    public void OnSkinsClicked()
    {
        SceneManager.LoadScene("SkinSelection");
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }
}
