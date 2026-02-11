using Core.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Extensions
{
    public class MenuButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private AudioClip hoverSound;
    
        private Vector3 originalScale;
        private IAudioService audioService;

        [Inject]
        public void Construct(IAudioService audio)
        {
            audioService = audio;
        }

        void Awake()
        {
            originalScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.localScale = originalScale * hoverScale;
            if (hoverSound != null && audioService != null)
                audioService.PlaySFX(hoverSound);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = originalScale;
        }
    }
}