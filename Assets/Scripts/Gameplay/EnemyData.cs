using UnityEngine;

namespace Gameplay.Enemy
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Gameplay/Enemy/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [Header("Basic Info")]
        [SerializeField] private string enemyName = "Bandit";
        [SerializeField] private EnemyBehaviorType behaviorType = EnemyBehaviorType.Charger;
        
        [Header("Stats")]
        [SerializeField] private int maxHealth = 50;
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private int contactDamage = 10;
        [SerializeField] private float attackRange = 0.5f;
        [SerializeField] private float attackCooldown = 1f;
        
        [Header("AI Settings")]
        [SerializeField] private bool targetsPlayer = false; // If false, targets caravan
        [SerializeField] private float dodgeSpeed = 5f;
        [SerializeField] private float dodgeInterval = 2f;
        
        [Header("Rewards")]
        [SerializeField] private int minCoins = 1;
        [SerializeField] private int maxCoins = 3;
        [SerializeField] private float chronoShardDropChance = 0.05f; // 5% for normal, higher for elites
        
        // Getters
        public string EnemyName => enemyName;
        public EnemyBehaviorType BehaviorType => behaviorType;
        public int MaxHealth => maxHealth;
        public float MoveSpeed => moveSpeed;
        public int ContactDamage => contactDamage;
        public float AttackRange => attackRange;
        public float AttackCooldown => attackCooldown;
        public bool TargetsPlayer => targetsPlayer;
        public float DodgeSpeed => dodgeSpeed;
        public float DodgeInterval => dodgeInterval;
        public int MinCoins => minCoins;
        public int MaxCoins => maxCoins;
        public float ChronoShardDropChance => chronoShardDropChance;
    }
    
    public enum EnemyBehaviorType
    {
        Charger,    // Straight toward target
        Dodger,     // Erratic movement
        Flanker,    // Circle around
        Tank        // Slow, direct
    }
}