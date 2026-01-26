using Gameplay.Managers;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("Coin Settings")]
    [SerializeField] private int coinValue = 10;
    [SerializeField] private float magnetRange = 3f;
    [SerializeField] private float magnetSpeed = 8f;
    [SerializeField] private float pickupRange = 0.5f;
    
    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.2f;
    
    [Header("Lifetime")]
    [SerializeField] private float lifetime = 30f;
    [SerializeField] private float blinkStartTime = 25f;
    [SerializeField] private float blinkSpeed = 10f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;
    
    private Transform player;
    private Rigidbody2D rb;
    private Vector3 startPosition;
    private float spawnTime;
    private bool isBeingCollected = false;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
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
        transform.position = new Vector3(
            transform.position.x, 
            startPosition.y + bobOffset, 
            transform.position.z
        );
        
        // Magnetic pull
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= magnetRange)
        {
            Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * magnetSpeed;
            
            if (distanceToPlayer <= pickupRange)
            {
                Collect();
            }
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, Time.deltaTime * 5f);
        }
        
        // Lifetime and blinking
        float timeAlive = Time.time - spawnTime;
        if (timeAlive >= lifetime)
        {
            Destroy(gameObject);
        }
        else if (timeAlive >= blinkStartTime)
        {
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
        
        // Add coins to game manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddCoins(coinValue);
        }
        
        // Play sound
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
        
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
}