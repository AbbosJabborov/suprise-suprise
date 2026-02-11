using UnityEngine;

namespace Gameplay.Weapon
{
    [RequireComponent(typeof(Collider2D))]
    public class WeaponPickup : MonoBehaviour
    {
        [Header("Weapon Data")]
        [SerializeField] private WeaponData weaponData;
        
        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private float bobHeight = 0.2f;
        [SerializeField] private float rotationSpeed = 90f;
        
        private Vector3 startPosition;
        private float bobTimer;
        
        void Awake()
        {
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        void Start()
        {
            startPosition = transform.position;
            
            if (weaponData != null && spriteRenderer != null)
            {
                spriteRenderer.sprite = weaponData.WeaponSprite;
            }
        }
        
        void Update()
        {
            // Bob animation
            bobTimer += Time.deltaTime * bobSpeed;
            float bobOffset = Mathf.Sin(bobTimer) * bobHeight;
            transform.position = new Vector3(startPosition.x, startPosition.y + bobOffset, startPosition.z);
            
            // Rotation
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<Player.PlayerController>();
                
                if (player != null && player.Inventory != null)
                {
                    bool wasDropped = player.Inventory.AddWeapon(weaponData);
                    
                    if (wasDropped)
                    {
                        // Inventory was full, subscribe to drop event
                        player.Inventory.OnWeaponDropped += DropWeapon;
                    }
                    
                    Destroy(gameObject);
                }
            }
        }
        
        void DropWeapon(WeaponData droppedWeapon)
        {
            // Unsubscribe
            var player = FindFirstObjectByType<Player.PlayerController>();
            if (player != null)
                player.Inventory.OnWeaponDropped -= DropWeapon;
            
            // Spawn dropped weapon
            CreateWeaponPickup(droppedWeapon, transform.position);
        }
        
        public static GameObject CreateWeaponPickup(WeaponData weapon, Vector3 position)
        {
            GameObject pickupObj = new GameObject($"Weapon_{weapon.WeaponName}");
            pickupObj.transform.position = position;
            pickupObj.tag = "WeaponPickup";
            
            // Add WeaponPickup component
            WeaponPickup pickup = pickupObj.AddComponent<WeaponPickup>();
            pickup.weaponData = weapon;
            
            // Add SpriteRenderer
            SpriteRenderer sr = pickupObj.AddComponent<SpriteRenderer>();
            sr.sprite = weapon.WeaponSprite;
            sr.sortingOrder = -1;
            
            // Add collider
            CircleCollider2D col = pickupObj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.5f;
            
            // Add Rigidbody2D (for trigger detection)
            Rigidbody2D rb = pickupObj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
            
            Debug.Log($"[WeaponPickup] Created pickup for {weapon.WeaponName}");
            return pickupObj;
        }
        
        public void SetWeaponData(WeaponData data)
        {
            weaponData = data;
            if (spriteRenderer != null && data != null)
            {
                spriteRenderer.sprite = data.WeaponSprite;
            }
        }
    }
}