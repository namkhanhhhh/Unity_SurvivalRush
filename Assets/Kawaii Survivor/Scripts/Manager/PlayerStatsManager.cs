using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    [Header(" Data ")]
    [SerializeField]private CharacterDataSO playerData;

    [Header(" Settings ")]
    private Dictionary<Stat, float> playerStats = new Dictionary<Stat, float>();
    private Dictionary<Stat, float> addends = new Dictionary<Stat, float>();
    private Dictionary<Stat, float> objectAddends = new Dictionary<Stat, float>();

    private void Awake()
    {
        playerStats = playerData.BaseStats;
        foreach (KeyValuePair<Stat, float> kvp in playerStats)
        {
            addends.Add(kvp.Key, 0);
            objectAddends.Add(kvp.Key, 0);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() =>  UpdatePlayerStats();

    public void AddPlayerStat(Stat stat, float value)
    {
        if (addends.ContainsKey(stat))
            addends[stat] += value;
        
        else  
            Debug.LogError($"The key {stat} has not been found, this is not normal !!! Review your code !!!!");
       
        UpdatePlayerStats();
    }
    public void AddObject(Dictionary<Stat, float> objectStats)
    {
        foreach (KeyValuePair<Stat, float> kvp in objectStats)
            objectAddends[kvp.Key] += kvp.Value;

        UpdatePlayerStats();
    }
    public float GetStatValue(Stat stat) => playerStats[stat] + addends[stat] + objectAddends[stat]; 

    private void UpdatePlayerStats()
    {
        IEnumerable<IPlayerStatsDependency> playerStatsDependencies =
                 FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .OfType<IPlayerStatsDependency>();

        foreach (IPlayerStatsDependency dependency in playerStatsDependencies)
            dependency.UpdateStats(this);
    }
}
