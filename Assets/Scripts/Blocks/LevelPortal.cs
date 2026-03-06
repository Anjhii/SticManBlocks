using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class LevelPortal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.CompleteLevel();
            // Desactivamos su collider para que no lance el evento dos veces
            GetComponent<BoxCollider2D>().enabled = false; 
        }
    }
}