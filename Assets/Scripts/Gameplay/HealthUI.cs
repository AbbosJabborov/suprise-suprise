using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class HealthUI: MonoBehaviour
    {
        [SerializeField] private Health objectHealth;
        [SerializeField] private Image healthBarFill;
        //[SerializeField] private TextMeshProUGUI healthText;

        void Awake()
        {
            
        }
        void Start()
        {
            if (objectHealth != null)
            {
                objectHealth.OnHealthChanged.AddListener(UpdateHealthUI);
                UpdateHealthUI(objectHealth.GetCurrentHealth(), objectHealth.GetMaxHealth());
                
            }
        }
        
        private void UpdateHealthUI(int currentHealth, int maxHealth)
        {
            if (healthBarFill != null)
            {
                healthBarFill.fillAmount = (float)currentHealth / maxHealth;
            }

            // if (healthText != null)
            // {
            //     healthText.text = $"{currentHealth} / {maxHealth}";
            // }
        }
        void OnDestroy()
        {
            if (objectHealth != null)
            {
                objectHealth.OnHealthChanged.RemoveListener(UpdateHealthUI);
            }
        }
    }
}