using UnityEngine;

namespace Gameplay
{
    public interface IInteractable
    {
        /// <summary>
        /// Called when player interacts with this object
        /// </summary>
        void Interact(GameObject interactor);
    
        /// <summary>
        /// Returns the name to display in UI (e.g., "Shotgun", "Health Pack")
        /// </summary>
        string GetInteractionName();
    
        /// <summary>
        /// Returns the prompt text (e.g., "Pick up", "Open", "Use")
        /// </summary>
        string GetInteractionPrompt();
    
        /// <summary>
        /// Can this object be interacted with right now?
        /// </summary>
        bool CanInteract(GameObject interactor);
    
        /// <summary>
        /// Transform position for UI placement
        /// </summary>
        Transform GetTransform();
    }
}