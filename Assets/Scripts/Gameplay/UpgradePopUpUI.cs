using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VContainer;
using Gameplay.Upgrades;

namespace Gameplay.UI
{
    public class UpgradePopupUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private GameObject popupPanel;
        [SerializeField] private UpgradeCardUI[] cardUIs;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Button skipButton;
        
        private UpgradeManager upgradeManager;
        private MailboxManager mailboxManager;
        private Player.PlayerController player;
        
        private List<UpgradeCardData> currentChoices;
        
        [Inject]
        public void Construct(UpgradeManager manager, MailboxManager mailbox, Player.PlayerController playerController)
        {
            upgradeManager = manager;
            mailboxManager = mailbox;
            player = playerController;
        }
        
        void Start()
        {
            if (skipButton != null)
                skipButton.onClick.AddListener(OnSkipClicked);
            
            if (mailboxManager != null)
                mailboxManager.OnMailDelivered += ShowUpgradePopup;
            
            HidePopup();
        }
        
        void ShowUpgradePopup()
        {
            currentChoices = upgradeManager.GenerateUpgradeChoices();
            
            // Setup cards
            for (int i = 0; i < cardUIs.Length; i++)
            {
                if (i < currentChoices.Count)
                {
                    cardUIs[i].Setup(currentChoices[i], i, this);
                    cardUIs[i].gameObject.SetActive(true);
                }
                else
                {
                    cardUIs[i].gameObject.SetActive(false);
                }
            }
            
            // Update title
            if (titleText != null && mailboxManager != null)
            {
                int current = mailboxManager.CurrentMailboxIndex + 1;
                titleText.text = $"Mailbox {current} - Choose Upgrade";
            }
            
            popupPanel.SetActive(true);
            Time.timeScale = 0f;
        }
        
        public void OnCardSelected(int index)
        {
            if (index < 0 || index >= currentChoices.Count)
                return;
            
            upgradeManager.ApplyUpgrade(currentChoices[index], player);
            HidePopup();
            
            if (mailboxManager != null)
                mailboxManager.ResumeJourney();
        }
        
        void OnSkipClicked()
        {
            HidePopup();
            if (mailboxManager != null)
                mailboxManager.ResumeJourney();
        }
        
        void HidePopup()
        {
            popupPanel.SetActive(false);
            Time.timeScale = 1f;
        }
        
        void OnDestroy()
        {
            if (mailboxManager != null)
                mailboxManager.OnMailDelivered -= ShowUpgradePopup;
        }
    }
    
    [System.Serializable]
    public class UpgradeCardUI
    {
        public GameObject gameObject;
        public Image background;
        public Image icon;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI descriptionText;
        public TextMeshProUGUI rarityText;
        public Button selectButton;
        
        private int index;
        private UpgradePopupUI popup;
        
        public void Setup(UpgradeCardData upgrade, int cardIndex, UpgradePopupUI popupUI)
        {
            index = cardIndex;
            popup = popupUI;
            
            if (nameText != null)
                nameText.text = upgrade.UpgradeName;
            
            if (descriptionText != null)
                descriptionText.text = upgrade.GetFormattedDescription();
            
            if (icon != null && upgrade.Icon != null)
                icon.sprite = upgrade.Icon;
            
            if (rarityText != null)
                rarityText.text = upgrade.Rarity.ToString();
            
            if (background != null)
                background.color = GetRarityColor(upgrade.Rarity);
            
            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(OnClick);
            }
        }
        
        void OnClick() => popup?.OnCardSelected(index);
        
        Color GetRarityColor(UpgradeCardData.UpgradeRarity rarity) => rarity switch
        {
            UpgradeCardData.UpgradeRarity.Common => new Color(0.7f, 0.7f, 0.7f),
            UpgradeCardData.UpgradeRarity.Uncommon => new Color(0.3f, 0.8f, 0.3f),
            UpgradeCardData.UpgradeRarity.Rare => new Color(0.3f, 0.6f, 1f),
            UpgradeCardData.UpgradeRarity.Epic => new Color(0.7f, 0.3f, 1f),
            UpgradeCardData.UpgradeRarity.Legendary => new Color(1f, 0.8f, 0f),
            _ => Color.white
        };
    }
}