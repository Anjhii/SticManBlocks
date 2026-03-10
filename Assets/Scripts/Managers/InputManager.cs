using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Accelerometer Settings")]
    [SerializeField] private float smoothSpeed = 10f;
    private float smoothedAccelX;
    
    public float MovementX => smoothedAccelX;
    public bool WasJumpPressed => controls.GamePlay.Jump.WasPressedThisFrame();
    public Action<float> OnSwipeUp;

    public System.Action OnDoubleTapTwoFingers;

    private PlayerControls controls;
    private float lastTapTime = 0f;
    private const float doubleTapThreshold = 0.3f;
    private Vector2 touchStartPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        controls = new PlayerControls();
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

        // 2. Detección de Swipe Up con un dedo
        if (Touch.activeTouches.Count == 1)
        {
            var touch = Touch.activeTouches[0];
            
            if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
            {
                touchStartPos = touch.screenPosition;
            }
            else if (touch.phase == UnityEngine.InputSystem.TouchPhase.Ended)
            {
                Vector2 swipeDelta = touch.screenPosition - touchStartPos;

                // Verificamos si el movimiento fue hacia ARRIBA y si fue más vertical que horizontal
                if (swipeDelta.y > 50f && Mathf.Abs(swipeDelta.y) > Mathf.Abs(swipeDelta.x))
                {
                    // Convertimos la longitud de píxeles a un porcentaje de la pantalla (0.0 a 1.0)
                    float normalizedMagnitude = Mathf.Clamp01(swipeDelta.y / Screen.height);
                    OnSwipeUp?.Invoke(normalizedMagnitude);
                }
            }
        }

        // 3. Detección de Doble Tap con dos dedos
        if (Touch.activeTouches.Count == 2)
        {
            bool bothTapped = true;
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase != UnityEngine.InputSystem.TouchPhase.Began) bothTapped = false;
            }

            if (bothTapped)
            {
                if (Time.time - lastTapTime <= doubleTapThreshold)
                {
                    OnDoubleTapTwoFingers?.Invoke();
                    lastTapTime = 0f;
                }
                else lastTapTime = Time.time;
            }
        }
    }
}