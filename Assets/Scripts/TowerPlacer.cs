using UnityEngine;

/// <summary>
/// TowerPlacer - CHỈ SPAWN TOWER, KHÔNG CÓ PREVIEW
/// Gọi từ BuildMenuUI khi người chơi chọn tower
/// </summary>
public class TowerPlacer : MonoBehaviour
{
    public static TowerPlacer Instance { get; private set; }

    [Header("Tower Prefabs")]
    public GameObject[] towerPrefabs; // 0=Archer, 1=Barracks, 2=Mage

    [Header("Tower Costs")]
    public int[] towerCosts = { 75, 50, 100 }; // Phải khớp với towerPrefabs

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Đặt tower tại vị trí cụ thể (gọi từ BuildMenuUI)
    /// </summary>
    /// <param name="index">Loại tower (0=Archer, 1=Barracks, 2=Mage)</param>
    /// <param name="position">Vị trí BuildNode</param>
    /// <returns>True nếu build thành công</returns>
    public bool PlaceTower(int index, Vector3 position)
    {
        // Validate
        if (index < 0 || index >= towerPrefabs.Length)
        {
            Debug.LogError($"[TowerPlacer] Invalid index: {index}");
            return false;
        }

        if (towerPrefabs[index] == null)
        {
            Debug.LogError($"[TowerPlacer] Tower prefab at index {index} is null!");
            return false;
        }

        // Kiểm tra vàng
        int cost = towerCosts[index];
        if (!GameManager.Instance.SpendGold(cost))
        {
            Debug.Log($"[TowerPlacer] Không đủ vàng! Cần {cost}G");
            return false;
        }

        // Spawn tower
        Instantiate(towerPrefabs[index], position, Quaternion.identity);
        Debug.Log($"[TowerPlacer] Đã đặt tower {index} tại {position}");
        
        return true;
    }
}