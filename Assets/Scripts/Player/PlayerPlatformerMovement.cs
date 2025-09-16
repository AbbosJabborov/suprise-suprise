using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerPlatformerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 10f;

        private PlayerInput playerInput;
        private Rigidbody2D rb;
        private Animator animator;

        private bool isGrounded = true;
        private float moveInput;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            // Update animator parameters
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
        }

        private void FixedUpdate()
        {
            // Apply horizontal movement
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            // Read movement input
            moveInput = context.ReadValue<Vector2>().x;
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed && isGrounded)
            {
                // Apply jump force
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                isGrounded = false;
                animator.SetBool("IsJumping", true);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Ground"))
            {
                isGrounded = true;
                animator.SetBool("IsJumping", false);
            }
        }
    }
}