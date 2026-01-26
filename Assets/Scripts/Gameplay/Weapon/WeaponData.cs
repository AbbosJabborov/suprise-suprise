using UnityEngine;

namespace Gameplay.Weapon
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Gameplay/Weapon/Weapon Data", order = 1)]
    public class WeaponData : ScriptableObject
    {
        [Header("Weapon Settings")]
        [SerializeField] private string weaponName = "Pistol";
        [SerializeField] private float fireRate = 0.2f;
        [SerializeField] private float bulletSpeed = 20f;
        [SerializeField] private int damage = 10;
        [SerializeField] private float bulletLifetime = 3f;
        
        [Header("Spread Settings")]
        [SerializeField] private bool isShotgun = false;
        [SerializeField] private int pelletCount = 5; // Only used if isShotgun
        [SerializeField] private float spreadAngle = 15f; // Total cone angle
        
        [Header("Fire Mode")]
        [SerializeField] private bool isSingleShot = false; // If true, must release to shoot again
        
        [Header("Player Movement Modifier")]
        [SerializeField] private float movementSpeedMultiplier = 1f; // 1 = normal, 0.5 = half speed when using this weapon
        
        [Header("Visual")]
        [SerializeField] private Sprite weaponSprite;
        [SerializeField] private Color bulletColor = Color.yellow;
        
        // Public getters
        public string WeaponName => weaponName;
        public float FireRate => fireRate;
        public float BulletSpeed => bulletSpeed;
        public int Damage => damage;
        public float BulletLifetime => bulletLifetime;
        public bool IsShotgun => isShotgun;
        public int PelletCount => pelletCount;
        public float SpreadAngle => spreadAngle;
        public bool IsSingleShot => isSingleShot;
        public float MovementSpeedMultiplier => movementSpeedMultiplier;
        public Sprite WeaponSprite => weaponSprite;
        public Color BulletColor => bulletColor;
    }
}