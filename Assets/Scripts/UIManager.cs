using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Quan ly toan bo UI: HUD, Win/Lose screen.
/// SETUP: Tao GameObject "UIManager" → gan script nay
///        → keo cac Text va Panel tu Canvas vao cac slot.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD - Thong tin tren man hinh")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI waveText;

    [Header("Skill Cooldown")]
    public Image meteorCooldownFill;
    public Image reinforceCooldownFill;

    [Header("Screens")]
    public GameObject winScreen;
    public GameObject gameOverScreen;

    [Header("Wave Notification")]
    public GameObject waveNotification;
    public TextMeshProUGUI waveNotificationText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // Cap nhat thanh cooldown skill theo thoi gian thuc
        if (GlobalSkillSystem.Instance != null)
        {
            if (meteorCooldownFill != null)
                meteorCooldownFill.fillAmount = GlobalSkillSystem.Instance.MeteorCooldownPercent();
            if (reinforceCooldownFill != null)
                reinforceCooldownFill.fillAmount = GlobalSkillSystem.Instance.ReinforceCooldownPercent();
        }

        // Cap nhat wave text
        if (WaveManager.Instance != null && waveText != null)
            waveText.text = $"Wave: {WaveManager.Instance.currentWave + 1}/{WaveManager.Instance.totalWaves}";
    }

    // ── Cap nhat HUD ──────────────────────────────────────

    public void UpdateGold(int value)
    {
        if (goldText != null) goldText.text = $"Gold: {value}";
    }

    public void UpdateLives(int value)
    {
        if (livesText != null) livesText.text = $"Lives: {value}";
    }

    // ── Win / Lose screens ────────────────────────────────

    public void ShowWinScreen()
    {
        if (winScreen != null) winScreen.SetActive(true);
    }

    public void ShowGameOverScreen()
    {
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
    }

    // ── Wave notification ─────────────────────────────────

    public void ShowWaveNotification(int waveNumber)
    {
        if (waveNotification == null) return;
        waveNotificationText.text = $"Wave {waveNumber} !";
        waveNotification.SetActive(true);
        Invoke(nameof(HideWaveNotification), 2f);
    }

    private void HideWaveNotification()
    {
        if (waveNotification != null) waveNotification.SetActive(false);
    }
}
