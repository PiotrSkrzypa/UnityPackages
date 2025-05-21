using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    [Serializable]
    public class EventToTrigger : UnityEvent<BaseEventData>
    {
    }
}