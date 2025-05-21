using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PSkrzypa.ObservableSelectables
{
    public class ObservableButton : Button, IObservableSelectable
    {
        public UnityAction<CustomSelectionState> onStateChanged;
        bool isPointerInside;
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
                    if (isPointerInside)
                    {
                        DoStateTransition(CustomSelectionState.DisabledSelected, instant);
                    }
                    else
                    {
                        DoStateTransition(CustomSelectionState.Disabled, instant);
                    }
                    break;
            }
        }
        public void DoStateTransition(CustomSelectionState state, bool instant)
        {
            onStateChanged?.Invoke(state);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            isPointerInside = true;
            ModifiedEvaluateAndTransitionToSelectionState();
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            isPointerInside = false;
            ModifiedEvaluateAndTransitionToSelectionState();
        }
        private void ModifiedEvaluateAndTransitionToSelectionState()
        {
            if (!IsActive())
                return;
            DoStateTransition(currentSelectionState, false);
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

        public void SimulateOnClick()
        {
            EventSystem eventSystem = EventSystem.current;
            ExecuteEvents.Execute(gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerDownHandler);
            ExecuteEvents.Execute(gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerUpHandler);
            ExecuteEvents.Execute(gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerClickHandler);
        }
    }
}