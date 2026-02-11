using System;
using UnityEngine;

namespace Core.Services 
{
    public interface IProgressionService
    {
        /// <summary>
        /// Get player's progression data
        /// </summary>
        PlayerProgressionData GetProgressionData();
        
        /// <summary>
        /// Add Chrono Shards
        /// </summary>
        void AddChronoShards(int amount);
        
        /// <summary>
        /// Spend Chrono Shards
        /// </summary>
        bool SpendChronoShards(int amount);
        
        /// <summary>
        /// Complete a route
        /// </summary>
        void CompleteRoute(int routeID);
        
        /// <summary>
        /// Unlock weapon
        /// </summary>
        void UnlockWeapon(string weaponID);
        
        /// <summary>
        /// Is weapon unlocked
        /// </summary>
        bool IsWeaponUnlocked(string weaponID);
        
        /// <summary>
        /// Save progression
        /// </summary>
        void SaveProgression();
    }
    
    [Serializable]
    public class PlayerProgressionData
    {
        public int[] completedRoutes = new int[0];
        public int currentRoute = 0;
        public int chronoShardsTotal = 0;
        public int chronoShardsSpent = 0;
        public string[] unlockedWeapons = new string[0];
        public string[] unlockedBuildings = new string[0];
        public SerializableDictionary<string, bool> metaUpgrades = new SerializableDictionary<string, bool>();
        public SerializableDictionary<string, bool> achievements = new SerializableDictionary<string, bool>();
    }
    
    // Simple serializable dictionary for Unity
    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        [SerializeField] private TKey[] keys = new TKey[0];
        [SerializeField] private TValue[] values = new TValue[0];
        
        public bool TryGetValue(TKey key, out TValue value)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i].Equals(key))
                {
                    value = values[i];
                    return true;
                }
            }
            value = default;
            return false;
        }
        
        public void Add(TKey key, TValue value)
        {
            var keysList = new System.Collections.Generic.List<TKey>(keys);
            var valuesList = new System.Collections.Generic.List<TValue>(values);
            
            keysList.Add(key);
            valuesList.Add(value);
            
            keys = keysList.ToArray();
            values = valuesList.ToArray();
        }
    }
}