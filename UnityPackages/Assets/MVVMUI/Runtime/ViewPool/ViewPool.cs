using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PSkrzypa.MVVMUI
{
    public class ViewPool
    {
        private readonly Stack<IWindowView> _pool = new Stack<IWindowView>();
        private readonly WindowFactory _factory;
        private readonly MenuWindowConfig _config;
        private readonly GameObject _rootObject;
        private ConditionalWeakTable<IWindowViewModel, IWindowView> _activeViews = new ();
        public ViewPool(WindowFactory factory, MenuWindowConfig config, GameObject rootObject)
        {
            _factory = factory;
            _config = config;
            _rootObject = rootObject;
            // prepopulate pool
            for (int i = 0; i < config.initialPoolSize; i++)
            {
                var (vm, view) = _factory.Create(config, rootObject);
                (view as MonoBehaviour).gameObject.SetActive(false);
                _pool.Push(view);
                _activeViews.Add(vm, view);
            }
        }

        public IWindowView Get()
        {
            IWindowViewModel vm;
            if (_pool.Count > 0)
            {
                var view = _pool.Pop();
                vm = _factory.ResolveViewModel(_config, view);
                ( view as MonoBehaviour ).gameObject.SetActive(true);
                _activeViews.AddOrUpdate(vm, view);
                return view;
            }

            var (newVm, newView) = _factory.Create(_config, _rootObject);
            vm = newVm;
            ( newView as MonoBehaviour ).gameObject.SetActive(true);
            _activeViews.AddOrUpdate(vm, newView);
            return newView;
        }

        public void Release(IWindowViewModel viewModel)
        {
            // deactivate and store
            if(!_activeViews.TryGetValue(viewModel, out var view))
            {
                Debug.LogWarning($"Trying to release view for ViewModel {viewModel.GetType().Name} but it wasn't found in active views");
                return;
            }
            ( view as MonoBehaviour ).gameObject.SetActive(false);
            _pool.Push(view);
        }
    }

}