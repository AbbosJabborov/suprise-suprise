using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class InteractionUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject promptPanel;
        [SerializeField] private TextMeshProUGUI promptText;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private Image keyIconImage; // Optional: Shows key/button icon
    
        [Header("Settings")]
        [SerializeField] private string interactKey = "E"; // Change based on platform
        [SerializeField] private float fadeSpeed = 5f;
        [SerializeField] private bool worldSpace = false; // Follow item in world or stay on screen?
    
        [Header("World Space Settings")]
        [SerializeField] private Vector3 worldOffset = new Vector3(0, 1, 0); // Offset above item
        [SerializeField] private Camera mainCamera;
    
        private CanvasGroup canvasGroup;
        private IInteractable currentInteractable;
        private bool isVisible;
    
        void Awake()
        {
            canvasGroup = promptPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = promptPanel.AddComponent<CanvasGroup>();
            }
        
            if (mainCamera == null)
                mainCamera = Camera.main;
        
            HidePrompt();
        }
    
        void Update()
        {
            // Fade in/out
            float targetAlpha = isVisible ? 1f : 0f;
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, fadeSpeed * Time.deltaTime);
        
            // Update world space position
            if (worldSpace && isVisible && currentInteractable != null)
            {
                UpdateWorldPosition();
            }
        }
    
        public void ShowPrompt(IInteractable interactable)
        {
            if (interactable == null)
            {
                HidePrompt();
                return;
            }
        
            currentInteractable = interactable;
            isVisible = true;
            promptPanel.SetActive(true);
        
            // Update text
            string prompt = interactable.GetInteractionPrompt();
            string itemName = interactable.GetInteractionName();
        
            if (promptText != null)
            {
                promptText.text = $"[{interactKey}] {prompt}";
            }
        
            if (itemNameText != null)
            {
                itemNameText.text = itemName;
            }
        
            // Update position if world space
            if (worldSpace)
            {
                UpdateWorldPosition();
            }
        }
    
        public void HidePrompt()
        {
            isVisible = false;
            currentInteractable = null;
        
            // Delay disable until fully faded
            if (canvasGroup.alpha < 0.01f)
            {
                promptPanel.SetActive(false);
            }
        }
    
        void UpdateWorldPosition()
        {
            if (currentInteractable == null || mainCamera == null)
                return;
        
            Vector3 worldPos = currentInteractable.GetTransform().position + worldOffset;
            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        
            // Only show if in front of camera
            if (screenPos.z > 0)
            {
                promptPanel.transform.position = screenPos;
            }
        }
    
        public void SetInteractKey(string key)
        {
            interactKey = key;
        }
    }
}