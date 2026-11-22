using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>Runtime inventory: mission loot is banked only after victory.</summary>
public class EconomyInventory : MonoBehaviour
{
    public static EconomyInventory Instance { get; private set; }

    [SerializeField, Min(0)] private int credits;
    private readonly Dictionary<ResourceType, int> stored = new Dictionary<ResourceType, int>();
    private readonly Dictionary<ResourceType, int> mission = new Dictionary<ResourceType, int>();

    public int Credits => credits;
    public event Action Changed;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetStored(ResourceType type) => stored.TryGetValue(type, out int value) ? value : 0;
    public int GetMission(ResourceType type) => mission.TryGetValue(type, out int value) ? value : 0;

    public void Collect(ResourceType type, int amount)
    {
        if (amount <= 0) return;
        mission[type] = GetMission(type) + amount;
        Changed?.Invoke();
    }

    public void BankMission()
    {
        foreach (var entry in mission)
            stored[entry.Key] = GetStored(entry.Key) + entry.Value;
        mission.Clear();
        Changed?.Invoke();
    }

    public void LoseMission()
    {
        mission.Clear();
        Changed?.Invoke();
    }

    public void AddCredits(int amount)
    {
        if (amount <= 0) return;
        credits += amount;
        Changed?.Invoke();
    }

    public bool SpendCredits(int amount)
    {
        if (amount < 0 || credits < amount) return false;
        credits -= amount;
        Changed?.Invoke();
        return true;
    }

    public bool SpendResource(ResourceType type, int amount)
    {
        if (amount < 0 || GetStored(type) < amount) return false;
        stored[type] = GetStored(type) - amount;
        Changed?.Invoke();
        return true;
    }

    public void BeginMission()
    {
        // Any unbanked loot from a previous attempt is forfeited.
        LoseMission();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
