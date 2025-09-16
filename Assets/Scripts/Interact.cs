using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [SerializeField] private float interactionRadius = 1f;
    
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
            foreach (var hitCollider in hitColliders)
            {
                var interactable = hitCollider.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}
