using UnityEngine;

public class SpawnStopTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Evitamos que se ejecute dos veces si el jugador entra y sale
        if (hasTriggered) return; 

        // Si el que entró fue el jugador
        if (collision.CompareTag("Player"))
        {
            hasTriggered = true;
            
            // Llamamos al freno de emergencia del Spawner
            if (SpawnerManager.Instance != null)
            {
                SpawnerManager.Instance.StopSpawning();
                Debug.Log("¡El jugador cruzó el límite! Spawner detenido.");
            }
        }
    }
}
