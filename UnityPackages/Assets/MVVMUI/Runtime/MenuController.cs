using System.Collections.Generic;
using UnityEngine;
using System;
using PSkrzypa.EventBus;
using PSkrzypa.MVVMUI.BaseMenuWindow;

namespace PSkrzypa.MVVMUI
{
    public class MenuController : IDisposable, IEventListener<RegisterWindowEvent>, IEventListener<DeregisterWindowEvent>, IEventListener<OpenWindowEvent>, IEventListener<CloseWindowEvent>, IEventListener<CloseLastWindowEvent>, IEventListener<CloseAllWindowsEvent>
    {

        Dictionary<string, IWindowViewModel> windowsDictionary = new Dictionary<string, IWindowViewModel>();

        [SerializeField] Stack<string> windowsHistory = new Stack<string>();

        public MenuController()
        {
            GlobalEventBus<RegisterWindowEvent>.Register(this);
            GlobalEventBus<DeregisterWindowEvent>.Register(this);
            GlobalEventBus<OpenWindowEvent>.Register(this);
            GlobalEventBus<CloseWindowEvent>.Register(this);
            GlobalEventBus<CloseLastWindowEvent>.Register(this);
            GlobalEventBus<CloseAllWindowsEvent>.Register(this);
        }
        public void Dispose()
        {
            GlobalEventBus<RegisterWindowEvent>.Deregister(this);
            GlobalEventBus<DeregisterWindowEvent>.Deregister(this);
            GlobalEventBus<OpenWindowEvent>.Deregister(this);
            GlobalEventBus<CloseWindowEvent>.Deregister(this);
            GlobalEventBus<CloseLastWindowEvent>.Deregister(this);
            GlobalEventBus<CloseAllWindowsEvent>.Deregister(this);
        }
        public void OnEvent(OpenWindowEvent @event)
        {
            if (@event.isExclusive)
            {
                OpenWindowExclusive(@event.windowID);
            }
            else
            {
                OpenWindowAdditive(@event.windowID);
            }
        }
        public void OnEvent(CloseWindowEvent @event)
        {
            CloseLastWindow();
        }
        public void OnEvent(CloseLastWindowEvent @event)
        {

        }
        public void OnEvent(CloseAllWindowsEvent @event)
        {
            CloseAllWindows();
        }

        public void OnEvent(RegisterWindowEvent @event)
        {
            AddMenuWindowToGlobalController(@event.windowViewModel);
        }
        public void OnEvent(DeregisterWindowEvent @event)
        {
            RemoveMenuWindowFromGlobalController(@event.windowViewModel);
        }
        bool AddMenuWindowToGlobalController(IWindowViewModel menuWindowPresenter)
        {
            if (!windowsDictionary.ContainsKey(menuWindowPresenter.MenuWindowConfig.windowID))
            {
                windowsDictionary.Add(menuWindowPresenter.MenuWindowConfig.windowID, menuWindowPresenter);
                return true;
            }
            else
            {
                //Debug.LogWarning($"Failed adding {menuWindowController.name} to Windows Dictionary, already contains window of type {menuWindowController.WindowType}");
                return false;
            }
        }
        bool RemoveMenuWindowFromGlobalController(IWindowViewModel windowViewModel)
        {
            if (windowsDictionary.ContainsKey(windowViewModel.MenuWindowConfig.windowID))
            {
                windowsDictionary.Remove(windowViewModel.MenuWindowConfig.windowID);
                return true;
            }
            else
            {
                //Debug.LogWarning($"Failed removing {menuWindowController.name} from Windows Dictionary, it doesn't contain window of type {menuWindowController.WindowType}");
                return false;
            }
        }
        void OpenWindowAdditive(string windowType)
        {
            if (!windowsDictionary.ContainsKey(windowType))
            {
                //Debug.LogWarning($"The key <b>{windowType}</b> doesn't exist so you can't activate the menu!");
                return;
            }
            UnfocusCurrentWindow();
            IWindowViewModel windowToOpen = windowsDictionary[windowType];
            windowToOpen.ActivateWindow();
            windowsHistory.Push(windowType);
        }
        void OpenWindowExclusive(string windowType)
        {
            if (!windowsDictionary.ContainsKey(windowType))
            {
                //Debug.LogWarning($"The key <b>{windowType}</b> doesn't exist so you can't activate the menu!");
                return;
            }
            UnfocusCurrentWindow();
            IWindowViewModel windowToOpen = windowsDictionary[windowType];
            windowToOpen.ActivateWindow();
            windowsHistory.Push(windowType);
        }
        void CloseLastWindow()
        {
            if (windowsHistory.Count == 0)
            {
                return;
            }
            string windowType = windowsHistory.Peek();
            if (!windowsDictionary.ContainsKey(windowType))
            {
                //Debug.LogWarning($"The key <b>{windowType}</b> doesn't exist so you can't deactivate the menu!");
                return;
            }
            IWindowViewModel windowToClose = windowsDictionary[windowType];
            if (windowToClose.MenuWindowConfig.isInitialScreen || !windowToClose.MenuWindowConfig.canBeClosed)
            {
                return;
            }
            windowsHistory.Pop();
            windowToClose.DeactivateWindow();
            RefocusCurrentWindow();
        }
        void CloseAllWindows()
        {
            while (windowsHistory.Count > 0)
            {
                string windowName = windowsHistory.Pop();
                if (windowsDictionary.TryGetValue(windowName, out IWindowViewModel windowViewModel))
                {
                    windowViewModel.DeactivateWindow();
                }
            }
            windowsHistory.Clear();
        }
        void UnfocusCurrentWindow()
        {
            if (windowsHistory.Count <= 0)
            {
                return;
            }
            string windowType = windowsHistory.Peek();
            if (windowsDictionary.TryGetValue(windowType, out IWindowViewModel windowToUnfocus))
            {
                windowToUnfocus.LooseFocus();
            }
        }
        void RefocusCurrentWindow()
        {
            if (windowsHistory.Count <= 0)
            {
                return;
            }
            string windowType = windowsHistory.Peek();
            if (windowsDictionary.TryGetValue(windowType, out IWindowViewModel windowToUnfocus))
            {
                windowToUnfocus.GainFocus();
            }
        }
    }
}