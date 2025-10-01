using System;
using System.Collections.Generic;
using UnityEngine;
using PSkrzypa.EventBus;

namespace PSkrzypa.MVVMUI
{
    public class MenuController : IMenuController, IDisposable
    {

        [SerializeField] private Stack<IWindowViewModel> _windowsHistory = new Stack<IWindowViewModel>();

        private Dictionary<string, IWindowViewModel> _singletonWindows = new Dictionary<string, IWindowViewModel>();
        private Dictionary<string, ViewPool> _multipleWindows = new Dictionary<string, ViewPool>();
        private IEventBus _eventBus;
        private WindowFactory _windowFactory;
        private GameObject _windowsRoot;

        public MenuController(WindowFactory windowFactory, IEventBus eventBus, GameObject windowsRoot)
        {
            _windowFactory = windowFactory;
            _eventBus = eventBus;
            _windowsRoot = windowsRoot;
        }

        public void RegisterWindows(IEnumerable<MenuWindowConfig> configs)
        {
            configs.ForEach(config =>
            {
                if (!config.allowMultipleInstances)
                {
                    if (!_singletonWindows.ContainsKey(config.windowID))
                    {
                        var (vm, view) = _windowFactory.CreateOrFind(config, _windowsRoot);

                        _singletonWindows[config.windowID] = vm;
                    }
                }
                else
                {
                    if (!_multipleWindows.ContainsKey(config.windowID))
                    {
                        var viewPool = new ViewPool(_windowFactory, config, _windowsRoot);
                        _multipleWindows[config.windowID] = viewPool;
                    }
                }
            });
        }
        public void Dispose()
        {
            CloseAllWindows();
            _singletonWindows.Clear();
            _windowsHistory.Clear();
        }

        public void RegisterWindow(IWindowViewModel windowViewModel)
        {
            if (windowViewModel.MenuWindowConfig.allowMultipleInstances)
            {

            }
            if (_singletonWindows.TryAdd(windowViewModel.MenuWindowConfig.windowID, windowViewModel))
            {
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
            if (!_singletonWindows.Remove(windowViewModel.MenuWindowConfig.windowID))
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
        public void CloseActiveWindow()
        {
            if (_windowsHistory.Count == 0)
            {
                return;
            }
            IWindowViewModel windowToClose = _windowsHistory.Pop();
            if (windowToClose.MenuWindowConfig.isInitialScreen || !windowToClose.MenuWindowConfig.canBeClosed)
            {
                return;
            }
            windowToClose.CloseWindow();
            if(windowToClose.MenuWindowConfig.allowMultipleInstances && _multipleWindows.TryGetValue(windowToClose.MenuWindowConfig.windowID, out ViewPool viewPool))
            {
                viewPool.Release(windowToClose);
            }
            FocusCurrentWindow();
        }
        public void CloseWindow(IWindowViewModel windowVM)
        {
            if (_windowsHistory.Count == 0)
            {
                return;
            }
            IWindowViewModel activeWindow = _windowsHistory.Peek();
            if (activeWindow != windowVM)
            {
                Debug.LogWarning($"Trying to close active window from other window");
                return;
            }
            IWindowViewModel windowToClose = windowVM;
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
                IWindowViewModel windowVM = _windowsHistory.Pop();
                windowVM.CloseWindow();
            }
            _windowsHistory.Clear();
        }

        void OpenWindowAdditive(string windowType, IWindowArgs windowArgs)
        {
            if (!_singletonWindows.ContainsKey(windowType) && !_multipleWindows.ContainsKey(windowType))
            {
                Debug.LogWarning($"The key <b>{windowType}</b> doesn't exist so you can't activate the menu!");
                return;
            }
            UnfocusCurrentWindow();
            if (_singletonWindows.TryGetValue(windowType, out IWindowViewModel singletonWindow))
            {
                singletonWindow.OpenWindow(windowArgs);
                _windowsHistory.Push(singletonWindow);
                return;
            }
            if (_multipleWindows.TryGetValue(windowType, out ViewPool viewPool))
            {
                IWindowViewModel windowInstance = viewPool.Get().GetBoundViewModel();
                windowInstance.OpenWindow(windowArgs);
                _windowsHistory.Push(windowInstance);
                return;
            }
        }

        void OpenWindowExclusive(string windowType, IWindowArgs windowArgs)
        {
            OpenWindowAdditive(windowType, windowArgs);
        }

        void FocusCurrentWindow()
        {
            if (_windowsHistory.Count <= 0)
            {
                return;
            }
            IWindowViewModel windowToFocus = _windowsHistory.Peek();
            windowToFocus.GainFocus();
        }

        void UnfocusCurrentWindow()
        {
            if (_windowsHistory.Count <= 0)
            {
                return;
            }
            IWindowViewModel windowToUnfocus = _windowsHistory.Peek();
            windowToUnfocus.LooseFocus();
        }
    }
}