using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class TowerMenuButton : MonoBehaviour
{
    [Header("Tower Info")]
    public int towerIndex;
    public int towerCost;

    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI costText;
    public Image buttonBackground;

    [Header("Visual States")]
    public Color affordableColor    = Color.white;
    public Color notAffordableColor = new Color(0.5f, 0.5f, 0.5f, 0.7f);

    private Button _button;

    // 🔥 FIX 1: luôn đảm bảo có Button
    private void Awake()
    {
        _button = GetComponent<Button>();

        if (_button == null)
        {
            Debug.LogError("❌ Button component missing on " + name);
            return;
        }

        _button.onClick.AddListener(OnButtonClick);
    }

    private void Start()
    {
        if (costText != null)
            costText.text = towerCost + "G";
    }

    // 🔥 FIX 2: tự bảo vệ khỏi null
    public void RefreshAffordability(int currentGold)
    {
        if (_button == null)
            _button = GetComponent<Button>();

        if (_button == null) return;

        bool canAfford = currentGold >= towerCost;

        _button.interactable = canAfford;

        if (buttonBackground != null)
            buttonBackground.color = canAfford ? affordableColor : notAffordableColor;

        if (iconImage != null)
        {
            Color c = iconImage.color;
            c.a = canAfford ? 1f : 0.5f;
            iconImage.color = c;
        }

        if (costText != null)
        {
            Color c = costText.color;
            c.a = canAfford ? 1f : 0.6f;
            costText.color = c;
        }
    }

    private void OnButtonClick()
    {
        if (BuildMenuUI.Instance != null)
            BuildMenuUI.Instance.SelectTower(towerIndex);
    }

    // 🔥 hiệu ứng rung khi thiếu tiền (giống KR)
    public void PlayErrorFeedback()
    {
        StartCoroutine(ShakeAnimation());
    }

    private System.Collections.IEnumerator ShakeAnimation()
    {
        Vector3 originalPos = transform.localPosition;
        float duration = 0.25f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float x = Random.Range(-4f, 4f);
            transform.localPosition = originalPos + new Vector3(x, 0, 0);
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
