using System.Collections.Generic;

namespace PSkrzypa.MVVMUI
{
    public interface IMenuController
    {
        void RegisterWindows(IEnumerable<MenuWindowConfig> windowConfigs);
        void RegisterWindow(IWindowViewModel windowViewModel);
        void DeregisterWindow(IWindowViewModel windowViewModel);
        void OpenWindow(string windowID, bool isExclusive, IWindowArgs windowArgs);
        void CloseActiveWindow();
        void CloseWindow(IWindowViewModel windowViewModel);
        void CloseAllWindows();

    }
}