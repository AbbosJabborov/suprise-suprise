using Gameplay.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Building
{
    public class BuildingPlacementUI : MonoBehaviour
    {
        [SerializeField] private CaravanManager caravan;
        [SerializeField] private BuildingData[] availableBuildings;
        [SerializeField] private Transform buttonContainer;
        [SerializeField] private GameObject buildingButtonPrefab;
    
        private int selectedSlot = -1;
    
        void Start()
        {
            CreateBuildingButtons();
        }
    
        void CreateBuildingButtons()
        {
            foreach (var building in availableBuildings)
            {
                GameObject btn = Instantiate(buildingButtonPrefab, buttonContainer);
            
                // Setup button
                Button button = btn.GetComponent<Button>();
                Text text = btn.GetComponentInChildren<Text>();
            
                text.text = $"{building.BuildingName}\n${building.BaseCost}";
            
                button.onClick.AddListener(() => PlaceBuilding(building));
            }
        }
    
        void PlaceBuilding(BuildingData building)
        {
            if (selectedSlot >= 0)
            {
                caravan.PlaceBuildingInSlot(selectedSlot, building);
            }
        }
    
        public void SelectSlot(int slotIndex)
        {
            selectedSlot = slotIndex;
        }
    }
}