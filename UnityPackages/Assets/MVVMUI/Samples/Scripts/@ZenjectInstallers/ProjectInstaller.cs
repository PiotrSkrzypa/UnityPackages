using PSkrzypa.EventBus;
using PSkrzypa.EventBus.EventSubscriber;
using Zenject;

namespace PSkrzypa.MVVMUI.Samples
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MainThreadDispatcher>().AsSingle().NonLazy();
            Container.Bind<IEventSubscriberFactory>().To<EventSubscriberFactory>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<EventBus.EventBus>().AsSingle().NonLazy();
        }
    }
}