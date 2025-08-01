using PSkrzypa.MVVMUI.BaseMenuWindow;
using PSkrzypa.EventBus;
using TMPro;
using UnityEngine.Events;

namespace PSkrzypa.MVVMUI
{
    public struct RegisterWindowEvent : IEvent
    {
        public IWindowViewModel windowViewModel;
    }
    public struct DeregisterWindowEvent : IEvent
    {
        public IWindowViewModel windowViewModel;
    }
    public struct OpenWindowEvent : IEvent
    {
        public string windowID;
        public bool isExclusive;
    }
    public struct WindowOpenedEvent : IEvent
    {
        public string windowID;
    }
    public struct CloseWindowEvent : IEvent
    {
        public string windowID;
    }
    public struct CloseLastWindowEvent : IEvent
    {
    }
    public struct CloseAllWindowsEvent : IEvent
    {
    }
    public struct WindowClosedEvent : IEvent
    {
        public string windowID;
    }
    public struct UIScaleChangedEvent : IEvent
    {
        public float scale;
    }
    public struct ResolutionSettingChanged : IEvent
    {
    }
    public struct OpenVirtualKeyboard : IEvent 
    {
        public TMP_InputField inputField;
    }
    public struct OpenDialogWindow : IEvent
    {
        public string message;
        public string confirmText;
        public string cancelText;
        public UnityAction confirmAction;
        public UnityAction cancelAction;
    }


}