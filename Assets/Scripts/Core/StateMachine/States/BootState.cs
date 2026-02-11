using UnityEngine;
using Core.Services;
using Cysharp.Threading.Tasks;

namespace Core.StateMachine.States
{
    // ==================== BOOT STATE ====================
    
    public class BootState : IGameState
    {
        private readonly GameStateMachine stateMachine;
        private readonly ISaveService saveService;
        private readonly ISceneLoaderService sceneLoader;
        private readonly IProgressionService progressionService;
        
        public BootState(
            GameStateMachine stateMachine,
            ISaveService saveService,
            ISceneLoaderService sceneLoader,
            IProgressionService progressionService)
        {
            this.stateMachine = stateMachine;
            this.saveService = saveService;
            this.sceneLoader = sceneLoader;
            this.progressionService = progressionService;
        }
        
        public void Enter()
        {
            Debug.Log("[BootState] Initializing game...");
            InitializeAsync().Forget();
        }
        
        private async UniTaskVoid InitializeAsync()
        {
            // Load player progression
            var progressionData = saveService.Load<PlayerProgressionData>("PlayerProgression");
            if (progressionData == null)
            {
                Debug.Log("[BootState] No save found, creating new progression");
                progressionData = new PlayerProgressionData();
                saveService.Save("PlayerProgression", progressionData);
            }
            
            // Small delay to show splash screen
            await UniTask.Delay(System.TimeSpan.FromSeconds(1f));
            
            // Transition to main menu
            stateMachine.ChangeState(new MainMenuState(stateMachine, sceneLoader));
        }
        
        public void Update() { }
        
        public void Exit()
        {
            Debug.Log("[BootState] Boot complete");
        }
    }
}