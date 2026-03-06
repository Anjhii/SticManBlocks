using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PowerUpItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo el jugador puede recogerlo
        if (collision.CompareTag("Player"))
        {
            // Le avisamos al GameManager que recolectamos el poder (esto activará el botón UI)
            GameManager.Instance.CollectPowerUp();
            
            // Nos destruimos
            Destroy(gameObject);
        }
    }
}