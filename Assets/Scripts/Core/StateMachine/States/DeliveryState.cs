
    using Core.Services;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    namespace Core.StateMachine.States
     {
         public class DeliveryState : IGameState
         {
             private readonly GameStateMachine stateMachine;
             private readonly ISceneLoaderService sceneLoader;
             private readonly int routeID;
        
             private DeliverySubState currentSubState;
        
             private const string TownScene = "Town";
             private string deliverySceneName;
        
             public DeliveryState(GameStateMachine stateMachine, ISceneLoaderService sceneLoader, int routeID)
             {
                 this.stateMachine = stateMachine;
                 this.sceneLoader = sceneLoader;
                 this.routeID = routeID;
            
                 // Determine scene based on route
                 deliverySceneName = GetDeliverySceneName(routeID);
             }
        
             public void Enter()
             {
                 Debug.Log($"[DeliveryState] Starting delivery on route {routeID}");
                 LoadDeliverySceneAsync().Forget();
             }
        
             private async UniTaskVoid LoadDeliverySceneAsync()
             {
                 // Transition from town to delivery scene
                 await sceneLoader.TransitionBetweenScenes(TownScene, deliverySceneName);
            
                 // Start in combat substate
                 ChangeSubState(DeliverySubState.Combat);
             }
        
             public void Update()
             {
                 // Handle substates
                 switch (currentSubState)
                 {
                     case DeliverySubState.Combat:
                         // Gameplay is running
                         // Enemies spawning, player fighting
                         break;
                    
                     case DeliverySubState.Mailbox:
                         // Stopped at mailbox
                         // Show upgrade cards
                         break;
                    
                     case DeliverySubState.Paused:
                         // Game paused
                         break;
                 }
             }
        
             public void Exit()
             {
                 Debug.Log("[DeliveryState] Delivery complete");
             }
        
             public void ChangeSubState(DeliverySubState newState)
             {
                 currentSubState = newState;
                 Debug.Log($"[DeliveryState] SubState changed to: {newState}");
            
                 switch (newState)
                 {
                     case DeliverySubState.Combat:
                         Time.timeScale = 1f;
                         break;
                    
                     case DeliverySubState.Mailbox:
                         Time.timeScale = 0f; // Pause game during upgrade selection
                         break;
                    
                     case DeliverySubState.Paused:
                         Time.timeScale = 0f;
                         break;
                 }
             }
        
             // Called when delivery is complete (reached city)
             public void OnDeliveryComplete(int goldEarned, int chronoShardsEarned, int killCount)
             {
                 stateMachine.ChangeState(new ResultsState(
                                              stateMachine, 
                                              sceneLoader, 
                                              routeID, 
                                              goldEarned, 
                                              chronoShardsEarned, 
                                              killCount
                                          ));
             }
        
             // Called when player dies
             public void OnPlayerDeath()
             {
                 // Could go to results screen with penalty
                 // Or respawn at last checkpoint
                 Debug.Log("[DeliveryState] Player died!");
             }
        
             private string GetDeliverySceneName(int route)
             {
                 return route switch
                 {
                     1 => "Delivery_Desert",
                     2 => "Delivery_Forest",
                     3 => "Delivery_Mountains",
                     4 => "Delivery_Swamp",
                     5 => "Delivery_Coast",
                     _ => "Delivery_Desert"
                 };
             }
         }
     }
    
