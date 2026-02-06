using System.Threading;
using Core;
using Gameplay.Managers;
using VContainer;
using VContainer.Unity;

namespace System
{
    public class GameInstaller: LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<GameManager>(Lifetime.Scoped);
            
            builder.Register<SceneLoader>(Lifetime.Singleton);
        }
    }
}