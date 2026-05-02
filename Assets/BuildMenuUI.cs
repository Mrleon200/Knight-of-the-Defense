using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// BuildMenuUI - Menu vòng tròn chọn tower (Kingdom Rush style)
/// Hiện khi click vào BuildNode
/// </summary>
public class BuildMenuUI : MonoBehaviour
{
    public static BuildMenuUI Instance { get; private set; }

    [Header("References")]
    public Canvas menuCanvas; // Canvas của menu này (World Space)
    
    [Header("Tower Buttons")]
    public TowerMenuButton[] towerButtons; // Kéo các button vào đây theo thứ tự

    [Header("Animation")]
    public float showAnimDuration = 0.3f;
    public float hideAnimDuration = 0.2f;

    private BuildNode _currentNode;
    private bool _isAnimating = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Đảm bảo menu ẩn lúc đầu
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Hiện menu tại vị trí BuildNode
    /// </summary>
    public void Show(BuildNode node)
    {
        if (_isAnimating) return;

        _currentNode = node;

        // Di chuyển menu đến vị trí node
        if (menuCanvas != null && menuCanvas.renderMode == RenderMode.WorldSpace)
        {
            transform.position = node.transform.position;
        }
        else
        {
            Debug.LogWarning("[BuildMenuUI] Canvas phải là World Space!");
        }

        gameObject.SetActive(true);

        // Cập nhật trạng thái buttons (afford/not afford)
        RefreshButtons();

        // Animation scale in
        StartCoroutine(ShowAnimation());
    }

    /// <summary>
    /// Ẩn menu
    /// </summary>
    public void Hide()
    {
        if (_isAnimating) return;
        StartCoroutine(HideAnimation());
    }

    /// <summary>
    /// Gọi khi người chơi chọn tower (từ button onClick)
    /// </summary>
    public void SelectTower(int towerIndex)
    {
        if (_currentNode == null)
        {
            Debug.LogError("[BuildMenuUI] currentNode is null!");
            return;
        }

        // Thử build tower
        bool success = TowerPlacer.Instance.PlaceTower(towerIndex, _currentNode.transform.position);

        if (success)
        {
            // Đánh dấu node đã bị chiếm
            _currentNode.MarkAsOccupied();

            // Ẩn menu
            Hide();
        }
        else
        {
            // Build thất bại (thiếu tiền) → giữ menu mở, chỉ shake/flash button
            if (towerIndex >= 0 && towerIndex < towerButtons.Length)
            {
                towerButtons[towerIndex].PlayErrorFeedback();
            }
        }
    }

    /// <summary>
    /// Cập nhật trạng thái afford/not afford của buttons
    /// </summary>
    private void RefreshButtons()
    {
        if (towerButtons == null || GameManager.Instance == null) return;

        int currentGold = GameManager.Instance.gold;

        for (int i = 0; i < towerButtons.Length; i++)
        {
            if (towerButtons[i] != null)
                towerButtons[i].RefreshAffordability(currentGold);
        }
    }

    // ══════════════════════════════════════════════════════
    // Animations
    // ══════════════════════════════════════════════════════

    private System.Collections.IEnumerator ShowAnimation()
    {
        _isAnimating = true;
        transform.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < showAnimDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / showAnimDuration;
            
            // Elastic ease out
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.localScale = Vector3.one * scale;

            yield return null;
        }

        transform.localScale = Vector3.one;
        _isAnimating = false;
    }

    private System.Collections.IEnumerator HideAnimation()
    {
        _isAnimating = true;

        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < hideAnimDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / hideAnimDuration;
            transform.localScale = startScale * (1f - t);
            yield return null;
        }

        gameObject.SetActive(false);
        _isAnimating = false;
        _currentNode = null;
    }
}