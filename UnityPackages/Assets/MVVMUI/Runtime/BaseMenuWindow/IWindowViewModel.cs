namespace PSkrzypa.MVVMUI.BaseMenuWindow
{
    public interface IWindowViewModel
    {
        MenuWindowConfig MenuWindowConfig { get; }
        void OpenWindow(IWindowArgs windowArgs);
        void CloseWindow();
        void GainFocus();
        void LooseFocus();
    }
}