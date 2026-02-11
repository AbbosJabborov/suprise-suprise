using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerAiming : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform weaponPivot; // Empty GameObject child that holds the weapon sprite
        [SerializeField] private Transform weaponTip; // Where bullets spawn from
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private Camera mainCamera;
        
        [Header("Aiming Settings")]
        [SerializeField] private bool smoothRotation = true;
        [SerializeField] private float rotationSpeed = 720f; // Degrees per second
        [SerializeField] private float gamepadDeadzone = 0.2f;
        
        [Header("Crosshair (Optional)")]
        [SerializeField] private Transform crosshair; // UI crosshair object
        [SerializeField] private float crosshairDistance = 2f; // Distance from player for gamepad
        
        private Vector2 aimInput;
        private Vector2 mousePosition;
        private Vector2 worldMousePosition;
        private Vector2 aimDirection;
        private float targetAngle;
        private float currentAngle;
        private bool isUsingGamepad;
        
        void Start()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;
                
            // Subscribe to input device changes
            InputSystem.onActionChange += OnActionChange;
        }
        
        void OnDestroy()
        {
            InputSystem.onActionChange -= OnActionChange;
        }
        
        void Update()
        {
            HandleAimInput();
            RotateTowardsAim();
            UpdateCrosshair();
        }
        
        void HandleAimInput()
        {
            // Get aim input from the new Input System
            aimInput = playerInput.actions["Aim"].ReadValue<Vector2>();
            
            // Determine if using gamepad based on aim input magnitude
            isUsingGamepad = aimInput.sqrMagnitude > gamepadDeadzone * gamepadDeadzone;
            
            if (isUsingGamepad)
            {
                // GAMEPAD AIMING: Use right stick direction
                if (aimInput.sqrMagnitude > gamepadDeadzone * gamepadDeadzone)
                {
                    aimDirection = aimInput.normalized;
                    targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
                }
                // If stick is neutral, keep current aim direction
            }
            else
            {
                mousePosition = playerInput.actions["MousePosition"].ReadValue<Vector2>();
                worldMousePosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.nearClipPlane));
                aimDirection = ((Vector2)worldMousePosition - (Vector2)transform.position).normalized;
                targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            }
        }
        
        void RotateTowardsAim()
        {
            if (aimDirection.sqrMagnitude < 0.01f)
                return;
            
            if (smoothRotation)
            {
                // Smooth rotation
                currentAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);
            }
            else
            {
                // Instant rotation (snappier, good for mouse)
                currentAngle = targetAngle;
            }
            
            // Apply rotation to weapon pivot
            if (weaponPivot != null)
            {
                weaponPivot.rotation = Quaternion.Euler(0, 0, currentAngle);
            }
        }
        
        void UpdateCrosshair()
        {
            if (crosshair == null)
                return;
            
            if (isUsingGamepad)
            {
                // Gamepad: Position crosshair at fixed distance in aim direction
                Vector2 crosshairPos = (Vector2)transform.position + aimDirection * crosshairDistance;
                crosshair.position = crosshairPos;
            }
            else
            {
                // Mouse: Position crosshair at mouse position
                crosshair.position = worldMousePosition;
            }
        }
        
        // Call this when you want to get the current aim direction (for shooting)
        public Vector2 GetAimDirection()
        {
            return aimDirection;
        }
        
        // Call this to get the weapon tip position (where bullets spawn)
        public Vector3 GetWeaponTipPosition()
        {
            return weaponTip != null ? weaponTip.position : transform.position;
        }
        
        // Get the angle in degrees (useful for rotating bullets)
        public float GetAimAngle()
        {
            return currentAngle;
        }
        
        // Callback for input device changes
        private void OnActionChange(object obj, InputActionChange change)
        {
            if (change == InputActionChange.ActionPerformed)
            {
                var action = obj as InputAction;
                if (action != null && action.activeControl != null)
                {
                    // Auto-detect device type
                    var device = action.activeControl.device;
                    isUsingGamepad = device is Gamepad;
                }
            }
        }
    }
}