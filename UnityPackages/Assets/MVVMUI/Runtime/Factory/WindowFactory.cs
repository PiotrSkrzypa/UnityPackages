using System;
using UnityEngine;

namespace PSkrzypa.MVVMUI
{
    public class WindowFactory
    {
        private readonly IResolver _resolver;

        public WindowFactory(IResolver resolver)
        {
            _resolver = resolver;
        }

        public (BaseViewModel vm, IWindowView view) Create(MenuWindowConfig config, GameObject rootObject)
        {
            var vmType = config.viewModelType.StoredType;
            // container decides how to inject
            var viewModel = (BaseViewModel)_resolver.Resolve(vmType); 

            var view = UnityEngine.Object.Instantiate(config.windowPrefab, rootObject.transform)
                          .GetComponent<IWindowView>();

            // Bind via reflection
            BindViewModelToView(view, viewModel);

            return (viewModel, view);
        }

        public BaseViewModel ResolveViewModel(MenuWindowConfig config, IWindowView windowView)
        {
            var vmType = config.viewModelType.StoredType;
            // container decides how to inject
            var viewModel = (BaseViewModel)_resolver.Resolve(vmType);
            BindViewModelToView(windowView, viewModel);
            return viewModel;
        }

        private void BindViewModelToView(IWindowView view, BaseViewModel vm)
        {
            var viewType = view.GetType();
            var viewModelType = vm.GetType();
            var method = viewType.GetMethod("BindViewModel", new[] { viewModelType });

            if (method != null)
            {
                method.Invoke(view, new[] { vm });
            }
            else
            {
                throw new InvalidOperationException(
                    $"View {viewType.Name} does not support binding {vm.GetType().Name}");
            }
        }
    }

}