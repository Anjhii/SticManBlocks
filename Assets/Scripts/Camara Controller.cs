using UnityEngine;

public class CamaraController : MonoBehaviour
{
    public Transform objetivo;
    public float velocidadCamara = 0.025f;
    public Vector3 desplazamiento;

    private float maxYAlcanzada = 0f;

    private void Start()
    {
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    private void LateUpdate()
    {
        float targetY = objetivo.position.y + desplazamiento.y;
        if (targetY > maxYAlcanzada) maxYAlcanzada = targetY;

        float nuevaY = maxYAlcanzada > 0f
            ? Mathf.Lerp(transform.position.y, maxYAlcanzada, velocidadCamara)
            : 0f;

        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
    }
}