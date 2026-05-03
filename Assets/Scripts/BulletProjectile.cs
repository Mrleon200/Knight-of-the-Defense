using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Transform  _target;
    private float      _damage;
    private float      _speed;
    private DamageType _type;

    public void Init(Transform target, float damage, float speed, DamageType type)
    {
        _target = target;
        _damage = damage;
        _speed  = speed;
        _type   = type;

        Destroy(gameObject, 5f); // tránh leak
    }

    private void Update()
    {
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 dir = (_target.position - transform.position).normalized;

        // 🟢 Move
        transform.position += dir * _speed * Time.deltaTime;

        // 🟢 Rotate theo hướng bay (optional)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // 🟢 Check hit (dùng sqrMagnitude cho chính xác hơn)
        float dist = (_target.position - transform.position).sqrMagnitude;

        if (dist < 0.04f) // ~0.2^2
        {
            HitTarget();
        }
    }

    private void HitTarget()
    {
        if (_target == null) return;

        EnemyStats stats = _target.GetComponent<EnemyStats>();
        if (stats != null)
        {
            stats.TakeDamage(_damage, _type);
        }

        Destroy(gameObject);
    }
}