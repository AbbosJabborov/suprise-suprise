using Dreamteck.Splines;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Checkpoint
{
    public class CheckpointManager : MonoBehaviour
    {
        [Header("Spline References")]
        [SerializeField] private SplineFollower splineFollower;
        
        [Header("Checkpoint Settings")]
        [SerializeField] private int totalCheckpoints = 4;
        [SerializeField] private int currentCheckpoint = 0;
        [SerializeField] private float[] checkpointPositions; // Percent along spline (0-1)
        
        [Header("Caravan Control")]
        [SerializeField] private bool autoGenerateCheckpoints = true;
        [SerializeField] private float stopDuration = 0f; // 0 = wait for player to continue
        
        [Header("Events")]
        public UnityEvent<int> OnCheckpointReached; // Checkpoint number
        public UnityEvent OnCityReached;
        public UnityEvent OnCaravanStopped;
        public UnityEvent OnCaravanResumed;
        
        private bool isWaitingAtCheckpoint;
        private bool hasReachedCity;
        private float stopTimer;
        
        void Start()
        {
            if (splineFollower == null)
            {
                splineFollower = GetComponent<SplineFollower>();
            }
            
            if (autoGenerateCheckpoints)
            {
                GenerateEvenCheckpoints();
            }
            
            // Subscribe to spline follower events if available
            // if (splineFollower != null)
            // {
            //     splineFollower.onEndReached += OnSplineEndReached;
            // }
        }
        
        void Update()
        {
            if (hasReachedCity || splineFollower == null)
                return;
            
            CheckForCheckpoint();
            
            // Auto-resume after duration
            if (isWaitingAtCheckpoint && stopDuration > 0)
            {
                stopTimer += Time.deltaTime;
                if (stopTimer >= stopDuration)
                {
                    ResumeCaravan();
                }
            }
        }
        
        void CheckForCheckpoint()
        {
            if (isWaitingAtCheckpoint || checkpointPositions == null)
                return;
            
            if (currentCheckpoint >= totalCheckpoints)
                return;
            
            // Get current position on spline (0 to 1)
            double currentPercent = splineFollower.result.percent;
            
            // Check if we've reached the next checkpoint
            if (currentCheckpoint < checkpointPositions.Length)
            {
                float checkpointPercent = checkpointPositions[currentCheckpoint];
                
                // Allow small tolerance
                if (currentPercent >= checkpointPercent - 0.01f)
                {
                    ReachedCheckpoint(currentCheckpoint);
                }
            }
        }

        private void ReachedCheckpoint(int checkpointNumber)
        {
            currentCheckpoint++;
            isWaitingAtCheckpoint = true;
            stopTimer = 0f;
            
            // Stop the caravan
            StopCaravan();
            
            // Trigger events
            OnCheckpointReached?.Invoke(checkpointNumber);
            OnCaravanStopped?.Invoke();
            
            Debug.Log($"Reached checkpoint {checkpointNumber + 1}/{totalCheckpoints}");
            
            // Check if this was the last checkpoint (city reached)
            if (currentCheckpoint >= totalCheckpoints)
            {
                CityReached();
            }
        }

        private void CityReached()
        {
            hasReachedCity = true;
            OnCityReached?.Invoke();
            Debug.Log("City Reached!");
        }

        private void OnSplineEndReached()
        {
            if (!hasReachedCity)
            {
                CityReached();
            }
        }
        
        public void StopCaravan()
        {
            if (splineFollower != null)
            {
                splineFollower.follow = false;
            }
        }
        
        public void ResumeCaravan()
        {
            if (splineFollower != null && !hasReachedCity)
            {
                splineFollower.follow = true;
                isWaitingAtCheckpoint = false;
                OnCaravanResumed?.Invoke();
                Debug.Log("Caravan resumed");
            }
        }
        
        void GenerateEvenCheckpoints()
        {
            checkpointPositions = new float[totalCheckpoints];
            
            for (int i = 0; i < totalCheckpoints; i++)
            {
                // Distribute checkpoints evenly along the spline
                // Skip 0% (start) and 100% (end), use middle portions
                float percent = (i + 1f) / (totalCheckpoints + 1f);
                checkpointPositions[i] = percent;
            }
            
            Debug.Log($"Generated {totalCheckpoints} checkpoints at: {string.Join(", ", checkpointPositions)}");
        }
        
        // Public methods
        public bool IsWaitingAtCheckpoint() => isWaitingAtCheckpoint;
        public bool HasReachedCity() => hasReachedCity;
        public int GetCurrentCheckpoint() => currentCheckpoint;
        public int GetTotalCheckpoints() => totalCheckpoints;
        public float GetProgress() => (float)currentCheckpoint / totalCheckpoints;
        
        // Manual checkpoint trigger (for testing)
        public void ForceNextCheckpoint()
        {
            if (currentCheckpoint < totalCheckpoints)
            {
                ReachedCheckpoint(currentCheckpoint);
            }
        }
        
        // void OnDestroy()
        // {
        //     if (splineFollower != null)
        //     {
        //         splineFollower.onEndReached -= OnSplineEndReached;
        //     }
        // }
    }
}