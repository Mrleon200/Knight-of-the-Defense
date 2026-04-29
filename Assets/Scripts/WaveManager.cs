using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public string     waveName      = "Wave 1";
    public GameObject enemyPrefab;
    public int        enemyCount    = 5;
    public float      spawnInterval = 1.5f;
    public float      hpMultiplier  = 1f;
}

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    public Transform      spawnPoint;
    public List<WaveData> waves            = new List<WaveData>();
    public float          timeBetweenWaves = 8f;

    public int currentWave  { get; private set; } = 0;
    public int enemiesAlive { get; private set; } = 0;
    public int totalWaves   => waves.Count;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (waves.Count == 0) { Debug.LogError("[WaveManager] Chua co wave!"); return; }
        StartCoroutine(RunAllWaves());
    }

    private IEnumerator RunAllWaves()
    {
        yield return new WaitForSeconds(2f);

        while (currentWave < waves.Count)
        {
            WaveData wave = waves[currentWave];
            Debug.Log($"[Wave] {wave.waveName} bat dau");

            yield return StartCoroutine(SpawnWave(wave));
            yield return new WaitUntil(() => enemiesAlive <= 0);

            currentWave++;

            if (currentWave >= waves.Count)
            {
                GameManager.Instance?.TriggerWin();
                yield break;
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private IEnumerator SpawnWave(WaveData wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave);
            yield return new WaitForSeconds(wave.spawnInterval);
        }
    }

    private void SpawnEnemy(WaveData wave)
    {
        if (wave.enemyPrefab == null || spawnPoint == null) return;
        GameObject obj = Instantiate(wave.enemyPrefab, spawnPoint.position, Quaternion.identity);
        EnemyStats stats = obj.GetComponent<EnemyStats>();
        if (stats != null) stats.ScaleHP(wave.hpMultiplier);
        enemiesAlive++;
    }

    public void NotifyEnemyDied()
        => enemiesAlive = Mathf.Max(0, enemiesAlive - 1);
}
