using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private float scrollSpeed = 2f;
    [SerializeField] private float resetPositionY = 10f;
    [SerializeField] private float startPositionY = -10f;

    private float startY;

    private void Start()
    {
        startY = transform.position.y;
    }

    private void Update()
    {
        // Space.World fuerza que suba en Y del mundo, ignorando la rotación del objeto
        transform.Translate(Vector3.up * scrollSpeed * Time.deltaTime, Space.World);

        if (transform.position.y >= startY + resetPositionY)
        {
            transform.position = new Vector3(
                transform.position.x,
                startY + startPositionY,
                transform.position.z
            );
        }
    }
}