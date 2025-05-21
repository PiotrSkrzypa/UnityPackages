using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PSkrzypa.ObservableSelectables
{
    public class ObservableScrollbar : Scrollbar, IObservableSelectable
    {
        public UnityAction<CustomSelectionState> onStateChanged;
        [SerializeField] float sensivity = 0.1f;
        public void ChangeScrollbarValue(float axisValue)
        {
            if (axisValue == 0)
            {
                return;
            }
            float currentValue = value;
            currentValue += axisValue * sensivity;
            value = currentValue;
            value = Mathf.Clamp(value, 0f, 1f);
        }
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
    }
}