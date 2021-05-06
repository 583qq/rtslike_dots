using Unity;
using Unity.Entities;
using UnityEngine;


[CreateAssetMenu(fileName="buildingTask", menuName="Project/Unit/Building Task", order = 1)]
public class ConstructionTaskObject : ScriptableObject
{
    public GameObject buildingPrefab;
    public GameObject previewPrefab;

    public bool isAttackable;

    [Range(0, 300)]
    public uint constructionTime;

    public uint durability;

    public Game.UnitPriceData[] prices;
}