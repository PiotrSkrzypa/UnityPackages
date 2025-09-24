using PSkrzypa.EventBus;
using PSkrzypa.MVVMUI.BaseMenuWindow;
using R3;
using UnityEngine;
using UnityEngine.Events;

namespace PSkrzypa.MVVMUI.Samples
{
    public class DialogWindowViewModel : BaseViewModel
    {
        public ReactiveProperty<string> message = new ReactiveProperty<string>();
        public ReactiveProperty<string> confirmText= new ReactiveProperty<string>();
        public ReactiveProperty<string> denyText= new ReactiveProperty<string>();

        UnityAction confirmAction, cancelAction;

        public DialogWindowViewModel(IMenuController menuController, MenuWindowConfig menuWindowConfig) : base(menuController, menuWindowConfig)
        {
            this.menuWindowConfig = menuWindowConfig;
        }
        public override void OpenWindow(IWindowArgs windowArgs = null)
        {
            DialogWindowArgs args = (DialogWindowArgs)windowArgs ;
            ConfigureWindow(args.message, args.confirmText, args.confirmAction, args.cancelText, args.cancelAction);
            base.OpenWindow(windowArgs);
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
    }
}
