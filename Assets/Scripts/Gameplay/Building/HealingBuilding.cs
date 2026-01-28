using UnityEngine;

namespace Gameplay.Building
{
    public class HealingBuilding : Building
    {
        [Header("Healing Settings")]
        [SerializeField] private int healPerTick = 1;
        [SerializeField] private float healTickRate = 1f;
        [SerializeField] private bool healPlayer;
        [SerializeField] private float playerHealRange = 5f;
        [SerializeField] private ParticleSystem healEffect;
    
        private float lastHealTime;
        private Health caravanHealth;
        private GameObject player;
    
        protected override void OnInitialized()
        {
            base.OnInitialized();
        
            // Find caravan
            GameObject caravan = GameObject.FindGameObjectWithTag("Caravan");
            if (caravan != null)
            {
                caravanHealth = caravan.GetComponent<Health>();
            }
        
            // Find player
            player = GameObject.FindGameObjectWithTag("Player");
        }
    
        protected override void Update()
        {
            if (!isActive)
                return;
        
            if (Time.time >= lastHealTime + healTickRate)
            {
                PerformHealing();
                lastHealTime = Time.time;
            }
        }
    
        protected override void Attack()
        {
            // Healing doesn't use the standard attack
            PerformHealing();
        }
    
        void PerformHealing()
        {
            bool healed = false;
        
            // Heal caravan
            if (caravanHealth != null && !caravanHealth.IsAtFullHealth())
            {
                int scaledHeal = Mathf.RoundToInt(healPerTick * currentTier);
                caravanHealth.Heal(scaledHeal);
                healed = true;
            }
        
            // Heal player (if tier 2+)
            if (currentTier >= 2 && healPlayer && player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            
                if (distanceToPlayer <= playerHealRange)
                {
                    Health playerHealth = player.GetComponent<Health>();
                    if (playerHealth != null && !playerHealth.IsAtFullHealth())
                    {
                        int scaledHeal = Mathf.RoundToInt(healPerTick * currentTier * 0.5f); // Half heal for player
                        playerHealth.Heal(scaledHeal);
                        healed = true;
                    }
                }
            }
        
            // Play effect if healed
            if (healed && healEffect != null)
            {
                healEffect.Play();
            }
        }
    
        protected override void OnUpgraded()
        {
            base.OnUpgraded();
        
            // Unlock player healing at tier 2
            if (currentTier >= 2)
            {
                healPlayer = true;
            }
        }
    
        protected override void FindTarget()
        {
            // Healing building doesn't need to find targets
            // It always heals the caravan
        }
    
        protected override void OnDrawGizmosSelected()
        {
            // Draw healing range for player (tier 2+)
            if (currentTier >= 2 && healPlayer)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, playerHealRange);
            }
        
            // Draw line to caravan
            GameObject caravan = GameObject.FindGameObjectWithTag("Caravan");
            if (caravan != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(transform.position, caravan.transform.position);
            }
        }
    }
}