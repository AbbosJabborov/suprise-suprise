using UnityEngine;
using VContainer;
using VContainer.Unity;
using Core.Services;

namespace Core.StateMachine
{
    /// <summary>
    /// Game entry point - starts the state machine
    /// Registered as EntryPoint in RootLifetimeScope
    /// </summary>
    public class GameBootstrap : IStartable
    {
        private readonly GameStateMachine stateMachine;
        private readonly ISaveService saveService;
        private readonly ISceneLoaderService sceneLoader;
        private readonly IProgressionService progressionService;
        
        [Inject]
        public GameBootstrap(
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
        
        public void Start()
        {
            Debug.Log("[GameBootstrap] Starting game...");
            
            // Start in Boot state
            var bootState = new States.BootState(
                stateMachine,
                saveService,
                sceneLoader,
                progressionService
            );
            
            stateMachine.ChangeState(bootState);
        }
    }
}