using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class ActiveStateTrigger : MonoBehaviour
    {
        [SerializeField] List<EventToTrigger> eventsToTriggerOnEnable;
        [SerializeField] List<EventToTrigger> eventsToTriggerOnDisable;
        public void OnEnable()
        {
            if (eventsToTriggerOnEnable != null)
            {
                for (int i = 0; i < eventsToTriggerOnEnable.Count; i++)
                {
                    eventsToTriggerOnEnable[i]?.Invoke(new BaseEventData(EventSystem.current));
                }
            }
        }
        public void OnDisable()
        {
            if (eventsToTriggerOnDisable != null)
            {
                for (int i = 0; i < eventsToTriggerOnDisable.Count; i++)
                {
                    eventsToTriggerOnDisable[i]?.Invoke(new BaseEventData(EventSystem.current));
                }
            }
        }
    }
}