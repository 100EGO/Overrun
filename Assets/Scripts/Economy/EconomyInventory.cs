using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>Persistent credits and stored resources; mission loot remains temporary.</summary>
public class EconomyInventory : MonoBehaviour
{
    [Serializable]
    private class SaveData
    {
        public int version = 1;
        public int credits;
        public int scrap;
        public int heavyAlloy;
        public int electronicComponent;
    }

    public static EconomyInventory Instance { get; private set; }

    [SerializeField, Min(0)] private int credits;
    private readonly Dictionary<ResourceType, int> stored = new Dictionary<ResourceType, int>();
    private readonly Dictionary<ResourceType, int> mission = new Dictionary<ResourceType, int>();

    public int Credits => credits;
    public event Action Changed;
    private string SavePath => Path.Combine(Application.persistentDataPath, "economy.json");

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Load();
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
        Save();
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
        Save();
        Changed?.Invoke();
    }

    public bool SpendCredits(int amount)
    {
        if (amount < 0 || credits < amount) return false;
        credits -= amount;
        Save();
        Changed?.Invoke();
        return true;
    }

    public bool SpendResource(ResourceType type, int amount)
    {
        if (amount < 0 || GetStored(type) < amount) return false;
        stored[type] = GetStored(type) - amount;
        Save();
        Changed?.Invoke();
        return true;
    }

    public void BeginMission()
    {
        LoseMission();
    }

    public void Save()
    {
        try
        {
            SaveData data = new SaveData
            {
                credits = credits,
                scrap = GetStored(ResourceType.Scrap),
                heavyAlloy = GetStored(ResourceType.HeavyAlloy),
                electronicComponent = GetStored(ResourceType.ElectronicComponent)
            };
            string path = SavePath;
            string tempPath = path + ".tmp";
            File.WriteAllText(tempPath, JsonUtility.ToJson(data, true));
            // Replace the existing save only after the new JSON was written successfully.
            File.Move(tempPath, path, true);
        }
        catch (Exception exception)
        {
            Debug.LogError($"EconomyInventory: failed to save economy: {exception.Message}", this);
        }
    }

    public void Load()
    {
        try
        {
            if (!File.Exists(SavePath)) return;
            SaveData data = JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
            if (data == null || data.version != 1)
            {
                Debug.LogWarning("EconomyInventory: unsupported or empty save; keeping current values.", this);
                return;
            }
            credits = Mathf.Max(0, data.credits);
            stored[ResourceType.Scrap] = Mathf.Max(0, data.scrap);
            stored[ResourceType.HeavyAlloy] = Mathf.Max(0, data.heavyAlloy);
            stored[ResourceType.ElectronicComponent] = Mathf.Max(0, data.electronicComponent);
            mission.Clear();
            Changed?.Invoke();
        }
        catch (Exception exception)
        {
            Debug.LogError($"EconomyInventory: failed to load economy: {exception.Message}", this);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
