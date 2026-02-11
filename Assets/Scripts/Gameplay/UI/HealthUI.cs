using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gameplay.UI
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private Image healthFill;
        [SerializeField] private TextMeshProUGUI healthText;
        
        public void UpdateHealth(int current, int max)
        {
            float percent = (float)current / max;
            
            if (healthFill != null)
                healthFill.fillAmount = percent;
            
            if (healthText != null)
                healthText.text = $"{current}/{max}";
        }
    }
}