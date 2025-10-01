using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PSkrzypa.MVVMUI
{
    public class ViewPool
    {
        private readonly WindowFactory _factory;
        private readonly MenuWindowConfig _config;
        private readonly GameObject _rootObject;
        private readonly Stack<IWindowView> _pool = new Stack<IWindowView>();
        private ConditionalWeakTable<IWindowViewModel, IWindowView> _viewsByViewModel = new ();

        public ViewPool(WindowFactory factory, MenuWindowConfig config, GameObject rootObject)
        {
            _factory = factory;
            _config = config;
            _rootObject = new GameObject($"{config.windowPrefab.name} ViewPool");
            _rootObject.transform.SetParent(rootObject.transform);
            // find existing views in the scene and add them to the pool
            IWindowView[] existingViews = _factory.FindExistingViews(config, rootObject, false);
            for(int i = 0; i < existingViews.Length; i++)
            {
                var view = existingViews[i];
                view.ViewGameObject.transform.SetParent(_rootObject.transform);
                var vm = _factory.ResolveViewModel(config, view);
                _pool.Push(view);
                _viewsByViewModel.Add(vm, view);
            }
            // prepopulate pool
            int existingCount = _pool.Count;
            for (int i = existingCount; i < config.initialPoolSize; i++)
            {
                var (vm, view) = _factory.Create(config, _rootObject);
                _pool.Push(view);
                _viewsByViewModel.Add(vm, view);
            }
        }

        public IWindowView Get()
        {
            IWindowViewModel vm;
            if (_pool.Count > 0)
            {
                var view = _pool.Pop();
                if (view.GetBoundViewModel() == null)
                {
                    vm = _factory.ResolveViewModel(_config, view);
                    _viewsByViewModel.AddOrUpdate(vm, view);
                }
                return view;
            }

            var (newVm, newView) = _factory.Create(_config, _rootObject);
            vm = newVm;
            _pool.Push(newView);
            _viewsByViewModel.AddOrUpdate(vm, newView);
            return newView;
        }

        public void Release(IWindowViewModel viewModel)
        {
            // deactivate and store
            if (!_viewsByViewModel.TryGetValue(viewModel, out var view))
            {
                Debug.LogWarning($"Trying to release view for ViewModel {viewModel.GetType().Name} but it wasn't found in active views");
                return;
            }
            _pool.Push(view);
        }
    }

}