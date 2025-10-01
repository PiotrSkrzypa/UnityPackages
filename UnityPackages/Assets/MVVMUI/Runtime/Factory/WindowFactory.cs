using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

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

            var view = Object.Instantiate(config.windowPrefab, rootObject.transform)
                          .GetComponent<IWindowView>();

            // Bind via reflection
            BindViewModelToView(view, viewModel);

            return (viewModel, view);
        }

        public (BaseViewModel vm, IWindowView view) CreateOrFind(MenuWindowConfig config, GameObject rootObject)
        {
            var vmType = config.viewModelType.StoredType;
            // container decides how to inject
            var viewModel = (BaseViewModel)_resolver.Resolve(vmType);

            IWindowView existingView = FindExistingViews(config, rootObject, true).FirstOrDefault();
            var view = existingView ?? Object.Instantiate(config.windowPrefab, rootObject.transform)
                          .GetComponent<IWindowView>();

            // Bind via reflection
            BindViewModelToView(view, viewModel);

            return (viewModel, view);
        }

        public IWindowView[] FindExistingViews(MenuWindowConfig config, GameObject rootObject, bool destroyDuplicates)
        {
            IWindowView prefabView = config.windowPrefab.GetComponent<IWindowView>();
            Type prefabViewType = prefabView.GetType();
            IWindowView[] existingViews = rootObject.GetComponentsInChildren<IWindowView>(true);
            existingViews = existingViews.Where(v => v.GetType() == prefabViewType).ToArray();
            for (int i = existingViews.Length - 1; i >= 0; i--)
            {
                IWindowView ev = existingViews[i];
                if (i > 0 && destroyDuplicates)
                {
                    existingViews[i] = null;
                    ev.DestroyView();
                }
            }
            existingViews = existingViews.Length == 1 ? new IWindowView[] { existingViews[0] } : existingViews;
            return existingViews;
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