using Core.Services;
using VContainer;
using VContainer.Unity;

namespace Core.Scope
{
    public class MainMenuLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // Register only services needed for main menu
            builder.Register<IAudioService, AudioService>(Lifetime.Singleton);
            builder.Register<ISceneLoaderService, SceneLoaderService>(Lifetime.Singleton);
        
            builder.RegisterComponentInHierarchy<MainMenuController>();
        }
    }
}