using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int startGold  = 150;
    public int startLives = 20;

    public int  gold       { get; private set; }
    public int  lives      { get; private set; }
    public bool isGameOver { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    private void Start()
    {
        gold  = startGold;
        lives = startLives;
        UIManager.Instance?.UpdateGold(gold);
        UIManager.Instance?.UpdateLives(lives);
    }

    // ── Vang ──────────────────────────────────────────────

    public void AddGold(int amount)
    {
        gold += amount;
        UIManager.Instance?.UpdateGold(gold);
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        UIManager.Instance?.UpdateGold(gold);
        return true;
    }

    // ── Mau ───────────────────────────────────────────────

    public void LoseLife(int amount = 1)
    {
        if (isGameOver) return;
        lives = Mathf.Max(lives - amount, 0);
        UIManager.Instance?.UpdateLives(lives);
        if (lives <= 0) TriggerGameOver();
    }

    // ── Thang / Thua ──────────────────────────────────────

    public void TriggerWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        UIManager.Instance?.ShowWinScreen();
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        UIManager.Instance?.ShowGameOverScreen();
    }

    public void RestartGame()
        => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}