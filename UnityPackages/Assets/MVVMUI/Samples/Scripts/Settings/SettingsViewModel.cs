using PSkrzypa.MVVMUI.BaseMenuWindow;

namespace PSkrzypa.MVVMUI.Samples
{
    public class SettingsViewModel : BaseViewModel
    {
        SettingsModel model;
        public SettingsViewModel(MenuWindowConfig menuWindowConfig, SettingsModel model) : base(menuWindowConfig)
        {
            this.menuWindowConfig = menuWindowConfig;
            this.model = model;
        }
    }
}