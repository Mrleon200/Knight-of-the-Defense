using UnityEngine;

/// <summary>
/// Di chuyen enemy theo 1 WaypointPath duoc assign khi spawn.
/// Path duoc chon ngau nhien tu WaypointManager.
/// pathProgress (0→1): Tower dung de target enemy gan base nhat.
/// State Machine: Move → Blocked → Dead.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    [Header("Di chuyen")]
    public float moveSpeed             = 3f;
    public float waypointReachDistance = 0.1f;

    // Tower doc gia tri nay de sort target
    [HideInInspector] public float pathProgress = 0f;

    // State Machine
    public enum EnemyState { Move, Blocked, Dead }
    public EnemyState state { get; private set; } = EnemyState.Move;

    [HideInInspector] public SoldierUnit blockedBy = null;

    // Path duoc gan khi spawn
    private WaypointPath _assignedPath;
    private int          _waypointIndex     = 0;
    private Transform    _targetWaypoint;
    private float        _totalPathLength   = 0f;
    private float        _distanceTravelled = 0f;

    private void Start()
    {
        // Lay path ngau nhien tu WaypointManager
        _assignedPath = WaypointManager.Instance?.GetRandomPath();

        if (_assignedPath == null || _assignedPath.Count == 0)
        {
            Debug.LogError($"[EnemyMovement] {gameObject.name}: Khong tim thay path!");
            return;
        }

        // Dich chuyen den diem bat dau cua path
        transform.position = new Vector3(
            _assignedPath.GetWaypoint(0).position.x,
            _assignedPath.GetWaypoint(0).position.y,
            0f
        );

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

    // ── Di chuyen ─────────────────────────────────────────

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
        if (_waypointIndex >= _assignedPath.Count)
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
        => _targetWaypoint = _assignedPath.GetWaypoint(_waypointIndex);

    private void CalculateTotalPathLength()
    {
        _totalPathLength = 0f;
        for (int i = 0; i < _assignedPath.Count - 1; i++)
        {
            Transform a = _assignedPath.GetWaypoint(i);
            Transform b = _assignedPath.GetWaypoint(i + 1);
            if (a != null && b != null)
                _totalPathLength += Vector2.Distance(a.position, b.position);
        }
    }

    // ── Block / Unblock ───────────────────────────────────

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

    // ── Toi Base ──────────────────────────────────────────

    private void ReachBase()
    {
        state = EnemyState.Dead;
        GameManager.Instance?.LoseLife();
        WaveManager.Instance?.NotifyEnemyDied();
        Destroy(gameObject);
    }
}