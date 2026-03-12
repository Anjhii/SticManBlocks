using UnityEngine;

public class PortalActivator : MonoBehaviour
{
    private Animator animator;
    private bool isActivated = false;

    [Header("Configuración")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private string animatorParameter = "isActive";

    void Start()
    {
        animator = GetComponent<Animator>();

        // Asegura que la animación NO corre al inicio
        animator.SetBool(animatorParameter, false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !isActivated)
        {
            isActivated = true;
            animator.SetBool(animatorParameter, true);
            Debug.Log("¡Portal activado!");
        }
    }

    // Opcional: desactivar al salir del rango
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            // Comenta esto si no quieres que se desactive al salir
            // animator.SetBool(animatorParameter, false);
        }
    }
}