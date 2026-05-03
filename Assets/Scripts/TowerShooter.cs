using UnityEngine;

// Script cho tower bắn (Nhà cung / Pháp sư)
public class TowerShooter : MonoBehaviour
{
    [Header("Data")]
    public TowerData data;

    [Header("References")]
    public Transform characterOnTower;
    public Transform firePoint;

    private float _nextFireTime = 0f;
    private Transform _currentTarget;

    private SpriteRenderer _sr;

    private void Awake()
    {
        if (characterOnTower != null)
            _sr = characterOnTower.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        FindBestTarget();

        if (_currentTarget == null) return;

        RotateCharacter();

        if (Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + 1f / data.attackSpeed;
        }
    }

    // 🔍 Tìm enemy tiến xa nhất trong range
    private void FindBestTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, data.range);

        EnemyMovement best = null;
        float highest = -1f;

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            EnemyMovement em = hit.GetComponent<EnemyMovement>();
            if (em == null || em.state == EnemyMovement.EnemyState.Dead) continue;

            if (em.pathProgress > highest)
            {
                highest = em.pathProgress;
                best = em;
            }
        }

        _currentTarget = best != null ? best.transform : null;
    }

    private void RotateCharacter()
    {
        if (_currentTarget == null || _sr == null) return;

        Vector3 dir = _currentTarget.position - characterOnTower.position;

        // flipX = true khi quay trái
        if (Mathf.Abs(dir.x) > 0.1f)
            _sr.flipX = dir.x < 0;
    }

    //  Bắn đạn
    private void Shoot()
    {
        if (data.bulletPrefab == null || firePoint == null) return;

        GameObject obj = Instantiate(data.bulletPrefab, firePoint.position, firePoint.rotation);

        BulletProjectile bp = obj.GetComponent<BulletProjectile>();
        if (bp != null)
        {
            bp.Init(_currentTarget, data.damage, data.bulletSpeed, data.damageType);
        }

        Debug.Log("Tower " + name + " ยิง"); // debug bắn
    }

    //  Vẽ range trong Scene
    private void OnDrawGizmosSelected()
    {
        if (data == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }
}