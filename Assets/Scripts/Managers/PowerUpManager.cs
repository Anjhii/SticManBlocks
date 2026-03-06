using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private void Start()
    {
        InputManager.Instance.OnDoubleTapTwoFingers += ActivatePowerUp;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.CollectPowerUp();
            Destroy(gameObject);
        }
    }

    public void ActivatePowerUp()
    {
        Debug.Log("¡Efecto del PowerUp Ejecutado!");
        
        // Aquí irá tu lógica de invencibilidad, doble salto, etc.

        // Le avisamos al GameManager que ya lo gastamos
        GameManager.Instance.ConsumePowerUp(); 
    }
}