using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Accelerometer Settings")]
    [SerializeField] private float smoothSpeed = 10f;
    private float smoothedAccelX;
    
    public float MovementX => smoothedAccelX;
    
    // NUEVO: Propiedad de estado continuo. Devuelve true mientras el dedo siga en la pantalla.
    public bool IsJumpHeld => controls.GamePlay.Jump.IsPressed();

    public System.Action OnDoubleTapTwoFingers;

    private PlayerControls controls;
    private float lastTapTime = 0f;
    private const float doubleTapThreshold = 0.3f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        controls = new PlayerControls();
        // ELIMINADO: controls.GamePlay.Jump.performed += ...
    }

    private void OnEnable()
    {
        controls.Enable();
        EnhancedTouchSupport.Enable();
        if (Accelerometer.current != null) InputSystem.EnableDevice(Accelerometer.current);
    }

    private void OnDisable()
    {
        controls.Disable();
        EnhancedTouchSupport.Disable();
    }

    private void Update()
    {
        // 1. Suavizado de Acelerómetro
        if (Accelerometer.current != null)
        {
            float rawX = Accelerometer.current.acceleration.ReadValue().x;
            smoothedAccelX = Mathf.Lerp(smoothedAccelX, rawX, Time.deltaTime * smoothSpeed);
        }

        // 2. Detección del PowerUp (Dos dedos)
        if (Touch.activeTouches.Count == 2)
        {
            bool bothTapped = true;
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began)
                    bothTapped = false;
            }

            if (bothTapped)
            {
                if (Time.time - lastTapTime <= doubleTapThreshold)
                {
                    OnDoubleTapTwoFingers?.Invoke();
                    lastTapTime = 0f;
                }
                else
                {
                    lastTapTime = Time.time;
                }
            }
        }
    }
}