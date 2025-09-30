using UnityEngine;
using Zenject;

namespace PSkrzypa.MVVMUI.Samples
{
    public class MenuInstaller : MonoInstaller<MenuInstaller>
    {
        [SerializeField] GameObject windowsRoot;
        [SerializeField] MenuWindowConfig mainMenuConfig;
        [SerializeField] MenuWindowConfig settingsMenuConfig;
        [SerializeField] MenuWindowConfig dialogWindowConfig;
        public override void InstallBindings()
        {
            Container.Bind<IResolver>().To<ZenjectResolver>().AsSingle();
            Container.Bind<WindowFactory>().AsSingle();
            Container.BindInterfacesAndSelfTo<MenuController>().AsSingle().WithArguments(windowsRoot).NonLazy();
            Container.InstallWindow<MainMenuViewModel, MainMenuModel>(mainMenuConfig);
            Container.InstallWindow<SettingsViewModel, SettingsModel>(settingsMenuConfig);
            Container.InstallWindow<DialogWindowViewModel, DialogWindowModel>(dialogWindowConfig);
        }
        private void Awake()
        {
            MenuWindowConfig[] configs = { mainMenuConfig, settingsMenuConfig, dialogWindowConfig };
            IMenuController menuController = Container.Resolve<IMenuController>();
            menuController.RegisterWindows(configs);
        }
    }
}