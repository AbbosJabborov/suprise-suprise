using Gameplay.Managers;
using UnityEngine;

namespace Gameplay
{
    public class Coin : MonoBehaviour
    {
        [Header("Coin Settings")]
        [SerializeField] private int coinValue = 10;
        [SerializeField] private float magnetRange = 3f;
        [SerializeField] private float magnetSpeed = 8f;
        [SerializeField] private float pickupRange = 0.5f;
    
        [Header("Visual")]
        [SerializeField] private float rotationSpeed = 180f;
        [SerializeField] private float bobSpeed = 2f;
        [SerializeField] private float bobHeight = 0.2f;
        [SerializeField] private SpriteRenderer spriteRenderer;
    
        [Header("Lifetime")]
        [SerializeField] private float lifetime = 30f; // Despawn after this time
        [SerializeField] private float blinkStartTime = 25f; // Start blinking before despawn
        [SerializeField] private float blinkSpeed = 10f;
    
        private Transform player;
        private Rigidbody2D rb;
        private Vector3 startPosition;
        private float spawnTime;
        private bool isBeingCollected;
    
        void Awake()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
        }
    
        void Start()
        {
            // Find player
            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        
            startPosition = transform.position;
            spawnTime = Time.time;
        }
    
        void Update()
        {
            if (player == null || isBeingCollected)
                return;
        
            // Rotation animation
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        
            // Bob up and down
            float bobOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, startPosition.y + bobOffset, transform.position.z);
        
            // Check for magnetic pull
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
            if (distanceToPlayer <= magnetRange)
            {
                // Pull toward player
                Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
                rb.linearVelocity = direction * magnetSpeed;
            
                // Check if close enough to collect
                if (distanceToPlayer <= pickupRange)
                {
                    Collect();
                }
            }
            else
            {
                // Slow down if not being pulled
                rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 5f);
            }
        
            // Handle lifetime and blinking
            float timeAlive = Time.time - spawnTime;
            if (timeAlive >= lifetime)
            {
                Despawn();
            }
            else if (timeAlive >= blinkStartTime)
            {
                // Blink warning
                float alpha = Mathf.PingPong(Time.time * blinkSpeed, 1f);
                Color color = spriteRenderer.color;
                color.a = alpha;
                spriteRenderer.color = color;
            }
        }
    
        void Collect()
        {
            if (isBeingCollected)
                return;
        
            isBeingCollected = true;
            
            GameManager.Instance?.AddCoins(coinValue);
        
            // Play collection effect/sound
            // You can add particle effects or sound here
        
            // Destroy coin
            Destroy(gameObject);
        }
    
        void Despawn()
        {
            Destroy(gameObject);
        }
    
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                Collect();
            }
        }
    
        public void SetValue(int value)
        {
            coinValue = value;
        }
    
        public int GetValue()
        {
            return coinValue;
        }
    
        void OnDrawGizmosSelected()
        {
            // Visualize magnet range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, magnetRange);
        
            // Visualize pickup range
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, pickupRange);
        }
    }
}