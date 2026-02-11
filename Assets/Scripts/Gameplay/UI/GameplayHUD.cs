using UnityEngine;
using VContainer;

namespace Gameplay.UI
{
    public class GameplayHUD : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private HealthUI healthUI;
        [SerializeField] private WeaponInventoryUI inventoryUI;
        
        private Player.PlayerController player;
        
        [Inject]
        public void Construct(Player.PlayerController playerController)
        {
            player = playerController;
        }
        
        void Start()
        {
            if (player != null && inventoryUI != null)
            {
                inventoryUI.Initialize(player.Inventory);
            }
        }
    }
}