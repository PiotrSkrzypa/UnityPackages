using PSkrzypa.MVVMUI.BaseMenuWindow;
using UnityEngine;
using Zenject;

namespace PSkrzypa.MVVMUI.Samples
{
    public class MenuInstaller : MonoInstaller<MenuInstaller>
    {
        [SerializeField] MenuWindowConfig mainMenuConfig;
        public override void InstallBindings()
        {
            Container.Bind<MainMenuViewModel>().AsTransient().WithArguments(mainMenuConfig);
            Container.Bind<MainMenuModel>().AsTransient();
        }
    }
}