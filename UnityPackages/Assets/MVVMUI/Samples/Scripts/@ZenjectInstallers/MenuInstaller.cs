using PSkrzypa.MVVMUI.BaseMenuWindow;
using UnityEngine;
using Zenject;

namespace PSkrzypa.MVVMUI.Samples
{
    public class MenuInstaller : MonoInstaller<MenuInstaller>
    {
        [SerializeField] MenuWindowConfig mainMenuConfig;
        [SerializeField] MenuWindowConfig settingsMenuConfig;
        public override void InstallBindings()
        {
            Container.Bind<MainMenuViewModel>().AsTransient().WithArguments(mainMenuConfig);
            Container.Bind<MainMenuModel>().AsTransient();
            Container.Bind<SettingsViewModel>().AsTransient().WithArguments(settingsMenuConfig);
            Container.Bind<SettingsModel>().AsTransient();
        }
    }
}