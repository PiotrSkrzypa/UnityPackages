using PSkrzypa.EventBus;

namespace PSkrzypa.MVVMUI.Samples
{
    public class MainMenuModel
    {
        string SettingsWindowID = "settings";

        public MainMenuModel()
        {
        }

        public void StartNewGame()
        {
            DeleteProgress();
        }
        public void ContinueSavedGame()
        {
        }
        public void OpenSettings()
        {
            GlobalEventBus<OpenWindowEvent>.Raise(new OpenWindowEvent { windowID = SettingsWindowID });
        }
        void DeleteProgress()
        {
        }
        public bool ProgressExists()
        {
            return false;
        }
    }
}