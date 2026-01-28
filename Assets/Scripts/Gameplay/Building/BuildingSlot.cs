using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Building
{
    public class BuildingSlot : MonoBehaviour
    {
        [Header("Slot Info")]
        [SerializeField] private int slotIndex;
        [SerializeField] private Transform buildingSpawnPoint;
        [SerializeField] private BuildingSlotPosition slotPosition = BuildingSlotPosition.Top;
    
        [Header("Current Building")]
        [SerializeField] private BuildingData currentBuildingData;
        [SerializeField] private GameObject currentBuildingInstance;
        [SerializeField] private int currentTier = 1;
    
        [Header("Visual")]
        [SerializeField] private SpriteRenderer slotVisual;
        [SerializeField] private Color emptyColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        [SerializeField] private Color occupiedColor = new Color(1f, 1f, 1f, 0.5f);
        [SerializeField] private GameObject highlightEffect;
    
        [Header("Events")]
        public UnityEvent<BuildingData, int> OnBuildingPlaced; // Building, tier
        public UnityEvent<int> OnBuildingUpgraded; // New tier
        public UnityEvent OnBuildingRemoved;
    
        public enum BuildingSlotPosition
        {
            Top,
            Left,
            Right,
            Back
        }
    
        void Start()
        {
            UpdateVisual();
        
            if (highlightEffect != null)
                highlightEffect.SetActive(false);
        }
    
        public bool IsEmpty()
        {
            return currentBuildingData == null;
        }
    
        public bool CanPlaceBuilding(BuildingData buildingData)
        {
            return IsEmpty();
        }
    
        public bool PlaceBuilding(BuildingData buildingData)
        {
            if (!CanPlaceBuilding(buildingData))
                return false;
        
            currentBuildingData = buildingData;
            currentTier = 1;
        
            // Spawn building visual
            SpawnBuildingVisual();
        
            // Update slot visual
            UpdateVisual();
        
            // Trigger event
            OnBuildingPlaced?.Invoke(buildingData, currentTier);
        
            return true;
        }
    
        public bool UpgradeBuilding()
        {
            if (currentBuildingData == null)
                return false;
        
            if (currentTier >= currentBuildingData.MaxTier)
                return false; // Already max tier
        
            currentTier++;
        
            // Update visual
            UpdateBuildingVisual();
        
            // Trigger event
            OnBuildingUpgraded?.Invoke(currentTier);
        
            return true;
        }
    
        public void RemoveBuilding()
        {
            if (currentBuildingInstance != null)
            {
                Destroy(currentBuildingInstance);
            }
        
            currentBuildingData = null;
            currentBuildingInstance = null;
            currentTier = 1;
        
            UpdateVisual();
            OnBuildingRemoved?.Invoke();
        }
    
        void SpawnBuildingVisual()
        {
            if (currentBuildingData == null || currentBuildingData.BuildingPrefab == null)
                return;
        
            Vector3 spawnPos = buildingSpawnPoint != null ? buildingSpawnPoint.position : transform.position;
        
            currentBuildingInstance = Instantiate(currentBuildingData.BuildingPrefab, spawnPos, Quaternion.identity, transform);
        
            // Initialize building script
            Building buildingScript = currentBuildingInstance.GetComponent<Building>();
            if (buildingScript != null)
            {
                buildingScript.Initialize(currentBuildingData, currentTier, this);
            }
        
            UpdateBuildingVisual();
        }
    
        void UpdateBuildingVisual()
        {
            if (currentBuildingInstance == null)
                return;
        
            // Update sprite based on tier
            SpriteRenderer sr = currentBuildingInstance.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Sprite tierSprite = currentBuildingData.GetTierSprite(currentTier);
                if (tierSprite != null)
                {
                    sr.sprite = tierSprite;
                }
            }
        
            // Update effects
            GameObject tierEffect = currentBuildingData.GetTierEffect(currentTier);
            if (tierEffect != null)
            {
                // Spawn tier effect as child
                Transform effectPoint = currentBuildingInstance.transform.Find("EffectPoint");
                Vector3 effectPos = effectPoint != null ? effectPoint.position : currentBuildingInstance.transform.position;
            
                GameObject effect = Instantiate(tierEffect, effectPos, Quaternion.identity, currentBuildingInstance.transform);
            }
        
            // Notify building script of upgrade
            Building buildingScript = currentBuildingInstance.GetComponent<Building>();
            if (buildingScript != null)
            {
                buildingScript.OnTierUpgraded(currentTier);
            }
        }
    
        void UpdateVisual()
        {
            if (slotVisual != null)
            {
                slotVisual.color = IsEmpty() ? emptyColor : occupiedColor;
            }
        }
    
        public void SetHighlight(bool active)
        {
            if (highlightEffect != null)
            {
                highlightEffect.SetActive(active);
            }
        }
    
        // Public getters
        public int GetSlotIndex() => slotIndex;
        public BuildingData GetCurrentBuilding() => currentBuildingData;
        public int GetCurrentTier() => currentTier;
        public GameObject GetBuildingInstance() => currentBuildingInstance;
        public BuildingSlotPosition GetSlotPosition() => slotPosition;
        public int GetUpgradeCost()
        {
            if (currentBuildingData == null)
                return -1;
        
            return currentBuildingData.GetUpgradeCost(currentTier);
        }
    }
}