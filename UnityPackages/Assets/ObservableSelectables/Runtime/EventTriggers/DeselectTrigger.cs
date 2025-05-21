using System.Collections.Generic;
using PSkrzypa.ObservableSelectables.EventTriggers;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PBG.UI
{
    public class DeselectTrigger : MonoBehaviour, IDeselectHandler
    {
        [SerializeField] List<EventToTrigger> eventsToTrigger;
        public void OnDeselect(BaseEventData eventData)
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