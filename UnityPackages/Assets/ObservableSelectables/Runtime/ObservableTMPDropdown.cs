using System.Collections.Generic;
using System.Linq;
using PSkrzypa.MVVMUI.Navigation;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables
{
    public class ObservableTMPDropdown : TMP_Dropdown, IObservableSelectable
    {
        public UnityAction<CustomSelectionState> onStateChanged;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            List<DropdownItem> items = GetComponentsInChildren<DropdownItem>().ToList();
            GetComponentInParent<NavigationGroup>()?.SelectElement(items[0].toggle);
        }
        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            List<DropdownItem> items = GetComponentsInChildren<DropdownItem>().ToList();
            GetComponentInParent<NavigationGroup>()?.SelectElement(items[0].toggle);
        }
        protected override void DestroyDropdownList(GameObject dropdownList)
        {
            base.DestroyDropdownList(dropdownList);
            GetComponentInParent<NavigationGroup>()?.SelectElement(this);
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