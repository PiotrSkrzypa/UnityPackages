using System;
using System.Collections.Generic;
using System.Linq;
using PSkrzypa.MVVMUI.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace PSkrzypa.MVVMUI.Navigation
{
    [CreateAssetMenu(menuName = "Input Prompt Icons Database")]
    public class InputPromptIconsDatabase : SingletonScriptableObject<InputPromptIconsDatabase>
    {
        [Inject] InputDeviceObserver _inputDeviceObserver;
        InputPromptIconsSet[] inputPromptIconsSets;
        [ContextMenu("Refresh database")]
        public override void RefreshDatabase()
        {
            if (inputPromptIconsSets == null)
            {
                return;
            }
            for (int i = 0; i < inputPromptIconsSets.Length; i++)
            {
                InputPromptIconsSet inputPromptIconsSet = inputPromptIconsSets[i];
                if (inputPromptIconsSet.iconsByAction == null)
                {
                    inputPromptIconsSet.iconsByAction = new Dictionary<InputActionReference, Sprite>();
                }
            }
        }

        public bool TryGetInputPromptSprite(InputActionReference actionReference, out Sprite sprite)
        {
            bool result = false;
            sprite = null;
            InputDeviceType activeInputDevice = _inputDeviceObserver.ActiveDevice;
            if (inputPromptIconsSets != null)
            {
                InputPromptIconsSet inputPromptIconsSet = inputPromptIconsSets.FirstOrDefault(x=>x.inputDeviceType==activeInputDevice);
                if (inputPromptIconsSet != null)
                {
                    if (inputPromptIconsSet.iconsByAction.TryGetValue(actionReference, out sprite))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }
    }
    [Serializable]
    public class InputPromptIconsSet
    {
        public InputDeviceType inputDeviceType;
        public Dictionary<InputActionReference, Sprite> iconsByAction;
    }

}