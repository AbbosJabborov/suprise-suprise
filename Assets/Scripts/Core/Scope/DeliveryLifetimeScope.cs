using System.Collections.Generic;
using Gameplay;
using Gameplay.Caravan;
using Gameplay.Managers;
using Gameplay.Player;
using Gameplay.UI;
using Gameplay.Upgrades;
using Gameplay.Weapon;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Scope
{
    /// <summary>
    /// LifetimeScope for Delivery (gameplay) scenes
    /// Inherits services from RootLifetimeScope
    /// </summary>
    public class DeliveryLifetimeScope : LifetimeScope
    {
        [Header("Player")]
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private Transform playerSpawnPoint;
        
        [Header("Caravan")]
        [SerializeField] private CaravanController caravanController;
        [SerializeField] private CaravanHealth caravanHealth;
        [SerializeField] private CaravanManager caravanManager;
        
        [Header("Enemy System")]
        [SerializeField] private EnemyFactory.EnemyPrefabMap[] enemyPrefabMappings;
        [SerializeField] private Transform enemyPoolContainer;
        
        [Header("Upgrades")]
        [SerializeField] private List<UpgradeCardData> upgradePool;
        
        [Header("Bullet System")]
        [SerializeField] private GameObject bulletPrefab;
        
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("[DeliveryLifetimeScope] Configuring gameplay scene...");
            
            // ==================== PLAYER ====================
            
            if (playerPrefab != null && playerSpawnPoint != null)
            {
                var player = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
                builder.RegisterComponent(player);
                Debug.Log("[DeliveryLifetimeScope] Player spawned and registered");
            }
            else
            {
                Debug.LogWarning("[DeliveryLifetimeScope] Player prefab or spawn point not assigned!");
            }
            
            // ==================== CARAVAN ====================
            
            // Register caravan components (should exist in scene)
            if (caravanController != null)
            {
                builder.RegisterComponent(caravanController);
                Debug.Log("[DeliveryLifetimeScope] CaravanController registered");
            }
            
            if (caravanHealth != null)
            {
                builder.RegisterComponent(caravanHealth);
                Debug.Log("[DeliveryLifetimeScope] CaravanHealth registered");
            }
            
            if (caravanManager != null)
            {
                builder.RegisterComponent(caravanManager);
                Debug.Log("[DeliveryLifetimeScope] CaravanManager registered");
            }
            
            // ==================== FACTORIES ====================
            
            // Bullet Factory
            builder.Register<IBulletFactory>(resolver =>
            {
                Transform bulletContainer = new GameObject("BulletPool").transform;
                return new BulletFactory(resolver, bulletPrefab, bulletContainer);
            }, Lifetime.Scoped);
            Debug.Log("[DeliveryLifetimeScope] Bullet factory registered");
            
            // Enemy Factory
            builder.Register<IEnemyFactory>(resolver =>
            {
                // Create pool container if not assigned
                Transform poolContainer = enemyPoolContainer;
                if (poolContainer == null)
                {
                    poolContainer = new GameObject("EnemyPool").transform;
                }
                
                return new EnemyFactory(enemyPrefabMappings, poolContainer);
            }, Lifetime.Scoped);
            Debug.Log("[DeliveryLifetimeScope] Enemy factory registered");
            
            // ==================== MANAGERS ====================
            
            // Upgrade Manager
            builder.Register<UpgradeManager>(resolver => 
                new UpgradeManager(upgradePool), Lifetime.Scoped);
            Debug.Log("[DeliveryLifetimeScope] Upgrade manager registered");
            
            // Register managers from hierarchy (these should exist in scene)
            builder.RegisterComponentInHierarchy<MailboxManager>();
            builder.RegisterComponentInHierarchy<EnemySpawnManager>();
            Debug.Log("[DeliveryLifetimeScope] Mailbox and Enemy spawn managers registered from hierarchy");
            
            // ==================== UI ====================
            
            // Register UI components from hierarchy
            builder.RegisterComponentInHierarchy<UpgradePopupUI>();
            builder.RegisterComponentInHierarchy<GameplayHUD>();
            Debug.Log("[DeliveryLifetimeScope] UI components registered from hierarchy");
            
            Debug.Log("[DeliveryLifetimeScope] Configuration complete");
        }
    }
}