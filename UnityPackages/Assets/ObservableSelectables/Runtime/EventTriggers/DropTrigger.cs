using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class DropTrigger : MonoBehaviour, IDropHandler
    {
        [SerializeField] List<EventToTrigger> eventsToTrigger;
        public void OnDrop(PointerEventData eventData)
        {
            if (eventsToTrigger != null)
            {
                for (int i = 0; i < eventsToTrigger.Count; i++)
                {
                    eventsToTrigger[i]?.Invoke(eventData);
                }
            }
        }
    }
}