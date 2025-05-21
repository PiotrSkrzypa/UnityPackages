using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class PointerEnterTrigger : MonoBehaviour, IPointerEnterHandler
    {
        [SerializeField] List<EventToTrigger> eventsToTrigger;
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventsToTrigger != null)
            {
                for (int i = 0; i < eventsToTrigger.Count; i++)
                {
                    eventsToTrigger[i]?.Invoke(eventData);
                }
            }
        }
        public void AddListner(EventToTrigger eventToTrigger)
        {
            if (eventsToTrigger == null)
            {
                eventsToTrigger = new List<EventToTrigger>();
            }
            eventsToTrigger.Add(eventToTrigger);
        }
        public void RemoveListener(EventToTrigger eventToTrigger)
        {
            if (eventsToTrigger == null)
            {
                return;
            }
            eventsToTrigger.Remove(eventToTrigger);
        }
    }
}