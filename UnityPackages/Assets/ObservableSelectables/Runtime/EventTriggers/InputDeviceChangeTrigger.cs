using System.Collections.Generic;
using PSkrzypa.EventBus;
using PSkrzypa.MMVMUI.Input.Events;
using PSkrzypa.MVVMUI.Input;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class InputDeviceChangeTrigger : MonoBehaviour, IEventListener<InputDeviceChangedEvent>
    {
        [SerializeField] List<InputDeviceType> observedInputDevices;
        [SerializeField] List<EventToTrigger> eventsOnExpectedDeviceDetected;
        [SerializeField] List<EventToTrigger> eventsOnUnexpectedDeviceDetected;

        InputDeviceObserver inputDeviceObserver;

        private void Awake()
        {
            GlobalEventBus<InputDeviceChangedEvent>.Register(this);
            if (inputDeviceObserver != null)
            {
                OnInputDeviceChange(inputDeviceObserver.ActiveDevice);
            }
            else
            {
                OnInputDeviceChange(InputDeviceType.MouseAndKeyboard);
            }
        }
        private void OnDestroy()
        {
            GlobalEventBus<InputDeviceChangedEvent>.Deregister(this);
        }
        public void OnEvent(InputDeviceChangedEvent @event)
        {
            OnInputDeviceChange(@event.inputDeviceType);
        }
        void OnInputDeviceChange(InputDeviceType inputDeviceType)
        {
            if (observedInputDevices == null)
            {
                enabled = false;
                return;
            }
            if (observedInputDevices.Contains(inputDeviceType))
            {
                if (eventsOnExpectedDeviceDetected != null)
                {
                    for (int i = 0; i < eventsOnExpectedDeviceDetected.Count; i++)
                    {
                        eventsOnExpectedDeviceDetected[i]?.Invoke(new BaseEventData(EventSystem.current));
                    }
                }
            }
            else
            {
                if (eventsOnUnexpectedDeviceDetected != null)
                {
                    for (int i = 0; i < eventsOnUnexpectedDeviceDetected.Count; i++)
                    {
                        eventsOnUnexpectedDeviceDetected[i]?.Invoke(new BaseEventData(EventSystem.current));
                    }
                }
            }
        }
    }
}