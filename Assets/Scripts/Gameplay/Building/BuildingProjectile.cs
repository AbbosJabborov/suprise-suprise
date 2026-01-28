using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Building
{
    public class BuildingProjectile : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private TrailRenderer trailRenderer;
    
        [Header("Hit Effects")]
        [SerializeField] private GameObject hitEffectPrefab;
        [SerializeField] private LayerMask enemyLayer;
    
        private Vector2 direction;
        private float speed;
        private int damage;
        private float lifetime;
        private bool piercing;
        private int piercingTargets;
        private float spawnTime;
        private List<GameObject> hitEnemies = new List<GameObject>();
    
        void Awake()
        {
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (trailRenderer == null)
                trailRenderer = GetComponent<TrailRenderer>();
        }
    
        void OnEnable()
        {
            spawnTime = Time.time;
            hitEnemies.Clear();
        
            if (trailRenderer != null)
            {
                trailRenderer.Clear();
            }
        }
    
        public void Initialize(Vector2 dir, float spd, int dmg, float life, bool pierce = false, int pierceCount = 1)
        {
            direction = dir.normalized;
            speed = spd;
            damage = dmg;
            lifetime = life;
            piercing = pierce;
            piercingTargets = pierceCount;
            spawnTime = Time.time;
        
            if (rb != null)
            {
                rb.linearVelocity = direction * speed;
            }
        }
    
        void Update()
        {
            if (Time.time - spawnTime >= lifetime)
            {
                DeactivateProjectile();
            }
        }
    
        void FixedUpdate()
        {
            if (rb != null)
            {
                rb.linearVelocity = direction * speed;
            }
        }
    
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & enemyLayer) != 0)
            {
                // Check if already hit this enemy (for piercing)
                if (hitEnemies.Contains(collision.gameObject))
                    return;
            
                // Deal damage
                Enemy.Enemy enemy = collision.GetComponent<Enemy.Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    hitEnemies.Add(collision.gameObject);
                }
            
                Health health = collision.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            
                // Spawn hit effect
                SpawnHitEffect(collision.transform.position);
            
                // Check if should destroy
                if (!piercing || hitEnemies.Count >= piercingTargets)
                {
                    DeactivateProjectile();
                }
            }
        }
    
        void SpawnHitEffect(Vector3 position)
        {
            if (hitEffectPrefab != null)
            {
                GameObject effect = Instantiate(hitEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 1f);
            }
        }
    
        void DeactivateProjectile()
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        
            hitEnemies.Clear();
            gameObject.SetActive(false);
        }
    }
}