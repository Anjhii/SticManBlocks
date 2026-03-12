using UnityEngine;
using System.Collections;

public class PlayerFeedback : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float flashDuration = 0.5f;

    [SerializeField] private Transform skinContainer;

    private SpriteRenderer skinSprite;
    private Color originalColor;

    private void Start()
    {
        GameManager.Instance.OnPlayerHit += TriggerDamageFlash;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPlayerHit -= TriggerDamageFlash;
    }

    private void TriggerDamageFlash()
    {
        if (skinSprite == null)
        {
            skinSprite = skinContainer.GetComponentInChildren<SpriteRenderer>();

            if (skinSprite != null)
            {
                originalColor = skinSprite.color;
                Debug.Log($"PlayerFeedback atrapó el sprite de: {skinSprite.gameObject.name}");
            }
        }

        if (skinSprite == null)
        {
            Debug.LogWarning("PlayerFeedback no encontró el SpriteRenderer de la Skin.");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        skinSprite.color = damageColor;

        yield return new WaitForSeconds(flashDuration);

        skinSprite.color = originalColor;
    }
}