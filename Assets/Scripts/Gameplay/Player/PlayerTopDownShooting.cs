using System;
using System.Reflection;
using Gameplay.Weapon;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Player
{
    public class PlayerTopDownShooting : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerTopDownAiming aimingScript;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform firePoint; // Where bullets spawn
        
        [Header("Weapon Settings")]
        [SerializeField] private float fireRate = 0.2f; // Time between shots
        [SerializeField] private float bulletSpeed = 20f;
        [SerializeField] private int damage = 10;
        [SerializeField] private float bulletLifetime = 3f;

        private WeaponData currentWeaponData;
        
        
        [Header("Visual/Audio")]
        [SerializeField] private ParticleSystem muzzleFlash;
        [SerializeField] private AudioClip shootSound;
        [SerializeField] private AudioSource audioSource;
        
        [Header("Bullet Pooling")]
        [SerializeField] private int poolSize = 50;
        
        private float nextFireTime;
        private bool isShooting;
        private GameObject[] bulletPool;
        private int currentBulletIndex;
        
        void Start()
        {
            if (aimingScript == null)
                aimingScript = GetComponent<PlayerTopDownAiming>();
            
            // Initialize bullet pool
            InitializeBulletPool();
        }
        
        void Update()
        {
            HandleShootInput();
        }
        
        void HandleShootInput()
        {
            // Get shoot input (left click or right trigger)
            isShooting = playerInput.actions["Shoot"].ReadValue<float>() > 0.1f;
            
            if (isShooting && Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
        
        void Shoot()
        {
            Vector2 aimDirection = aimingScript.GetAimDirection();
            
            if (aimDirection.sqrMagnitude < 0.01f)
                return; 
            
            // Get bullet from pool
            GameObject bullet = GetPooledBullet();
            
            if (bullet != null)
            {
                // Position bullet at fire point
                bullet.transform.position = firePoint != null ? firePoint.position : transform.position;
                bullet.transform.rotation = Quaternion.Euler(0, 0, aimingScript.GetAimAngle());
                bullet.SetActive(true);
                
                // Initialize bullet
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Initialize(aimDirection, bulletSpeed, damage, bulletLifetime);
                }
                
                // Visual/Audio effects
                PlayMuzzleFlash();
                PlayShootSound();
            }
        }
        
        void InitializeBulletPool()
        {
            bulletPool = new GameObject[poolSize];
            
            for (int i = 0; i < poolSize; i++)
            {
                bulletPool[i] = Instantiate(bulletPrefab);
                bulletPool[i].SetActive(false);
                bulletPool[i].transform.parent = transform; // Optional: organize in hierarchy
            }
        }
        
        GameObject GetPooledBullet()
        {
            // Simple round-robin pooling
            for (int i = 0; i < poolSize; i++)
            {
                currentBulletIndex = (currentBulletIndex + 1) % poolSize;
                
                if (!bulletPool[currentBulletIndex].activeInHierarchy)
                {
                    return bulletPool[currentBulletIndex];
                }
            }
            
            // If all bullets are active, reuse the oldest one
            currentBulletIndex = (currentBulletIndex + 1) % poolSize;
            return bulletPool[currentBulletIndex];
        }
        
        void PlayMuzzleFlash()
        {
            if (muzzleFlash)
            {
                muzzleFlash.Play();
            }
        }
        
        void PlayShootSound()
        {
            if (audioSource && shootSound)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }

        private void SetWeaponName(string newName)
        {
            // This method can be expanded to update UI or other components
            Debug.Log("Weapon name set to: " + newName);
        }
        
        public void SwitchWeapon(WeaponData newWeaponData)
        {
            currentWeaponData = newWeaponData;
            
            if (currentWeaponData != null)
            {
                SetWeaponName(currentWeaponData.WeaponName);
                SetFireRate(currentWeaponData.FireRate);
                SetBulletSpeed(currentWeaponData.BulletSpeed);
                SetDamage(currentWeaponData.Damage);
            }
            else
            {
                // Reset to default values if no weapon
                SetWeaponName("Unarmed");
                SetFireRate(0.5f);
                SetBulletSpeed(10f);
                SetDamage(1);
            }
        }
        
        public void SetFireRate(float newFireRate)
        {
            fireRate = newFireRate;
        }
        
        public void SetBulletSpeed(float newSpeed)
        {
            bulletSpeed = newSpeed;
        }
        
        public void SetDamage(int newDamage)
        {
            damage = newDamage;
        }
    }
}