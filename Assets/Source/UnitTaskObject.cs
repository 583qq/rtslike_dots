using Unity;

using UnityEngine;

[CreateAssetMenu(fileName="unitTask", menuName="Project/Unit/Unit Task", order=1)]
public class UnitTaskObject : ScriptableObject
{
    public string unitName;
    public GameObject unitPrefab;
    public bool isAttackable;
    public bool isHero;
    public bool isRanged;

    [Range(0, 60)]
    public uint spawnTime;

    public float health;
    public DamageType damageType;
    public Range baseDamage;

    public Game.UnitPriceData[] prices;
}