using System.Collections;
using PSkrzypa.MVVMUI.Input;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Zenject;

namespace PSkrzypa.ObservableSelectables
{
    public class ObservableTMPInputField : TMP_InputField, IObservableSelectable
    {
        UnityAction<CustomSelectionState> onStateChanged;
        [Inject] InputDeviceObserver _inputDeviceObserver;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            switch (state)
            {
                case SelectionState.Normal:
                    DoStateTransition(CustomSelectionState.Normal, instant);
                    break;
                case SelectionState.Highlighted:
                    DoStateTransition(CustomSelectionState.Highlighted, instant);
                    break;
                case SelectionState.Pressed:
                    DoStateTransition(CustomSelectionState.Pressed, instant);
                    break;
                case SelectionState.Selected:
                    DoStateTransition(CustomSelectionState.Selected, instant);
                    break;
                case SelectionState.Disabled:
                    DoStateTransition(CustomSelectionState.Disabled, instant);
                    break;
            }
        }
        public void DoStateTransition(CustomSelectionState state, bool instant)
        {
            onStateChanged?.Invoke(state);
        }

        public void Subscribe(UnityAction<CustomSelectionState> unityAction)
        {
            onStateChanged += unityAction;
        }

        public void Unsubscribe(UnityAction<CustomSelectionState> unityAction)
        {
            onStateChanged -= unityAction;
        }

        public CustomSelectionState GetSelectionState()
        {
            switch (currentSelectionState)
            {
                case SelectionState.Normal:
                    return CustomSelectionState.Normal;
                case SelectionState.Highlighted:
                    return CustomSelectionState.Highlighted;
                case SelectionState.Pressed:
                    return CustomSelectionState.Pressed;
                case SelectionState.Selected:
                    return CustomSelectionState.Selected;
                case SelectionState.Disabled:
                    return CustomSelectionState.Disabled;
                default:
                    return CustomSelectionState.Normal;
            }
        }
        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            SendOnFocus();

            if (_inputDeviceObserver.ActiveDevice == InputDeviceType.MouseAndKeyboard)
            {
                ActivateInputField();
            }
            else
            {
                StartCoroutine(OpenVirtualKeyboardNextFrame());
            }
        }
        IEnumerator OpenVirtualKeyboardNextFrame()
        {
            yield return null;
            //GlobalEventBus<OpenVirtualKeyboard>.Raise(new OpenVirtualKeyboard { inputField = this });
        }
    }
}