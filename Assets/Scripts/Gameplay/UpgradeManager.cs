using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Gameplay.Upgrades;

namespace Gameplay
{
    public class UpgradeManager
    {
        [Header("Upgrade Pool")]
        private readonly List<UpgradeCardData> allUpgrades;
        private readonly int cardsPerChoice = 3;
        
        private readonly Dictionary<UpgradeCardData, int> appliedUpgrades = new Dictionary<UpgradeCardData, int>();
        
        // Stats tracking
        private float damageMultiplier = 1f;
        private float fireRateMultiplier = 1f;
        private float moveSpeedMultiplier = 1f;
        private int maxHealthBonus;
        
        public UpgradeManager(List<UpgradeCardData> upgradePool)
        {
            allUpgrades = upgradePool ?? new List<UpgradeCardData>();
        }
        
        public List<UpgradeCardData> GenerateUpgradeChoices()
        {
            var choices = new List<UpgradeCardData>();
            var available = new List<UpgradeCardData>(allUpgrades);
            
            // Filter maxed upgrades
            available.RemoveAll(upgrade =>
            {
                if (appliedUpgrades.TryGetValue(upgrade, out int count))
                {
                    if (!upgrade.CanStack) return true;
                    if (count >= upgrade.MaxStacks) return true;
                }
                return false;
            });
            
            // Select weighted random
            for (int i = 0; i < cardsPerChoice && available.Count > 0; i++)
            {
                var selected = SelectWeightedRandom(available);
                choices.Add(selected);
                available.Remove(selected);
            }
            
            return choices;
        }
        
        UpgradeCardData SelectWeightedRandom(List<UpgradeCardData> upgrades)
        {
            float totalWeight = 0f;
            foreach (var upgrade in upgrades)
                totalWeight += GetRarityWeight(upgrade.Rarity);
            
            float random = Random.Range(0f, totalWeight);
            float current = 0f;
            
            foreach (var upgrade in upgrades)
            {
                current += GetRarityWeight(upgrade.Rarity);
                if (random <= current)
                    return upgrade;
            }
            
            return upgrades[upgrades.Count - 1];
        }
        
        float GetRarityWeight(UpgradeCardData.UpgradeRarity rarity) => rarity switch
        {
            UpgradeCardData.UpgradeRarity.Common => 10f,
            UpgradeCardData.UpgradeRarity.Uncommon => 7f,
            UpgradeCardData.UpgradeRarity.Rare => 4f,
            UpgradeCardData.UpgradeRarity.Epic => 2f,
            UpgradeCardData.UpgradeRarity.Legendary => 1f,
            _ => 5f
        };
        
        public void ApplyUpgrade(UpgradeCardData upgrade, Player.PlayerController player)
        {
            if (upgrade == null) return;
            
            // Track
            if (appliedUpgrades.ContainsKey(upgrade))
                appliedUpgrades[upgrade]++;
            else
                appliedUpgrades[upgrade] = 1;
            
            // Apply
            ApplyUpgradeEffect(upgrade, player);
            
            Debug.Log($"[UpgradeManager] Applied: {upgrade.UpgradeName} (Stack: {appliedUpgrades[upgrade]})");
        }
        
        void ApplyUpgradeEffect(UpgradeCardData upgrade, Player.PlayerController player)
        {
            switch (upgrade.Type)
            {
                case UpgradeCardData.UpgradeType.PlayerDamage:
                    damageMultiplier += upgrade.Value;
                    break;
                
                case UpgradeCardData.UpgradeType.PlayerFireRate:
                    fireRateMultiplier += upgrade.Value;
                    break;
                
                case UpgradeCardData.UpgradeType.PlayerMoveSpeed:
                    moveSpeedMultiplier += upgrade.Value;
                    // Apply to player movement system
                    break;
                
                case UpgradeCardData.UpgradeType.PlayerMaxHealth:
                    maxHealthBonus += upgrade.IntValue;
                    break;
                
                case UpgradeCardData.UpgradeType.HealNow:
                    if (player != null)
                        player.Heal(upgrade.IntValue);
                    break;
                
                // Add more cases...
            }
        }
        
        public float GetDamageMultiplier() => damageMultiplier;
        public float GetFireRateMultiplier() => fireRateMultiplier;
        public float GetMoveSpeedMultiplier() => moveSpeedMultiplier;
        public int GetMaxHealthBonus() => maxHealthBonus;
    }
}