using UnityEngine;
using VContainer.Unity;

namespace Core.StateMachine
{
    /// <summary>
    /// Central game state machine that manages all game states
    /// Registered as EntryPoint in VContainer
    /// </summary>
    public class GameStateMachine : ITickable
    {
        private IGameState currentState;
        
        public IGameState CurrentState => currentState;
        
        public GameStateMachine()
        {
            Debug.Log("[GameStateMachine] Initialized");
        }
        
        public void Tick()
        {
            currentState?.Update();
        }
        
        public void ChangeState(IGameState newState)
        {
            if (newState == null)
            {
                Debug.LogError("[GameStateMachine] Attempted to change to null state!");
                return;
            }
            
            // Exit current state
            if (currentState != null)
            {
                Debug.Log($"[GameStateMachine] Exiting state: {currentState.GetType().Name}");
                currentState.Exit();
            }
            
            // Set new state
            currentState = newState;
            
            // Enter new state
            Debug.Log($"[GameStateMachine] Entering state: {currentState.GetType().Name}");
            currentState.Enter();
        }
        
        public T GetCurrentState<T>() where T : class, IGameState
        {
            return currentState as T;
        }
        
        public bool IsInState<T>() where T : class, IGameState
        {
            return currentState is T;
        }
    }
}