using UnityEngine;

// Nha cung va Phap su dung script nay
// Ten class la TowerShooter (tranh trung voi ten folder Tower/)
public class TowerShooter : MonoBehaviour
{
    public TowerData data;
    public Transform characterOnTower; // Chi cai nay xoay
    public Transform firePoint;

    private float     _nextFireTime  = 0f;
    private Transform _currentTarget;

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

    private void FindBestTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, data.range);

        EnemyMovement best     = null;
        float         highest  = -1f;

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            EnemyMovement em = hit.GetComponent<EnemyMovement>();
            if (em == null || em.state == EnemyMovement.EnemyState.Dead) continue;
            if (em.pathProgress > highest)
            {
                highest = em.pathProgress;
                best    = em;
            }
        }

        _currentTarget = best != null ? best.transform : null;
    }

    private void RotateCharacter()
    {
        if (characterOnTower == null || _currentTarget == null) return;
        Vector2 dir   = _currentTarget.position - characterOnTower.position;
        float   angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        characterOnTower.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Shoot()
    {
        if (data.bulletPrefab == null || firePoint == null) return;
        GameObject obj = Instantiate(data.bulletPrefab, firePoint.position, firePoint.rotation);
        BulletProjectile bp = obj.GetComponent<BulletProjectile>();
        if (bp != null) bp.Init(_currentTarget, data.damage, data.bulletSpeed, data.damageType);
        Debug.Log("Tower: " + name + " FirePoint: " + firePoint.position);
    }

    private void OnDrawGizmosSelected()
    {
        if (data == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, data.range);
    }
}
