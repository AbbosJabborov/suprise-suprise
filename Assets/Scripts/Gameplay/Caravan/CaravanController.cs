using UnityEngine;
using Dreamteck.Splines;
using System;

namespace Gameplay.Caravan
{
    [RequireComponent(typeof(SplineFollower))]
    public class CaravanController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float baseSpeed = 2f;
        [SerializeField] private bool autoStart = true;
        
        [Header("References")]
        [SerializeField] private SplineFollower splineFollower;
        
        private bool isMoving;
        private float currentSpeed;
        private float speedMultiplier = 1f;
        
        public event Action OnJourneyStarted;
        public event Action OnJourneyStopped;
        public event Action OnJourneyCompleted;
        public event Action<float> OnProgressChanged; // 0-1
        
        public bool IsMoving => isMoving;
        public float Progress => (float)splineFollower.result.percent;
        public Vector3 Position => transform.position;
        
        void Awake()
        {
            if (splineFollower == null)
                splineFollower = GetComponent<SplineFollower>();
            
            currentSpeed = baseSpeed;
        }
        
        void Start()
        {
            // Configure spline follower
            splineFollower.followSpeed = currentSpeed;
            splineFollower.follow = autoStart;
            //splineFollower.autoFollow = false; // We control it manually
            
            // Subscribe to spline events
            splineFollower.onEndReached += OnEndReached;
            
            if (autoStart)
            {
                StartJourney();
            }
        }
        
        void Update()
        {
            if (isMoving)
            {
                OnProgressChanged?.Invoke(Progress);
            }
        }
        
        public void StartJourney()
        {
            if (isMoving)
                return;
            
            isMoving = true;
            splineFollower.follow = true;
            OnJourneyStarted?.Invoke();
            
            Debug.Log("[CaravanController] Journey started");
        }
        
        public void Stop()
        {
            if (!isMoving)
                return;
            
            isMoving = false;
            splineFollower.follow = false;
            OnJourneyStopped?.Invoke();
            
            Debug.Log("[CaravanController] Stopped");
        }
        
        public void Resume()
        {
            if (isMoving)
                return;
            
            isMoving = true;
            splineFollower.follow = true;
            
            Debug.Log("[CaravanController] Resumed");
        }
        
        public void SetSpeed(float speed)
        {
            currentSpeed = speed;
            splineFollower.followSpeed = currentSpeed * speedMultiplier;
        }
        
        public void SetSpeedMultiplier(float multiplier)
        {
            speedMultiplier = multiplier;
            splineFollower.followSpeed = currentSpeed * speedMultiplier;
        }
        
        void OnEndReached(double percent)
        {
            isMoving = false;
            OnJourneyCompleted?.Invoke();
            
            Debug.Log("[CaravanController] Journey completed!");
        }
        
        public float GetProgressPercent()
        {
            return Progress * 100f;
        }
        
        // public Vector3 GetForwardDirection()
        // {
        //     return splineFollower.result.direction;
        // }
        
        void OnDestroy()
        {
            if (splineFollower != null)
                splineFollower.onEndReached -= OnEndReached;
        }
    }
}