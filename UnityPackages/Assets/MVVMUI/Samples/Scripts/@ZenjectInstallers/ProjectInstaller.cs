using PSkrzypa.EventBus;
using Zenject;

namespace PSkrzypa.MVVMUI.Samples
{
    public class ProjectInstaller : MonoInstaller<ProjectInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MainThreadDispatcher>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<EventBus.EventBus>().AsSingle().NonLazy();
        }
    }
}