using UnityEngine;
using System;

namespace Gameplay.Player
{
    /// <summary>
    /// Handles player health and regeneration
    /// Pure C# class, no MonoBehaviour
    /// </summary>
    public class PlayerHealth
    {
        private readonly PlayerData playerData;
        
        private int currentHealth;
        private int maxHealth;
        private bool isDead;
        
        // Events
        public event Action<int, int> OnHealthChanged; // current, max
        public event Action OnDeath;
        
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsDead => isDead;
        
        public PlayerHealth(PlayerData playerData)
        {
            this.playerData = playerData;
            maxHealth = playerData.MaxHealth;
            currentHealth = maxHealth;
        }
        
        public void Update(float deltaTime)
        {
            if (isDead)
                return;
            
            // Health regeneration
            if (playerData.HealthRegenRate > 0 && currentHealth < maxHealth)
            {
                float regenAmount = playerData.HealthRegenRate * deltaTime;
                currentHealth = Mathf.Min(currentHealth + Mathf.RoundToInt(regenAmount), maxHealth);
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
        }
        
        public void TakeDamage(int damage)
        {
            if (isDead)
                return;
            
            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);
            
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        public void Heal(int amount)
        {
            if (isDead)
                return;
            
            int oldHealth = currentHealth;
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            
            if (currentHealth > oldHealth)
            {
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
            }
        }
        
        public void IncreaseMaxHealth(int amount)
        {
            maxHealth += amount;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        public void FullHeal()
        {
            Heal(maxHealth);
        }
        
        void Die()
        {
            if (isDead)
                return;
            
            isDead = true;
            OnDeath?.Invoke();
        }
        
        public float GetHealthPercent()
        {
            return (float)currentHealth / maxHealth;
        }
    }
}