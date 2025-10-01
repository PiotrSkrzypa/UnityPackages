using UnityEngine;

namespace PSkrzypa.MVVMUI
{
    public interface IWindowView
    {
        GameObject ViewGameObject { get; }
        IWindowViewModel GetBoundViewModel();
        void DestroyView();
    }
    public interface IWindowView<T> : IWindowView where T : BaseViewModel
    {
        void OpenView();
        void CloseView();
        void BindViewModel(T viewModel);
    }
}