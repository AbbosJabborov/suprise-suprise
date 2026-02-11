using UnityEngine;
using System;

namespace Gameplay.Player
{
    /// <summary>
    /// Handles dodge roll with i-frames
    /// Pure C# class, no MonoBehaviour
    /// </summary>
    public class PlayerDodge
    {
        private readonly PlayerData playerData;
        private readonly Rigidbody2D rb;
        private readonly Transform transform;
        private readonly SpriteRenderer spriteRenderer;
        
        // State
        private bool isDodging;
        private bool isInvincible;
        private float dodgeTimer;
        private float iFramesTimer;
        private float cooldownTimer;
        private Vector2 dodgeDirection;
        
        // Events
        public event Action OnDodgeStarted;
        public event Action OnDodgeEnded;
        
        public PlayerDodge(PlayerData playerData, Rigidbody2D rb, Transform transform, SpriteRenderer spriteRenderer)
        {
            this.playerData = playerData;
            this.rb = rb;
            this.transform = transform;
            this.spriteRenderer = spriteRenderer;
        }
        
        public void Update(float deltaTime)
        {
            // Update cooldown
            if (cooldownTimer > 0)
            {
                cooldownTimer -= deltaTime;
            }
            
            // Update dodge
            if (isDodging)
            {
                dodgeTimer -= deltaTime;
                
                if (dodgeTimer <= 0)
                {
                    EndDodge();
                }
            }
            
            // Update i-frames
            if (isInvincible)
            {
                iFramesTimer -= deltaTime;
                
                if (iFramesTimer <= 0)
                {
                    EndInvincibility();
                }
                else
                {
                    // Flash sprite during i-frames
                    float alpha = Mathf.PingPong(Time.time * 20f, 0.5f) + 0.5f;
                    Color color = spriteRenderer.color;
                    color.a = alpha;
                    spriteRenderer.color = color;
                }
            }
        }
        
        public bool CanDodge()
        {
            return !isDodging && cooldownTimer <= 0;
        }
        
        public void PerformDodge(Vector2 direction)
        {
            if (!CanDodge())
                return;
            
            dodgeDirection = direction.normalized;
            isDodging = true;
            isInvincible = true;
            dodgeTimer = playerData.DodgeDuration;
            iFramesTimer = playerData.IFramesDuration;
            cooldownTimer = playerData.DodgeCooldown;
            
            OnDodgeStarted?.Invoke();
        }
        
        public void FixedUpdate()
        {
            if (!isDodging)
                return;
            
            // Move player during dodge
            float speed = playerData.DodgeDistance / playerData.DodgeDuration;
            rb.linearVelocity = dodgeDirection * speed;
        }
        
        void EndDodge()
        {
            isDodging = false;
            rb.linearVelocity = Vector2.zero;
            OnDodgeEnded?.Invoke();
        }
        
        void EndInvincibility()
        {
            isInvincible = false;
            
            // Reset sprite alpha
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
        
        public bool IsDodging() => isDodging;
        public bool IsInvincible() => isInvincible;
        public float GetCooldownPercent() => Mathf.Clamp01(1f - (cooldownTimer / playerData.DodgeCooldown));
    }
}