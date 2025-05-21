using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class BeginDragTrigger : MonoBehaviour, IBeginDragHandler
    {
        [SerializeField] List<EventToTrigger> eventsToTrigger;
        public void OnBeginDrag(PointerEventData eventData)
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