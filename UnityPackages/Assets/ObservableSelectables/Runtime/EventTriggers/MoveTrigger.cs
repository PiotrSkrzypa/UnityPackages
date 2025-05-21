using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class MoveTrigger : MonoBehaviour, IMoveHandler
    {
        [SerializeField] List<EventToTrigger> eventsToTrigger;
        public void OnMove(AxisEventData eventData)
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