using DG.Tweening;
using Gameplay.Managers;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class CoinCountUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private GameManager gameManager;

        [Header("Tween Settings")]
        [SerializeField] private float numberTweenDuration = 0.6f;
        [SerializeField] private float popPunchStrength = 0.25f;
        [SerializeField] private int popVibrato = 8;
        [SerializeField] private float popElasticity = 0.8f;

        private int displayedCoins;
        private GameManager gm;
        private int tweenId;

        void Start()
        {
            gm = gameManager ? gameManager : GameManager.Instance;
            tweenId = GetInstanceID();

            if (!gm || !coinText)
                return;

            // Initialize displayed value
            displayedCoins = gm.GetCoins();
            coinText.text = $"Coins: {displayedCoins}";
            
        }

        public void SetCoins(int newAmount)
        {
            DOTween.Kill(tweenId);

            // Use a local float so tween can animate smoothly and avoid weird capture of the field
            float current = displayedCoins;
            DOTween.To(() => current, x =>
                                    {
                                        current = x;
                                        displayedCoins = Mathf.RoundToInt(current);
                                        coinText.text = $"Coins: {displayedCoins}";
                                    }, newAmount, numberTweenDuration)
                .SetId(tweenId)
                .SetEase(Ease.OutCubic);

            if (coinText.rectTransform != null)
            {
                coinText.rectTransform.DOPunchScale(Vector3.one * popPunchStrength, 0.25f, popVibrato, popElasticity)
                    .SetId($"{tweenId}_pop");
            }
        }
    }
}
