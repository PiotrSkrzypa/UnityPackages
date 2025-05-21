using System;
using PSkrzypa.EventBus;
using PSkrzypa.MVVMUI;
using R3;

namespace PSkrzypa.MMVMUI.BaseMenuWindow
{
    public abstract class BaseViewModel : IWindowViewModel, IDisposable
    {
        protected MenuWindowConfig menuWindowConfig;
        public ReactiveCommand openCommand = new ReactiveCommand();
        public ReactiveCommand closeCommand = new ReactiveCommand();

        #region Properties
        public string WindowID { get => menuWindowConfig.windowID; }
        public MenuWindowConfig MenuWindowConfig { get => menuWindowConfig; }
        public ReactiveProperty<bool> HasFocus { get; private set; } = new ReactiveProperty<bool>(false);
        #endregion


        public BaseViewModel(MenuWindowConfig menuWindowConfig)
        {
            this.menuWindowConfig = menuWindowConfig;
            Register();
        }
        public virtual void Register()
        {
            GlobalEventBus<RegisterWindowEvent>.Raise(new RegisterWindowEvent { windowViewModel = this });
        }
        public virtual void Dispose()
        {
            GlobalEventBus<DeregisterWindowEvent>.Raise(new DeregisterWindowEvent { windowViewModel = this });
            openCommand?.Dispose();
            closeCommand?.Dispose();
        }
        protected virtual void OpenWindowExclusive()
        {
            GlobalEventBus<OpenWindowEvent>.Raise(new OpenWindowEvent { windowID = menuWindowConfig.windowID, isExclusive = true });
        }
        protected virtual void OpenWindowAdditive()
        {
            GlobalEventBus<OpenWindowEvent>.Raise(new OpenWindowEvent { windowID = menuWindowConfig.windowID, isExclusive = false });
        }
        public virtual void ActivateWindow()
        {
            GlobalEventBus<WindowOpenedEvent>.Raise(new WindowOpenedEvent() { windowID = menuWindowConfig.windowID });
            openCommand.Execute(Unit.Default);
            GainFocus();
        }
        public virtual void DeactivateWindow()
        {
            GlobalEventBus<WindowClosedEvent>.Raise(new WindowClosedEvent() { windowID = menuWindowConfig.windowID });
            closeCommand.Execute(Unit.Default);
            LooseFocus();
        }
        public virtual void CloseWindow()
        {
            GlobalEventBus<CloseWindowEvent>.Raise(new CloseWindowEvent() { windowID = menuWindowConfig.windowID });
        }
        public virtual void CloseAllWindows()
        {
            GlobalEventBus<CloseAllWindowsEvent>.Raise(new CloseAllWindowsEvent());
        }
        public virtual void CloseLastWindow()
        {
            GlobalEventBus<CloseLastWindowEvent>.Raise(new CloseLastWindowEvent());
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