using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerData", menuName = "KnightDefense/TowerData")]
public class TowerData : ScriptableObject
{
    public string     towerName;
    public Sprite     towerSprite;
    public int        cost;
    public float      damage;
    public float      range;
    public float      attackSpeed;
    public DamageType damageType  = DamageType.Physical;
    public GameObject bulletPrefab;
    public float      bulletSpeed = 8f;
}
