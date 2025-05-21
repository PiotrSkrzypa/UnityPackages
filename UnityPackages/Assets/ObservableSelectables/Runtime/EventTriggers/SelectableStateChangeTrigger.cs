using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class SelectableStateChangeTrigger : MonoBehaviour
    {
        [SerializeField] Dictionary<CustomSelectionState, List<EventToTrigger>> eventsToTrigger;
        [SerializeField] IObservableSelectable observableSelectable;

        private void OnEnable()
        {
            if (observableSelectable == null)
            {
                observableSelectable = GetComponent<IObservableSelectable>();
            }
            if (observableSelectable != null)
            {
                observableSelectable.Subscribe(OnSelectStateChange);
                OnSelectStateChange(observableSelectable.GetSelectionState());
            }
        }
        private void OnDisable()
        {
            observableSelectable.Unsubscribe(OnSelectStateChange);
        }
        void OnSelectStateChange(CustomSelectionState selectionState)
        {
            if (eventsToTrigger.TryGetValue(selectionState, out List<EventToTrigger> foundEvents))
            {
                if (foundEvents == null)
                {
                    return;
                }
                for (int i = 0; i < foundEvents.Count; i++)
                {
                    foundEvents[i]?.Invoke(new BaseEventData(EventSystem.current));
                }
            }
        }
    }
}