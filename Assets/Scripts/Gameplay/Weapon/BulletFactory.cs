using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Gameplay.Weapon
{
    public class BulletFactory : IBulletFactory
    {
        private readonly IObjectResolver container;
        private readonly GameObject bulletPrefab;
        private readonly Transform bulletContainer;
        private readonly int poolSize = 100;
        
        private GameObject[] bulletPool;
        private int currentIndex;
        
        public BulletFactory(IObjectResolver container, GameObject bulletPrefab, Transform bulletContainer = null)
        {
            this.container = container;
            this.bulletPrefab = bulletPrefab;
            this.bulletContainer = bulletContainer;
            
            InitializePool();
        }
        
        void InitializePool()
        {
            bulletPool = new GameObject[poolSize];
            
            for (int i = 0; i < poolSize; i++)
            {
                bulletPool[i] = Object.Instantiate(bulletPrefab);
                bulletPool[i].SetActive(false);
                
                if (bulletContainer != null)
                    bulletPool[i].transform.SetParent(bulletContainer);
            }
            
            Debug.Log($"[BulletFactory] Initialized pool with {poolSize} bullets");
        }
        
        public void CreateBullet(Vector3 position, Vector2 direction, WeaponData weapon, bool isPlayerBullet)
        {
            GameObject bullet = GetPooledBullet();
            
            if (bullet == null)
                return;
            
            bullet.transform.position = position;
            bullet.transform.rotation = Quaternion.Euler(0, 0, 
                Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            
            // Set layer
            bullet.layer = LayerMask.NameToLayer(isPlayerBullet ? "PlayerBullet" : "EnemyBullet");
            
            // Initialize bullet
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Initialize(direction, weapon, isPlayerBullet);
            }
            
            bullet.SetActive(true);
        }
        
        GameObject GetPooledBullet()
        {
            // Find inactive bullet
            for (int i = 0; i < poolSize; i++)
            {
                currentIndex = (currentIndex + 1) % poolSize;
                
                if (!bulletPool[currentIndex].activeInHierarchy)
                {
                    return bulletPool[currentIndex];
                }
            }
            
            // Reuse oldest if all active
            currentIndex = (currentIndex + 1) % poolSize;
            return bulletPool[currentIndex];
        }
    }
}