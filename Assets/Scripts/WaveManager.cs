using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ── EnemySpawnInfo: 1 loai enemy trong wave ───────────────────
[System.Serializable]
public class EnemySpawnInfo
{
    public GameObject enemyPrefab;   // Loai enemy (Normal, Fast, Tank...)
    public int        count    = 3;  // So luong loai nay trong wave
    public float      hpMultiplier = 1f; // Nhan HP rieng cho loai nay
}

// ── WaveData: 1 wave chua nhieu loai enemy ────────────────────
[System.Serializable]
public class WaveData
{
    public string waveName = "Wave 1";

    [Tooltip("Danh sach cac loai enemy se xuat hien trong wave nay")]
    public List<EnemySpawnInfo> enemyTypes = new List<EnemySpawnInfo>();

    [Tooltip("Thoi gian cho giua moi lan spawn (giay)")]
    public float spawnInterval = 1.5f;
}

// ── WaveManager ───────────────────────────────────────────────
/// <summary>
/// Quan ly spawn wave voi nhieu loai enemy.
/// Cach hoat dong: tao danh sach tat ca enemy can spawn
/// → tron ngau nhien → spawn lan luot.
/// SETUP: Tao GameObject "WaveManager" → gan script nay
///        → keo SpawnPoint vao + dien danh sach waves.
/// </summary>
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Spawn Point")]
    public Transform spawnPoint;

    [Header("Danh sach Wave")]
    public List<WaveData> waves = new List<WaveData>();

    [Header("Thoi gian nghi giua cac wave (giay)")]
    public float timeBetweenWaves = 8f;

    public int  currentWave  { get; private set; } = 0;
    public int  enemiesAlive { get; private set; } = 0;
    public int  totalWaves   => waves.Count;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (waves.Count == 0)
        {
            Debug.LogError("[WaveManager] Chua co wave nao!");
            return;
        }
        StartCoroutine(RunAllWaves());
    }

    // ── Wave Loop ─────────────────────────────────────────

    private IEnumerator RunAllWaves()
    {
        yield return new WaitForSeconds(2f);

        for (int i = 0; i < waves.Count; i++)
        {
            WaveData wave = waves[i];

            // Cap nhat wave counter TRUOC khi spawn
            currentWave = i + 1;
            UIManager.Instance?.UpdateWave(currentWave, totalWaves);
            UIManager.Instance?.ShowWaveNotification(currentWave);

            Debug.Log($"[Wave {currentWave}] Bat dau voi {CountTotalEnemies(wave)} enemy");

            // Spawn toan bo enemy trong wave
            yield return StartCoroutine(SpawnWave(wave));

            // Cho het toan bo enemy
            yield return new WaitUntil(() => enemiesAlive <= 0);

            Debug.Log($"[Wave {currentWave}] Ket thuc!");

            // Nghi truoc wave tiep theo
            if (i < waves.Count - 1)
            {
                Debug.Log($"[WaveManager] Nghi {timeBetweenWaves}s...");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

        Debug.Log("[WaveManager] Tat ca wave ket thuc! CHIEN THANG!");
        GameManager.Instance?.TriggerWin();
    }

    // ── Spawn Wave ────────────────────────────────────────

    /// <summary>
    /// Tao danh sach tat ca enemy can spawn (theo so luong tung loai)
    /// → tron ngau nhien → spawn lan luot theo spawnInterval.
    /// </summary>
    private IEnumerator SpawnWave(WaveData wave)
    {
        // Tao danh sach tat ca enemy
        List<EnemySpawnInfo> spawnList = BuildSpawnList(wave);

        // Tron ngau nhien de enemy xuat hien khong theo thu tu co dinh
        ShuffleList(spawnList);

        // Spawn tung enemy
        foreach (EnemySpawnInfo info in spawnList)
        {
            SpawnEnemy(info);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    /// <summary>
    /// Tao danh sach phang: moi EnemySpawnInfo co count=3
    /// thi them 3 phan tu vao danh sach.
    /// </summary>
    private List<EnemySpawnInfo> BuildSpawnList(WaveData wave)
    {
        List<EnemySpawnInfo> list = new List<EnemySpawnInfo>();
        foreach (EnemySpawnInfo info in wave.enemyTypes)
        {
            for (int i = 0; i < info.count; i++)
                list.Add(info);
        }
        return list;
    }

    /// <summary>Tron danh sach ngau nhien (Fisher-Yates shuffle).</summary>
    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private void SpawnEnemy(EnemySpawnInfo info)
    {
        if (info.enemyPrefab == null || spawnPoint == null) return;

        GameObject obj = Instantiate(info.enemyPrefab,
            spawnPoint.position, Quaternion.identity);

        EnemyStats stats = obj.GetComponent<EnemyStats>();
        if (stats != null) stats.ScaleHP(info.hpMultiplier);

        enemiesAlive++;
    }

    // ── Helpers ───────────────────────────────────────────

    private int CountTotalEnemies(WaveData wave)
    {
        int total = 0;
        foreach (EnemySpawnInfo info in wave.enemyTypes)
            total += info.count;
        return total;
    }

    public void NotifyEnemyDied()
        => enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
}