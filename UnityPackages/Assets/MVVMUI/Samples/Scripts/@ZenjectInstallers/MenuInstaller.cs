using PSkrzypa.MVVMUI.BaseMenuWindow;
using UnityEngine;
using Zenject;

namespace PSkrzypa.MVVMUI.Samples
{
    public class MenuInstaller : MonoInstaller<MenuInstaller>
    {
        [SerializeField] MenuWindowConfig mainMenuConfig;
        [SerializeField] MenuWindowConfig settingsMenuConfig;
        [SerializeField] MenuWindowConfig dialogWindowConfig;
        public override void InstallBindings()
        {
            Container.InstallWindow<MainMenuViewModel, MainMenuModel>(mainMenuConfig);
            Container.InstallWindow<SettingsViewModel, SettingsModel>(settingsMenuConfig);
            Container.InstallWindow<DialogWindowViewModel, DialogWindowModel>(dialogWindowConfig);
        }
    }
}