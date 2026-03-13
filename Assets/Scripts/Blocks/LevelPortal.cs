using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelPortal : MonoBehaviour
{
    [Header("Objeto visual del portal (el que tiene la animación)")]
    [SerializeField] private Animator portalAnimator;
    [SerializeField] private string activateParam = "isActive";

    [Header("Tiempo de espera antes de cambiar de nivel")]
    [SerializeField] private float delayBeforeLevelChange = 2f;

    private bool playerEntered = false;
    private BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        col.enabled = false;
    }

    public void EnableCollider()
    {
        col.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !playerEntered)
        {
            playerEntered = true;
            col.enabled = false;
            StartCoroutine(WaitForPortalAnimation());
            AudioManager.Instance.PlaySFX(AudioManager.Instance.nextLevelPortal);
        }
    }

    private IEnumerator WaitForPortalAnimation()
    {
        if (portalAnimator != null)
        {
            portalAnimator.SetBool(activateParam, true);
        }

        yield return new WaitForSeconds(delayBeforeLevelChange);

        GameManager.Instance.CompleteLevel();
    }
}