using System.Collections.Generic;
using Gameplay.Player;
using Gameplay.Upgrades;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class UpgradeManager : MonoBehaviour
    {
        public static UpgradeManager Instance { get; private set; }
    
        [Header("Available Upgrades Pool")]
        [SerializeField] private List<UpgradeCardData> allUpgrades = new List<UpgradeCardData>();
    
        [Header("Upgrade Selection")]
        [SerializeField] private int cardsPerChoice = 3;
    
        [Header("References")]
        [SerializeField] private GameObject player;
        [SerializeField] private GameObject caravan;
    
        [Header("Events")]
        public UnityEvent<UpgradeCardData> OnUpgradeApplied;
        public UnityEvent<List<UpgradeCardData>> OnUpgradeChoicesGenerated;
    
        // Track applied upgrades and their stack counts
        private Dictionary<UpgradeCardData, int> appliedUpgrades = new Dictionary<UpgradeCardData, int>();
    
        // Current stats multipliers
        private float damageMultiplier = 1f;
        private float fireRateMultiplier = 1f;
        private float moveSpeedMultiplier = 1f;
        private int maxHealthBonus;
    
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    
        void Start()
        {
            // Auto-find references if not set
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player");
        
            if (caravan == null)
                caravan = GameObject.FindGameObjectWithTag("Caravan");
        }
    
        public List<UpgradeCardData> GenerateUpgradeChoices()
        {
            var choices = new List<UpgradeCardData>();
            var availableUpgrades = new List<UpgradeCardData>(allUpgrades);
        
            // Filter out upgrades that can't stack and are already at max
            availableUpgrades.RemoveAll(upgrade => 
                                        {
                                            if (appliedUpgrades.TryGetValue(upgrade, out int appliedUpgrade))
                                            {
                                                if (!upgrade.CanStack)
                                                    return true; // Remove non-stackable that's already applied
                
                                                if (appliedUpgrade >= upgrade.MaxStacks)
                                                    return true; // Remove maxed out upgrades
                                            }
                                            return false;
                                        });
        
            // Randomly select cards
            for (int i = 0; i < cardsPerChoice && availableUpgrades.Count > 0; i++)
            {
                // Weighted random based on rarity
                UpgradeCardData selected = SelectWeightedRandom(availableUpgrades);
                choices.Add(selected);
                availableUpgrades.Remove(selected);
            }
        
            OnUpgradeChoicesGenerated?.Invoke(choices);
            return choices;
        }
    
        UpgradeCardData SelectWeightedRandom(List<UpgradeCardData> upgrades)
        {
            // Calculate total weight
            float totalWeight = 0f;
            foreach (var upgrade in upgrades)
            {
                totalWeight += GetRarityWeight(upgrade.Rarity);
            }
        
            // Random selection
            float randomValue = Random.Range(0f, totalWeight);
            float currentWeight = 0f;
        
            foreach (var upgrade in upgrades)
            {
                currentWeight += GetRarityWeight(upgrade.Rarity);
                if (randomValue <= currentWeight)
                {
                    return upgrade;
                }
            }
        
            return upgrades[upgrades.Count - 1];
        }
    
        float GetRarityWeight(UpgradeCardData.UpgradeRarity rarity)
        {
            return rarity switch
            {
                UpgradeCardData.UpgradeRarity.Common => 10f,
                UpgradeCardData.UpgradeRarity.Uncommon => 7f,
                UpgradeCardData.UpgradeRarity.Rare => 4f,
                UpgradeCardData.UpgradeRarity.Epic => 2f,
                UpgradeCardData.UpgradeRarity.Legendary => 1f,
                _ => 5f
            };
        }
    
        public void ApplyUpgrade(UpgradeCardData upgrade)
        {
            if (upgrade == null)
                return;
        
            // Track upgrade
            if (appliedUpgrades.ContainsKey(upgrade))
            {
                appliedUpgrades[upgrade]++;
            }
            else
            {
                appliedUpgrades[upgrade] = 1;
            }
        
            // Apply upgrade effect
            ApplyUpgradeEffect(upgrade);
        
            // Trigger event
            OnUpgradeApplied?.Invoke(upgrade);
        
            Debug.Log($"Applied upgrade: {upgrade.UpgradeName} (Stack: {appliedUpgrades[upgrade]})");
        }
    
        void ApplyUpgradeEffect(UpgradeCardData upgrade)
        {
            switch (upgrade.Type)
            {
                case UpgradeCardData.UpgradeType.PlayerDamage:
                    ApplyPlayerDamageUpgrade(upgrade.Value);
                    break;
            
                case UpgradeCardData.UpgradeType.PlayerFireRate:
                    ApplyPlayerFireRateUpgrade(upgrade.Value);
                    break;
            
                case UpgradeCardData.UpgradeType.PlayerMoveSpeed:
                    ApplyPlayerMoveSpeedUpgrade(upgrade.Value);
                    break;
            
                case UpgradeCardData.UpgradeType.PlayerMaxHealth:
                    ApplyPlayerMaxHealthUpgrade(upgrade.IntValue);
                    break;
            
                case UpgradeCardData.UpgradeType.WeaponPiercing:
                    ApplyWeaponPiercingUpgrade(upgrade.IntValue);
                    break;
            
                case UpgradeCardData.UpgradeType.WeaponKnockback:
                    ApplyWeaponKnockbackUpgrade(upgrade.Value);
                    break;
            
                case UpgradeCardData.UpgradeType.HealNow:
                    ApplyInstantHeal(upgrade.IntValue);
                    break;
            
                // Add more cases as needed
            
                default:
                    Debug.LogWarning($"Upgrade type {upgrade.Type} not implemented yet");
                    break;
            }
        }
    
        // Specific upgrade implementations
        void ApplyPlayerDamageUpgrade(float percentage)
        {
            damageMultiplier += percentage;
        
            // Apply to player's weapon (you'll need to implement this in PlayerTopDownShooting)
            if (player != null)
            {
                // player.GetComponent<PlayerTopDownShooting>().AddDamageMultiplier(percentage);
                Debug.Log($"Player damage increased by {percentage * 100}%. Total: {damageMultiplier * 100}%");
            }
        }
    
        void ApplyPlayerFireRateUpgrade(float percentage)
        {
            fireRateMultiplier -= percentage; // Lower fire rate = faster shooting
        
            if (player != null)
            {
                // Apply to weapon
                Debug.Log($"Fire rate increased by {percentage * 100}%");
            }
        }
    
        void ApplyPlayerMoveSpeedUpgrade(float percentage)
        {
            moveSpeedMultiplier += percentage;

            var movement = player.GetComponent<PlayerTopDownMovement>();

            if (player != null)
            {
                if (movement != null)
                {
                    movement.AddSpeedBonus(percentage);
                }
            }
        }
    
        void ApplyPlayerMaxHealthUpgrade(int amount)
        {
            maxHealthBonus += amount;
        
            if (player != null)
            {
                Health health = player.GetComponent<Health>();
                if (health != null)
                {
                    health.IncreaseMaxHealth(amount);
                }
            }
        }
    
        void ApplyWeaponPiercingUpgrade(int amount)
        {
            if (player != null)
            {
                // Add piercing to current weapon
                // You'll need to implement this
                Debug.Log($"Added {amount} piercing to weapons");
            }
        }
    
        void ApplyWeaponKnockbackUpgrade(float amount)
        {
            if (player != null)
            {
                Debug.Log($"Increased knockback by {amount}");
            }
        }
    
        void ApplyInstantHeal(int amount)
        {
            if (player != null)
            {
                Health health = player.GetComponent<Health>();
                if (health != null)
                {
                    health.Heal(amount);
                }
            }
        }
    
        // Public getters
        public float GetDamageMultiplier() => damageMultiplier;
        public float GetFireRateMultiplier() => fireRateMultiplier;
        public float GetMoveSpeedMultiplier() => moveSpeedMultiplier;
        public int GetMaxHealthBonus() => maxHealthBonus;
    
        public int GetUpgradeStack(UpgradeCardData upgrade)
        {
            return appliedUpgrades.ContainsKey(upgrade) ? appliedUpgrades[upgrade] : 0;
        }
    }
}