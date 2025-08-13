using System;
using DG.Tweening;
using R3;
using UnityEngine;
using Zenject;
using PSkrzypa.UnityFX;
using PSkrzypa.EventBus;
using PSkrzypa.MVVMUI.Input;
using UnityEngine.Events;
using Alchemy.Inspector;
namespace PSkrzypa.MVVMUI.BaseMenuWindow
{
    [RequireComponent(typeof(Canvas), typeof(CanvasGroup), typeof(RectTransform))]
    [RequireComponent(typeof(ActionMap))]
    public abstract class BaseWindowView<T> : MonoBehaviour, IWindowView where T : BaseViewModel
    {
        [FoldoutGroup("Open Animation")][SerializeField]protected FXSequence openAnimation;
        [FoldoutGroup("Close Animation")][SerializeField]protected FXSequence closeAnimation;

        [Inject] protected T viewModel;
        Canvas windowCanvas;
        CanvasGroup windowCanvasGroup;
        RectTransform rectTransform;
        ActionMap actionMap;


        IDisposable disposable;

        protected virtual void Awake()
        {
            var d = Disposable.CreateBuilder();
            windowCanvas = GetComponent<Canvas>();
            windowCanvasGroup = GetComponent<CanvasGroup>();
            windowCanvas.enabled = false;
            windowCanvasGroup.alpha = 0;
            rectTransform = GetComponent<RectTransform>();
            actionMap = GetComponent<ActionMap>();
            actionMap.enabled = false;
            viewModel.openCommand.Subscribe(_ => OpenView()).AddTo(ref d);
            viewModel.closeCommand.Subscribe(_ => CloseView()).AddTo(ref d);
            viewModel.HasFocus.Subscribe(hasFocus => actionMap.enabled = hasFocus).AddTo(ref d);
            disposable = d.Build();
            if (viewModel.MenuWindowConfig.isInitialScreen)
            {
                GlobalEventBus<OpenWindowEvent>.Raise(new OpenWindowEvent { windowID = viewModel.MenuWindowConfig.windowID, isExclusive = true });
            }
        }
        protected virtual void OnDestroy()
        {
            disposable?.Dispose();
            windowCanvasGroup.DOKill();
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
                GlobalEventBus<WindowClosedEvent>.Raise(new WindowClosedEvent() { windowID = "" });
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
    }
}