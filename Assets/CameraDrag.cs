using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDrag : MonoBehaviour
{
    [Header("Drag")]
    public float dragSpeed = 0.01f;
    public float inertia = 5f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    [Header("Bounds")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Camera cam;

    private Vector3 lastPos;
    private Vector3 velocity;
    private bool isDragging = false;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleMouse();
        HandleTouch();
        HandleZoom();
        ApplyInertia();
        ClampPosition();
    }

    // ================= PC =================
    void HandleMouse()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            lastPos = Input.mousePosition;
            velocity = Vector3.zero;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastPos;
            Vector3 move = new Vector3(delta.x, delta.y, 0) * dragSpeed;

            transform.position -= move;
            velocity = move;

            lastPos = Input.mousePosition;
        }
    }

    // ================= MOBILE =================
    void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;

            if (touch.phase == TouchPhase.Began)
            {
                lastPos = touch.position;
                velocity = Vector3.zero;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 delta = touch.position - (Vector2)lastPos;
                Vector3 move = new Vector3(delta.x, delta.y, 0) * dragSpeed;

                transform.position -= move;
                velocity = move;

                lastPos = touch.position;
            }
        }
    }

    // ================= ZOOM =================
    void HandleZoom()
    {
        // PC scroll
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
        }

        // Mobile pinch
        if (Input.touchCount == 2)
        {
            Touch t0 = Input.GetTouch(0);
            Touch t1 = Input.GetTouch(1);

            Vector2 prev0 = t0.position - t0.deltaPosition;
            Vector2 prev1 = t1.position - t1.deltaPosition;

            float prevDist = Vector2.Distance(prev0, prev1);
            float currentDist = Vector2.Distance(t0.position, t1.position);

            float delta = currentDist - prevDist;

            cam.orthographicSize -= delta * 0.01f;
        }

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    // ================= INERTIA =================
    void ApplyInertia()
    {
        if (!isDragging)
        {
            transform.position -= velocity;
            velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * inertia);
        }
    }

    // ================= LIMIT =================
    void ClampPosition()
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float minX = minBounds.x + camWidth;
        float maxX = maxBounds.x - camWidth;
        float minY = minBounds.y + camHeight;
        float maxY = maxBounds.y - camHeight;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        transform.position = pos;
    }
}