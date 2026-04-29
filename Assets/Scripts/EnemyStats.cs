using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float maxHP      = 100f;
    public float armor      = 0f;
    public int   goldReward = 10;

    public float CurrentHP { get; private set; }
    public bool  IsDead    => CurrentHP <= 0;

    private void Start() => CurrentHP = maxHP;

    public void TakeDamage(float damage, DamageType type = DamageType.Physical)
    {
        if (IsDead) return;
        float final = (type == DamageType.Physical)
            ? Mathf.Max(damage - armor, 1f)
            : damage;
        CurrentHP = Mathf.Max(CurrentHP - final, 0);
        if (IsDead) Die();
    }

    public void ScaleHP(float multiplier) => maxHP *= multiplier;

    private void Die()
    {
        GameManager.Instance?.AddGold(goldReward);
        WaveManager.Instance?.NotifyEnemyDied();
        Destroy(gameObject);
    }
}
