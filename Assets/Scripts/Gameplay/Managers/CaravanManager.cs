using Gameplay.Building;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Managers
{
    public class CaravanManager : MonoBehaviour
    {
        [Header("Building Slots")]
        [SerializeField] private BuildingSlot[] buildingSlots;
    
        [Header("Events")]
        public UnityEvent<BuildingSlot, BuildingData> OnBuildingPlaced;
        public UnityEvent<BuildingSlot, int> OnBuildingUpgraded;
        public UnityEvent<BuildingSlot> OnBuildingRemoved;
    
        private Rigidbody2D rb;
    
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        
            // Auto-find slots if not assigned
            if (buildingSlots == null || buildingSlots.Length == 0)
            {
                buildingSlots = GetComponentsInChildren<BuildingSlot>();
            }
        
            // Subscribe to slot events
            foreach (var slot in buildingSlots)
            {
                slot.OnBuildingPlaced.AddListener((data, tier) => OnBuildingPlaced?.Invoke(slot, data));
                slot.OnBuildingUpgraded.AddListener((tier) => OnBuildingUpgraded?.Invoke(slot, tier));
                slot.OnBuildingRemoved.AddListener(() => OnBuildingRemoved?.Invoke(slot));
            }
        }
        
        public bool PlaceBuildingInSlot(int slotIndex, BuildingData buildingData)
        {
            if (slotIndex < 0 || slotIndex >= buildingSlots.Length)
                return false;
        
            BuildingSlot slot = buildingSlots[slotIndex];
        
            // Check cost
            if (GameManager.Instance != null)
            {
                if (!GameManager.Instance.SpendCoins(buildingData.BaseCost))
                {
                    Debug.Log($"Not enough coins! Need {buildingData.BaseCost}");
                    return false;
                }
            }
        
            return slot.PlaceBuilding(buildingData);
        }
    
        public bool UpgradeBuildingInSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= buildingSlots.Length)
                return false;
        
            BuildingSlot slot = buildingSlots[slotIndex];
        
            if (slot.IsEmpty())
                return false;
        
            int upgradeCost = slot.GetUpgradeCost();
            if (upgradeCost < 0)
            {
                Debug.Log("Building is already max tier!");
                return false;
            }
        
            // Check cost
            if (GameManager.Instance != null)
            {
                if (!GameManager.Instance.SpendCoins(upgradeCost))
                {
                    Debug.Log($"Not enough coins! Need {upgradeCost}");
                    return false;
                }
            }
        
            return slot.UpgradeBuilding();
        }
    
        public void RemoveBuildingFromSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= buildingSlots.Length)
                return;
        
            buildingSlots[slotIndex].RemoveBuilding();
        }
    
        public BuildingSlot GetSlot(int index)
        {
            if (index >= 0 && index < buildingSlots.Length)
                return buildingSlots[index];
        
            return null;
        }

        // public BuildingSlot GetEmptySlot()
        // {
        //     foreach (var slot in buildingSlots)
        //     {
        //         if (slot.IsEmpty())
        //             return slot;
        //     }
        //     return null;
        // }
        //
        public int GetSlotCount()
        {
            return buildingSlots.Length;
        }
    
        public BuildingSlot[] GetAllSlots()
        {
            return buildingSlots;
        }
        
        // Get stats
        public int GetTotalDamagePerSecond()
        {
            int totalDPS = 0;
        
            foreach (var slot in buildingSlots)
            {
                if (!slot.IsEmpty())
                {
                    BuildingData data = slot.GetCurrentBuilding();
                    int tier = slot.GetCurrentTier();
                
                    if (data.Type == BuildingData.BuildingType.Offensive)
                    {
                        int damage = data.GetDamageForTier(tier);
                        float fireRate = data.GetFireRateForTier(tier);
                        totalDPS += Mathf.RoundToInt(damage / fireRate);
                    }
                }
            }
        
            return totalDPS;
        }
    
        public int GetOccupiedSlotCount()
        {
            int count = 0;
            foreach (var slot in buildingSlots)
            {
                if (!slot.IsEmpty())
                    count++;
            }
            return count;
        }
    }
}