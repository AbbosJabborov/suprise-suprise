using System.Collections;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

namespace Enemy
{
    public class Enemy : MonoBehaviour
    {
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Die1 = Animator.StringToHash("Die");
        private static readonly int Attack1 = Animator.StringToHash("Attack");
        private static readonly int Hurt = Animator.StringToHash("Hurt");

        [Header("References")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;

        [Header("Stats")]
        [SerializeField] private int maxHealth = 50;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int damage = 10;
        [SerializeField] private float attackRange = 0.5f;
        [SerializeField] private float attackCooldown = 1f;

        [Header("Target")]
        [SerializeField] private Transform target; // The caravan or player
        [SerializeField] private bool targetPlayer = false; // If false, targets caravan

        [Header("Drops")]
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private int minCoins = 1;
        [SerializeField] private int maxCoins = 3;
        [SerializeField] private float coinDropForce = 3f;

        [Header("Health Bar")]
        [SerializeField] private Image healthBarFill;

        [Header("Effects")]
        [SerializeField] private GameObject deathEffectPrefab;
        [SerializeField] private Color damageFlashColor = Color.red;
        [SerializeField] private float damageFlashDuration = 0.1f;

        [Header("AI Behavior")]
        [SerializeField] private EnemyBehaviorType behaviorType = EnemyBehaviorType.Charger;
        [SerializeField] private float wanderRadius = 3f;
        [SerializeField] private float dodgeSpeed = 5f;
        [SerializeField] private float dodgeInterval = 2f;

        // Private variables
        private int currentHealth;
        private float lastAttackTime;
        private bool isDead;
        private Vector2 moveDirection;
        private Color originalColor;
        private Coroutine damageFlashCoroutine;

        // AI state
        private float nextDodgeTime;
        private Vector2 dodgeDirection;
        private bool isDodging ;
        private GameObject[] caravans;
        private GameObject[] players;

        public enum EnemyBehaviorType
        {
            Charger,    // Moves straight toward target
            Dodger,     // Moves erratically, dodges
            Flanker,    // Tries to circle around target
            Tank        // Slow, direct approach
        }

        void Awake()
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            caravans = GameObject.FindGameObjectsWithTag("Caravan");
            if (rb == null)
                rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (animator == null)
                animator = GetComponent<Animator>();
        }

        void Start()
        {
            currentHealth = maxHealth;
            originalColor = spriteRenderer.color;

            currentHealth = maxHealth;
            originalColor = spriteRenderer.color;


            AcquireClosestTarget();
            UpdateHealthBar();
        }
        

        void Update()
        {
            if (isDead)
                return;
        
            
            AcquireClosestTarget();
        
            if (target == null)
                return;
        
            CalculateMovement();
            UpdateAnimation();
            CheckAttackRange();
        }
        
        void AcquireClosestTarget()
        {
            Transform closest = null;
            float closestDistSqr = float.MaxValue;
            bool closestIsPlayer = false;
        
            Vector2 myPos = transform.position;
        
            // Check all players (handles multiple players if present)
            foreach (var p in players)
            {
                float d = (p.transform.position - (Vector3)myPos).sqrMagnitude;
                if (d < closestDistSqr)
                {
                    closest = p.transform;
                    closestDistSqr = d;
                    closestIsPlayer = true;
                }
            }
        
            // Check all caravans
            foreach (var c in caravans)
            {
                float d = (c.transform.position - (Vector3)myPos).sqrMagnitude;
                if (d < closestDistSqr)
                {
                    closest = c.transform;
                    closestDistSqr = d;
                    closestIsPlayer = false;
                }
            }
        
            // If nothing found, keep inspector-assigned target (if any)
            if (closest != null)
            {
                target = closest;
                targetPlayer = closestIsPlayer;
            }
        }
        

        void FixedUpdate()
        {
            if (isDead || target == null)
                return;

            Move();
        }

        void CalculateMovement()
        {
            Vector2 directionToTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;

            switch (behaviorType)
            {
                case EnemyBehaviorType.Charger:
                    // Straight line to target
                    moveDirection = directionToTarget;
                    break;

                case EnemyBehaviorType.Dodger:
                    // Erratic movement with dodges
                    if (Time.time >= nextDodgeTime)
                    {
                        StartDodge();
                    }

                    if (isDodging)
                    {
                        moveDirection = dodgeDirection;
                    }
                    else
                    {
                        moveDirection = directionToTarget;
                    }
                    break;

                case EnemyBehaviorType.Flanker:
                    // Circle around target
                    Vector2 perpendicular = new Vector2(-directionToTarget.y, directionToTarget.x);
                    float distanceToTarget = Vector2.Distance(transform.position, target.position);

                    if (distanceToTarget > attackRange * 2)
                    {
                        // Move toward target with perpendicular bias
                        moveDirection = (directionToTarget + perpendicular * 0.5f).normalized;
                    }
                    else
                    {
                        // Circle around when close
                        moveDirection = perpendicular;
                    }
                    break;

                case EnemyBehaviorType.Tank:
                    // Slow, direct approach
                    moveDirection = directionToTarget;
                    break;
            }
        }

        void Move()
        {
            float currentSpeed = isDodging ? dodgeSpeed : moveSpeed;
            rb.MovePosition(rb.position + moveDirection * (currentSpeed * Time.fixedDeltaTime));
        }

        void StartDodge()
        {
            isDodging = true;
            nextDodgeTime = Time.time + dodgeInterval;

            // Random perpendicular dodge
            Vector2 directionToTarget = ((Vector2)target.position - (Vector2)transform.position).normalized;
            Vector2 perpendicular = new Vector2(-directionToTarget.y, directionToTarget.x);
            dodgeDirection = (Random.value > 0.5f ? perpendicular : -perpendicular);

            StartCoroutine(EndDodge(0.3f));
        }

        IEnumerator EndDodge(float duration)
        {
            yield return new WaitForSeconds(duration);
            isDodging = false;
        }

        void CheckAttackRange()
        {
            float distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (distanceToTarget <= attackRange && Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }

        void Attack()
        {
            // Deal damage to target
            Health targetHealth = target.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }

            // Play attack animation if exists
            if (animator != null)
            {
                animator.SetTrigger(Attack1);
            }
        }

        public void TakeDamage(int damageAmount)
        {
            if (isDead)
                return;

            currentHealth -= damageAmount;
            currentHealth = Mathf.Max(currentHealth, 0);


            UpdateHealthBar();

            // Flash damage color
            if (damageFlashCoroutine != null)
                StopCoroutine(damageFlashCoroutine);
            damageFlashCoroutine = StartCoroutine(DamageFlash());

            // Play hurt animation
            if (animator != null)
            {
                animator.SetTrigger(Hurt);
            }

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        IEnumerator DamageFlash()
        {
            spriteRenderer.color = damageFlashColor;
            yield return new WaitForSeconds(damageFlashDuration);
            spriteRenderer.color = originalColor;
        }

        void UpdateHealthBar()
        {
            if (healthBarFill != null)
            {
                healthBarFill.fillAmount = (float)currentHealth / maxHealth;
            }
        }

        void UpdateAnimation()
        {
            if (animator == null)
                return;

            // Update movement animation parameters
            animator.SetFloat(Speed, rb.linearVelocity.magnitude);

            if (moveDirection.x != 0)
            {
                animator.SetFloat(Horizontal, moveDirection.x);
            }
            if (moveDirection.y != 0)
            {
                animator.SetFloat(Vertical, moveDirection.y);
            }
        }

        void Die()
        {
            if (isDead)
                return;

            isDead = true;

            // Spawn death effect
            if (deathEffectPrefab != null)
            {
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            }

            // Drop coins
            DropCoins();

            // Play death animation or destroy immediately
            if (animator != null)
            {
                animator.SetTrigger(Die1);
                // Destroy after animation (adjust timing as needed)
                Destroy(gameObject, 1f);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void DropCoins()
        {
            if (coinPrefab == null)
                return;

            int coinCount = Random.Range(minCoins, maxCoins + 1);

            for (int i = 0; i < coinCount; i++)
            {
                GameObject coin = Instantiate(coinPrefab, transform.position, Quaternion.identity);

                // Add random force to spread coins
                Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
                if (coinRb != null)
                {
                    Vector2 randomDirection = Random.insideUnitCircle.normalized;
                    coinRb.AddForce(randomDirection * coinDropForce, ForceMode2D.Impulse);
                }
            }
        }


        public int GetCurrentHealth() => currentHealth;
        public int GetMaxHealth() => maxHealth;
        public bool IsDead() => isDead;
        public Transform GetTarget() => target;

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        public void SetStats(int health, float speed, int dmg)
        {
            maxHealth = health;
            currentHealth = health;
            moveSpeed = speed;
            damage = dmg;
            UpdateHealthBar();
        }

        void OnDrawGizmosSelected()
        {
            // Visualize attack range
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            // Visualize movement direction
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, moveDirection * 2f);
        }
    }
}
