using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed             = 3f;
    public float waypointReachDistance = 0.1f;

    [HideInInspector] public float pathProgress = 0f;

    public enum EnemyState { Move, Blocked, Dead }
    public EnemyState state { get; private set; } = EnemyState.Move;

    [HideInInspector] public SoldierUnit blockedBy = null;

    private int       _waypointIndex     = 0;
    private Transform _targetWaypoint;
    private float     _totalPathLength   = 0f;
    private float     _distanceTravelled = 0f;

    private void Start()
    {
        CalculateTotalPathLength();
        SetNextWaypoint();
    }

    private void Update()
    {
        if (state != EnemyState.Move) return;
        MoveTowards();
        CheckReached();
        UpdateProgress();
    }

    private void MoveTowards()
    {
        if (_targetWaypoint == null) return;
        Vector3 dir  = (_targetWaypoint.position - transform.position).normalized;
        float   step = moveSpeed * Time.deltaTime;
        transform.position += dir * step;
        _distanceTravelled += step;
    }

    private void CheckReached()
    {
        if (_targetWaypoint == null) return;
        if (Vector3.Distance(transform.position, _targetWaypoint.position)
            > waypointReachDistance) return;

        _waypointIndex++;
        if (_waypointIndex >= WaypointManager.Instance.GetWaypointCount())
            ReachBase();
        else
            SetNextWaypoint();
    }

    private void UpdateProgress()
    {
        if (_totalPathLength > 0)
            pathProgress = Mathf.Clamp01(_distanceTravelled / _totalPathLength);
    }

    private void SetNextWaypoint()
        => _targetWaypoint = WaypointManager.Instance.GetWaypoint(_waypointIndex);

    private void CalculateTotalPathLength()
    {
        _totalPathLength = 0f;
        int count = WaypointManager.Instance.GetWaypointCount();
        for (int i = 0; i < count - 1; i++)
        {
            Transform a = WaypointManager.Instance.GetWaypoint(i);
            Transform b = WaypointManager.Instance.GetWaypoint(i + 1);
            if (a != null && b != null)
                _totalPathLength += Vector2.Distance(a.position, b.position);
        }
    }

    public void GetBlocked(SoldierUnit soldier)
    {
        state     = EnemyState.Blocked;
        blockedBy = soldier;
    }

    public void GetUnblocked()
    {
        state     = EnemyState.Move;
        blockedBy = null;
    }

    private void ReachBase()
    {
        state = EnemyState.Dead;
        GameManager.Instance?.LoseLife();
        WaveManager.Instance?.NotifyEnemyDied();
        Destroy(gameObject);
    }
}
