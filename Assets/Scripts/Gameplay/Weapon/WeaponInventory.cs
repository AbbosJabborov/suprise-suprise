using UnityEngine;
using System;

namespace Gameplay.Weapon
{
    public class WeaponInventory
    {
        private const int MaxSlots = 3;
        private readonly WeaponData[] weaponSlots;
        private int currentSlotIndex;
        
        public event Action<WeaponData, int> OnWeaponAdded;
        public event Action<int> OnWeaponSwitched;
        public event Action<WeaponData> OnWeaponDropped;
        
        public WeaponData CurrentWeapon => GetWeaponInSlot(currentSlotIndex);
        public int CurrentSlotIndex => currentSlotIndex;
        public bool IsFull => FindEmptySlot() < 0;
        
        public WeaponInventory()
        {
            weaponSlots = new WeaponData[MaxSlots];
        }
        
        public bool AddWeapon(WeaponData weapon)
        {
            if (weapon == null)
                return false;
            
            // Check if already owned
            for (int i = 0; i < MaxSlots; i++)
            {
                if (weaponSlots[i] == weapon)
                {
                    SwitchToSlot(i);
                    return false;
                }
            }
            
            int emptySlot = FindEmptySlot();
            
            if (emptySlot >= 0)
            {
                weaponSlots[emptySlot] = weapon;
                SwitchToSlot(emptySlot);
                OnWeaponAdded?.Invoke(weapon, emptySlot);
                return false;
            }
            else
            {
                // Replace current weapon
                WeaponData dropped = weaponSlots[currentSlotIndex];
                weaponSlots[currentSlotIndex] = weapon;
                
                OnWeaponDropped?.Invoke(dropped);
                OnWeaponAdded?.Invoke(weapon, currentSlotIndex);
                return true;
            }
        }
        
        public void SwitchToSlot(int slot)
        {
            if (slot < 0 || slot >= MaxSlots || weaponSlots[slot] == null)
                return;
            
            currentSlotIndex = slot;
            OnWeaponSwitched?.Invoke(currentSlotIndex);
        }
        
        public void SwitchNext()
        {
            for (int i = 1; i <= MaxSlots; i++)
            {
                int index = (currentSlotIndex + i) % MaxSlots;
                if (weaponSlots[index] != null)
                {
                    SwitchToSlot(index);
                    return;
                }
            }
        }
        
        public void SwitchPrevious()
        {
            for (int i = 1; i <= MaxSlots; i++)
            {
                int index = (currentSlotIndex - i + MaxSlots) % MaxSlots;
                if (weaponSlots[index] != null)
                {
                    SwitchToSlot(index);
                    return;
                }
            }
        }
        
        public WeaponData GetWeaponInSlot(int slot) =>
            slot >= 0 && slot < MaxSlots ? weaponSlots[slot] : null;
        
        public WeaponData[] GetAllWeapons() => weaponSlots;
        
        int FindEmptySlot()
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                if (weaponSlots[i] == null)
                    return i;
            }
            return -1;
        }
    }
}