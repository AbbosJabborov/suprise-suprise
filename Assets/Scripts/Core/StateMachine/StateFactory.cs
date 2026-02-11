using VContainer;
using Core.Services;

namespace Core.StateMachine
{
    /// <summary>
    /// Factory for creating game states with injected dependencies
    /// Optional - makes state creation cleaner
    /// </summary>
    public class StateFactory
    {
        private readonly GameStateMachine stateMachine;
        private readonly ISceneLoaderService sceneLoader;
        private readonly ISaveService saveService;
        private readonly IProgressionService progressionService;
        
        [Inject]
        public StateFactory(
            GameStateMachine stateMachine,
            ISceneLoaderService sceneLoader,
            ISaveService saveService,
            IProgressionService progressionService)
        {
            this.stateMachine = stateMachine;
            this.sceneLoader = sceneLoader;
            this.saveService = saveService;
            this.progressionService = progressionService;
        }
        
        public States.BootState CreateBootState()
        {
            return new States.BootState(stateMachine, saveService, sceneLoader, progressionService);
        }
        
        public States.MainMenuState CreateMainMenuState()
        {
            return new States.MainMenuState(stateMachine, sceneLoader);
        }
        
        public States.TownState CreateTownState()
        {
            return new States.TownState(stateMachine, sceneLoader);
        }
        
        public States.PreparationState CreatePreparationState(int routeID)
        {
            return new States.PreparationState(stateMachine, sceneLoader, routeID);
        }
        
        public States.DeliveryState CreateDeliveryState(int routeID)
        {
            return new States.DeliveryState(stateMachine, sceneLoader, routeID);
        }
        
        public States.ResultsState CreateResultsState(int routeID, int gold, int shards, int kills)
        {
            return new States.ResultsState(stateMachine, sceneLoader, routeID, gold, shards, kills);
        }
    }
}