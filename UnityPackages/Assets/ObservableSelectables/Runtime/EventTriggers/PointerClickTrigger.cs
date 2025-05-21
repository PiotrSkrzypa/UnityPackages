using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class PointerClickTrigger : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] PointerEventData.InputButton buttonToReact =  PointerEventData.InputButton.Left;
        [SerializeField] List<EventToTrigger> eventsToTrigger;
        public void OnPointerClick(PointerEventData eventData)
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
        public void SetButtonToReact(PointerEventData.InputButton inputButton)
        {
            buttonToReact = inputButton;
        }
    }
}