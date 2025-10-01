using System;
using R3;
using UnityEngine;
using UnityEngine.Events;
using Alchemy.Inspector;
using PSkrzypa.UnityFX;
using PSkrzypa.MVVMUI.Input;

namespace PSkrzypa.MVVMUI
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(RectTransform))]
    [RequireComponent(typeof(ActionMap))]
    public abstract class BaseWindowView<T> : MonoBehaviour, IWindowView<T> where T : BaseViewModel
    {
        [FoldoutGroup("Open Animation")][SerializeField]protected FXSequence openAnimation;
        [FoldoutGroup("Close Animation")][SerializeField]protected FXSequence closeAnimation;

        public GameObject ViewGameObject => gameObject;

        protected T viewModel;
        Canvas windowCanvas;
        CanvasGroup windowCanvasGroup;
        RectTransform rectTransform;
        ActionMap actionMap;


        IDisposable disposable;


        public void BindViewModel(T viewModel)
        {
            if(this.viewModel != null)
            {
                disposable?.Dispose();
                OnDispose();
                Debug.LogWarning($"View {gameObject.name} has already bound view model, DISPOSING before binding new one");
                return;
            }
            this.viewModel = viewModel;
            var d = Disposable.CreateBuilder();
            viewModel.openCommand.Subscribe(_ => OpenView()).AddTo(ref d);
            viewModel.closeCommand.Subscribe(_ => CloseView()).AddTo(ref d);
            disposable = d.Build();
            OnViewModelBind();
        }

        public IWindowViewModel GetBoundViewModel()
        {
            return viewModel;
        }
        void Awake()
        {
            windowCanvas = GetComponent<Canvas>();
            windowCanvasGroup = GetComponent<CanvasGroup>();
            windowCanvas.enabled = false;
            windowCanvasGroup.alpha = 0;
            rectTransform = GetComponent<RectTransform>();
            actionMap = GetComponent<ActionMap>();
            actionMap.enabled = false;
        }

        void OnDestroy()
        {
            disposable?.Dispose();
            OnDispose();
        }

        protected virtual void OnViewModelBind()
        {
            var d = viewModel.HasFocus.Subscribe(hasFocus => actionMap.enabled = hasFocus);
            disposable = Disposable.Combine(disposable, d);
            if (viewModel.MenuWindowConfig.isInitialScreen)
            {
                viewModel.OpenWindow();
            }
        }

        protected virtual void OnDispose()
        {
        }

        public virtual void OpenView()
        {
            if (windowCanvas != null)
            {
                windowCanvas.enabled = true;
            }
            if (closeAnimation != null)
            {
                closeAnimation.Stop();
            }
            SetViewInteractable(true);
            if (openAnimation != null)
            {
                openAnimation.Play();

            }
        }
        public virtual void CloseView()
        {
            if (openAnimation != null)
            {
                openAnimation.Stop();
            }
            if (actionMap != null)
            {
                actionMap.enabled = false;
            }
            if (closeAnimation != null)
            {
                closeAnimation.OnCompleteEvent.AddListener(OnCloseAnimationComplete());
                closeAnimation.Play();
            }
            else
            {
                OnCloseAnimationComplete().Invoke();
            }
        }

        private UnityAction OnCloseAnimationComplete()
        {
            return () =>
            {
                SetViewInteractable(false);
                if (windowCanvas != null)
                {
                    windowCanvas.enabled = false;
                }
            };
        }

        public virtual void SetViewInteractable(bool value)
        {
            if (windowCanvasGroup != null)
            {
                windowCanvasGroup.interactable = value;
            }
            if (actionMap != null)
            {
                actionMap.enabled = value;
            }
        }

        public void DestroyView()
        {
            disposable?.Dispose();
            OnDispose();
            Destroy(gameObject);
        }
    }
}