using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class PointerUpTrigger : MonoBehaviour, IPointerUpHandler
    {
        [SerializeField] PointerEventData.InputButton buttonToReact =  PointerEventData.InputButton.Left;
        [SerializeField] List<EventToTrigger> eventsToTrigger;
        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != buttonToReact)
            {
                return;
            }
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