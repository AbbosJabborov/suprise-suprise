using UnityEngine;

namespace Gameplay.Player
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Gameplay/Player/Player Data")]
    public class PlayerData : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] private float baseMovementSpeed = 5f;
        [SerializeField] private float sprintSpeedMultiplier = 1.5f;
        
        [Header("Shooting")]
        [SerializeField] private Transform firePoint;
        
        [Header("Dodge Roll")]
        [SerializeField] private float dodgeDistance = 3f;
        [SerializeField] private float dodgeDuration = 0.3f;
        [SerializeField] private float dodgeCooldown = 1f;
        [SerializeField] private float iFramesDuration = 0.2f; // Invincibility frames
        
        [Header("Melee Slash")]
        [SerializeField] private float slashRange = 2f;
        [SerializeField] private float slashDuration = 0.3f;
        [SerializeField] private float slashCooldown = 0.5f;
        [SerializeField] private int slashDamage = 10;
        [SerializeField] private float deflectionRadius = 2f;
        
        [Header("Health")]
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private float healthRegenRate = 0f; // HP per second
        
        [Header("Visual")]
        [SerializeField] private float rotationSpeed = 720f; // Degrees per second
        
        // Public getters
        public float BaseMovementSpeed => baseMovementSpeed;
        public float SprintSpeedMultiplier => sprintSpeedMultiplier;
        public Transform FirePoint => firePoint;
        public float DodgeDistance => dodgeDistance;
        public float DodgeDuration => dodgeDuration;
        public float DodgeCooldown => dodgeCooldown;
        public float IFramesDuration => iFramesDuration;
        public float SlashRange => slashRange;
        public float SlashDuration => slashDuration;
        public float SlashCooldown => slashCooldown;
        public int SlashDamage => slashDamage;
        public float DeflectionRadius => deflectionRadius;
        public int MaxHealth => maxHealth;
        public float HealthRegenRate => healthRegenRate;
        public float RotationSpeed => rotationSpeed;
    }
}