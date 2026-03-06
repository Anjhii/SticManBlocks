using UnityEngine;
using UnityEngine.SceneManagement;

public class SkinSelectionUI : MonoBehaviour
{
    public void OnEquipSkinClicked(int skinId)
    {
        GameManager.Instance.SetSkin(skinId);
    }

    public void OnBackClicked()
    {
        GameManager.Instance.GoToMainMenu();
    }
}
