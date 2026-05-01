using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Chi so co ban")]
    public float maxHP      = 100f;
    public float armor      = 0f;   // giam Physical damage
    public int   goldReward = 10;

    private float _currentHP;
    private bool  _isDead = false;

    // [FIX] Them property public de SoldierUnit co the kiem tra
    public bool IsDead => _isDead;

    private void Awake()
    {
        _currentHP = maxHP;
    }

    // Goi tu WaveManager khi spawn de scale do kho
    public void ScaleHP(float multiplier)
    {
        maxHP      *= multiplier;
        _currentHP  = maxHP;
    }

    public void TakeDamage(float amount, DamageType type)
    {
        if (_isDead) return;

        float finalDamage = amount;

        if (type == DamageType.Physical)
            finalDamage = Mathf.Max(amount - armor, 1f); // toi thieu 1 damage
        // Magic: bo qua armor hoan toan

        _currentHP -= finalDamage;

        if (_currentHP <= 0f) Die();
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        // Thuong vang
        GameManager.Instance?.AddGold(goldReward);

        // [QUAN TRONG] Bao WaveManager enemy nay da chet
        WaveManager.Instance?.NotifyEnemyDied();

        Destroy(gameObject);
    }
}