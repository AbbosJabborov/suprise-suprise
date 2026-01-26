using UnityEngine;
using UnityEngine.Events;

namespace Gameplay
{
    public class Health : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private int currentHealth;
        [SerializeField] private bool destroyOnDeath = true;
        [SerializeField] private float destroyDelay = 0f;
    
        [Header("Invincibility")]
        [SerializeField] private bool useInvincibilityFrames = false;
        [SerializeField] private float invincibilityDuration = 0.5f;
        private float lastDamageTime;
        private bool isInvincible = false;
    
        [Header("Visual Feedback")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private bool flashOnDamage = true;
        [SerializeField] private Color damageFlashColor = Color.red;
        [SerializeField] private float flashDuration = 0.1f;
    
        [Header("Effects")]
        [SerializeField] private GameObject deathEffectPrefab;
        [SerializeField] private AudioClip hurtSound;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioSource audioSource;
    
        [Header("Events")]
        public UnityEvent<int> OnDamaged; // Passes damage amount
        public UnityEvent<int, int> OnHealthChanged; // Passes (current, max)
        public UnityEvent OnDeath;
        public UnityEvent OnHealed;
    
        private Color originalColor;
        private bool isDead = false;
    
        void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }
    
        void Start()
        {
            currentHealth = maxHealth;
        
            if (spriteRenderer != null)
                originalColor = spriteRenderer.color;
        
            // Initialize events if null
            if (OnDamaged == null) OnDamaged = new UnityEvent<int>();
            if (OnHealthChanged == null) OnHealthChanged = new UnityEvent<int, int>();
            if (OnDeath == null) OnDeath = new UnityEvent();
            if (OnHealed == null) OnHealed = new UnityEvent();
        
            OnHealthChanged.Invoke(currentHealth, maxHealth);
        }
    
        void Update()
        {
            // Update invincibility
            if (isInvincible && Time.time - lastDamageTime >= invincibilityDuration)
            {
                isInvincible = false;
            }
        }
    
        public void TakeDamage(int damage)
        {
            if (isDead)
                return;
        
            // Check invincibility frames
            if (useInvincibilityFrames && isInvincible)
                return;
        
            // Apply damage
            currentHealth -= damage;
            currentHealth = Mathf.Max(currentHealth, 0);
        
            // Trigger events
            OnDamaged.Invoke(damage);
            OnHealthChanged.Invoke(currentHealth, maxHealth);
        
            // Visual/Audio feedback
            if (flashOnDamage && spriteRenderer != null)
            {
                StopAllCoroutines();
                StartCoroutine(DamageFlash());
            }
        
            if (audioSource != null && hurtSound != null)
            {
                audioSource.PlayOneShot(hurtSound);
            }
        
            // Set invincibility
            if (useInvincibilityFrames)
            {
                isInvincible = true;
                lastDamageTime = Time.time;
            }
        
            // Check death
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
            currentHealth += amount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        
            // Only invoke if actually healed
            if (currentHealth > oldHealth)
            {
                OnHealed.Invoke();
                OnHealthChanged.Invoke(currentHealth, maxHealth);
            }
        }
    
        public void SetHealth(int amount)
        {
            currentHealth = Mathf.Clamp(amount, 0, maxHealth);
            OnHealthChanged.Invoke(currentHealth, maxHealth);
        
            if (currentHealth <= 0 && !isDead)
            {
                Die();
            }
        }
    
        public void IncreaseMaxHealth(int amount)
        {
            maxHealth += amount;
            OnHealthChanged.Invoke(currentHealth, maxHealth);
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
        
            // Trigger death event
            OnDeath.Invoke();
        
            // Spawn death effect
            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }
        
            // Play death sound
            if (audioSource != null && deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);
            }
        
            // Destroy object
            if (destroyOnDeath)
            {
                Destroy(gameObject, destroyDelay);
            }
        }
    
        System.Collections.IEnumerator DamageFlash()
        {
            spriteRenderer.color = damageFlashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    
        // Public getters
        public int GetCurrentHealth() => currentHealth;
        public int GetMaxHealth() => maxHealth;
        public float GetHealthPercent() => (float)currentHealth / maxHealth;
        public bool IsDead() => isDead;
        public bool IsInvincible() => isInvincible;
        public bool IsAtFullHealth() => currentHealth >= maxHealth;
    }
}