using UnityEngine;
using System;

namespace Gameplay.Caravan
{
    public class CaravanHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 500;
        [SerializeField] private bool invulnerable = false;
        
        private int currentHealth;
        private bool isDead;
        
        public event Action<int, int> OnHealthChanged; // current, max
        public event Action<int> OnDamageTaken; // damage amount
        public event Action OnDeath;
        
        public int CurrentHealth => currentHealth;
        public int MaxHealth => maxHealth;
        public bool IsDead => isDead;
        public float HealthPercent => (float)currentHealth / maxHealth;
        
        void Start()
        {
            currentHealth = maxHealth;
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        public void TakeDamage(int damage)
        {
            if (isDead || invulnerable || damage <= 0)
                return;
            
            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);
            
            OnDamageTaken?.Invoke(damage);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
            
            Debug.Log($"[CaravanHealth] Took {damage} damage. HP: {currentHealth}/{maxHealth}");
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        public void Heal(int amount)
        {
            if (isDead || amount <= 0)
                return;
            
            int oldHealth = currentHealth;
            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
            
            if (currentHealth > oldHealth)
            {
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
                Debug.Log($"[CaravanHealth] Healed {amount}. HP: {currentHealth}/{maxHealth}");
            }
        }
        
        public void IncreaseMaxHealth(int amount)
        {
            maxHealth += amount;
            currentHealth += amount; // Also increase current
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
        }
        
        public void SetInvulnerable(bool invuln)
        {
            invulnerable = invuln;
        }
        
        void Die()
        {
            if (isDead)
                return;
            
            isDead = true;
            OnDeath?.Invoke();
            
            Debug.Log("[CaravanHealth] Caravan destroyed!");
        }
        
        void OnCollisionEnter2D(Collision2D collision)
        {
            // Enemies deal damage on collision
            if (collision.gameObject.CompareTag("Enemy"))
            {
                var enemy = collision.gameObject.GetComponent<Enemy.Enemy>();
                if (enemy != null)
                {
                    TakeDamage(enemy.GetContactDamage());
                }
            }
        }
    }
}