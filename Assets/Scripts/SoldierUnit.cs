using UnityEngine;

// SETUP: Them Circle Collider 2D → IS TRIGGER = true, radius = 0.8
public class SoldierUnit : MonoBehaviour
{
    public float maxHP       = 60f;
    public float damage      = 12f;
    public float attackSpeed = 1f;

    private float         _currentHP;
    private float         _nextAttackTime = 0f;
    private EnemyMovement _blockedEnemy;
    private EnemyStats    _blockedStats;
    private EnemyCombat   _blockedCombat;
    private BarracksTower _parentBarracks;

    private void Start() => _currentHP = maxHP;

    public void Init(BarracksTower barracks) => _parentBarracks = barracks;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_blockedEnemy != null) return;
        if (!other.CompareTag("Enemy")) return;

        EnemyMovement em = other.GetComponent<EnemyMovement>();
        if (em == null || em.state != EnemyMovement.EnemyState.Move) return;

        _blockedEnemy  = em;
        _blockedStats  = other.GetComponent<EnemyStats>();
        _blockedCombat = other.GetComponent<EnemyCombat>();
        em.GetBlocked(this);
    }

    private void Update()
    {
        if (_blockedEnemy == null) return;
        if (_blockedStats == null || _blockedStats.IsDead) { ClearBlock(); return; }
        if (Time.time < _nextAttackTime) return;

        _blockedStats.TakeDamage(damage, DamageType.Physical);

        float enemyDmg = _blockedCombat != null ? _blockedCombat.meleeDamage / attackSpeed : 5f;
        TakeDamage(enemyDmg);

        _nextAttackTime = Time.time + 1f / attackSpeed;
    }

    public void TakeDamage(float dmg)
    {
        _currentHP -= dmg;
        if (_currentHP <= 0) Die();
    }

    private void Die()
    {
        if (_blockedEnemy != null && _blockedStats != null && !_blockedStats.IsDead)
            _blockedEnemy.GetUnblocked();
        _parentBarracks?.OnSoldierDied();
        Destroy(gameObject);
    }

    private void ClearBlock()
    {
        _blockedEnemy  = null;
        _blockedStats  = null;
        _blockedCombat = null;
    }
}
