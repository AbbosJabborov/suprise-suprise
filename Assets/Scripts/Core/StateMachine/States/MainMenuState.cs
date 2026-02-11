using Core.Services;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Core.StateMachine.States
{
    public class MainMenuState : IGameState
    {
        private readonly GameStateMachine stateMachine;
        private readonly ISceneLoaderService sceneLoader;
        private readonly StateFactory stateFactory;
        
        private const string SceneName = "MainMenu";
        
        public MainMenuState(GameStateMachine stateMachine, ISceneLoaderService sceneLoader, StateFactory factory = null)
        {
            this.stateMachine = stateMachine;
            this.sceneLoader = sceneLoader;
            this.stateFactory = factory;
        }
        
        public void Enter()
        {
            Debug.Log("[MainMenuState] Entering main menu");
            LoadMainMenuAsync().Forget();
        }
        
        private async UniTaskVoid LoadMainMenuAsync()
        {
            await sceneLoader.LoadSceneWithTransition(SceneName);
            
            // UI will call OnContinueGame() or OnNewGame()
        }
        
        public void Update() { }
        
        public void Exit()
        {
            Debug.Log("[MainMenuState] Exiting main menu");
        }
        
        // Called by UI buttons
        public void OnContinueGame()
        {
            if (stateFactory != null)
                stateMachine.ChangeState(stateFactory.CreateTownState());
            else
                stateMachine.ChangeState(new TownState(stateMachine, sceneLoader));
        }
        
        public void OnNewGame()
        {
            // Reset progression and start new game
            stateMachine.ChangeState(new TownState(stateMachine, sceneLoader));
        }
        
        public void OnQuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }
    }
}