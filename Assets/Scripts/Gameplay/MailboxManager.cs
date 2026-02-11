using UnityEngine;
using System;
using System.Collections.Generic;
using Gameplay.Caravan;

namespace Gameplay
{
    public class MailboxManager : MonoBehaviour
    {
        [Header("Mailbox Settings")]
        [SerializeField] private Mailbox[] mailboxes;
        [SerializeField] private float detectionRadius = 5f;
        
        [Header("References")]
        [SerializeField] private Transform playerTransform;
        [SerializeField] private CaravanController caravan;
        
        private int currentMailboxIndex = -1;
        private bool isAtMailbox = false;
        
        public event Action<int> OnMailboxReached; // mailbox index
        public event Action OnMailDelivered;
        public event Action OnResumeJourney;
        
        public bool IsAtMailbox => isAtMailbox;
        public int CurrentMailboxIndex => currentMailboxIndex;
        
        void Start()
        {
            // Initialize mailboxes
            for (int i = 0; i < mailboxes.Length; i++)
            {
                if (mailboxes[i] != null)
                {
                    mailboxes[i].Initialize(i);
                    mailboxes[i].OnMailDelivered += HandleMailDelivered;
                }
            }
        }
        
        void Update()
        {
            CheckMailboxProximity();
        }
        
        void CheckMailboxProximity()
        {
            if (playerTransform == null)
                return;
            
            // Check each mailbox
            for (int i = 0; i < mailboxes.Length; i++)
            {
                if (mailboxes[i] == null || mailboxes[i].IsDelivered)
                    continue;
                
                float distance = Vector3.Distance(playerTransform.position, mailboxes[i].transform.position);
                
                if (distance <= detectionRadius && !isAtMailbox)
                {
                    EnterMailboxZone(i);
                }
            }
        }
        
        void EnterMailboxZone(int index)
        {
            currentMailboxIndex = index;
            isAtMailbox = true;
            
            // Stop caravan
            if (caravan)
                caravan.Stop();
            
            // Activate mailbox
            mailboxes[index].Activate();
            
            OnMailboxReached?.Invoke(index);
            
            Debug.Log($"[MailboxManager] Reached mailbox {index + 1}");
        }
        
        void HandleMailDelivered(int index)
        {
            Debug.Log($"[MailboxManager] Mail delivered at mailbox {index + 1}");
            OnMailDelivered?.Invoke();
            
            // Show upgrade cards UI here
        }
        
        public void ResumeJourney()
        {
            if (!isAtMailbox)
                return;
            
            isAtMailbox = false;
            
            // Resume caravan
            if (caravan != null)
                caravan.Resume();

            OnResumeJourney?.Invoke();
            
            Debug.Log("[MailboxManager] Journey resumed");
        }
        
        public float GetProgressPercent()
        {
            if (mailboxes.Length == 0)
                return 0f;
            
            int delivered = 0;
            foreach (var mailbox in mailboxes)
            {
                if (mailbox != null && mailbox.IsDelivered)
                    delivered++;
            }
            
            return (float)delivered / mailboxes.Length;
        }
        
        void OnDestroy()
        {
            foreach (var mailbox in mailboxes)
            {
                if (mailbox != null)
                    mailbox.OnMailDelivered -= HandleMailDelivered;
            }
        }
    }
}