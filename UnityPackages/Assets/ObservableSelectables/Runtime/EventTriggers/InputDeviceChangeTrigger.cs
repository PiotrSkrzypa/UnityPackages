using System.Collections.Generic;
using PSkrzypa.EventBus;
using PSkrzypa.MVVMUI.Input.Events;
using PSkrzypa.MVVMUI.Input;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class InputDeviceChangeTrigger : MonoBehaviour
    {
        [SerializeField] List<InputDeviceType> _observedInputDevices;
        [SerializeField] List<EventToTrigger> _eventsOnExpectedDeviceDetected;
        [SerializeField] List<EventToTrigger> _eventsOnUnexpectedDeviceDetected;

        InputDeviceObserver _inputDeviceObserver;
        [Inject]IEventBus _eventBus;

        private void Awake()
        {
            _eventBus.Subscribe<InputDeviceChangedEvent>(OnInputDeviceChangeEvent);
            if (_inputDeviceObserver != null)
            {
                OnInputDeviceChange(_inputDeviceObserver.ActiveDevice);
            }
            else
            {
                OnInputDeviceChange(InputDeviceType.MouseAndKeyboard);
            }
        }
        private void OnDestroy()
        {
            _eventBus.Unsubscribe<InputDeviceChangedEvent>(OnInputDeviceChangeEvent);
        }
        private void OnInputDeviceChangeEvent(InputDeviceChangedEvent inputDeviceChangedEvent)
        {
            OnInputDeviceChange(inputDeviceChangedEvent.inputDeviceType);
        }
        void OnInputDeviceChange(InputDeviceType inputDeviceType)
        {
            if (_observedInputDevices == null)
            {
                enabled = false;
                return;
            }
            if (_observedInputDevices.Contains(inputDeviceType))
            {
                if (_eventsOnExpectedDeviceDetected != null)
                {
                    for (int i = 0; i < _eventsOnExpectedDeviceDetected.Count; i++)
                    {
                        _eventsOnExpectedDeviceDetected[i]?.Invoke(new BaseEventData(EventSystem.current));
                    }
                }
            }
            else
            {
                if (_eventsOnUnexpectedDeviceDetected != null)
                {
                    for (int i = 0; i < _eventsOnUnexpectedDeviceDetected.Count; i++)
                    {
                        _eventsOnUnexpectedDeviceDetected[i]?.Invoke(new BaseEventData(EventSystem.current));
                    }
                }
            }
        }
    }
}