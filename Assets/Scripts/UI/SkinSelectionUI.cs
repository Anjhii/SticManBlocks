using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI skinNameText;
    [SerializeField] private Button selectButton;
    [SerializeField] private TextMeshProUGUI selectButtonText;

    [Header("Preview Settings")]
    [SerializeField] private Transform previewContainer;
    [SerializeField] private float previewScale = 2.5f; 

    private int currentIndex = 0;
    private int originalSkinIndex = 0; //  Guardamos la skin original
    private GameObject currentPreviewObject;

    private void Start()
    {
        currentIndex = GameManager.Instance.SelectedSkinId;
        originalSkinIndex = currentIndex; // Guardamos cuál tenía antes de entrar
        UpdatePreview();
    }

    public void OnNextClicked()
    {
        int totalSkins = GameManager.Instance.GetTotalSkins();
        if (totalSkins == 0) return;
        currentIndex = (currentIndex + 1) % totalSkins;
        UpdatePreview();
    }

    public void OnPreviousClicked()
    {
        int totalSkins = GameManager.Instance.GetTotalSkins();
        if (totalSkins == 0) return;
        currentIndex = (currentIndex - 1 + totalSkins) % totalSkins;
        UpdatePreview();
    }

    public void OnEquipClicked()
    {
        GameManager.Instance.SetSkin(currentIndex);
        UpdatePreview();
    }

    //  Cancelar restaura la skin original y vuelve al menú
    public void OnCancelClicked()
    {
        GameManager.Instance.SetSkin(originalSkinIndex);
        GameManager.Instance.GoToMainMenu();
    }

    
    public void OnBackClicked()
    {
        GameManager.Instance.GoToMainMenu();
    }

    private void UpdatePreview()
    {
        if (currentPreviewObject != null)
            Destroy(currentPreviewObject);

        GameObject skinPrefab = GameManager.Instance.GetSkinPrefab(currentIndex);
        if (skinPrefab != null)
        {
            currentPreviewObject = Instantiate(skinPrefab, previewContainer);
            currentPreviewObject.transform.localPosition = Vector3.zero;
            currentPreviewObject.transform.localScale = Vector3.one * previewScale;

            Animator previewAnim = currentPreviewObject.GetComponent<Animator>();
            if (previewAnim != null)
            {
                previewAnim.SetBool("EnSuelo", true);
                previewAnim.SetFloat("Movimiento", 0f); 
            }

            skinNameText.text = skinPrefab.name.Replace("_", " ").ToUpper();
        }

        if (currentIndex == GameManager.Instance.SelectedSkinId)
        {
            selectButton.interactable = false;
            if (selectButtonText != null) selectButtonText.text = "EQUIPPED";
        }
        else
        {
            selectButton.interactable = true;
            if (selectButtonText != null) selectButtonText.text = "SELECT";
        }
    }
}