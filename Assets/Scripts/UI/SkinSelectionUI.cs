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
    [Tooltip("El objeto vacío donde aparecerá el personaje")]
    [SerializeField] private Transform previewContainer;
    [Tooltip("Escala para que el personaje se vea grande en el menú")]
    [SerializeField] private float previewScale = 2.5f; 

    private int currentIndex = 0;
    private GameObject currentPreviewObject;

    private void Start()
    {
        // 1. Iniciamos el carrusel en la skin que el jugador tiene equipada actualmente
        currentIndex = GameManager.Instance.SelectedSkinId;
        UpdatePreview();
    }

    // Vinculado al botón ">"
    public void OnNextClicked()
    {
        int totalSkins = GameManager.Instance.GetTotalSkins();
        if (totalSkins == 0) return;

        // Navegación circular hacia adelante (0 -> 1 -> 2 -> 0)
        currentIndex = (currentIndex + 1) % totalSkins;
        UpdatePreview();
    }

    // Vinculado al botón "<"
    public void OnPreviousClicked()
    {
        int totalSkins = GameManager.Instance.GetTotalSkins();
        if (totalSkins == 0) return;

        // Navegación circular hacia atrás (0 -> 2 -> 1 -> 0)
        currentIndex = (currentIndex - 1 + totalSkins) % totalSkins;
        UpdatePreview();
    }

    // Vinculado al botón "Seleccionar"
    public void OnEquipClicked()
    {
        GameManager.Instance.SetSkin(currentIndex);
        UpdatePreview(); // Refrescamos para que el botón cambie a "Equipado"
    }

    // Vinculado al botón "Volver"
    public void OnBackClicked()
    {
        GameManager.Instance.GoToMainMenu();
    }

    private void UpdatePreview()
    {
        // 1. Destruimos el modelo anterior para optimizar memoria
        if (currentPreviewObject != null)
        {
            Destroy(currentPreviewObject);
        }

        // 2. Traemos el prefab de la skin actual
        GameObject skinPrefab = GameManager.Instance.GetSkinPrefab(currentIndex);
        if (skinPrefab != null)
        {
            // 3. Instanciamos el personaje en el contenedor
            currentPreviewObject = Instantiate(skinPrefab, previewContainer);
            
            // Lo centramos y lo hacemos más grande para que luzca bien en la UI
            currentPreviewObject.transform.localPosition = Vector3.zero;
            currentPreviewObject.transform.localScale = Vector3.one * previewScale;

            // Si el prefab tiene un Animator, le decimos que está en el suelo para que no haga animaciones de salto
            Animator previewAnim = currentPreviewObject.GetComponent<Animator>();
            if (previewAnim != null)
            {
                // Le decimos que está tocando el suelo para que no salte
                previewAnim.SetBool("EnSuelo", true);
                
                // (Opcional) Aseguramos que la velocidad sea 0 para que no corra
                previewAnim.SetFloat("Movimiento", 0f); 
            }

            // 4. Actualizamos el nombre (Usa el nombre del Prefab y quita guiones bajos)
            skinNameText.text = skinPrefab.name.Replace("_", " ").ToUpper();
        }

        // 5. Lógica del Botón: ¿Ya la tenemos equipada?
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