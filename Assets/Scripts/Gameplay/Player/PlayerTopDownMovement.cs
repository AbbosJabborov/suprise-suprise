using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerTopDownMovement : MonoBehaviour
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int Vertical = Animator.StringToHash("Vertical");
        private static readonly int Horizontal = Animator.StringToHash("Horizontal");
        [SerializeField] private float baseSpeed = 5f;
        [SerializeField]private float speedMultiplier = 1f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;
        [SerializeField] private PlayerInput playerInput;
        private Vector2 movement;

        void Update()
        {
            // Input
            movement = playerInput.actions["Move"].ReadValue<Vector2>();
            
            // Animation
            if (movement != Vector2.zero)
            {
                animator.SetFloat(Horizontal, movement.x);
                animator.SetFloat(Vertical, movement.y);
                animator.SetFloat(Speed, movement.sqrMagnitude);
                animator.SetBool(IsWalking, true);
            }
            else
            {
                animator.SetBool(IsWalking, false);
            }
        }
        void FixedUpdate()
        {
            float currentSpeed = baseSpeed * speedMultiplier;
            rb.MovePosition(rb.position + movement * (currentSpeed * Time.fixedDeltaTime));
        }
    
        public void SetSpeedMultiplier(float multiplier)
        {
            
            speedMultiplier = multiplier;
        }
    }
}
