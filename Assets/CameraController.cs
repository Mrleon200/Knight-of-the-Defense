using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public Camera cam;

    [Header("Pan Settings")]
    public float dragSpeed = 1f;

    [Header("Bounds")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Vector3 lastMousePos;
    private bool isDragging = false;

    private void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    private void Update()
    {
        HandleMouseDrag();
        HandleTouchDrag();
    }

    // PC Drag
    private void HandleMouseDrag()
    {
    if (Input.GetMouseButtonDown(0))
    {
        if (IsPointerOverUI()) return;

        lastMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        isDragging = true;
    }

    if (Input.GetMouseButtonUp(0))
    {
        isDragging = false;
    }

    if (isDragging)
    {
        Vector3 currentPos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 delta = lastMousePos - currentPos;

        cam.transform.position += delta;

        ClampCamera();

        lastMousePos = cam.ScreenToWorldPoint(Input.mousePosition);
    }
    }

    // Mobile Drag
    private void HandleTouchDrag()
    {
    if (Input.touchCount != 1) return;

    Touch touch = Input.GetTouch(0);

    if (IsPointerOverUI(touch.fingerId)) return;

    if (touch.phase == TouchPhase.Began)
    {
        lastMousePos = cam.ScreenToWorldPoint(touch.position);
    }

    if (touch.phase == TouchPhase.Moved)
    {
        Vector3 currentPos = cam.ScreenToWorldPoint(touch.position);
        Vector3 delta = lastMousePos - currentPos;

        cam.transform.position += delta;

        ClampCamera();

        lastMousePos = currentPos;
    }
    }

    // Check UI block
    private bool IsPointerOverUI(int fingerId = -1)
    {
        if (fingerId >= 0)
            return EventSystem.current.IsPointerOverGameObject(fingerId);

        return EventSystem.current.IsPointerOverGameObject();
    }

    //  Clamp camera trong map
    private void ClampCamera()
    {
        Vector3 pos = cam.transform.position;

        float height = cam.orthographicSize;
        float width = height * cam.aspect;

        pos.x = Mathf.Clamp(pos.x, minBounds.x + width, maxBounds.x - width);
        pos.y = Mathf.Clamp(pos.y, minBounds.y + height, maxBounds.y - height);

        cam.transform.position = pos;
    }
}