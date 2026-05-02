using UnityEngine;
using System.Collections.Generic;

// ── WaypointPath: 1 duong di hoan chinh ──────────────────────
[System.Serializable]
public class WaypointPath
{
    public string      pathName  = "Path Main";   // Ten duong (de nhan biet)
    public Transform[] waypoints;                  // Cac diem tren duong nay

    public Transform GetWaypoint(int index)
    {
        if (index < 0 || index >= waypoints.Length) return null;
        return waypoints[index];
    }

    public int Count => waypoints.Length;
}

// ── WaypointManager ───────────────────────────────────────────
/// <summary>
/// Quan ly nhieu duong di song song.
/// Enemy duoc gan ngau nhien vao 1 trong cac duong khi spawn.
///
/// SETUP TRONG UNITY:
/// - Tao GameObject "WaypointManager" → gan script nay
/// - Tao cac Empty GameObject lam waypoint cho tung duong
/// - Trong Inspector: mo rong Paths → them tung WaypointPath
///   → keo cac Transform waypoint vao dung duong
///
/// VI DU CAU TRUC HIERARCHY:
///   WaypointManager
///   ├── Path_Main
///   │   ├── WP_Main_01
///   │   ├── WP_Main_02
///   │   └── WP_Main_03
///   └── Path_Branch
///       ├── WP_Branch_01
///       ├── WP_Branch_02
///       └── WP_Branch_03
/// </summary>
public class WaypointManager : MonoBehaviour
{
    public static WaypointManager Instance { get; private set; }

    [Header("Danh sach cac duong di (them/bot tuy y)")]
    public List<WaypointPath> paths = new List<WaypointPath>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ── Public API ────────────────────────────────────────

    /// <summary>
    /// Tra ve 1 duong ngau nhien trong so cac duong hien co.
    /// WaveManager goi khi spawn enemy.
    /// </summary>
    public WaypointPath GetRandomPath()
    {
        if (paths == null || paths.Count == 0) return null;
        int index = Random.Range(0, paths.Count);
        return paths[index];
    }

    /// <summary>Tra ve duong theo index cu the.</summary>
    public WaypointPath GetPath(int index)
    {
        if (index < 0 || index >= paths.Count) return null;
        return paths[index];
    }

    public int PathCount => paths.Count;

    // ── Gizmos: ve tat ca duong di trong Scene View ───────

    private void OnDrawGizmos()
    {
        if (paths == null) return;

        // Mau sac cho tung duong
        Color[] colors = new Color[]
        {
            Color.yellow,
            Color.cyan,
            Color.green,
            Color.magenta,
            Color.white
        };

        for (int p = 0; p < paths.Count; p++)
        {
            WaypointPath path = paths[p];
            if (path.waypoints == null || path.waypoints.Length < 2) continue;

            Gizmos.color = colors[p % colors.Length];

            for (int i = 0; i < path.waypoints.Length - 1; i++)
            {
                if (path.waypoints[i] == null || path.waypoints[i + 1] == null) continue;
                Gizmos.DrawLine(path.waypoints[i].position, path.waypoints[i + 1].position);
                Gizmos.DrawSphere(path.waypoints[i].position, 0.15f);
            }

            // Diem cuoi mau do
            Transform last = path.waypoints[path.waypoints.Length - 1];
            if (last != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(last.position, 0.25f);
            }
        }
    }
}