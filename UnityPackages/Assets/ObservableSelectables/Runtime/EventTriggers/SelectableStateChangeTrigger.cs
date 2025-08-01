using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class SelectableStateChangeTrigger : MonoBehaviour
    {
        [SerializeField]List<EventsToTriggerOnState> eventsToTrigger;
        [SerializeField]IObservableSelectable observableSelectable;
        Dictionary<CustomSelectionState, List<EventToTrigger>> eventsToTriggerDict;
        CustomSelectionState lastSelectionState = CustomSelectionState.Normal;

        private void Awake()
        {
            eventsToTriggerDict = new Dictionary<CustomSelectionState, List<EventToTrigger>>();
            for (int i = 0; i < eventsToTrigger.Count; i++)
            {
                eventsToTriggerDict.TryAdd(eventsToTrigger[i].CustomSelectionState, eventsToTrigger[i].EventsList);
            }
        }

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
            if (lastSelectionState == selectionState)
                return;
            if (eventsToTriggerDict.TryGetValue(selectionState, out List<EventToTrigger> foundEvents))
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
            lastSelectionState = selectionState;
        }
    }
    [Serializable]
    public class EventsToTriggerOnState
    {
        public CustomSelectionState CustomSelectionState;
        public List<EventToTrigger> EventsList;
    }
}