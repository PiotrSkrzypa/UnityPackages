using UnityEngine.Events;

namespace PSkrzypa.MVVMUI.Samples
{
    public class DialogWindowArgs : IWindowArgs
    {
        public string message;
        public string confirmText;
        public string cancelText;
        public UnityAction confirmAction;
        public UnityAction cancelAction;

    }
}