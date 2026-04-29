using UnityEngine;

// SETUP: Tao GameObject "TowerPlacer" → gan script nay
// UI Button: OnClick → TowerPlacer.Instance.SelectTower(0/1/2)
public class TowerPlacer : MonoBehaviour
{
    public static TowerPlacer Instance { get; private set; }

    [Header("Prefabs: [0]NhaCung [1]PhapSu [2]NhaLinh")]
    public GameObject[] towerPrefabs;

    [Header("Gia tuong ung voi tung prefab")]
    public int[] towerCosts;

    private int        _selectedIndex = -1;
    private GameObject _previewObj;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (_selectedIndex < 0) return;
        MovePreview();
        if (Input.GetMouseButtonDown(0))         PlaceTower();
        if (Input.GetMouseButtonDown(1))         Cancel();
        if (Input.GetKeyDown(KeyCode.Escape))    Cancel();
    }

    public void SelectTower(int index)
    {
        if (index < 0 || index >= towerPrefabs.Length) return;
        int cost = GetCost(index);
        if (GameManager.Instance != null && GameManager.Instance.gold < cost)
        {
            Debug.Log($"[TowerPlacer] Khong du vang! Can {cost}");
            return;
        }
        _selectedIndex = index;
        if (_previewObj != null) Destroy(_previewObj);
        _previewObj = Instantiate(towerPrefabs[index]);
        SetAlpha(_previewObj, 0.5f);
        DisableLogic(_previewObj);
    }

    private void MovePreview()
    {
        if (_previewObj == null) return;
        _previewObj.transform.position = MouseWorldPos();
    }

    private void PlaceTower()
    {
        int cost = GetCost(_selectedIndex);
        if (GameManager.Instance != null && !GameManager.Instance.SpendGold(cost))
        {
            Debug.Log("[TowerPlacer] Khong du vang de dat!");
            Cancel(); return;
        }
        Instantiate(towerPrefabs[_selectedIndex], MouseWorldPos(), Quaternion.identity);
        Cancel();
    }

    private void Cancel()
    {
        _selectedIndex = -1;
        if (_previewObj != null) { Destroy(_previewObj); _previewObj = null; }
    }

    private int GetCost(int index)
        => (towerCosts != null && index < towerCosts.Length) ? towerCosts[index] : 0;

    private Vector3 MouseWorldPos()
    {
        Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        p.z = 0f; return p;
    }

    private void SetAlpha(GameObject obj, float alpha)
    {
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null) { Color c = sr.color; c.a = alpha; sr.color = c; }
    }

    private void DisableLogic(GameObject obj)
    {
        TowerShooter ts = obj.GetComponent<TowerShooter>();
        if (ts != null) ts.enabled = false;

        BarracksTower bt = obj.GetComponent<BarracksTower>();
        if (bt != null) bt.enabled = false;
    }
}
