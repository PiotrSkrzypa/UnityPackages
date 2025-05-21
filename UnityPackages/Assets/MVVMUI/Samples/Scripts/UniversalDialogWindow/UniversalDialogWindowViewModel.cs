using UnityEngine.Events;
using R3;
using PSkrzypa.EventBus;
using PSkrzypa.MMVMUI.BaseMenuWindow;

namespace PSkrzypa.MVVMUI.Samples
{
    public class UniversalDialogWindowViewModel : BaseViewModel, IEventListener<OpenDialogWindow>
    {
        public ReactiveProperty<string> message = new ReactiveProperty<string>();
        public ReactiveProperty<string> confirmText= new ReactiveProperty<string>();
        public ReactiveProperty<string> denyText= new ReactiveProperty<string>();

        UnityAction confirmAction, cancelAction;

        public UniversalDialogWindowViewModel(MenuWindowConfig menuWindowConfig) : base(menuWindowConfig)
        {
            this.menuWindowConfig = menuWindowConfig;
            GlobalEventBus<OpenDialogWindow>.Register(this);
        }
        public void ConfigureWindow(string message, string confirmText, UnityAction confirmAction, string cancelText, UnityAction cancelAction)
        {
            this.message.Value = message;
            this.confirmText.Value = confirmText;
            this.denyText.Value = cancelText;
            this.confirmAction = confirmAction;
            this.cancelAction = cancelAction;
        }
        public override void Dispose()
        {
            GlobalEventBus<OpenDialogWindow>.Deregister(this);
            base.Dispose();
        }
        public void OnConfirm()
        {
            confirmAction?.Invoke();
            CloseWindow();
        }
        public void OnCancel()
        {
            cancelAction?.Invoke();
            CloseWindow();
        }
        public void OnEvent(OpenDialogWindow @event)
        {
            ConfigureWindow(@event.message, @event.confirmText, @event.confirmAction, @event.cancelText, @event.cancelAction);
            GlobalEventBus<OpenWindowEvent>.Raise(new OpenWindowEvent { windowID = menuWindowConfig.windowID });
        }
    }
}
