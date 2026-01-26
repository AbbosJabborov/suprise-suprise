using Player;
using UnityEngine;

namespace Gameplay.Weapon
{
    public class WeaponPickup : MonoBehaviour, IInteractable
    {
        [Header("Weapon Data")]
        [SerializeField] private WeaponData weaponData;
    
        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private float bobHeight = 0.2f;
        [SerializeField] private float rotationSpeed = 90f;
    
        [Header("Drop Settings")]
        [SerializeField] private bool wasDropped = false;
        [SerializeField] private float dropForce = 5f;
    
        private Vector3 startPosition;
        private Rigidbody2D rb;
    
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }
    
        void Start()
        {
            startPosition = transform.position;
        
            // Set visual to match weapon
            if (weaponData != null && spriteRenderer != null && weaponData.WeaponSprite != null)
            {
                spriteRenderer.sprite = weaponData.WeaponSprite;
            }
        }
    
        void Update()
        {
            // Floating animation (if not dropped)
            if (!wasDropped || (rb != null && rb.linearVelocity.magnitude < 0.1f))
            {
                float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
                transform.position = new Vector3(
                    transform.position.x, 
                    startPosition.y + bobOffset, 
                    transform.position.z
                );
            
                // Rotation animation
                transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }
        }
    
        public void Interact(GameObject interactor)
        {
            PlayerTopDownShooting shooting = interactor.GetComponent<PlayerTopDownShooting>();
            if (shooting != null && weaponData != null)
            {
                // Get current weapon before switching
                WeaponData oldWeapon = shooting.GetCurrentWeapon();
            
                // Switch to new weapon
                shooting.SwitchWeapon(weaponData);
            
                // Drop old weapon if exists
                if (oldWeapon != null)
                {
                    DropWeapon(oldWeapon, interactor.transform.position);
                }
            
                // Destroy this pickup
                Destroy(gameObject);
            }
        }
    
        public string GetInteractionName()
        {
            return weaponData != null ? weaponData.WeaponName : "Unknown Weapon";
        }
    
        public string GetInteractionPrompt()
        {
            return "Pick up";
        }
    
        public bool CanInteract(GameObject interactor)
        {
            // Can interact if player has shooting component
            return interactor.GetComponent<PlayerTopDownShooting>() != null;
        }
    
        public Transform GetTransform()
        {
            return transform;
        }
    
        void DropWeapon(WeaponData weapon, Vector3 dropPosition)
        {
            // Create weapon pickup at player's position
            GameObject droppedWeapon = new GameObject($"Weapon_{weapon.WeaponName}");
            droppedWeapon.transform.position = dropPosition;
        
            // Add components
            WeaponPickup pickup = droppedWeapon.AddComponent<WeaponPickup>();
            pickup.weaponData = weapon;
            pickup.wasDropped = true;
        
            SpriteRenderer sr = droppedWeapon.AddComponent<SpriteRenderer>();
            sr.sprite = weapon.WeaponSprite;
            sr.sortingOrder = -1; // Behind player
        
            CircleCollider2D col = droppedWeapon.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
        
            Rigidbody2D droppedRb = droppedWeapon.AddComponent<Rigidbody2D>();
            droppedRb.gravityScale = 0;
        
            // Add random drop force
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            droppedRb.AddForce(randomDirection * dropForce, ForceMode2D.Impulse);
        }
    
        public static GameObject CreateWeaponPickup(WeaponData weapon, Vector3 position)
        {
            GameObject weaponObj = new GameObject($"Weapon_{weapon.WeaponName}");
            weaponObj.transform.position = position;
        
            WeaponPickup pickup = weaponObj.AddComponent<WeaponPickup>();
            pickup.weaponData = weapon;
        
            SpriteRenderer sr = weaponObj.AddComponent<SpriteRenderer>();
            sr.sprite = weapon.WeaponSprite;
        
            CircleCollider2D col = weaponObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
        
            Rigidbody2D rb = weaponObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        
            return weaponObj;
        }
    }
}