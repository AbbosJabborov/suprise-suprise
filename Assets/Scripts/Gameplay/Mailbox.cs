using UnityEngine;
using System;

namespace Gameplay
{
    public class Mailbox : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private GameObject interactPrompt;
        [SerializeField] private ParticleSystem glowEffect;
        
        [Header("Settings")]
        [SerializeField] private Color inactiveColor = Color.gray;
        [SerializeField] private Color activeColor = Color.yellow;
        [SerializeField] private Color deliveredColor = Color.green;
        
        private int mailboxIndex;
        private bool isActive;
        private bool isDelivered;
        
        public event Action<int> OnMailDelivered;
        
        public bool IsDelivered => isDelivered;
        public int Index => mailboxIndex;
        
        public void Initialize(int index)
        {
            mailboxIndex = index;
            SetVisualState(MailboxState.Inactive);
            
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
        
        public void Activate()
        {
            if (isDelivered)
                return;
            
            isActive = true;
            SetVisualState(MailboxState.Active);
            
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
            
            if (glowEffect != null)
                glowEffect.Play();
        }
        
        public void Deactivate()
        {
            isActive = false;
            
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
        
        void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive || isDelivered)
                return;
            
            if (other.CompareTag("Player"))
            {
                // Check for interact input in player controller
                // or handle via interaction system
            }
        }
        
        public void DeliverMail()
        {
            if (isDelivered || !isActive)
                return;
            
            isDelivered = true;
            isActive = false;
            
            SetVisualState(MailboxState.Delivered);
            
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
            
            OnMailDelivered?.Invoke(mailboxIndex);
            
            Debug.Log($"[Mailbox] Mail delivered at mailbox {mailboxIndex + 1}");
        }
        
        void SetVisualState(MailboxState state)
        {
            if (spriteRenderer == null)
                return;
            
            switch (state)
            {
                case MailboxState.Inactive:
                    spriteRenderer.color = inactiveColor;
                    break;
                case MailboxState.Active:
                    spriteRenderer.color = activeColor;
                    break;
                case MailboxState.Delivered:
                    spriteRenderer.color = deliveredColor;
                    break;
            }
        }
        
        enum MailboxState
        {
            Inactive,
            Active,
            Delivered
        }
    }
}