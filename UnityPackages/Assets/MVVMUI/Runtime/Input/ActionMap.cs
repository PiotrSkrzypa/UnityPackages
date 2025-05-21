using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PSkrzypa.MVVMUI.Input
{
    public class ActionMap : MonoBehaviour
    {
        [SerializeField] List<ActionMapEntry> actions;

        private void OnEnable()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].Subscribe();
            }
        }
        private void OnDisable()
        {
            for (int i = 0; i < actions.Count; i++)
            {
                actions[i].Unsubscribe();
            }
        }
    }
    [Serializable]
    class ActionMapEntry
    {
        [SerializeField] InputActionReference actionReference;
        [SerializeField] UnityEvent eventToTrigger;
        public void Subscribe()
        {
            actionReference.action.Enable();
            actionReference.action.performed += TriggerEvent;
        }
        public void Unsubscribe()
        {
            actionReference.action.Disable();
            actionReference.action.performed -= TriggerEvent;
        }
        private void TriggerEvent(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                eventToTrigger?.Invoke();
            }
        }
    }

}
