using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Gameplay.UI
{
    public class WeaponInventoryUI : MonoBehaviour
    {
        [Header("Slot UI")]
        [SerializeField] private WeaponSlotUI[] slots = new WeaponSlotUI[3];
        
        [Header("Colors")]
        [SerializeField] private Color activeColor = Color.white;
        [SerializeField] private Color inactiveColor = new Color(0.7f, 0.7f, 0.7f);
        [SerializeField] private Color emptyColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        
        private Weapon.WeaponInventory inventory;
        
        public void Initialize(Weapon.WeaponInventory weaponInventory)
        {
            inventory = weaponInventory;
            
            // Subscribe to events
            inventory.OnWeaponAdded += OnInventoryChanged;
            inventory.OnWeaponSwitched += OnInventoryChanged;
            
            UpdateAllSlots();
        }
        
        void OnInventoryChanged(int slot) => UpdateAllSlots();
        void OnInventoryChanged(Weapon.WeaponData weapon, int slot) => UpdateAllSlots();
        
        void UpdateAllSlots()
        {
            if (inventory == null) return;
            
            var weapons = inventory.GetAllWeapons();
            int currentSlot = inventory.CurrentSlotIndex;
            
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                {
                    Weapon.WeaponData weapon = weapons[i];
                    bool isActive = i == currentSlot;
                    slots[i].UpdateSlot(weapon, isActive, i + 1, activeColor, inactiveColor, emptyColor);
                }
            }
        }
        
        void OnDestroy()
        {
            if (inventory != null)
            {
                inventory.OnWeaponAdded -= OnInventoryChanged;
                inventory.OnWeaponSwitched -= OnInventoryChanged;
            }
        }
    }
    
    [Serializable]
    public class WeaponSlotUI
    {
        [Header("Components")]
        public Image background;
        public Image weaponIcon;
        public TextMeshProUGUI weaponName;
        public TextMeshProUGUI slotNumber;
        public GameObject highlight;
        
        public void UpdateSlot(Weapon.WeaponData weapon, bool isActive, int number, Color activeCol, Color inactiveCol, Color emptyCol)
        {
            // Slot number
            if (slotNumber != null)
                slotNumber.text = number.ToString();
            
            if (weapon == null)
            {
                // Empty slot
                if (weaponIcon != null) weaponIcon.enabled = false;
                if (weaponName != null)
                {
                    weaponName.text = "Empty";
                    weaponName.color = new Color(1, 1, 1, 0.3f);
                }
                if (background != null) background.color = emptyCol;
                if (highlight != null) highlight.SetActive(false);
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
                
                if (weaponName != null)
                {
                    weaponName.text = weapon.WeaponName;
                    weaponName.color = Color.white;
                }
                
                if (background != null)
                    background.color = isActive ? activeCol : inactiveCol;
                
                if (highlight != null)
                    highlight.SetActive(isActive);
            }
        }
    }
}