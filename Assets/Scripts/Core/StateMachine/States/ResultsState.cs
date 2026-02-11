using Core.Services;
using UnityEngine;

namespace Core.StateMachine.States
{
    public class ResultsState : IGameState
    {
        private readonly GameStateMachine stateMachine;
        private readonly ISceneLoaderService sceneLoader;
        private readonly int routeID;
        private readonly int goldEarned;
        private readonly int chronoShardsEarned;
        private readonly int killCount;
        
        public ResultsState(
            GameStateMachine stateMachine, 
            ISceneLoaderService sceneLoader,
            int routeID,
            int goldEarned,
            int chronoShardsEarned,
            int killCount)
        {
            this.stateMachine = stateMachine;
            this.sceneLoader = sceneLoader;
            this.routeID = routeID;
            this.goldEarned = goldEarned;
            this.chronoShardsEarned = chronoShardsEarned;
            this.killCount = killCount;
        }
        
        public void Enter()
        {
            Debug.Log($"[ResultsState] Showing results for route {routeID}");
            Debug.Log($"Gold: {goldEarned}, Chrono Shards: {chronoShardsEarned}, Kills: {killCount}");
            
            // Show results UI
            // - Gold earned
            // - Chrono Shards earned
            // - Kills
            // - Time
            // - Leaderboard position
            
            // Save progression
            // progressionService.CompleteRoute(routeID);
            // progressionService.AddChronoShards(chronoShardsEarned);
        }
        
        public void Update() { }
        
        public void Exit()
        {
            Debug.Log("[ResultsState] Returning to town");
        }
        
        // Called by UI
        public void OnContinue()
        {
            stateMachine.ChangeState(new TownState(stateMachine, sceneLoader));
        }
    }
}