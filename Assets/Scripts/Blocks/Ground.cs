using System.Collections;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Altura Y a la que consideras que el primer bloque ya es alcanzable")]
    [SerializeField] private float reachableY = -1.5f; 

    private void Start()
    {
        // Usamos una corrutina para asegurar que los Managers ya se inicializaron
        StartCoroutine(DisappearRoutine());
    }

    private IEnumerator DisappearRoutine()
    {
        // 1. Esperamos 1 frame para que SpawnerManager y LevelManager configuren sus variables
        yield return new WaitForEndOfFrame();

        // 2. Traemos los datos parametrizados de nuestra arquitectura
        float spawnY = SpawnerManager.Instance.GetSpawnY();
        float blockSpeed = LevelManager.Instance.GetBlockSpeed();

        // 3. Calculamos la distancia de caída (Desde donde nace hasta donde lo podemos saltar)
        float distance = spawnY - reachableY;

        // 4. Calculamos el tiempo exacto. 
        // Le sumamos 0.5f porque el SpawnerManager tiene un delay inicial de 0.5s en su Start.
        float timeToDisappear = (distance / blockSpeed) + 0.5f;

        // 5. Esperamos ese tiempo exacto
        yield return new WaitForSeconds(timeToDisappear);

        // 6. ¡Magia! El suelo desaparece
        Debug.Log("¡El suelo inicial desapareció! Obligando al jugador a escalar.");
        
        // Opcional: Podrías reproducir un sonido de derrumbe o partículas aquí
        gameObject.SetActive(false); 
    }
}
