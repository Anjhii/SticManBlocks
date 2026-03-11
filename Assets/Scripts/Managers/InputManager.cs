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

    [Header("Swipe Settings")]
    [Tooltip("Porcentaje de la altura de la pantalla para evitar toques accidentales (Ej: 0.05 = 5%)")]
    [Range(0.01f, 0.2f)]
    [SerializeField] private float minSwipePercentage = 0.05f; 

    [Tooltip("Porcentaje de la altura de pantalla necesario para el 100% del poder (Ej: 0.35 = 35%)")]
    [Range(0.2f, 0.8f)]
    [SerializeField] private float maxSwipePercentage = 0.45f;

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

                // Calculamos en píxeles cuánto representan nuestros porcentajes en este celular específico
                float minSwipePixels = Screen.height * minSwipePercentage;
                float maxSwipePixels = Screen.height * maxSwipePercentage;
                Debug.Log($"Swipe Delta: {swipeDelta}, Min Swipe Pixels: {minSwipePixels}, Max Swipe Pixels: {maxSwipePixels}");

                // 1. Verificamos que el swipe sea mayor al mínimo (evita accidentes)
                // 2. Verificamos que sea predominantemente vertical
                if (swipeDelta.y > minSwipePixels && Mathf.Abs(swipeDelta.y) > Mathf.Abs(swipeDelta.x))
                {
                    // Mathf.InverseLerp :
                    // Si swipeDelta.y es igual a minSwipePixels, devuelve 0.0.
                    // Si swipeDelta.y es igual o mayor a maxSwipePixels, devuelve 1.0 (limitado).
                    // Si está a la mitad, devuelve 0.5.
                    float normalizedMagnitude = Mathf.InverseLerp(minSwipePixels, maxSwipePixels, swipeDelta.y);
                    
                    OnSwipeUp?.Invoke(normalizedMagnitude);
                    Debug.Log($"Swipe Up Detectado con magnitud normalizada: {normalizedMagnitude}");
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