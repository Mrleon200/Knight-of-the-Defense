using UnityEngine;
using UnityEngine.EventSystems;

public class CameraDrag : MonoBehaviour
{
    [Header("Drag")]
    public float dragSpeed = 0.01f;

    [Header("Zoom")]
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 20f;

    [Header("Bounds")]
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Camera cam;

    private Vector3 lastPos;
    private bool isDragging = false;

    void Start()
    {
        cam = Camera.main;
        FitCameraToMap(); // 🔥 fix mobile + aspect
    }

    void Update()
    {
        HandleMouseDrag();
        HandleTouchDrag();
        HandleZoom();

        ClampCamera(); // 🔥 QUAN TRỌNG: luôn clamp sau khi di chuyển
    }

    // ================= DRAG PC =================
    void HandleMouseDrag()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(1))
        {
            isDragging = true;
            lastPos = Input.mousePosition;
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
            lastPos = Input.mousePosition;
        }
    }

    // ================= DRAG MOBILE =================
    void HandleTouchDrag()
    {
        if (Input.touchCount != 1) return;

        Touch touch = Input.GetTouch(0);

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        if (touch.phase == TouchPhase.Began)
        {
            lastPos = touch.position;
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            Vector3 delta = touch.position - (Vector2)lastPos;
            Vector3 move = new Vector3(delta.x, delta.y, 0) * dragSpeed;

            transform.position -= move;
            lastPos = touch.position;
        }
    }

    // ================= ZOOM =================
    void HandleZoom()
    {
        // PC
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
            float currDist = Vector2.Distance(t0.position, t1.position);

            float delta = currDist - prevDist;

            cam.orthographicSize -= delta * 0.01f;
        }

        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }

    // ================= CLAMP =================
    void ClampCamera()
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float mapWidth = maxBounds.x - minBounds.x;
        float mapHeight = maxBounds.y - minBounds.y;

        Vector3 pos = transform.position;

        // X axis
        if (mapWidth <= camWidth * 2)
        {
            pos.x = (minBounds.x + maxBounds.x) / 2;
        }
        else
        {
            pos.x = Mathf.Clamp(pos.x,
                minBounds.x + camWidth,
                maxBounds.x - camWidth);
        }

        // Y axis
        if (mapHeight <= camHeight * 2)
        {
            pos.y = (minBounds.y + maxBounds.y) / 2;
        }
        else
        {
            pos.y = Mathf.Clamp(pos.y,
                minBounds.y + camHeight,
                maxBounds.y - camHeight);
        }

        transform.position = pos;
    }

    // ================= FIT MAP =================
    void FitCameraToMap()
    {
        float mapWidth = maxBounds.x - minBounds.x;
        float mapHeight = maxBounds.y - minBounds.y;

        float screenRatio = cam.aspect;
        float targetRatio = mapWidth / mapHeight;

        if (screenRatio >= targetRatio)
        {
            cam.orthographicSize = mapHeight / 2f;
        }
        else
        {
            float difference = targetRatio / screenRatio;
            cam.orthographicSize = mapHeight / 2f * difference;
        }
    }
}