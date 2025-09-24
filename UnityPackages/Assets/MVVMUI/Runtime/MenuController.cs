using System.Collections.Generic;
using UnityEngine;
using System;
using PSkrzypa.EventBus;
using PSkrzypa.MVVMUI.BaseMenuWindow;

namespace PSkrzypa.MVVMUI
{
    public class MenuController : IMenuController, IDisposable
    {

        Dictionary<string, IWindowViewModel> windowsDictionary = new Dictionary<string, IWindowViewModel>();

        [SerializeField] Stack<string> windowsHistory = new Stack<string>();

        IEventBus _eventBus;

        public MenuController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }
      
        public void Dispose()
        {
        }

        public (bool, string) RegisterWindow(IWindowViewModel windowViewModel)
        {
            if (!windowsDictionary.ContainsKey(windowViewModel.MenuWindowConfig.windowID))
            {
                // TODO give window unique ID
                windowsDictionary.Add(windowViewModel.MenuWindowConfig.windowID, windowViewModel);
                return (true, windowViewModel.MenuWindowConfig.windowID);
            }
            else
            {
                //Debug.LogWarning($"Failed adding {menuWindowController.name} to Windows Dictionary, already contains window of type {menuWindowController.WindowType}");
                return (false, string.Empty);
            }
        }

        public void DeregisterWindow(IWindowViewModel windowViewModel)
        {
            if (windowsDictionary.ContainsKey(windowViewModel.MenuWindowConfig.windowID))
            {
                windowsDictionary.Remove(windowViewModel.MenuWindowConfig.windowID);
            }
            else
            {
                //Debug.LogWarning($"Failed removing {menuWindowController.name} from Windows Dictionary, it doesn't contain window of type {menuWindowController.WindowType}");
            }
        }

        public void OpenWindow(string windowID, bool isExclusive, IWindowArgs windowArgs)
        {
            if (isExclusive)
            {
                OpenWindowExclusive(windowID, windowArgs);
            }
            else
            {
                OpenWindowAdditive(windowID, windowArgs);
            }
        }

        public void CloseWindow(string windowID)
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
            if (windowToClose.MenuWindowConfig.windowID != windowID)
            {
                return;
            }
            if (windowToClose.MenuWindowConfig.isInitialScreen || !windowToClose.MenuWindowConfig.canBeClosed)
            {
                return;
            }
            windowsHistory.Pop();
            windowToClose.CloseWindow();
            FocusCurrentWindow();
        }

        public void CloseAllWindows()
        {
            while (windowsHistory.Count > 0)
            {
                string windowName = windowsHistory.Pop();
                if (windowsDictionary.TryGetValue(windowName, out IWindowViewModel windowViewModel))
                {
                    windowViewModel.CloseWindow();
                }
            }
            windowsHistory.Clear();
        }

        void OpenWindowAdditive(string windowType, IWindowArgs windowArgs)
        {
            if (!windowsDictionary.ContainsKey(windowType))
            {
                //Debug.LogWarning($"The key <b>{windowType}</b> doesn't exist so you can't activate the menu!");
                return;
            }
            UnfocusCurrentWindow();
            IWindowViewModel windowToOpen = windowsDictionary[windowType];
            windowToOpen.OpenWindow(windowArgs);
            windowsHistory.Push(windowType);
        }

        void OpenWindowExclusive(string windowType, IWindowArgs windowArgs)
        {
            if (!windowsDictionary.ContainsKey(windowType))
            {
                //Debug.LogWarning($"The key <b>{windowType}</b> doesn't exist so you can't activate the menu!");
                return;
            }
            UnfocusCurrentWindow();
            IWindowViewModel windowToOpen = windowsDictionary[windowType];
            windowToOpen.OpenWindow(windowArgs);
            windowsHistory.Push(windowType);
        }

        void FocusCurrentWindow()
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
    }
}