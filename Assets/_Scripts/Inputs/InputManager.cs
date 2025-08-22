using UnityEngine;


public class InputManager : MonoBehaviour
{
    PlayerControls input;
    Camera mainCamera;

    public event System.Action OnTouchBegin;
    public event System.Action OnTouchEnd;


    protected void Awake()
    {
        input = new PlayerControls();
        mainCamera = Camera.main;
    }
    private void OnEnable()
    {
        input.Enable();
        input.Base.Touch.started += ctx => OnTouchBegin?.Invoke();
        input.Base.Touch.canceled += ctx => OnTouchEnd?.Invoke();
    }

    private void OnDisable()
    {
        input.Disable();
        input.Base.Touch.started -= ctx => OnTouchBegin?.Invoke();
        input.Base.Touch.canceled -= ctx => OnTouchEnd?.Invoke();
    }

    public Vector2 PrimaryPosition()
    {
        if (!mainCamera) return Vector2.zero;

        Vector2 touchPos = input.Base.PrimaryPosition.ReadValue<Vector2>();
        return mainCamera.ScreenToWorldPoint(new Vector3(touchPos.x, touchPos.y, mainCamera.nearClipPlane));
    }
}
