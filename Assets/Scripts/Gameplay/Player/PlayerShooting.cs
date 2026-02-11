using UnityEngine;
using System;
using Core.Services;
using Gameplay.Weapon;

namespace Gameplay.Player
{
    public class PlayerShooting
    {
        private readonly PlayerData playerData;
        private readonly Transform transform;
        private readonly IAudioService audioService;
        private readonly IBulletFactory bulletFactory;
        
        private float fireRateTimer;
        private WeaponData currentWeapon;
        private bool lastShotWasHeld; // For single-shot weapons
        
        public event Action<Vector2> OnWeaponFired;
        public event Action<WeaponData> OnWeaponEquipped;
        
        public WeaponData CurrentWeapon => currentWeapon;
        
        public PlayerShooting(PlayerData playerData, Transform transform, IAudioService audioService, IBulletFactory bulletFactory)
        {
            this.playerData = playerData;
            this.transform = transform;
            this.audioService = audioService;
            this.bulletFactory = bulletFactory;
        }
        
        public void Update(float deltaTime)
        {
            if (fireRateTimer > 0)
                fireRateTimer -= deltaTime;
        }
        
        public bool CanShoot(bool isHeld)
        {
            if (currentWeapon == null || fireRateTimer > 0)
                return false;
            
            // Single-shot weapons require release between shots
            if (currentWeapon.IsSingleShot && isHeld && lastShotWasHeld)
                return false;
            
            return true;
        }
        
        public void Shoot(Vector2 direction, bool isHeld)
        {
            if (!CanShoot(isHeld))
                return;
            
            fireRateTimer = currentWeapon.FireRate;
            lastShotWasHeld = isHeld;
            
            if (currentWeapon.IsShotgun)
            {
                ShootShotgun(direction);
            }
            else
            {
                ShootSingle(direction);
            }
            
            OnWeaponFired?.Invoke(direction);
        }
        
        void ShootSingle(Vector2 direction)
        {
            bulletFactory.CreateBullet(
                transform.position,
                direction,
                currentWeapon,
                true // isPlayerBullet
            );
        }
        
        void ShootShotgun(Vector2 direction)
        {
            float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float startAngle = baseAngle - (currentWeapon.SpreadAngle / 2f);
            float angleStep = currentWeapon.SpreadAngle / (currentWeapon.PelletCount - 1);
            
            for (int i = 0; i < currentWeapon.PelletCount; i++)
            {
                float angle = startAngle + (angleStep * i);
                Vector2 pelletDir = new Vector2(
                    Mathf.Cos(angle * Mathf.Deg2Rad),
                    Mathf.Sin(angle * Mathf.Deg2Rad)
                );
                
                bulletFactory.CreateBullet(
                    transform.position,
                    pelletDir,
                    currentWeapon,
                    true
                );
            }
        }
        
        public void EquipWeapon(WeaponData weapon)
        {
            currentWeapon = weapon;
            fireRateTimer = 0; // Allow immediate shooting
            lastShotWasHeld = false;
            OnWeaponEquipped?.Invoke(weapon);
        }
        
        public float GetFireRateProgress() => currentWeapon != null ? 
            Mathf.Clamp01(1f - (fireRateTimer / currentWeapon.FireRate)) : 1f;
    }
}