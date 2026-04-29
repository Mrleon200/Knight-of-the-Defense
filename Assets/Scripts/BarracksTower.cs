using System.Collections;
using UnityEngine;

// Nha linh: spawn linh ra chan duong
// SETUP: Gan script nay vao Tower_NhaLinh Prefab (KHONG gan TowerShooter)
public class BarracksTower : MonoBehaviour
{
    public GameObject soldierPrefab;
    public int        maxSoldiers  = 2;
    public float      respawnDelay = 5f;
    public float      spawnRadius  = 1.2f;

    private int  _soldierCount = 0;
    private bool _isRespawning = false;

    private void Start()
    {
        if (soldierPrefab == null) { Debug.LogError("[Barracks] Chua gan Soldier Prefab!"); return; }
        SpawnAll();
    }

    private void SpawnAll()
    {
        for (int i = _soldierCount; i < maxSoldiers; i++) SpawnOne(i);
    }

    private void SpawnOne(int slot)
    {
        float   angle  = (360f / maxSoldiers) * slot * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spawnRadius;
        GameObject obj = Instantiate(soldierPrefab, transform.position + offset, Quaternion.identity);
        SoldierUnit s  = obj.GetComponent<SoldierUnit>();
        if (s != null) s.Init(this);
        _soldierCount++;
    }

    public void OnSoldierDied()
    {
        _soldierCount = Mathf.Max(0, _soldierCount - 1);
        if (!_isRespawning) StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        _isRespawning = true;
        yield return new WaitForSeconds(respawnDelay);
        while (_soldierCount < maxSoldiers)
        {
            SpawnOne(_soldierCount);
            yield return new WaitForSeconds(0.3f);
        }
        _isRespawning = false;
    }
}
