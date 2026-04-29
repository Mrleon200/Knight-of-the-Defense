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
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        if (_target == null) { Destroy(gameObject); return; }

        Vector3 dir = (_target.position - transform.position).normalized;
        transform.position += dir * _speed * Time.deltaTime;
        transform.rotation  = Quaternion.Euler(0f, 0f,
            Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        if (Vector2.Distance(transform.position, _target.position) < 0.2f)
            HitTarget();
    }

    private void HitTarget()
    {
        EnemyStats stats = _target.GetComponent<EnemyStats>();
        if (stats != null) stats.TakeDamage(_damage, _type);
        Destroy(gameObject);
    }
}
