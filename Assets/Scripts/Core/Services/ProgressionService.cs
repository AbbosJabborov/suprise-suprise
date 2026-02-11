using UnityEngine;
using System.Linq;

namespace Core.Services
{
    public class ProgressionService : IProgressionService
    {
        private readonly ISaveService saveService;
        private PlayerProgressionData progressionData;
        
        private const string SAVE_KEY = "PlayerProgression";
        
        public ProgressionService(ISaveService saveService)
        {
            this.saveService = saveService;
            LoadProgression();
        }
        
        private void LoadProgression()
        {
            progressionData = saveService.Load<PlayerProgressionData>(SAVE_KEY);
            
            if (progressionData == null)
            {
                Debug.Log("[ProgressionService] No save found, creating new progression");
                progressionData = new PlayerProgressionData
                {
                    completedRoutes = new int[0],
                    currentRoute = 1,
                    chronoShardsTotal = 0,
                    chronoShardsSpent = 0,
                    unlockedWeapons = new string[] { "pistol" }, // Start with pistol
                    unlockedBuildings = new string[] { "ballista" }, // Start with one building
                    metaUpgrades = new SerializableDictionary<string, bool>(),
                    achievements = new SerializableDictionary<string, bool>()
                };
                SaveProgression();
            }
            else
            {
                Debug.Log($"[ProgressionService] Loaded progression - Route: {progressionData.currentRoute}, Shards: {progressionData.chronoShardsTotal}");
            }
        }
        
        public PlayerProgressionData GetProgressionData()
        {
            return progressionData;
        }
        
        public void AddChronoShards(int amount)
        {
            progressionData.chronoShardsTotal += amount;
            Debug.Log($"[ProgressionService] Added {amount} Chrono Shards. Total: {progressionData.chronoShardsTotal}");
            SaveProgression();
        }
        
        public bool SpendChronoShards(int amount)
        {
            int available = progressionData.chronoShardsTotal - progressionData.chronoShardsSpent;
            
            if (available >= amount)
            {
                progressionData.chronoShardsSpent += amount;
                Debug.Log($"[ProgressionService] Spent {amount} Chrono Shards. Remaining: {available - amount}");
                SaveProgression();
                return true;
            }
            
            Debug.LogWarning($"[ProgressionService] Not enough Chrono Shards. Need: {amount}, Have: {available}");
            return false;
        }
        
        public void CompleteRoute(int routeID)
        {
            // Add to completed routes if not already there
            if (!progressionData.completedRoutes.Contains(routeID))
            {
                var completedList = progressionData.completedRoutes.ToList();
                completedList.Add(routeID);
                progressionData.completedRoutes = completedList.ToArray();
                
                Debug.Log($"[ProgressionService] Completed route {routeID}");
            }
            
            // Update current route
            progressionData.currentRoute = routeID + 1;
            
            SaveProgression();
            
        }
        
        public void UnlockWeapon(string weaponID)
        {
            if (!IsWeaponUnlocked(weaponID))
            {
                var weaponsList = progressionData.unlockedWeapons.ToList();
                weaponsList.Add(weaponID);
                progressionData.unlockedWeapons = weaponsList.ToArray();
                
                Debug.Log($"[ProgressionService] Unlocked weapon: {weaponID}");
                SaveProgression();
            }
        }
        
        public bool IsWeaponUnlocked(string weaponID)
        {
            return progressionData.unlockedWeapons.Contains(weaponID);
        }
        
        public void SaveProgression()
        {
            saveService.Save(SAVE_KEY, progressionData);
            Debug.Log("[ProgressionService] Progression saved");
        }
    }
}