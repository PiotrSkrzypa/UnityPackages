using PSkrzypa.MVVMUI.BaseMenuWindow;
using Zenject;

namespace PSkrzypa.MVVMUI
{
    public static class ZenjectExtensions
    {

        public static void InstallWindow<VM>(this DiContainer container, MenuWindowConfig menuWindowConfig)
            where VM : IWindowViewModel
        {
            container.Bind<VM>().AsSingle().WithArguments(menuWindowConfig);
        }
        public static void InstallWindow<VM, M>(this DiContainer container, MenuWindowConfig menuWindowConfig)
            where VM : IWindowViewModel
            where M : IWindowModel
        {
            container.Bind<VM>().AsSingle().WithArguments(menuWindowConfig);
            container.Bind<M>().AsSingle();
        }
    }
}