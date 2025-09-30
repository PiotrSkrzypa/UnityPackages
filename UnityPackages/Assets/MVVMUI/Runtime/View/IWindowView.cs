namespace PSkrzypa.MVVMUI
{
    public interface IWindowView
    {
        IWindowViewModel GetBoundViewModel();
    }
    public interface IWindowView<T> : IWindowView where T : BaseViewModel
    {
        void OpenView();
        void CloseView();
        void BindViewModel(T viewModel);
    }
}