using Gameplay.Building;
using Gameplay.Managers;
using UnityEngine;

namespace Extensions
{
    public class BuildingTester : MonoBehaviour
    {
        [SerializeField] private CaravanManager caravan;
        [SerializeField] private BuildingData firstBuilding;
        [SerializeField] private BuildingData secondBuilding;
        [SerializeField] private BuildingData thirdBuilding;
        [SerializeField] private BuildingData fourthBuilding;

    
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                caravan.PlaceBuildingInSlot(0, firstBuilding);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                caravan.PlaceBuildingInSlot(1, secondBuilding);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                caravan.PlaceBuildingInSlot(2, thirdBuilding);
            }
            
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                caravan.PlaceBuildingInSlot(3, fourthBuilding);
            }
            
            if (Input.GetKeyDown(KeyCode.Y))
            {
                caravan.UpgradeBuildingInSlot(0);
            }
            
            if (Input.GetKeyDown(KeyCode.U))
            {
                caravan.UpgradeBuildingInSlot(1);
            }
            
            if (Input.GetKeyDown(KeyCode.I))
            {
                caravan.UpgradeBuildingInSlot(2);
            }
            
            if (Input.GetKeyDown(KeyCode.O))
            {
                caravan.UpgradeBuildingInSlot(3);
            }
            
        }
    }
}