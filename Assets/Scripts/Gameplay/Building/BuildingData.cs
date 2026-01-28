using UnityEngine;

namespace Gameplay.Building
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Gameplay/Buildings/Building Data", order = 1)]
    public class BuildingData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string buildingName = "Ballista";
        [SerializeField] private BuildingType buildingType = BuildingType.Offensive;
        [SerializeField] [TextArea(2, 4)] private string description = "Fires piercing bolts at enemies";
        
        [Header("Stats")]
        [SerializeField] private int baseCost = 100;
        [SerializeField] private int maxTier = 3;
        
        [Header("Tier Upgrades")]
        [SerializeField] private int[] upgradeCosts = new int[] { 150, 250 }; // Tier 2, Tier 3
        
        [Header("Combat Stats (if applicable)")]
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private int damage = 15;
        [SerializeField] private float range = 8f;
        [SerializeField] private float projectileSpeed = 20f;
        
        [Header("Special Effects")]
        [SerializeField] private bool piercing;
        [SerializeField] private int piercingTargets = 1;
        [SerializeField] private bool areaOfEffect;
        [SerializeField] private float aoeRadius;
        [SerializeField] private bool slowsEnemies;
        [SerializeField] private float slowAmount;
        [SerializeField] private float slowDuration;
        
        [Header("Visuals")]
        [SerializeField] private GameObject buildingPrefab;
        [SerializeField] private Sprite iconSprite;
        [SerializeField] private Color buildingColor = Color.white;
        
        [Header("Tier Visuals")]
        [SerializeField] private Sprite[] tierSprites; // Different sprites per tier
        [SerializeField] private GameObject[] tierEffects; // Particle effects per tier
        
        // Enums
        public enum BuildingType
        {
            Offensive,  
            Defensive,  
            Support,    
            Utility     
        }
        
        // Public getters
        public string BuildingName => buildingName;
        public BuildingType Type => buildingType;
        public string Description => description;
        public int BaseCost => baseCost;
        public int MaxTier => maxTier;
        public float FireRate => fireRate;
        public int Damage => damage;
        public float Range => range;
        public float ProjectileSpeed => projectileSpeed;
        public bool Piercing => piercing;
        public int PiercingTargets => piercingTargets;
        public bool AreaOfEffect => areaOfEffect;
        public float AOERadius => aoeRadius;
        public bool SlowsEnemies => slowsEnemies;
        public float SlowAmount => slowAmount;
        public float SlowDuration => slowDuration;
        public GameObject BuildingPrefab => buildingPrefab;
        public Sprite IconSprite => iconSprite;
        public Color BuildingColor => buildingColor;
        
        public int GetUpgradeCost(int currentTier)
        {
            if (currentTier >= maxTier || currentTier < 1)
                return -1; // Can't upgrade
            
            int index = currentTier - 1; // Tier 1 → index 0
            if (index < upgradeCosts.Length)
                return upgradeCosts[index];
            
            return -1;
        }
        
        public Sprite GetTierSprite(int tier)
        {
            if (tierSprites != null && tier > 0 && tier <= tierSprites.Length)
                return tierSprites[tier - 1];
            
            return null;
        }
        
        public GameObject GetTierEffect(int tier)
        {
            if (tierEffects != null && tier > 0 && tier <= tierEffects.Length)
                return tierEffects[tier - 1];
            
            return null;
        }
        
        // Get scaled stats for tier
        public int GetDamageForTier(int tier)
        {
            return Mathf.RoundToInt(damage * Mathf.Pow(1.5f, tier - 1));
        }
        
        public float GetFireRateForTier(int tier)
        {
            return fireRate * Mathf.Pow(0.85f, tier - 1); // Faster each tier
        }
        
        public float GetRangeForTier(int tier)
        {
            return range * (1 + (tier - 1) * 0.2f); // +20% range per tier
        }
    }
}