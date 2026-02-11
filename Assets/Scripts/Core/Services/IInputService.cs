using UnityEngine;

namespace Core.Services
{
    public interface IInputService
    {
        /// <summary>
        /// Enable/disable input
        /// </summary>
        void SetInputEnabled(bool enabled);
        
        /// <summary>
        /// Is input currently enabled
        /// </summary>
        bool IsInputEnabled { get; }
        
        /// <summary>
        /// Get movement input (WASD / Left Stick)
        /// </summary>
        Vector2 GetMovementInput();
        
        /// <summary>
        /// Get aim input (Mouse / Right Stick)
        /// </summary>
        Vector2 GetAimInput();
        
        /// <summary>
        /// Is shooting button held
        /// </summary>
        bool IsShootingHeld();
        
        /// <summary>
        /// Was shoot button pressed this frame
        /// </summary>
        bool WasShootPressed();
        
        /// <summary>
        /// Was dodge button pressed this frame
        /// </summary>
        bool WasDodgePressed();
        
        /// <summary>
        /// Was interact button pressed this frame
        /// </summary>
        bool WasInteractPressed();
        
        /// <summary>
        /// Was pause button pressed this frame
        /// </summary>
        bool WasPausePressed();
    }
}