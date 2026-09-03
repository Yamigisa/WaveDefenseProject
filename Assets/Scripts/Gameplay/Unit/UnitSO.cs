using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "ScriptableObjects/Unit")]
public class UnitSO : ScriptableObject
{
    [Header("Identity")]
    public string unitName;
    public Sprite sprite;
    public GameObject prefab;

    [Header("Defense")]
    public int maxHealth;

    [Header("Attack")]
    public int attack;

    [Tooltip("Maximum distance at which this unit can attack.")]
    public float attackRange;

    [Tooltip("Number of attacks this unit can perform per second.")]
    public float attacksPerSecond;

    [Header("Economy")]
    public int goldProduced;
    public float goldProductionInterval;

    [Header("Cost")]
    public int cost;

}
