using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public Camera cam;

    [Header("Zoom Settings")]
    public float zoomSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 12f;

    [Header("Mobile")]
    public float pinchSpeed = 0.02f;

    private void Start()
    {
        if (cam == null)
            cam = Camera.main;
    }

    private void Update()
    {
        HandleMouseZoom();
        HandlePinchZoom();
    }

    // 🖱️ PC Zoom
    private void HandleMouseZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }

    // 📱 Mobile Pinch Zoom
    private void HandlePinchZoom()
    {
        if (Input.touchCount != 2) return;

        Touch t0 = Input.GetTouch(0);
        Touch t1 = Input.GetTouch(1);

        Vector2 prevPos0 = t0.position - t0.deltaPosition;
        Vector2 prevPos1 = t1.position - t1.deltaPosition;

        float prevDistance = (prevPos0 - prevPos1).magnitude;
        float currentDistance = (t0.position - t1.position).magnitude;

        float delta = currentDistance - prevDistance;

        cam.orthographicSize -= delta * pinchSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
}