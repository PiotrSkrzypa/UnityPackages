using UnityEngine.Events;

namespace PSkrzypa.ObservableSelectables
{
    public interface IObservableSelectable
    {
        void DoStateTransition(CustomSelectionState state, bool instant);
        void Subscribe(UnityAction<CustomSelectionState> unityAction);
        void Unsubscribe(UnityAction<CustomSelectionState> unityAction);
        CustomSelectionState GetSelectionState();
    }
    public enum CustomSelectionState
    {
        Normal = 0,
        Highlighted = 1,
        Pressed = 2,
        Selected = 3,
        Disabled = 4,
        DisabledSelected = 5
    }
}