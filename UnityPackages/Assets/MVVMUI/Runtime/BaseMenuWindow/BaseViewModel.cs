using System;
using PSkrzypa.EventBus;
using R3;

namespace PSkrzypa.MVVMUI.BaseMenuWindow
{
    public abstract class BaseViewModel : IWindowViewModel, IDisposable
    {
        protected IMenuController menuController;
        protected MenuWindowConfig menuWindowConfig;
        public ReactiveCommand openCommand = new ReactiveCommand();
        public ReactiveCommand closeCommand = new ReactiveCommand();

        #region Properties
        public string WindowID { get => menuWindowConfig.windowID; }
        public MenuWindowConfig MenuWindowConfig { get => menuWindowConfig; }
        public ReactiveProperty<bool> HasFocus { get; private set; } = new ReactiveProperty<bool>(false);
        #endregion


        public BaseViewModel(IMenuController menuController, MenuWindowConfig menuWindowConfig)
        {
            this.menuController = menuController;
            this.menuWindowConfig = menuWindowConfig;
            Register();
        }
        public virtual void Register()
        {
            menuController.RegisterWindow(this);
        }
        public virtual void Dispose()
        {
            menuController.DeregisterWindow(this);
            openCommand?.Dispose();
            closeCommand?.Dispose();
        }
        public virtual void OpenWindow(IWindowArgs windowArgs = null)
        {
            openCommand.Execute(Unit.Default);
            GainFocus();
        }
        public virtual void CloseWindow()
        {
            closeCommand.Execute(Unit.Default);
            LooseFocus();
        }

        public void GainFocus()
        {
            HasFocus.Value = true;
        }

        public void LooseFocus()
        {
            HasFocus.Value = false;
        }

    }
}