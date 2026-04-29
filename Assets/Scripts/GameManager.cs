using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int startGold  = 150;
    public int startLives = 20;

    public int  gold      { get; private set; }
    public int  lives     { get; private set; }
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
    }

    public void AddGold(int amount)
    {
        gold += amount;
        Debug.Log($"[Gold] +{amount} | Tong: {gold}");
    }

    public bool SpendGold(int amount)
    {
        if (gold < amount) return false;
        gold -= amount;
        Debug.Log($"[Gold] -{amount} | Con: {gold}");
        return true;
    }

    public void LoseLife(int amount = 1)
    {
        if (isGameOver) return;
        lives = Mathf.Max(lives - amount, 0);
        Debug.Log($"[Lives] -{amount} | Con: {lives}");
        if (lives <= 0) TriggerGameOver();
    }

    public void TriggerWin()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("[GameManager] CHIEN THANG!");
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("[GameManager] GAME OVER!");
    }

    public void RestartGame()
        => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
}
