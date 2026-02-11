using UnityEngine;
using VContainer;
using Core.Services;
using Gameplay.Weapon;

namespace Gameplay.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int IsDodging = Animator.StringToHash("IsDodging");
        private static readonly int IsSlashing = Animator.StringToHash("IsSlashing");

        [Header("Data")]
        [SerializeField] private PlayerData playerData;
        [SerializeField] private WeaponData startingWeapon;
        
        [Header("Components")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Animator animator;
        
        // Injected services
        private IInputService inputService;
        private IAudioService audioService;
        private IBulletFactory bulletFactory;
        
        // Player systems
        private PlayerMovement movement;
        private PlayerDodge dodge;
        private PlayerMelee melee;
        private PlayerHealth health;
        private PlayerShooting shooting;
        private WeaponInventory inventory;
        
        // Current state
        private Vector2 moveInput;
        private Vector2 aimDirection;
        private bool isInputEnabled = true;
        
        public WeaponInventory Inventory => inventory;
        
        [Inject]
        public void Construct(IInputService inputService, IAudioService audioService, IBulletFactory bulletFactory)
        {
            this.inputService = inputService;
            this.audioService = audioService;
            this.bulletFactory = bulletFactory;
        }
        
        void Awake()
        {
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
            if (animator == null) animator = GetComponent<Animator>();
        }
        
        void Start()
        {
            InitializeSystems();
            
            // Add starting weapon
            if (startingWeapon != null)
            {
                inventory.AddWeapon(startingWeapon);
            }
        }
        
        void InitializeSystems()
        {
            movement = new PlayerMovement(playerData, rb, transform);
            dodge = new PlayerDodge(playerData, rb, transform, spriteRenderer);
            melee = new PlayerMelee(playerData, transform, audioService);
            health = new PlayerHealth(playerData);
            shooting = new PlayerShooting(playerData, transform, audioService, bulletFactory);
            inventory = new WeaponInventory();
            
            // Subscribe to events
            dodge.OnDodgeStarted += OnDodgeStarted;
            dodge.OnDodgeEnded += OnDodgeEnded;
            melee.OnSlashPerformed += OnSlashPerformed;
            health.OnHealthChanged += OnHealthChanged;
            health.OnDeath += OnDeath;
            shooting.OnWeaponFired += OnWeaponFired;
            inventory.OnWeaponSwitched += OnWeaponSwitched;
            
            Debug.Log("[PlayerController] Systems initialized");
        }
        
        void Update()
        {
            if (!isInputEnabled || inputService == null)
                return;
            
            // Get input
            moveInput = inputService.GetMovementInput();
            aimDirection = GetAimDirection();
            
            // Update systems
            dodge.Update(Time.deltaTime);
            melee.Update(Time.deltaTime);
            health.Update(Time.deltaTime);
            shooting.Update(Time.deltaTime);
            
            // Handle dodge input
            if (inputService.WasDodgePressed() && dodge.CanDodge())
            {
                Vector2 dodgeDir = moveInput.magnitude > 0.1f ? moveInput.normalized : aimDirection;
                dodge.PerformDodge(dodgeDir);
            }
            
            // Handle melee input
            if (inputService.WasInteractPressed() && melee.CanSlash())
            {
                melee.PerformSlash(aimDirection);
            }
            
            // Handle shooting input
            bool isShootHeld = inputService.IsShootingHeld();
            if (isShootHeld && shooting.CanShoot(isShootHeld))
            {
                shooting.Shoot(aimDirection, isShootHeld);
            }
            
            // Handle weapon switching
            HandleWeaponSwitching();
            
            // Update visuals
            UpdateRotation();
            UpdateAnimator();
        }
        
        void FixedUpdate()
        {
            if (!isInputEnabled)
                return;
            
            movement.FixedUpdate(moveInput, dodge.IsDodging());
            dodge.FixedUpdate();
        }
        
        void HandleWeaponSwitching()
        {
            // Number keys
            if (Input.GetKeyDown(KeyCode.Alpha1)) inventory.SwitchToSlot(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) inventory.SwitchToSlot(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) inventory.SwitchToSlot(2);
            
            // Q/E for prev/next
            if (Input.GetKeyDown(KeyCode.Q)) inventory.SwitchPrevious();
            else if (Input.GetKeyDown(KeyCode.E)) inventory.SwitchNext();
        }
        
        Vector2 GetAimDirection()
        {
            if (inputService == null)
                return Vector2.right;
            
            Vector2 aimInput = inputService.GetAimInput();
            
            if (aimInput.sqrMagnitude > 0.04f)
                return aimInput.normalized;
            
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(aimInput);
            mouseWorldPos.z = transform.position.z;
            Vector2 direction = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;
            
            if (direction.sqrMagnitude < 0.01f && moveInput.sqrMagnitude > 0.1f)
                return moveInput.normalized;
            
            return direction.sqrMagnitude > 0.01f ? direction : Vector2.right;
        }
        
        void UpdateRotation()
        {
            if (aimDirection.sqrMagnitude < 0.01f)
                return;
            
            float targetAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            float currentAngle = transform.eulerAngles.z;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, playerData.RotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, newAngle);
        }
        
        void UpdateAnimator()
        {
            if (animator == null) return;
            
            animator.SetFloat(Speed, moveInput.magnitude);
            animator.SetBool(IsDodging, dodge.IsDodging());
            animator.SetBool(IsSlashing, melee.IsSlashing());
        }
        
        // Event handlers
        void OnDodgeStarted() => Debug.Log("[Player] Dodge started!");
        void OnDodgeEnded() => Debug.Log("[Player] Dodge ended");
        void OnSlashPerformed(Vector2 dir) => Debug.Log("[Player] Slash!");
        void OnHealthChanged(int cur, int max) => Debug.Log($"[Player] Health: {cur}/{max}");
        void OnWeaponFired(Vector2 dir) => Debug.Log("[Player] Shot fired!");
        
        void OnWeaponSwitched(int slotIndex)
        {
            WeaponData weapon = inventory.CurrentWeapon;
            shooting.EquipWeapon(weapon);
    
            if (weapon != null)
                movement.SetSpeedMultiplier(weapon.MovementSpeedMultiplier);
    
            Debug.Log($"[Player] Switched to: {weapon?.WeaponName ?? "None"}");
        }
        
        public Vector3 GetDropPosition()
        {
            return transform.position + (Vector3)(aimDirection * 1.5f);
        }
        
        void OnDeath()
        {
            Debug.Log("[Player] Player died!");
            SetInputEnabled(false);
        }
        
        // Public API
        public void SetInputEnabled(bool enabled) => isInputEnabled = enabled;
        
        public void TakeDamage(int damage)
        {
            if (dodge.IsInvincible())
            {
                Debug.Log("[Player] Damage blocked!");
                return;
            }
            health.TakeDamage(damage);
        }
        
        public void Heal(int amount) => health.Heal(amount);
        
        void OnDestroy()
        {
            if (dodge != null)
            {
                dodge.OnDodgeStarted -= OnDodgeStarted;
                dodge.OnDodgeEnded -= OnDodgeEnded;
            }
            if (melee != null)
                melee.OnSlashPerformed -= OnSlashPerformed;
            if (health != null)
            {
                health.OnHealthChanged -= OnHealthChanged;
                health.OnDeath -= OnDeath;
            }
            if (shooting != null)
                shooting.OnWeaponFired -= OnWeaponFired;
            if (inventory != null)
                inventory.OnWeaponSwitched -= OnWeaponSwitched;
        }
    }
}