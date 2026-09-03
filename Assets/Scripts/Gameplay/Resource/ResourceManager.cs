using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [Header("Resources Text")]
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI diamondText;

    [Header("Gold Regeneration")]
    [Min(0)] [SerializeField] private int goldPerRegen = 1;
    [Min(0.01f)] [SerializeField] private float goldRegenInterval = 1f;

    private Dictionary<ResourceType, int> amounts = new();
    private Coroutine goldRegenCoroutine;

    public event Action<ResourceType, int> ResourceChanged;

    public int GetAmount(ResourceType type) => amounts.GetValueOrDefault(type);

    public static ResourceManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        amounts[ResourceType.Gold] = 0;
        amounts[ResourceType.Diamond] = 0;
    }

    private void OnEnable()
    {
        ResourceChanged += UpdateResourceText;
        RefreshResourceTexts();
        goldRegenCoroutine = StartCoroutine(RegenerateGold());
    }

    private void OnDisable()
    {
        ResourceChanged -= UpdateResourceText;

        if (goldRegenCoroutine != null)
        {
            StopCoroutine(goldRegenCoroutine);
            goldRegenCoroutine = null;
        }
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

    private void RefreshResourceTexts()
    {
        UpdateResourceText(ResourceType.Gold, GetAmount(ResourceType.Gold));
        UpdateResourceText(ResourceType.Diamond, GetAmount(ResourceType.Diamond));
    }

    private IEnumerator RegenerateGold()
    {
        while (true)
        {
            yield return new WaitForSeconds(goldRegenInterval);
            Add(ResourceType.Gold, goldPerRegen);
        }
    }

    private void UpdateResourceText(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Gold:
                if (goldText != null)
                    goldText.text = amount.ToString();
                break;

            case ResourceType.Diamond:
                if (diamondText != null)
                    diamondText.text = amount.ToString();
                break;
        }
    }
}

public enum ResourceType
{
    Gold,
    Diamond
}
