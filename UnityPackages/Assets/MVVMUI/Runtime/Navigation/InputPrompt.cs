using System.Linq;
using PSkrzypa.EventBus;
using PSkrzypa.MVVMUI.Input.Events;
using PSkrzypa.MVVMUI.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace PBG.UI
{
    public class InputPrompt : MonoBehaviour
    {
        [SerializeField] InputActionReference _actionReference;
        [SerializeField] Image _image;
        [SerializeField] bool _turnOffImageGameObjectIfEmpty;
        [SerializeField] bool _ignoreParentNavigationGroupActiveState;
        Color _promptColor = new Color(1f,0.9382353f,0.85f);
        NavigationGroup _navigationGroup;
        bool _interactable = true;
        bool _rememberedInteractableState = true;

        [Inject]private IEventBus _eventBus;

        private void Awake()
        {
            _eventBus.Subscribe<InputDeviceChangedEvent>(OnInputDeviceChangeEvent);
        }
        private void OnEnable()
        {
        }
        private void OnDisable()
        {
        }
        private void OnDestroy()
        {
            _eventBus.Unsubscribe<InputDeviceChangedEvent>(OnInputDeviceChangeEvent);
            if (_navigationGroup != null && !_ignoreParentNavigationGroupActiveState)
            {
                _navigationGroup.onGroupActivated.RemoveListener(OnParentGroupActivated);
                _navigationGroup.onGroupDeactivated.RemoveListener(OnParentGroupDeactivated);
            }
        }
        private void Start()
        {
            if (_image == null)
            {
                _image = GetComponent<Image>();
            }
            _image.color = _promptColor;
            _navigationGroup = GetComponentsInParent<NavigationGroup>(true)?.FirstOrDefault();
            if (_navigationGroup != null && !_ignoreParentNavigationGroupActiveState)
            {
                _navigationGroup.onGroupActivated.AddListener(OnParentGroupActivated);
                _navigationGroup.onGroupDeactivated.AddListener(OnParentGroupDeactivated);
                _rememberedInteractableState = _interactable;
                if (_navigationGroup.GroupIsActive)
                {
                    OnParentGroupActivated();
                }
                else
                {
                    OnParentGroupDeactivated();
                }
            }
        }

        private void OnParentGroupActivated()
        {
            SetInteractableState(_rememberedInteractableState);
        }
        private void OnParentGroupDeactivated()
        {
            _rememberedInteractableState = _interactable;
            SetInteractableState(false);
        }
        private void OnInputDeviceChangeEvent(InputDeviceChangedEvent inputDeviceChangedEvent)
        {
            OnInputDeviceChange(inputDeviceChangedEvent.inputDeviceType);
        }
        private void OnInputDeviceChange(InputDeviceType inputDevice)
        {
            InputPromptIconsDatabase inputPromptIconsDatabase = InputPromptIconsDatabase.Instance;
            if (_actionReference != null)
            {
                if (inputPromptIconsDatabase.TryGetInputPromptSprite(_actionReference, out Sprite sprite))
                {
                    if (sprite != null)
                    {
                        _image.sprite = sprite;
                        _image.enabled = true;
                    }
                    else
                    {
                        _image.enabled = false;
                    }
                }
                else
                {
                    _image.enabled = false;
                }
            }
            if (!_interactable)
            {
                _image.enabled = false;
            }
            if (!_image.enabled && _turnOffImageGameObjectIfEmpty)
            {
                _image.gameObject.SetActive(false);
            }
            else if (!_image.gameObject.activeInHierarchy)
            {
                _image.gameObject.SetActive(true);
            }
        }
        public void SetInteractableState(bool value)
        {
            _interactable = value;
        }

    }
}