using PSkrzypa.MVVMUI;

namespace PSkrzypa.MVVMUI
{
    public interface IMenuController
    {
        void RegisterWindow(IWindowViewModel windowViewModel);
        void DeregisterWindow(IWindowViewModel windowViewModel);
        void OpenWindow(string windowID, bool isExclusive, IWindowArgs windowArgs);
        void CloseWindow(string windowID);
        void CloseAllWindows();

    }
}