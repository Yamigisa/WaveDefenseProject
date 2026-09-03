using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private Dictionary<ResourceType, int> amounts = new();

    public event Action<ResourceType, int> ResourceChanged;

    public int GetAmount(ResourceType type) => amounts.GetValueOrDefault(type);

    public static ResourceManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void Add(ResourceType type, int amount)
    {
        amounts[type] = GetAmount(type) + amount;
        ResourceChanged?.Invoke(type, amounts[type]);
    }

    public bool TrySpend(ResourceType type, int amount)
    {
        if (GetAmount(type) < amount)
            return false;

        amounts[type] -= amount;
        ResourceChanged?.Invoke(type, amounts[type]);

        return true;
    }
}

public enum ResourceType
{
    Gold,
    Diamond
}