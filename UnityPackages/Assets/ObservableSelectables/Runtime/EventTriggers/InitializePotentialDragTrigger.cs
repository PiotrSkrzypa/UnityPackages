using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class InitializePotentialDragTrigger : MonoBehaviour, IInitializePotentialDragHandler
    {
        [SerializeField] List<EventToTrigger> eventsToTrigger;
        public void OnInitializePotentialDrag(PointerEventData eventData)
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