using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// TowerMenuButton - Nút tower trong BuildMenuUI
/// Gắn vào từng button trong menu vòng tròn
/// </summary>
[RequireComponent(typeof(Button))]
public class TowerMenuButton : MonoBehaviour
{
    [Header("Tower Info")]
    public int towerIndex; // 0=Archer, 1=Barracks, 2=Mage
    public int towerCost;  // Phải khớp với TowerPlacer.towerCosts

    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI costText;
    public Image buttonBackground;

    [Header("Visual States")]
    public Color affordableColor    = new Color(1f, 1f, 1f, 1f);
    public Color notAffordableColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        
        // Setup onClick
        _button.onClick.AddListener(OnButtonClick);
    }

    private void Start()
    {
        // Hiển thị cost
        if (costText != null)
            costText.text = towerCost + "G";
    }

    public void RefreshAffordability(int currentGold)
    {
        bool canAfford = currentGold >= towerCost;

        _button.interactable = canAfford;

        // Đổi màu background
        if (buttonBackground != null)
            buttonBackground.color = canAfford ? affordableColor : notAffordableColor;

        // Đổi alpha icon
        if (iconImage != null)
        {
            Color c = iconImage.color;
            c.a = canAfford ? 1f : 0.5f;
            iconImage.color = c;
        }

        // Đổi alpha text
        if (costText != null)
        {
            Color c = costText.color;
            c.a = canAfford ? 1f : 0.6f;
            costText.color = c;
        }
    }

    private void OnButtonClick()
    {
        // Gọi BuildMenuUI để xử lý
        if (BuildMenuUI.Instance != null)
        {
            BuildMenuUI.Instance.SelectTower(towerIndex);
        }
    }

    /// <summary>
    /// Hiệu ứng khi build thất bại (thiếu tiền)
    /// </summary>
    public void PlayErrorFeedback()
    {
        StartCoroutine(ShakeAnimation());
    }

    private System.Collections.IEnumerator ShakeAnimation()
    {
        Vector3 originalPos = transform.localPosition;
        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float x = Random.Range(-5f, 5f);
            transform.localPosition = originalPos + new Vector3(x, 0, 0);
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}