namespace PSkrzypa.MVVMUI.Samples
{
    public class MainMenuModel : IWindowModel
    {
        string SettingsWindowID = "settingswindowconfig";
        string DialogWindowID = "dialogwindowconfig";
        IMenuController menuController;

        public MainMenuModel(IMenuController menuController)
        {
            this.menuController = menuController;
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
            menuController.OpenWindow(SettingsWindowID, false, null);
        }
        public void QuitGame()
        {
            menuController.OpenWindow(DialogWindowID, false, new DialogWindowArgs()
            {
                message = "Are you sure you want to quit the game?",
                confirmText = "Yes",
                cancelText = "No",
                confirmAction = () =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
                },
                cancelAction = null
            });

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