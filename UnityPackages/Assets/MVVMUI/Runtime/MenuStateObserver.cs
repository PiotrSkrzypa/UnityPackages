using System.Collections.Generic;
using PSkrzypa.EventBus;
using UnityEngine;
using UnityEngine.Events;

namespace PSkrzypa.MVVMUI
{
    public class MenuStateObserver : MonoBehaviour
    {
        [SerializeField] MonoBehaviour[] componentsToControl;
        [SerializeField] List<string> disablingWindows;
        [SerializeField] bool disableOnAnyWindow;
        [SerializeField] UnityEvent onDisabling;
        [SerializeField] UnityEvent onEnabling;
        bool disabled;

       

        private void Awake()
        {
            if (disablingWindows == null)
            {
                disablingWindows = new List<string>();
            }
            //GlobalEventBus<WindowOpenedEvent>.Register(this);
        }
        private void OnDestroy()
        {
            //GlobalEventBus<WindowOpenedEvent>.Deregister(this);
        }
        private void OnMenuStateChanged(string windowID)
        {
            if (disabled)
            {
                if (!string.IsNullOrEmpty(windowID) && disableOnAnyWindow)
                {
                    return;
                }
                if (disablingWindows.Contains(windowID))
                {
                    return;
                }
                for (int i = 0; i < componentsToControl.Length; i++)
                {
                    if (!componentsToControl[i].enabled)
                    {
                        componentsToControl[i].enabled = true;
                    }
                }
                disabled = false;
                onEnabling?.Invoke();
            }
            else
            {
                if (string.IsNullOrEmpty(windowID))
                {
                    return;
                }
                if (disableOnAnyWindow || disablingWindows.Contains(windowID))
                {
                    for (int i = 0; i < componentsToControl.Length; i++)
                    {
                        componentsToControl[i].enabled = false;
                    }
                    disabled = true;
                    onDisabling?.Invoke();
                    return;
                }
            }
        }
    }
}