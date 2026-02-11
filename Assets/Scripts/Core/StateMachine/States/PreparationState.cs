using Core.Services;
using UnityEngine;

namespace Core.StateMachine.States
{
    public class PreparationState : IGameState
    {
        private readonly GameStateMachine stateMachine;
        private readonly ISceneLoaderService sceneLoader;
        private readonly int routeID;
        
        public PreparationState(GameStateMachine stateMachine, ISceneLoaderService sceneLoader, int routeID)
        {
            this.stateMachine = stateMachine;
            this.sceneLoader = sceneLoader;
            this.routeID = routeID;
        }
        
        public void Enter()
        {
            Debug.Log($"[PreparationState] Preparing for route {routeID}");
            
            // Show route preview UI
            // - Difficulty
            // - Enemy types
            // - Rewards
            // - "Start Delivery" button
        }
        
        public void Update() { }
        
        public void Exit()
        {
            Debug.Log("[PreparationState] Starting delivery");
        }
        
        // Called by UI
        public void OnStartDelivery()
        {
            stateMachine.ChangeState(new DeliveryState(stateMachine, sceneLoader, routeID));
        }
        
        public void OnCancel()
        {
            stateMachine.ChangeState(new TownState(stateMachine, sceneLoader));
        }
    }
}