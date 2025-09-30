using System;
using System.Collections.Generic;
using UnityEngine;
using PSkrzypa.EventBus;

namespace PSkrzypa.MVVMUI
{
    public class MenuController : IMenuController, IDisposable
    {

        [SerializeField] private Stack<string> _windowsHistory = new Stack<string>();

        private Dictionary<string, IWindowViewModel> _windowsDictionary = new Dictionary<string, IWindowViewModel>();
        private IEventBus _eventBus;

        public MenuController(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void Dispose()
        {
            CloseAllWindows();
            _windowsDictionary.Clear();
            _windowsHistory.Clear();
        }

        public void RegisterWindow(IWindowViewModel windowViewModel)
        {
            if (!_windowsDictionary.ContainsKey(windowViewModel.MenuWindowConfig.windowID))
            {
                // TODO give window unique ID
                _windowsDictionary.Add(windowViewModel.MenuWindowConfig.windowID, windowViewModel);
                return;
            }
            else
            {
                Debug.LogWarning($"Failed adding {windowViewModel.MenuWindowConfig.windowID} to Windows Dictionary, already contains window of type {windowViewModel.MenuWindowConfig}");
                return;
            }
        }

        public void DeregisterWindow(IWindowViewModel windowViewModel)
        {
            if (_windowsDictionary.ContainsKey(windowViewModel.MenuWindowConfig.windowID))
            {
                _windowsDictionary.Remove(windowViewModel.MenuWindowConfig.windowID);
            }
            else
            {
                Debug.LogWarning($"Failed removing {windowViewModel.MenuWindowConfig.windowID} from Windows Dictionary, it doesn't contain window of type {windowViewModel.MenuWindowConfig}");
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
            if (_windowsHistory.Count == 0)
            {
                return;
            }
            string windowType = _windowsHistory.Peek();
            if (!_windowsDictionary.ContainsKey(windowType))
            {
                Debug.LogWarning($"The key <b>{windowType}</b> doesn't exist so you can't deactivate the menu!");
                return;
            }
            IWindowViewModel windowToClose = _windowsDictionary[windowType];
            if (windowToClose.MenuWindowConfig.windowID != windowID)
            {
                return;
            }
            if (windowToClose.MenuWindowConfig.isInitialScreen || !windowToClose.MenuWindowConfig.canBeClosed)
            {
                return;
            }
            _windowsHistory.Pop();
            windowToClose.CloseWindow();
            FocusCurrentWindow();
        }

        public void CloseAllWindows()
        {
            while (_windowsHistory.Count > 0)
            {
                string windowName = _windowsHistory.Pop();
                if (_windowsDictionary.TryGetValue(windowName, out IWindowViewModel windowViewModel))
                {
                    windowViewModel.CloseWindow();
                }
            }
            _windowsHistory.Clear();
        }

        void OpenWindowAdditive(string windowType, IWindowArgs windowArgs)
        {
            if (!_windowsDictionary.ContainsKey(windowType))
            {
                Debug.LogWarning($"The key <b>{windowType}</b> doesn't exist so you can't activate the menu!");
                return;
            }
            UnfocusCurrentWindow();
            IWindowViewModel windowToOpen = _windowsDictionary[windowType];
            windowToOpen.OpenWindow(windowArgs);
            _windowsHistory.Push(windowType);
        }

        void OpenWindowExclusive(string windowType, IWindowArgs windowArgs)
        {
            if (!_windowsDictionary.ContainsKey(windowType))
            {
                Debug.LogWarning($"The key <b>{windowType}</b> doesn't exist so you can't activate the menu!");
                return;
            }
            UnfocusCurrentWindow();
            IWindowViewModel windowToOpen = _windowsDictionary[windowType];
            windowToOpen.OpenWindow(windowArgs);
            _windowsHistory.Push(windowType);
        }

        void FocusCurrentWindow()
        {
            if (_windowsHistory.Count <= 0)
            {
                return;
            }
            string windowType = _windowsHistory.Peek();
            if (_windowsDictionary.TryGetValue(windowType, out IWindowViewModel windowToUnfocus))
            {
                windowToUnfocus.GainFocus();
            }
        }

        void UnfocusCurrentWindow()
        {
            if (_windowsHistory.Count <= 0)
            {
                return;
            }
            string windowType = _windowsHistory.Peek();
            if (_windowsDictionary.TryGetValue(windowType, out IWindowViewModel windowToUnfocus))
            {
                windowToUnfocus.LooseFocus();
            }
        }
    }
}