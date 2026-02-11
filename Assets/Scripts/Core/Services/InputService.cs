using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Services
{
    public class InputService : IInputService
    {
        private readonly PlayerInput playerInput;
        private bool inputEnabled = true;
        
        public bool IsInputEnabled => inputEnabled;
        
        public InputService()
        {
            // Find PlayerInput in scene (should be on Player or a dedicated InputManager)
            playerInput = Object.FindFirstObjectByType<PlayerInput>();
            
            if (playerInput == null)
            {
                Debug.LogWarning("[InputService] PlayerInput not found in scene. Input will not work until PlayerInput is available.");
            }
            else
            {
                Debug.Log("[InputService] Initialized with PlayerInput");
            }
        }
        
        public void SetInputEnabled(bool enabled)
        {
            inputEnabled = enabled;
            
            if (playerInput != null)
            {
                if (enabled)
                    playerInput.ActivateInput();
                else
                    playerInput.DeactivateInput();
            }
            
            Debug.Log($"[InputService] Input {(enabled ? "enabled" : "disabled")}");
        }
        
        public Vector2 GetMovementInput()
        {
            if (!inputEnabled || playerInput == null)
                return Vector2.zero;
            
            return playerInput.actions["Move"].ReadValue<Vector2>();
        }
        
        public Vector2 GetAimInput()
        {
            if (!inputEnabled || playerInput == null)
                return Vector2.zero;
            
            // For mouse aiming
            var mousePos = playerInput.actions["MousePosition"].ReadValue<Vector2>();
            
            // For gamepad aiming (right stick)
            var stickAim = playerInput.actions["Aim"].ReadValue<Vector2>();

            // Prefer stick if it's being used (magnitude > dead zone)
            return stickAim.sqrMagnitude > 0.04f ? // 0.2 dead zone squared
                stickAim : mousePos;
        }
        
        public bool IsShootingHeld()
        {
            if (!inputEnabled || playerInput == null)
                return false;
            
            return playerInput.actions["Shoot"].ReadValue<float>() > 0.1f;
        }
        
        public bool WasShootPressed()
        {
            if (!inputEnabled || playerInput == null)
                return false;
            
            return playerInput.actions["Shoot"].WasPressedThisFrame();
        }
        
        public bool WasDodgePressed()
        {
            if (!inputEnabled || playerInput == null)
                return false;
            
            return playerInput.actions["Dodge"].WasPressedThisFrame();
        }
        
        public bool WasInteractPressed()
        {
            if (!inputEnabled || playerInput == null)
                return false;
            
            // Check for both Interact action AND Melee Slash action
            // Using Right Click / RB for melee slash
            var interactAction = playerInput.actions.FindAction("Interact");
            var meleeAction = playerInput.actions.FindAction("Melee");
            
            bool interactPressed = interactAction != null && interactAction.WasPressedThisFrame();
            bool meleePressed = meleeAction != null && meleeAction.WasPressedThisFrame();
            
            return interactPressed || meleePressed;
        }
        
        public bool WasPausePressed()
        {
            if (!inputEnabled || playerInput == null)
                return false;
            
            return playerInput.actions["Pause"].WasPressedThisFrame();
        }
    }
}