using UnityEngine;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private PlayerInputActions playerInputActions;
    // Variabile per memorizzare se è stato rilevato un tap
    private bool tapDetected = false;
    private Touch? tap;
    void Update()
    {
        if (playerInputActions.Touch.enabled)
        {
            tapDetected = CheckForTap();
        }
    }
    // Funzione che verifica se è stato effettuato un tap
    private bool CheckForTap()
    {
        if (Touch.activeTouches.Count == 1)
        {
            Touch touch = Touch.activeTouches[0];
            // Se il tocco è appena iniziato
            if (touch.phase == TouchPhase.Began)
            {
                tap = null;
                return false; // Non è ancora un tap, dobbiamo aspettare la fine
            }

            // Se il tocco è finito, verifica se è un tap
            if (touch.phase == TouchPhase.Ended)
            {
                var isTap = IsTap(touch);
                if (isTap) tap = touch;
                return isTap;
            }
        }
        return false;
    }
    // Funzione per verificare se il tocco è un tap
    private bool IsTap(Touch touch)
    {
        // Verifica che la posizione finale sia simile a quella iniziale
        return Vector2.Distance(touch.screenPosition, touch.startScreenPosition) < 10f;  // Tolleranza di 10px
    }
    // Funzione pubblica per ricevere lo stato del tap
    private bool IsTapDetected()
    {
        return tapDetected;
    }
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InputManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();
        if (Application.isMobilePlatform)
        {
            playerInputActions.Touch.Enable();
        }
        else
        {
            playerInputActions.PC.Enable();
        }
    }
    private void Start()
    {
        Debug.Log("InputManager initialized");
        Debug.Log("Touchscreen enabled: " + playerInputActions.Touch.enabled);
        Debug.Log("PC controls enabled: " + playerInputActions.PC.enabled);
    }

    public Vector2 GetMouseScreenPosition()
    {
        if (playerInputActions.PC.enabled) return Mouse.current.position.ReadValue();
        if (playerInputActions.Touch.enabled && tap != null) return tap.Value.screenPosition;
        return Vector2.zero;
    }

    public bool IsMouseButtonDownThisFrame()
    {
        if (playerInputActions.PC.enabled) return Mouse.current.leftButton.wasPressedThisFrame;
        if (playerInputActions.Touch.enabled) return IsTapDetected();
        return false;
    }

    public Vector2 GetCameraMoveVector()
    {
        if (playerInputActions.PC.enabled) return playerInputActions.PC.CameraMovement.ReadValue<Vector2>();
        if (playerInputActions.Touch.enabled) return playerInputActions.Touch.CameraMovement.ReadValue<Vector2>();
        return Vector2.zero;
    }

    public float GetCameraRotateAmount()
    {
        if (playerInputActions.PC.enabled) return playerInputActions.PC.CameraRotate.ReadValue<float>();
        if (playerInputActions.Touch.enabled) return playerInputActions.Touch.CameraRotate.ReadValue<float>();
        return 0;
    }

    public float GetCameraZoomAmount()
    {
        if (playerInputActions.PC.enabled) return playerInputActions.PC.CameraZoom.ReadValue<float>();
        if (playerInputActions.Touch.enabled) return playerInputActions.Touch.CameraZoom.ReadValue<float>();
        return 0;
    }

}
