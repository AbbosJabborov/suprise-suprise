using VContainer;
using VContainer.Unity;

namespace System
{
    public class MenuLifetimeScope: LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            //builder.Register<UIManager>(Lifetime.Singleton);
        }
        
    }
}