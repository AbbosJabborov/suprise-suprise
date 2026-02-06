using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Weapon
{
    public class WeaponInventoryUI : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private WeaponInventory weaponInventory;
    
        [Header("Slot UI Elements")]
        [SerializeField] private WeaponSlotUI[] slotUIs; // 3 slots
    
        [Header("Settings")]
        [SerializeField] private Color activeSlotColor = Color.white;
        [SerializeField] private Color inactiveSlotColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField] private Color emptySlotColor = new Color(0.3f, 0.3f, 0.3f, 0.3f);
    
        void Start()
        {
            if (weaponInventory == null)
            {
                weaponInventory = FindFirstObjectByType<WeaponInventory>();
            }
        
            // Subscribe to inventory events
            if (weaponInventory != null)
            {
                weaponInventory.OnWeaponAdded.AddListener(OnWeaponAdded);
                weaponInventory.OnWeaponSwitched.AddListener(OnWeaponSwitched);
            }
        
            // Initialize UI
            UpdateAllSlots();
        }
    
        void OnWeaponAdded(WeaponData weapon, int slotIndex)
        {
            UpdateAllSlots();
        }
    
        void OnWeaponSwitched(int newSlotIndex)
        {
            UpdateAllSlots();
        }
    
        void UpdateAllSlots()
        {
            if (weaponInventory == null || slotUIs == null)
                return;
        
            var weapons = weaponInventory.GetAllWeapons();
            int currentSlot = weaponInventory.GetCurrentSlotIndex();
        
            for (int i = 0; i < slotUIs.Length; i++)
            {
                if (slotUIs[i] != null)
                {
                    WeaponData weapon = i < weapons.Length ? weapons[i] : null;
                    bool isActive = i == currentSlot;
                
                    slotUIs[i].UpdateSlot(weapon, isActive, i, activeSlotColor, inactiveSlotColor, emptySlotColor);
                }
            }
        }
    }

    [System.Serializable]
    public class WeaponSlotUI
    {
        [Header("UI Components")]
        public Image backgroundImage;
        public Image weaponIcon;
        public TextMeshProUGUI weaponNameText;
        public TextMeshProUGUI slotNumberText;
        public GameObject highlightEffect;
    
        public void UpdateSlot(WeaponData weapon, bool isActive, int slotNumber, Color activeColor, Color inactiveColor, Color emptyColor)
        {
            // Update slot number
            if (slotNumberText != null)
            {
                slotNumberText.text = (slotNumber + 1).ToString();
            }
        
            if (weapon == null)
            {
                // Empty slot
                if (weaponIcon != null)
                {
                    weaponIcon.enabled = false;
                }
            
                if (weaponNameText != null)
                {
                    weaponNameText.text = "Empty";
                    weaponNameText.color = new Color(1, 1, 1, 0.3f);
                }
            
                if (backgroundImage != null)
                {
                    backgroundImage.color = emptyColor;
                }
            
                if (highlightEffect != null)
                {
                    highlightEffect.SetActive(false);
                }
            }
            else
            {
                // Has weapon
                if (weaponIcon != null)
                {
                    weaponIcon.enabled = true;
                    weaponIcon.sprite = weapon.WeaponSprite;
                    weaponIcon.color = isActive ? Color.white : new Color(1, 1, 1, 0.6f);
                }
            
                if (weaponNameText != null)
                {
                    weaponNameText.text = weapon.WeaponName;
                    weaponNameText.color = Color.white;
                }
            
                if (backgroundImage != null)
                {
                    backgroundImage.color = isActive ? activeColor : inactiveColor;
                }
            
                if (highlightEffect != null)
                {
                    highlightEffect.SetActive(isActive);
                }
            }
        }
    }
}