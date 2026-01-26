using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Gameplay
{
    public class Interact : MonoBehaviour
    {
        [Header("Interaction Settings")]
        [SerializeField] private float interactionRadius = 1.5f;
        [SerializeField] private LayerMask interactableLayer;
    
        [Header("Events")]
        public UnityEvent<IInteractable> OnInteractableInRange;
        public UnityEvent OnInteractableOutOfRange;
    
        private IInteractable currentInteractable;
        private List<IInteractable> interactablesInRange = new List<IInteractable>();
    
        void Update()
        {
            DetectInteractables();
        }
    
        void DetectInteractables()
        {
            interactablesInRange.Clear();
        
            // Find all colliders in range
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(
                transform.position, 
                interactionRadius,
                interactableLayer.value == 0 ? Physics2D.AllLayers : interactableLayer
            );
        
            // Get all interactables
            foreach (var hitCollider in hitColliders)
            {
                var interactable = hitCollider.GetComponent<IInteractable>();
                if (interactable != null && interactable.CanInteract(gameObject))
                {
                    interactablesInRange.Add(interactable);
                }
            }
        
            // Find closest interactable
            IInteractable closest = GetClosestInteractable();
        
            // Update current interactable and notify UI
            if (closest != currentInteractable)
            {
                currentInteractable = closest;
            
                if (currentInteractable != null)
                {
                    OnInteractableInRange?.Invoke(currentInteractable);
                }
                else
                {
                    OnInteractableOutOfRange?.Invoke();
                }
            }
        }
    
        IInteractable GetClosestInteractable()
        {
            if (interactablesInRange.Count == 0)
                return null;
        
            IInteractable closest = null;
            float closestDistance = float.MaxValue;
        
            foreach (var interactable in interactablesInRange)
            {
                float distance = Vector2.Distance(
                    transform.position, 
                    interactable.GetTransform().position
                );
            
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = interactable;
                }
            }
        
            return closest;
        }
    
        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed && currentInteractable != null)
            {
                currentInteractable.Interact(gameObject);
            }
        }
        
        public IInteractable GetCurrentInteractable() => currentInteractable;
        public bool HasInteractableInRange() => currentInteractable != null;
    
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
}