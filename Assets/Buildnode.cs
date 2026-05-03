using UnityEngine;

public class BuildNode : MonoBehaviour
{
    public GameObject[] towerPrefabs;

    private GameObject currentTower;

    void OnMouseDown()
    {
        if (currentTower != null)
            return;

        if (BuildMenuUI.Instance != null && BuildMenuUI.Instance.IsShowing())
            return;

        BuildMenuUI.Instance.Show(this);
    }

    public void BuildTower(int index)
    {
        if (index < 0 || index >= towerPrefabs.Length) return;

        currentTower = Instantiate(
            towerPrefabs[index],
            transform.position,
            Quaternion.identity
        );
    }

    public bool HasTower()
    {
        return currentTower != null;
    }
}