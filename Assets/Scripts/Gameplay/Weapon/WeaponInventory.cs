using Gameplay.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Gameplay.Weapon
{
    public class WeaponInventory : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int maxWeaponSlots = 3;
        [SerializeField] private int currentSlotIndex;
        
        [Header("Starting Weapon")]
        [SerializeField] private WeaponData startingWeapon;
        
        [Header("References")]
        [SerializeField] private PlayerTopDownShooting shootingScript;
        [SerializeField] private PlayerInput playerInput;
        
        [Header("Events")]
        public UnityEvent<WeaponData, int> OnWeaponAdded; // Weapon, slot index
        public UnityEvent<int> OnWeaponSwitched; // New slot index
        public UnityEvent<WeaponData> OnWeaponDropped;
        
        private WeaponData[] weaponSlots;
        private bool wasScrollingLastFrame;
        
        void Awake()
        {
            weaponSlots = new WeaponData[maxWeaponSlots];
            
            if (shootingScript == null)
                shootingScript = GetComponent<PlayerTopDownShooting>();
        }
        
        void Start()
        {
            // Add starting weapon
            if (startingWeapon != null)
            {
                AddWeapon(startingWeapon);
            }
        }
        
        void Update()
        {
            HandleWeaponSwitching();
        }
        
        void HandleWeaponSwitching()
        {
            // Number keys 1-3 for direct switching
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SwitchToSlot(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SwitchToSlot(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                SwitchToSlot(2);
            }
            
            // Mouse wheel scrolling
            if (playerInput != null)
            {
                Vector2 scroll = playerInput.actions["SwitchWeapon"].ReadValue<Vector2>();
                
                bool isScrolling = scroll.y != 0;
                
                if (isScrolling && !wasScrollingLastFrame)
                {
                    if (scroll.y > 0)
                    {
                        SwitchToNextWeapon();
                    }
                    else if (scroll.y < 0)
                    {
                        SwitchToPreviousWeapon();
                    }
                }
                
                wasScrollingLastFrame = isScrolling;
            }
            
            // Gamepad bumpers (RB/LB)
            if (Input.GetKeyDown(KeyCode.Q)) // Or map to LB
            {
                SwitchToPreviousWeapon();
            }
            else if (Input.GetKeyDown(KeyCode.E)) // Or map to RB (conflict with interact, can change)
            {
                SwitchToNextWeapon();
            }
        }
        
        public bool AddWeapon(WeaponData weapon)
        {
            if (weapon == null)
                return false;
            
            // Check if weapon already exists in inventory
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (weaponSlots[i] == weapon)
                {
                    // Already have this weapon, just switch to it
                    SwitchToSlot(i);
                    return false;
                }
            }
            
            // Find first empty slot
            int emptySlot = FindEmptySlot();
            
            if (emptySlot >= 0)
            {
                // Add to empty slot
                weaponSlots[emptySlot] = weapon;
                SwitchToSlot(emptySlot);
                OnWeaponAdded?.Invoke(weapon, emptySlot);
                return true;
            }
            else
            {
                // Inventory full - drop current weapon and replace it
                WeaponData droppedWeapon = weaponSlots[currentSlotIndex];
                weaponSlots[currentSlotIndex] = weapon;
                
                OnWeaponDropped?.Invoke(droppedWeapon);
                OnWeaponAdded?.Invoke(weapon, currentSlotIndex);
                
                // Equip new weapon
                EquipWeapon(weapon);
                
                return true; // Return true because we need to drop the weapon
            }
        }
        
        public WeaponData RemoveCurrentWeapon()
        {
            WeaponData removedWeapon = weaponSlots[currentSlotIndex];
            weaponSlots[currentSlotIndex] = null;
            
            // Switch to next available weapon
            SwitchToNextAvailableWeapon();
            
            return removedWeapon;
        }
        
        public void SwitchToSlot(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxWeaponSlots)
                return;
            
            if (weaponSlots[slotIndex] == null)
                return; // Can't switch to empty slot
            
            currentSlotIndex = slotIndex;
            EquipWeapon(weaponSlots[currentSlotIndex]);
            OnWeaponSwitched?.Invoke(currentSlotIndex);
        }
        
        public void SwitchToNextWeapon()
        {
            int startIndex = currentSlotIndex;
            int attempts = 0;
            
            do
            {
                currentSlotIndex = (currentSlotIndex + 1) % maxWeaponSlots;
                attempts++;
                
                if (weaponSlots[currentSlotIndex] != null)
                {
                    EquipWeapon(weaponSlots[currentSlotIndex]);
                    OnWeaponSwitched?.Invoke(currentSlotIndex);
                    return;
                }
            }
            while (currentSlotIndex != startIndex && attempts < maxWeaponSlots);
        }
        
        public void SwitchToPreviousWeapon()
        {
            int startIndex = currentSlotIndex;
            int attempts = 0;
            
            do
            {
                currentSlotIndex--;
                if (currentSlotIndex < 0)
                    currentSlotIndex = maxWeaponSlots - 1;
                
                attempts++;
                
                if (weaponSlots[currentSlotIndex] != null)
                {
                    EquipWeapon(weaponSlots[currentSlotIndex]);
                    OnWeaponSwitched?.Invoke(currentSlotIndex);
                    return;
                }
            }
            while (currentSlotIndex != startIndex && attempts < maxWeaponSlots);
        }
        
        void SwitchToNextAvailableWeapon()
        {
            // Try to find any weapon in inventory
            for (int i = 0; i < maxWeaponSlots; i++)
            {
                if (weaponSlots[i] != null)
                {
                    SwitchToSlot(i);
                    return;
                }
            }
            
            // No weapons left
            if (shootingScript != null)
            {
                shootingScript.SwitchWeapon(null);
            }
        }
        
        void EquipWeapon(WeaponData weapon)
        {
            if (shootingScript != null)
            {
                shootingScript.SwitchWeapon(weapon);
            }
        }
        
        int FindEmptySlot()
        {
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (weaponSlots[i] == null)
                    return i;
            }
            return -1; // No empty slots
        }
        
        // Public getters
        public WeaponData GetCurrentWeapon()
        {
            if (currentSlotIndex >= 0 && currentSlotIndex < weaponSlots.Length)
                return weaponSlots[currentSlotIndex];
            return null;
        }
        
        public WeaponData GetWeaponInSlot(int slotIndex)
        {
            if (slotIndex >= 0 && slotIndex < weaponSlots.Length)
                return weaponSlots[slotIndex];
            return null;
        }
        
        public int GetCurrentSlotIndex()
        {
            return currentSlotIndex;
        }
        
        public int GetWeaponCount()
        {
            int count = 0;
            foreach (var weapon in weaponSlots)
            {
                if (weapon != null)
                    count++;
            }
            return count;
        }
        
        public bool IsFull()
        {
            return FindEmptySlot() < 0;
        }
        
        public WeaponData[] GetAllWeapons()
        {
            return weaponSlots;
        }
    }
}