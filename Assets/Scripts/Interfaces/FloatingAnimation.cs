using UnityEngine;

public class FloatingAnimation : MonoBehaviour
{
    [Header("Floating Settings")]
    public float amplitude = 0.3f;    // Qué tan alto/bajo sube y baja
    public float frequency = 1.0f;    // Qué tan rápido oscila
    
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
