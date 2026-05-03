using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD - Thong tin tren man hinh")]
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI waveText;

    // [Header("Skill Cooldown")]
    // public Image meteorCooldownFill;
    // public Image reinforceCooldownFill;

    [Header("Screens")]
    public GameObject winScreen;
    public GameObject gameOverScreen;

    [Header("Wave Notification")]
    public GameObject       waveNotification;
    public TextMeshProUGUI  waveNotificationText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // An cac screen luc dau
        if (winScreen     != null) winScreen.SetActive(false);
        if (gameOverScreen!= null) gameOverScreen.SetActive(false);
        if (waveNotification != null) waveNotification.SetActive(false);
    }

    // private void Update()
    // {
    //     // Cap nhat thanh cooldown skill theo thoi gian thuc
    //     if (GlobalSkillSystem.Instance != null)
    //     {
    //         if (meteorCooldownFill != null)
    //             meteorCooldownFill.fillAmount = GlobalSkillSystem.Instance.MeteorCooldownPercent();
    //         if (reinforceCooldownFill != null)
    //             reinforceCooldownFill.fillAmount = GlobalSkillSystem.Instance.ReinforceCooldownPercent();
    //     }
    // }

    // ── Cap nhat HUD ──────────────────────────────────────
    public void UpdateGold(int value)
    {
        if (goldText != null) goldText.text = $"Gold: {value}";
    }

    public void UpdateLives(int value)
    {
        if (livesText != null) livesText.text = $"Lives: {value}";
    }

    // [FIX] Them ham UpdateWave de WaveManager co the goi
    public void UpdateWave(int current, int total)
    {
        if (waveText != null) waveText.text = $"Wave: {current}/{total}";
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

    
    public void ShowWaveNotification(int waveNumber)
    {
    if (waveNotification == null) return;

    if (waveNotificationText != null)
        waveNotificationText.text = $"WAVE {waveNumber}!";

    waveNotification.SetActive(true);

        CancelInvoke(nameof(HideWaveNotification));
        Invoke(nameof(HideWaveNotification), 2f);
    }
    private void HideWaveNotification()
    {
        if (waveNotification != null) waveNotification.SetActive(false);
    }
}