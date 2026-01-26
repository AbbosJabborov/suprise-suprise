using Gameplay;
using Gameplay.Weapon;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerTopDownShooting : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerTopDownAiming aimingScript;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private SpriteRenderer weaponSpriteRenderer; // Visual weapon sprite
        
        [Header("Current Weapon")]
        [SerializeField] private WeaponData currentWeapon;
        
        [Header("Visual/Audio")]
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private AudioClip shootSound;
        [SerializeField] private AudioSource audioSource;
        
        [Header("Bullet Pooling")]
        [SerializeField] private int poolSize = 100;
        
        private float nextFireTime;
        private bool isShooting;
        private bool wasShootingLastFrame;
        private GameObject[] bulletPool;
        private int currentBulletIndex;
        
        void Start()
        {
            if (aimingScript == null)
                aimingScript = GetComponent<PlayerTopDownAiming>();
            
            InitializeBulletPool();
            
            // Apply weapon visual
            if (currentWeapon != null && weaponSpriteRenderer != null && currentWeapon.WeaponSprite != null)
            {
                weaponSpriteRenderer.sprite = currentWeapon.WeaponSprite;
            }
        }
        
        void Update()
        {
            HandleShootInput();
        }
        
        void HandleShootInput()
        {
            if (currentWeapon == null)
                return;
            
            // Get shoot input
            isShooting = playerInput.actions["Shoot"].ReadValue<float>() > 0.1f;
            
            // Check if can shoot
            bool canShoot = Time.time >= nextFireTime;
            
            // Handle single shot mode
            if (currentWeapon.IsSingleShot)
            {
                // Only shoot on button press (not hold)
                bool justPressed = isShooting && !wasShootingLastFrame;
                
                if (justPressed && canShoot)
                {
                    Shoot();
                    nextFireTime = Time.time + currentWeapon.FireRate;
                }
            }
            else
            {
                // Automatic fire - shoot while holding
                if (isShooting && canShoot)
                {
                    Shoot();
                    nextFireTime = Time.time + currentWeapon.FireRate;
                }
            }
            
            wasShootingLastFrame = isShooting;
        }
        
        void Shoot()
        {
            Vector2 aimDirection = aimingScript.GetAimDirection();
            
            if (aimDirection.sqrMagnitude < 0.01f)
                return;
            
            if (currentWeapon.IsShotgun)
            {
                ShootShotgun(aimDirection);
            }
            else
            {
                ShootSingle(aimDirection);
            }
            
            // Effects
            PlayMuzzleFlash();
            PlayShootSound();
        }
        
        void ShootSingle(Vector2 aimDirection)
        {
            GameObject bullet = GetPooledBullet();
            
            if (bullet != null)
            {
                bullet.transform.position = firePoint != null ? firePoint.position : transform.position;
                bullet.transform.rotation = Quaternion.Euler(0, 0, aimingScript.GetAimAngle());
                bullet.SetActive(true);
                
                // Set bullet color if available
                SpriteRenderer bulletSprite = bullet.GetComponent<SpriteRenderer>();
                if (bulletSprite != null)
                {
                    bulletSprite.color = currentWeapon.BulletColor;
                }
                
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Initialize(aimDirection, currentWeapon.BulletSpeed, currentWeapon.Damage, currentWeapon.BulletLifetime);
                }
            }
        }
        
        void ShootShotgun(Vector2 baseDirection)
        {
            int pellets = currentWeapon.PelletCount;
            float spreadAngle = currentWeapon.SpreadAngle;
            
            // Calculate angle step between pellets
            float angleStep = spreadAngle / Mathf.Max(1, pellets - 1);
            float startAngle = -spreadAngle / 2f;
            
            for (int i = 0; i < pellets; i++)
            {
                GameObject bullet = GetPooledBullet();
                
                if (bullet != null)
                {
                    // Calculate spread direction
                    float currentAngle = startAngle + (angleStep * i);
                    Vector2 spreadDirection = RotateVector(baseDirection, currentAngle);
                    
                    // Position and rotate bullet
                    bullet.transform.position = firePoint != null ? firePoint.position : transform.position;
                    float bulletAngle = Mathf.Atan2(spreadDirection.y, spreadDirection.x) * Mathf.Rad2Deg;
                    bullet.transform.rotation = Quaternion.Euler(0, 0, bulletAngle);
                    bullet.SetActive(true);
                    
                    // Set bullet color
                    SpriteRenderer bulletSprite = bullet.GetComponent<SpriteRenderer>();
                    if (bulletSprite != null)
                    {
                        bulletSprite.color = currentWeapon.BulletColor;
                    }
                    
                    // Initialize bullet
                    Bullet bulletScript = bullet.GetComponent<Bullet>();
                    if (bulletScript != null)
                    {
                        bulletScript.Initialize(spreadDirection, currentWeapon.BulletSpeed, currentWeapon.Damage, currentWeapon.BulletLifetime);
                    }
                }
            }
        }
        
        Vector2 RotateVector(Vector2 vector, float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            
            return new Vector2(
                vector.x * cos - vector.y * sin,
                vector.x * sin + vector.y * cos
            );
        }
        
        void InitializeBulletPool()
        {
            bulletPool = new GameObject[poolSize];
            
            for (int i = 0; i < poolSize; i++)
            {
                bulletPool[i] = Instantiate(bulletPrefab);
                bulletPool[i].SetActive(false);
                bulletPool[i].transform.parent = transform;
            }
        }
        
        GameObject GetPooledBullet()
        {
            for (int i = 0; i < poolSize; i++)
            {
                currentBulletIndex = (currentBulletIndex + 1) % poolSize;
                
                if (!bulletPool[currentBulletIndex].activeInHierarchy)
                {
                    return bulletPool[currentBulletIndex];
                }
            }
            
            currentBulletIndex = (currentBulletIndex + 1) % poolSize;
            return bulletPool[currentBulletIndex];
        }
        
        void PlayMuzzleFlash()
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }
        }
        
        void PlayShootSound()
        {
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }
        
        public void SwitchWeapon(WeaponData newWeapon)
        {
            if (newWeapon == null)
                return;
            
            currentWeapon = newWeapon;
            
            if (weaponSpriteRenderer != null && currentWeapon.WeaponSprite != null)
            {
                weaponSpriteRenderer.sprite = currentWeapon.WeaponSprite;
            }
            
            PlayerTopDownMovement movement = GetComponent<PlayerTopDownMovement>();
            if (movement != null)
            {
                movement.SetSpeedMultiplier(currentWeapon.MovementSpeedMultiplier);
            }
            
            nextFireTime = 0f;
        }
        
        public WeaponData GetCurrentWeapon()
        {
            return currentWeapon;
        }
        
        public float GetFireCooldownPercent()
        {
            if (currentWeapon == null)
                return 1f;
            
            float timeSinceLastShot = Time.time - (nextFireTime - currentWeapon.FireRate);
            return Mathf.Clamp01(timeSinceLastShot / currentWeapon.FireRate);
        }
    }
}