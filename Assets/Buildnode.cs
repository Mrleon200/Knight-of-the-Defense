using UnityEngine;

/// <summary>
/// BuildNode - Ô đất có thể đặt tower
/// Gắn vào GameObject có Collider2D
/// Khi click → hiện BuildMenuUI
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class BuildNode : MonoBehaviour
{
    [Header("Node State")]
    public bool isOccupied = false; // True = đã có tower

    [Header("Visual Feedback (Optional)")]
    public SpriteRenderer nodeRenderer; // Để đổi màu khi hover/occupied
    public Color availableColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    public Color occupiedColor  = new Color(0.5f, 0.5f, 0.5f, 0.6f);
    public Color hoverColor     = new Color(1f, 1f, 0.8f, 1f);

    private bool _isHovered = false;

    private void Start()
    {
        // Auto-get SpriteRenderer nếu chưa gán
        if (nodeRenderer == null)
            nodeRenderer = GetComponent<SpriteRenderer>();

        UpdateVisual();
    }

    private void OnMouseEnter()
    {
        if (isOccupied) return;
        _isHovered = true;
        UpdateVisual();
    }

    private void OnMouseExit()
    {
        _isHovered = false;
        UpdateVisual();
    }

    private void OnMouseDown()
    {
        // Nếu đã có tower → không làm gì
        if (isOccupied)
        {
            Debug.Log("[BuildNode] Ô này đã có tower!");
            return;
        }

        // Hiện menu chọn tower
        if (BuildMenuUI.Instance != null)
        {
            BuildMenuUI.Instance.Show(this);
        }
        else
        {
            Debug.LogError("[BuildNode] BuildMenuUI.Instance is null!");
        }
    }

    /// <summary>
    /// Gọi sau khi build tower thành công
    /// </summary>
    public void MarkAsOccupied()
    {
        isOccupied = true;
        _isHovered = false;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (nodeRenderer == null) return;

        if (isOccupied)
            nodeRenderer.color = occupiedColor;
        else if (_isHovered)
            nodeRenderer.color = hoverColor;
        else
            nodeRenderer.color = availableColor;
    }
}