using UnityEngine;

namespace Gameplay.Upgrades
{
    [CreateAssetMenu(fileName = "UpgradeCard", menuName = "Gameplay/Upgrades/Upgrade Card", order = 1)]
    public class UpgradeCardData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string upgradeName = "Damage Up";
        [SerializeField] [TextArea(2, 4)] private string description = "Increase weapon damage by 15%";
        [SerializeField] private Sprite icon;
        
        [Header("Upgrade Type")]
        [SerializeField] private UpgradeType upgradeType;
        [SerializeField] private UpgradeRarity rarity = UpgradeRarity.Common;
        
        [Header("Upgrade Values")]
        [SerializeField] private float value = 0.15f; // Used for percentage/flat values
        [SerializeField] private int intValue = 0; // Used for countable upgrades
        
        [Header("Visual")]
        [SerializeField] private Color cardColor = Color.white;
        [SerializeField] private GameObject visualEffect; // Particle effect on selection
        
        [Header("Stacking")]
        [SerializeField] private bool canStack = true;
        [SerializeField] private int maxStacks = 5;
        
        public enum UpgradeType
        {
            // Player Stats
            PlayerDamage,           // Increase weapon damage
            PlayerFireRate,         // Increase fire rate
            PlayerMoveSpeed,        // Increase movement speed
            PlayerMaxHealth,        // Increase max HP
            PlayerHealthRegen,      // Gain HP regen
            
            // Weapon Mods
            WeaponPiercing,        // Add/increase piercing
            WeaponKnockback,       // Increase knockback
            WeaponProjectiles,     // Add extra projectiles
            WeaponRange,           // Increase bullet lifetime/range
            
            // Caravan
            CaravanMaxHealth,      // Increase caravan HP
            CaravanSpeed,          // Increase caravan speed
            
            // Buildings
            BuildingDamage,        // All buildings do more damage
            BuildingFireRate,      // All buildings fire faster
            BuildingRange,         // All buildings have more range
            
            // Utility
            CoinMagnet,           // Increase coin pickup radius
            ExperienceGain,       // Gain more XP
            Luck,                 // Better loot/rewards
            
            // Special
            HealNow,              // Instant heal
            NewWeapon,            // Unlock specific weapon
            NewBuilding,          // Unlock specific building
            Special               // Custom effect
        }
        
        public enum UpgradeRarity
        {
            Common,
            Uncommon,
            Rare,
            Epic,
            Legendary
        }
        
        // Public getters
        public string UpgradeName => upgradeName;
        public string Description => description;
        public Sprite Icon => icon;
        public UpgradeType Type => upgradeType;
        public UpgradeRarity Rarity => rarity;
        public float Value => value;
        public int IntValue => intValue;
        public Color CardColor => cardColor;
        public GameObject VisualEffect => visualEffect;
        public bool CanStack => canStack;
        public int MaxStacks => maxStacks;
        
        // Get formatted description with values
        public string GetFormattedDescription()
        {
            return description
                .Replace("{value}", (value * 100f).ToString("F0") + "%")
                .Replace("{intValue}", intValue.ToString());
        }
    }
}