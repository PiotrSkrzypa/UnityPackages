namespace PSkrzypa.MVVMUI.Samples
{
    public class SettingsViewModel : BaseViewModel
    {
        SettingsModel model;
        public SettingsViewModel(IMenuController menuController, MenuWindowConfig menuWindowConfig, SettingsModel model) : base(menuController, menuWindowConfig)
        {
            this.menuWindowConfig = menuWindowConfig;
            this.model = model;
        }
    }
}