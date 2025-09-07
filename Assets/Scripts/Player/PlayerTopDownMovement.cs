using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerTopDownMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
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
                animator.SetFloat("Horizontal", movement.x);
                animator.SetFloat("Vertical", movement.y);
                animator.SetFloat("Speed", movement.sqrMagnitude);
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }
        }
        void FixedUpdate()
        {
            // Movement
            rb.MovePosition(rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
        }
    }
}
