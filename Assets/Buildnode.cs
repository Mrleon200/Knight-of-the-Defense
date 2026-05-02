using UnityEngine;

public class BuildNode : MonoBehaviour
{
    public GameObject[] towerPrefabs;

    private GameObject currentTower;

    private void OnMouseDown()
    {
        if (currentTower != null) return;

        if (BuildMenuUI.Instance != null)
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
}