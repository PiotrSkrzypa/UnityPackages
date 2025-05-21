using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class SubmitTrigger : MonoBehaviour, ISubmitHandler
    {
        [SerializeField] List<EventToTrigger> eventsToTrigger;
        public void OnSubmit(BaseEventData eventData)
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