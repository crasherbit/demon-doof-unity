using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TouchInputManager : MonoBehaviour
{
    // Variabili statiche per la gestione del tocco
    private static bool isTouchActive = false;
    private static Vector2 lastTouchPosition;
    private static float pinchDistance;

    // Sensibilità degli input touch (per swipe, pinch, rotazione)
    public static float touchSensitivity = 1.0f;  // Può essere regolato

    // Gestione dello stato per limitare un solo movimento alla volta
    private static bool isPanActive = false;
    private static bool isZoomActive = false;
    private static bool isRotateActive = false;
    private static bool isTapActive = false;

    // Singleton
    public static TouchInputManager Instance { get; private set; }

    // Inizializzazione per EnhancedTouch, abilitato solo se siamo su dispositivi touch
    public static void Initialize()
    {
        if (Application.isMobilePlatform)
        {
            Debug.Log("Touchscreen not enabled, enabling it now");
            isTouchActive = true;
            EnhancedTouchSupport.Enable();
        }
        else if (!Application.isMobilePlatform)
        {
            // only for debug in the editor for now
            isTouchActive = true;
            EnhancedTouchSupport.Enable();
            Debug.Log("Touchscreen not enabled, not on mobile platform");
        }
        else
        {
            Debug.Log("Touchscreen already enabled");
        }
    }

    private void Awake()
    {
        // Gestione del singleton
        if (Instance != null)
        {
            Debug.LogError("There's more than one TouchInputManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Verifica se un tocco è in corso
    public static bool IsTouching()
    {
        if (isTouchActive && Touch.activeTouches.Count > 0)
        {
            return Touch.activeTouches[0].phase == TouchPhase.Moved || Touch.activeTouches[0].phase == TouchPhase.Stationary;
        }
        return false;
    }

    // Restituisce la posizione del primo tocco
    public static Vector2 GetTouchPosition()
    {
        if (isTouchActive && Touch.activeTouches.Count > 0)
        {
            return Touch.activeTouches[0].screenPosition;
        }
        return Vector2.zero;
    }

    // Verifica se è stato effettuato un singolo tap
    public static bool IsSingleTap()
    {
        if (isTouchActive && Touch.activeTouches.Count == 1)
        {
            Touch touch = Touch.activeTouches[0];
            if (touch.phase == TouchPhase.Began && !isPanActive && !isZoomActive && !isRotateActive)
            {
                // Inizializza il tap solo se non c'è pan, zoom o rotazione attivi
                lastTouchPosition = touch.screenPosition;
                isTapActive = true;
                return true;
            }
        }
        if (isTapActive && Touch.activeTouches.Count == 1)
        {
            Touch touch = Touch.activeTouches[0];
            if (touch.phase == TouchPhase.Ended)
            {
                // Rimuovi il tap al rilascio del tocco
                isTapActive = false;
                return true;
            }
        }
        return false;
    }

    // Verifica se c'è un movimento di swipe (trascinamento del dito) con sensibilità
    public static bool IsSwipe(out Vector2 swipeDirection)
    {
        swipeDirection = Vector2.zero;
        if (isTouchActive && Touch.activeTouches.Count > 0 && !isPanActive)
        {
            Touch touch = Touch.activeTouches[0];
            if (touch.phase == TouchPhase.Moved)
            {
                swipeDirection = (touch.screenPosition - lastTouchPosition) * touchSensitivity;
                lastTouchPosition = touch.screenPosition;
                isPanActive = true;  // Abilita il pan
                return swipeDirection.magnitude > 0.1f; // Controllo della sensibilità
            }
        }
        return false;
    }

    // Verifica se c'è un pinch (zoom) con sensibilità
    public static bool IsPinch(out float pinchAmount)
    {
        pinchAmount = 0f;
        if (isTouchActive && Touch.activeTouches.Count == 2 && !isZoomActive)
        {
            var touch0 = Touch.activeTouches[0];
            var touch1 = Touch.activeTouches[1];
            Vector2 touch0Pos = touch0.screenPosition;
            Vector2 touch1Pos = touch1.screenPosition;
            float currentPinchDistance = (touch0Pos - touch1Pos).magnitude;

            if (Mathf.Approximately(pinchDistance, 0f))
            {
                pinchDistance = currentPinchDistance;
            }
            pinchAmount = (pinchDistance - currentPinchDistance) * touchSensitivity; // Sensibilità applicata
            isZoomActive = true;  // Abilita il pinch
            return Mathf.Abs(pinchAmount) > 0.1f; // Controllo della sensibilità
        }
        pinchDistance = 0f;
        return false;
    }

    // Verifica se c'è una rotazione con due dita con sensibilità
    public static bool IsRotation(out float rotationAmount)
    {
        rotationAmount = 0f;
        if (isTouchActive && Touch.activeTouches.Count == 2 && !isRotateActive)
        {
            var touch0 = Touch.activeTouches[0];
            var touch1 = Touch.activeTouches[1];
            Vector2 touch0PrevPos = touch0.screenPosition;
            Vector2 touch1PrevPos = touch1.screenPosition;
            float prevAngle = Mathf.Atan2(touch1PrevPos.y - touch0PrevPos.y, touch1PrevPos.x - touch0PrevPos.x) * Mathf.Rad2Deg;

            Vector2 touch0Pos = touch0.screenPosition;
            Vector2 touch1Pos = touch1.screenPosition;
            float currentAngle = Mathf.Atan2(touch1Pos.y - touch0Pos.y, touch1Pos.x - touch0Pos.x) * Mathf.Rad2Deg;

            rotationAmount = (currentAngle - prevAngle) * touchSensitivity; // Sensibilità applicata
            isRotateActive = true;  // Abilita la rotazione
            return Mathf.Abs(rotationAmount) > 0.1f; // Controllo della sensibilità
        }
        return false;
    }

    // Reset dello stato quando il tocco è stato rilasciato
    public static void ResetTouchState()
    {
        isPanActive = false;
        isZoomActive = false;
        isRotateActive = false;
        isTapActive = false;
    }
}
