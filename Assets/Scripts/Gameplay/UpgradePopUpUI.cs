using System.Collections.Generic;
using Gameplay.Checkpoint;
using Gameplay.Upgrades;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class UpgradePopupUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private UpgradeCardUI[] cardUIs; // 3 cards
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button skipButton; 
    
        [Header("References")]
        [SerializeField] private CheckpointManager checkpointManager;
        [SerializeField] private UpgradeManager upgradeManager;
    
        [Header("Settings")]
        [SerializeField] private bool pauseGameOnShow = true;
    
        private List<UpgradeCardData> currentChoices;
    
        void Start()
        {
            // Find managers if not assigned
            if (checkpointManager == null)
                checkpointManager = FindFirstObjectByType<CheckpointManager>();
        
            if (upgradeManager == null)
                upgradeManager = UpgradeManager.Instance;
        
            // Subscribe to checkpoint events
            if (checkpointManager != null)
            {
                checkpointManager.OnCheckpointReached.AddListener(OnCheckpointReached);
            }
        
            // Setup skip button
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(OnSkipClicked);
            }
        
            // Hide popup initially
            HidePopup();
        }
    
        void OnCheckpointReached(int checkpointNumber)
        {
            ShowUpgradePopup();
        }
    
        public void ShowUpgradePopup()
        {
            if (upgradeManager == null)
            {
                Debug.LogError("UpgradeManager not found!");
                return;
            }
        
            // Generate upgrade choices
            currentChoices = upgradeManager.GenerateUpgradeChoices();
        
            // Display cards
            for (int i = 0; i < cardUIs.Length; i++)
            {
                if (i < currentChoices.Count)
                {
                    cardUIs[i].SetupCard(currentChoices[i], i, this);
                    //cardUIs[i].gameObject.SetActive(true);
                }
                else
                {
                    //cardUIs[i].gameObject.SetActive(false);
                }
            }
        
            // Update title
            if (titleText != null)
            {
                int checkpoint = checkpointManager != null ? checkpointManager.GetCurrentCheckpoint() : 0;
                titleText.text = $"Checkpoint {checkpoint}/{checkpointManager.GetTotalCheckpoints()} - Choose Upgrade";
            }
        
            // Show popup
            popupPanel.SetActive(true);
        
            // Pause game
            if (pauseGameOnShow)
            {
                Time.timeScale = 0f;
            }
        }
    
        public void OnCardSelected(int cardIndex)
        {
            if (cardIndex < 0 || cardIndex >= currentChoices.Count)
                return;
        
            UpgradeCardData selectedUpgrade = currentChoices[cardIndex];
        
            // Apply upgrade
            if (upgradeManager != null)
            {
                upgradeManager.ApplyUpgrade(selectedUpgrade);
            }
        
            // Hide popup and resume game
            HidePopup();
            ResumeCaravan();
        }
    
        void OnSkipClicked()
        {
            Debug.Log("Upgrade skipped");
            HidePopup();
            ResumeCaravan();
        }
    
        void HidePopup()
        {
            popupPanel.SetActive(false);
        
            // Unpause game
            if (pauseGameOnShow)
            {
                Time.timeScale = 1f;
            }
        }
    
        void ResumeCaravan()
        {
            if (checkpointManager != null)
            {
                checkpointManager.ResumeCaravan();
            }
        }
    }

    [System.Serializable]
    public class UpgradeCardUI
    {
        [Header("Card Components")]
        public GameObject cardObject;
        public Image backgroundImage;
        public Image iconImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI rarityText;
        public Button selectButton;
        public GameObject glowEffect;
    
        private int cardIndex;
        private UpgradePopupUI popupUI;
    
        public void SetupCard(UpgradeCardData upgrade, int index, UpgradePopupUI popup)
        {
            cardIndex = index;
            popupUI = popup;
        
            // Set name
            if (nameText != null)
            {
                nameText.text = upgrade.UpgradeName;
            }
        
            // Set description
            if (descriptionText != null)
            {
                descriptionText.text = upgrade.GetFormattedDescription();
            }
        
            // Set icon
            if (iconImage != null && upgrade.Icon != null)
            {
                iconImage.sprite = upgrade.Icon;
            }
        
            // Set rarity
            if (rarityText != null)
            {
                rarityText.text = upgrade.Rarity.ToString();
            }
        
            // Set background color based on rarity
            if (backgroundImage != null)
            {
                backgroundImage.color = GetRarityColor(upgrade.Rarity);
            }
        
            // Setup button
            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(OnCardClicked);
            }
        
            // Glow effect for higher rarities
            if (glowEffect != null)
            {
                glowEffect.SetActive(upgrade.Rarity >= UpgradeCardData.UpgradeRarity.Rare);
            }
        }
    
        void OnCardClicked()
        {
            if (popupUI != null)
            {
                popupUI.OnCardSelected(cardIndex);
            }
        }
    
        Color GetRarityColor(UpgradeCardData.UpgradeRarity rarity)
        {
            return rarity switch
            {
                UpgradeCardData.UpgradeRarity.Common => new Color(0.7f, 0.7f, 0.7f), // Gray
                UpgradeCardData.UpgradeRarity.Uncommon => new Color(0.3f, 0.8f, 0.3f), // Green
                UpgradeCardData.UpgradeRarity.Rare => new Color(0.3f, 0.6f, 1f), // Blue
                UpgradeCardData.UpgradeRarity.Epic => new Color(0.7f, 0.3f, 1f), // Purple
                UpgradeCardData.UpgradeRarity.Legendary => new Color(1f, 0.8f, 0f), // Gold
                _ => Color.white
            };
        }
    }
}