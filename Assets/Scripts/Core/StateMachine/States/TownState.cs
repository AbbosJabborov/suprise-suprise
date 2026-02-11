using Core.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.StateMachine.States
{
    public class TownState : IGameState
    {
        private readonly GameStateMachine stateMachine;
        private readonly ISceneLoaderService sceneLoader;
        
        private const string SceneName = "Town";
        private const string PrevScene = "MainMenu";
        
        public TownState(GameStateMachine stateMachine, ISceneLoaderService sceneLoader)
        {
            this.stateMachine = stateMachine;
            this.sceneLoader = sceneLoader;
        }
        
        public void Enter()
        {
            Debug.Log("[TownState] Entering town");
            LoadTownAsync().Forget();
        }
        
        private async UniTaskVoid LoadTownAsync()
        {
            // Transition from main menu to town
            if (sceneLoader.IsSceneLoaded(PrevScene))
            {
                await sceneLoader.TransitionBetweenScenes(PrevScene, SceneName);
            }
            else
            {
                await sceneLoader.LoadSceneWithTransition(SceneName);
            }
            
            // Town UI will handle:
            // - Weapon shop
            // - Building workshop
            // - Chrono Lab
            // - Map selection
        }
        
        public void Update() { }
        
        public void Exit()
        {
            Debug.Log("[TownState] Leaving town");
        }
        
        // Called by UI when player selects a route
        public void OnRouteSelected(int routeID)
        {
            stateMachine.ChangeState(new PreparationState(stateMachine, sceneLoader, routeID));
        }
    }
}