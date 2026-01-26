using UnityEngine;

namespace Gameplay
{
    public class HealthPickup : MonoBehaviour, IInteractable
    {
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        [Header("Health Settings")]
        [SerializeField] private int healAmount = 25;
        [SerializeField] private bool healToFull;
        [SerializeField] private string itemName = "Health Pack";
    
        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Color glowColor = Color.green;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseAmount = 0.2f;
    
        [Header("Audio")]
        [SerializeField] private AudioClip pickupSound;
    
        private Vector3 originalScale;
        private Material glowMaterial;
    
        void Start()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        
            originalScale = transform.localScale;
        
            // Create glow effect
            if (spriteRenderer != null)
            {
                glowMaterial = spriteRenderer.material;
                glowMaterial.SetColor(Color1, glowColor);
            }
        }
    
        void Update()
        {
            // Pulse animation
            float scale = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            transform.localScale = originalScale * scale;
        }
    
        public void Interact(GameObject interactor)
        {
            Health health = interactor.GetComponent<Health>();
        
            if (health != null)
            {
                // Check if player needs healing
                if (health.IsAtFullHealth())
                {
                    // Don't pick up if already at full health
                    return;
                }
            
                // Heal player
                if (healToFull)
                {
                    health.FullHeal();
                }
                else
                {
                    health.Heal(healAmount);
                }
            
                // Play sound
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }
            
                // Destroy pickup
                Destroy(gameObject);
            }
        }
    
        public string GetInteractionName()
        {
            if (healToFull)
                return $"{itemName} (Full Heal)";
            else
                return $"{itemName} (+{healAmount} HP)";
        }
    
        public string GetInteractionPrompt()
        {
            return "Use";
        }
    
        public bool CanInteract(GameObject interactor)
        {
            Health health = interactor.GetComponent<Health>();
        
            // Can only interact if has health component and not at full health
            return health != null && !health.IsAtFullHealth();
        }
    
        public Transform GetTransform()
        {
            return transform;
        }
    }
}