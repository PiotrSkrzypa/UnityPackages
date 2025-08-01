namespace PSkrzypa.MVVMUI.BaseMenuWindow
{
    public interface IWindowViewModel
    {
        MenuWindowConfig MenuWindowConfig { get; }
        void ActivateWindow();
        void DeactivateWindow();
        void GainFocus();
        void LooseFocus();
    }
}