using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PSkrzypa.ObservableSelectables
{
    public class ObservableSlider : Slider, IObservableSelectable
    {
        public UnityAction<CustomSelectionState> onStateChanged;
        [SerializeField] float sensivity = 0.1f;
        /*[ShowIf("wholeNumbers")]*/[SerializeField] private float repeatDelay = 0.3f;
        /*[ShowIf("wholeNumbers")]*/[SerializeField] private float minimumRepeatDelay = 0.1f;
        /*[ShowIf("wholeNumbers")]*/[SerializeField] private float speedUpStep = 0.05f;
        /*[ShowIf("wholeNumbers")]*/[SerializeField] private int speedUpAfterXRepeats = 2;
        private float currentRepeatDelay;
        private int repeats;
        private float lastValueChangeTime;
        private bool reactToOnPointerDown;

        public void ChangeSliderValue(float axisValue)
        {
            if (axisValue == 0)
            {
                repeats = 0;
                lastValueChangeTime = 0;
                return;
            }
            if (CheckRepeatDelay() || !wholeNumbers)
            {
                float currentValue = value;
                currentValue += wholeNumbers ? Mathf.Sign(axisValue) * 1 : axisValue * sensivity;
                Set(currentValue);
            }
        }
        bool CheckRepeatDelay()
        {
            float timeSinceStartUp = Time.time;
            if (timeSinceStartUp - lastValueChangeTime > 0.5f)
            {
                repeats = 0;
                currentRepeatDelay = repeatDelay;
            }
            bool result = currentRepeatDelay <= timeSinceStartUp - lastValueChangeTime;
            if (result)
            {
                repeats += 1;
                lastValueChangeTime = timeSinceStartUp;
                if (repeats % speedUpAfterXRepeats == 0 && currentRepeatDelay > minimumRepeatDelay)
                {
                    currentRepeatDelay -= speedUpStep;
                }
            }
            return result;
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
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (reactToOnPointerDown)
            {
                base.OnPointerDown(eventData);
            }
        }
        public void OnInputDeviceChange(bool mouseAndKeyboard)
        {
            reactToOnPointerDown = mouseAndKeyboard;
        }
    }
}