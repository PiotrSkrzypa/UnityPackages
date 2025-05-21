using System.Linq;
using PSkrzypa.EventBus;
using PSkrzypa.MMVMUI.Input.Events;
using PSkrzypa.MVVMUI.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PBG.UI
{
    public class InputPrompt : MonoBehaviour, IEventListener<InputDeviceChangedEvent>
    {
        [SerializeField] InputActionReference actionReference;
        [SerializeField] Image image;
        [SerializeField] bool turnOffImageGameObjectIfEmpty;
        [SerializeField] bool ignoreParentNavigationGroupActiveState;
        Color promptColor = new Color(1f,0.9382353f,0.85f);
        NavigationGroup navigationGroup;
        bool interactable = true;
        bool rememberedInteractableState = true;

        InputDeviceObserver inputDeviceObserver;

        private void OnEnable()
        {
            GlobalEventBus<InputDeviceChangedEvent>.Register(this);
        }
        private void OnDisable()
        {
            GlobalEventBus<InputDeviceChangedEvent>.Deregister(this);
        }
        private void Start()
        {
            inputDeviceObserver = new InputDeviceObserver();
            if (image == null)
            {
                image = GetComponent<Image>();
            }
            image.color = promptColor;
            navigationGroup = GetComponentsInParent<NavigationGroup>(true)?.FirstOrDefault();
            if (navigationGroup != null && !ignoreParentNavigationGroupActiveState)
            {
                navigationGroup.onGroupActivated.AddListener(OnParentGroupActivated);
                navigationGroup.onGroupDeactivated.AddListener(OnParentGroupDeactivated);
                rememberedInteractableState = interactable;
                if (navigationGroup.GroupIsActive)
                {
                    OnParentGroupActivated();
                }
                else
                {
                    OnParentGroupDeactivated();
                }
            }
            OnInputDeviceChange(inputDeviceObserver.ActiveDevice);
        }

        public void OnEvent(InputDeviceChangedEvent @event)
        {
            OnInputDeviceChange(@event.inputDeviceType);
        }
        private void OnParentGroupActivated()
        {
            SetInteractableState(rememberedInteractableState);
        }
        private void OnParentGroupDeactivated()
        {
            rememberedInteractableState = interactable;
            SetInteractableState(false);
        }
        private void OnInputDeviceChange(InputDeviceType inputDevice)
        {
            InputPromptIconsDatabase inputPromptIconsDatabase = InputPromptIconsDatabase.Instance;
            if (actionReference != null)
            {
                if (inputPromptIconsDatabase.TryGetInputPromptSprite(actionReference, out Sprite sprite))
                {
                    if (sprite != null)
                    {
                        image.sprite = sprite;
                        image.enabled = true;
                    }
                    else
                    {
                        image.enabled = false;
                    }
                }
                else
                {
                    image.enabled = false;
                }
            }
            if (!interactable)
            {
                image.enabled = false;
            }
            if (!image.enabled && turnOffImageGameObjectIfEmpty)
            {
                image.gameObject.SetActive(false);
            }
            else if (!image.gameObject.activeInHierarchy)
            {
                image.gameObject.SetActive(true);
            }
        }
        public void SetInteractableState(bool value)
        {
            interactable = value;
            OnInputDeviceChange(inputDeviceObserver.ActiveDevice);
        }

    }
}