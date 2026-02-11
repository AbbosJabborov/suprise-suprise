using Core.Services;
using Core.StateMachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Scope
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("[RootLifetimeScope] Configuring...");

            // ==================== SERVICES ====================
            // These persist for the entire game
            
            builder.Register<SaveService>(Lifetime.Singleton)
                .As<ISaveService>();
            
            builder.Register<AudioService>(Lifetime.Singleton)
                .As<IAudioService>();
            
            builder.Register<InputService>(Lifetime.Singleton)
                .As<IInputService>();
            
            builder.Register<SceneLoaderService>(Lifetime.Singleton)
                .As<ISceneLoaderService>();
            
            builder.Register<ProgressionService>(Lifetime.Singleton)
                .As<IProgressionService>();
            
            // ==================== STATE MACHINE ====================
            
            builder.Register<GameStateMachine>(Lifetime.Singleton);
            
            // ==================== ENTRY POINTS ====================
            // Entry points are automatically started by VContainer
            
            builder.RegisterEntryPoint<GameBootstrap>();
            
            Debug.Log("[RootLifetimeScope] Configuration complete");
        }
    }
}