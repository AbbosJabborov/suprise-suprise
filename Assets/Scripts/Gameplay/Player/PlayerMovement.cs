using UnityEngine;

namespace Gameplay.Player
{
    /// <summary>
    /// Handles player movement logic
    /// Pure C# class, no MonoBehaviour
    /// </summary>
    public class PlayerMovement
    {
        private readonly PlayerData playerData;
        private readonly Rigidbody2D rb;
        private readonly Transform transform;
        
        private float currentSpeed;
        private float speedMultiplier = 1f;
        
        public PlayerMovement(PlayerData playerData, Rigidbody2D rb, Transform transform)
        {
            this.playerData = playerData;
            this.rb = rb;
            this.transform = transform;
            currentSpeed = playerData.BaseMovementSpeed;
        }
        
        public void FixedUpdate(Vector2 moveInput, bool isDodging)
        {
            if (isDodging)
            {
                // Don't process movement during dodge (dodge system handles movement)
                return;
            }
            
            // Calculate speed
            currentSpeed = playerData.BaseMovementSpeed * speedMultiplier;
            
            // Apply movement
            var velocity = moveInput * currentSpeed;
            rb.linearVelocity = velocity;
        }
        
        public void AddSpeedBonus(float bonus)
        {
            speedMultiplier += bonus;
        }
        
        public void SetSpeedMultiplier(float multiplier)
        {
            speedMultiplier = multiplier;
        }
        
        public float GetCurrentSpeed() => currentSpeed;
    }
}