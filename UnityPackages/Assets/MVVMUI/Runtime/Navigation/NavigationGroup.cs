using System.Collections;
using System.Collections.Generic;
using PSkrzypa.EventBus;
using PSkrzypa.MVVMUI.BaseMenuWindow;
using PSkrzypa.MVVMUI.Input.Events;
using PSkrzypa.MVVMUI.Input;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace PBG.UI
{
    public class NavigationGroup : MonoBehaviour, IEventListener<InputDeviceChangedEvent>
    {
        public static UnityAction<NavigationGroup> OnNavigationGroupStateChange;
        [SerializeField] Selectable elementToSelectOnActivation;
        [FormerlySerializedAs("activateOnStart")]
        [SerializeField] bool activateOnEnable;
        [SerializeField] bool turnOffOtherGroups;
        [SerializeField] bool ignoreOtherGroupsStates;
        [SerializeField] bool rememberLastSelectedElement;
        [SerializeField] bool submitOnSelect;
        [SerializeField] bool skipUninteractable;
        [SerializeField] int searchDepth = 3;
        [SerializeField] bool limitNavigationToMenuWindow;
        [SerializeField] bool limitNavigationToNavGroup;
        [SerializeField] float repeatDelay = 0.3f;
        [SerializeField] private float minimumRepeatDelay = 0.15f;
        [SerializeField] private float speedUpStep = 0.05f;
        [SerializeField] private int speedUpAfterXRepeats = 3;
        public UnityEvent onGroupActivated;
        public UnityEvent onGroupDeactivated;
        private float currentRepeatDelay;
        private float lastNavigationRepeatCheck;
        private int repeats;
        Selectable currentlySelectedElement;
        Selectable lastSelectedElement;
        float lastNavigationEventTime;
        bool groupIsActive;
        bool navigationPaused;
        Stack<bool> rememberedActiveStates;
        bool initialized;

        public bool GroupIsActive { get => groupIsActive; }
        public bool TurnOffOtherGroups { get => turnOffOtherGroups; set => turnOffOtherGroups = value; }
        public bool LimitNavigationToNavGroup { get => limitNavigationToNavGroup; set => limitNavigationToNavGroup = value; }

        public bool GetNavigationPaused()
        {
            return navigationPaused;
        }

        public void SetNavigationPaused(bool value)
        {
            navigationPaused = value;
        }

        private void Awake()
        {
            rememberedActiveStates = new Stack<bool>();
        }
        private void OnEnable()
        {
            if (activateOnEnable)
            {
                ActivateNavigationGroup();
            }
            GlobalEventBus<InputDeviceChangedEvent>.Register(this);
        }
        private void OnDisable()
        {
            if (activateOnEnable)
            {
                DeactivateNavigationGroup();
            }
            GlobalEventBus<InputDeviceChangedEvent>.Deregister(this);
        }
        private void OnDestroy()
        {
            if (!ignoreOtherGroupsStates)
            {
                OnNavigationGroupStateChange -= ReactForOtherGroupStateChange;
            }
        }
        public void OnEvent(InputDeviceChangedEvent @event)
        {
            OnInputDeviceChanged(@event.inputDeviceType);
        }
        private void ReactForOtherGroupStateChange(NavigationGroup otherGroup)
        {
            if (!enabled)
            {
                return;
            }
            if (otherGroup == this)
            {
                return;
            }
            bool tmp = turnOffOtherGroups;
            turnOffOtherGroups = false;
            if (otherGroup.GroupIsActive)
            {
                rememberedActiveStates.Push(groupIsActive);
                if (groupIsActive)
                {
                    DeactivateNavigationGroup();
                }
            }
            else if (rememberedActiveStates.Count > 0)
            {
                bool activeStateToSet = rememberedActiveStates.Pop();
                if (activeStateToSet && !groupIsActive)
                {
                    ActivateNavigationGroup();
                }
                if (!activeStateToSet && groupIsActive)
                {
                    DeactivateNavigationGroup();
                }
            }
            turnOffOtherGroups = tmp;
        }
        public void FlushActiveStateStack()
        {
            rememberedActiveStates.Clear();
        }

        private void OnInputDeviceChanged(InputDeviceType newInputDevice)
        {
            if (!groupIsActive)
            {
                return;
            }
            if (newInputDevice == InputDeviceType.MouseAndKeyboard)
            {
                if (currentlySelectedElement != null)
                {
                    DeselectElement(currentlySelectedElement);
                }
            }
            else
            {
                if (currentlySelectedElement != null)
                {
                    SelectElement(currentlySelectedElement);
                }
            }
        }

        public void ActivateNavigationGroup()
        {
            if (groupIsActive)
            {
                return;
            }
            groupIsActive = true;
            if (!initialized)
            {
                Initialized();
            }
            if (rememberLastSelectedElement && lastSelectedElement != null)
            {
                elementToSelectOnActivation = lastSelectedElement;
            }
            if (turnOffOtherGroups)
            {
                OnNavigationGroupStateChange?.Invoke(this);
            }
            onGroupActivated?.Invoke();
            StartCoroutine(SelectElementOnActivation());
        }

        private IEnumerator SelectElementOnActivation()
        {
            yield return null;
            InputDeviceObserver inputDeviceObserver = new InputDeviceObserver();
            if (elementToSelectOnActivation != null && inputDeviceObserver.ActiveDevice != InputDeviceType.MouseAndKeyboard)
            {
                Selectable elementToSelect = elementToSelectOnActivation;
                if (ValidateElementToSelect(elementToSelect))
                {
                    SelectElement(elementToSelect);
                }
            }
        }

        private void Initialized()
        {
            if (!ignoreOtherGroupsStates)
            {
                OnNavigationGroupStateChange += ReactForOtherGroupStateChange;
            }
            initialized = true;
        }

        public void DeactivateNavigationGroup()
        {
            if (!groupIsActive)
            {
                return;
            }
            if (currentlySelectedElement != null)
            {
                DeselectElement(currentlySelectedElement);
            }
            groupIsActive = false;
            if (turnOffOtherGroups)
            {
                OnNavigationGroupStateChange?.Invoke(this);
            }
            onGroupDeactivated?.Invoke();
        }
        public void ClearSelectedElement()
        {
            if (currentlySelectedElement != null)
            {
                currentlySelectedElement = null;
            }
        }
        private bool TryGetNextValidElementToSelectOnDown(Selectable elementToSelect, out Selectable nextElementToSelect)
        {
            nextElementToSelect = null;
            if (elementToSelect == null)
            {
                return false;
            }
            bool automaticNavigation = false;
            if (elementToSelect.navigation.mode == Navigation.Mode.Explicit)
            {
                nextElementToSelect = elementToSelect.FindSelectableOnDown();
            }
            else if (( elementToSelect.navigation.mode & Navigation.Mode.Vertical ) != 0)
            {
                automaticNavigation = true;
                nextElementToSelect = FindSelectable(elementToSelect, Vector3.down);
            }
            if (ValidateElementToSelect(nextElementToSelect))
            {
                return true;
            }
            else if (nextElementToSelect != null)
            {
                for (int i = 0; i < searchDepth; i++)
                {
                    nextElementToSelect = automaticNavigation ? FindSelectable(nextElementToSelect, Vector3.down) : nextElementToSelect.FindSelectableOnDown();
                    if (ValidateElementToSelect(nextElementToSelect))
                    {
                        return true;
                    }
                    if (nextElementToSelect == null)
                    {
                        break;
                    }
                }
            }
            return false;
        }
        private bool TryGetNextValidElementToSelectOnUp(Selectable elementToSelect, out Selectable nextElementToSelect)
        {
            nextElementToSelect = null;
            if (elementToSelect == null)
            {
                return false;
            }
            bool automaticNavigation = false;
            if (elementToSelect.navigation.mode == Navigation.Mode.Explicit)
            {
                nextElementToSelect = elementToSelect.FindSelectableOnUp();
            }
            else if (( elementToSelect.navigation.mode & Navigation.Mode.Vertical ) != 0)
            {
                automaticNavigation = true;
                nextElementToSelect = FindSelectable(elementToSelect, Vector3.up);
            }
            if (ValidateElementToSelect(nextElementToSelect))
            {
                return true;
            }
            else if (nextElementToSelect != null)
            {
                for (int i = 0; i < searchDepth; i++)
                {
                    nextElementToSelect = automaticNavigation ? FindSelectable(nextElementToSelect, Vector3.up) : nextElementToSelect.FindSelectableOnUp();
                    if (ValidateElementToSelect(nextElementToSelect))
                    {
                        return true;
                    }
                    if (nextElementToSelect == null)
                    {
                        break;
                    }
                }
            }
            return false;
        }
        private bool TryGetNextValidElementToSelectOnRight(Selectable elementToSelect, out Selectable nextElementToSelect)
        {
            nextElementToSelect = null;
            if (elementToSelect == null)
            {
                return false;
            }
            bool automaticNavigation = false;
            if (elementToSelect.navigation.mode == Navigation.Mode.Explicit)
            {
                nextElementToSelect = elementToSelect.FindSelectableOnRight();
            }
            if (( elementToSelect.navigation.mode & Navigation.Mode.Horizontal ) != 0)
            {
                automaticNavigation = true;
                nextElementToSelect = FindSelectable(elementToSelect, Vector3.right);
            }
            if (ValidateElementToSelect(nextElementToSelect))
            {
                return true;
            }
            else if (nextElementToSelect != null)
            {
                for (int i = 0; i < searchDepth; i++)
                {
                    nextElementToSelect = automaticNavigation ? FindSelectable(nextElementToSelect, Vector3.right) : nextElementToSelect.FindSelectableOnRight();
                    if (ValidateElementToSelect(nextElementToSelect))
                    {
                        return true;
                    }
                    if (nextElementToSelect == null)
                    {
                        break;
                    }
                }
            }
            return false;
        }
        private bool TryGetNextValidElementToSelectOnLeft(Selectable elementToSelect, out Selectable nextElementToSelect)
        {
            nextElementToSelect = null;
            if (elementToSelect == null)
            {
                return false;
            }
            bool automaticNavigation = false;
            if (elementToSelect.navigation.mode == Navigation.Mode.Explicit)
            {
                nextElementToSelect = elementToSelect.FindSelectableOnLeft();
            }
            else if (( elementToSelect.navigation.mode & Navigation.Mode.Horizontal ) != 0)
            {
                automaticNavigation = true;
                nextElementToSelect = FindSelectable(elementToSelect, Vector3.left);
            }
            if (ValidateElementToSelect(nextElementToSelect))
            {
                return true;
            }
            else if (nextElementToSelect != null)
            {
                for (int i = 0; i < searchDepth; i++)
                {
                    nextElementToSelect = automaticNavigation ? FindSelectable(nextElementToSelect, Vector3.left) : nextElementToSelect.FindSelectableOnLeft();
                    if (ValidateElementToSelect(nextElementToSelect))
                    {
                        return true;
                    }
                    if (nextElementToSelect == null)
                    {
                        break;
                    }
                }
            }
            return false;
        }
        private bool ValidateElementToSelect(Selectable elementToSelect)
        {
            return elementToSelect != null && elementToSelect.isActiveAndEnabled && ( !skipUninteractable || elementToSelect.interactable );
        }
        public void SelectElement(Selectable selectable)
        {
            if (!groupIsActive)
            {
                return;
            }
            InputDeviceObserver inputDeviceObserver = new InputDeviceObserver();
            if (inputDeviceObserver.ActiveDevice == InputDeviceType.MouseAndKeyboard)
            {
                return;
            }
            EventSystem eventSystem = EventSystem.current;
            Selectable currentSelectedObject = eventSystem.currentSelectedGameObject ? eventSystem.currentSelectedGameObject.GetComponent<Selectable>() : null;
            if (currentlySelectedElement != null && currentlySelectedElement != selectable)
            {
                DeselectElement(currentlySelectedElement);
            }
            ExecuteEvents.Execute(selectable.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerEnterHandler);
            ExecuteEvents.Execute(selectable.gameObject, new PointerEventData(eventSystem), ExecuteEvents.selectHandler);
            if (!eventSystem.alreadySelecting)
            {
                eventSystem.SetSelectedGameObject(selectable.gameObject, new PointerEventData(eventSystem));
                currentlySelectedElement = selectable;
                if (rememberLastSelectedElement)
                {
                    lastSelectedElement = currentlySelectedElement;
                }
                if (submitOnSelect)
                {
                    SubmitElement(currentlySelectedElement);
                }
            }
        }
        private void DeselectElement(Selectable selectable)
        {
            EventSystem eventSystem = EventSystem.current;
            ExecuteEvents.Execute(selectable.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerExitHandler);
            ExecuteEvents.Execute(selectable.gameObject, new PointerEventData(eventSystem), ExecuteEvents.deselectHandler);
            //if (selectable == currentlySelectedElement)
            //{
            //    currentlySelectedElement = null;
            //}
        }
        public void SetElementToSelectOnActivation(Selectable selectable)
        {
            elementToSelectOnActivation = selectable;
            if (rememberLastSelectedElement)
            {
                lastSelectedElement = elementToSelectOnActivation;
            }
        }
        public void NavigateUp()
        {
            if (!groupIsActive || navigationPaused)
            {
                return;
            }
            SelectDefaultIfNeeded();
            if (CheckRepeatDelay())
            {
                lastNavigationEventTime = Time.time;
                if (TryGetNextValidElementToSelectOnUp(currentlySelectedElement, out Selectable nextElementToSelect))
                {
                    SelectElement(nextElementToSelect);
                }
            }
        }

        private void SelectDefaultIfNeeded()
        {
            if (!groupIsActive)
            {
                return;
            }
            if (currentlySelectedElement == null)
            {
                if (ValidateElementToSelect(elementToSelectOnActivation))
                {
                    SelectElement(elementToSelectOnActivation);
                }
            }
        }

        public void NavigateDown()
        {
            if (!groupIsActive || navigationPaused)
            {
                return;
            }
            SelectDefaultIfNeeded();
            if (CheckRepeatDelay())
            {
                if (TryGetNextValidElementToSelectOnDown(currentlySelectedElement, out Selectable nextElementToSelect))
                {
                    SelectElement(nextElementToSelect);
                }
            }
        }
        public void NavigateRight()
        {
            if (!groupIsActive || navigationPaused)
            {
                return;
            }
            SelectDefaultIfNeeded();
            if (CheckRepeatDelay())
            {
                if (TryGetNextValidElementToSelectOnRight(currentlySelectedElement, out Selectable nextElementToSelect))
                {
                    SelectElement(nextElementToSelect);
                }
            }
        }
        public void NavigateLeft()
        {
            if (!groupIsActive || navigationPaused)
            {
                return;
            }
            SelectDefaultIfNeeded();
            if (CheckRepeatDelay())
            {
                if (TryGetNextValidElementToSelectOnLeft(currentlySelectedElement, out Selectable nextElementToSelect))
                {
                    SelectElement(nextElementToSelect);
                }
            }
        }
        bool CheckRepeatDelay()
        {
            float timeSinceStartUp = Time.time;
            if (timeSinceStartUp - lastNavigationEventTime > 0.5f || Time.frameCount - lastNavigationRepeatCheck > 1)
            {
                repeats = 0;
                currentRepeatDelay = repeatDelay;
            }
            bool result = currentRepeatDelay <= timeSinceStartUp - lastNavigationEventTime || Time.frameCount - lastNavigationRepeatCheck > 1;
            if (result)
            {
                repeats += 1;
                lastNavigationEventTime = timeSinceStartUp;
                if (repeats % speedUpAfterXRepeats == 0 && currentRepeatDelay > minimumRepeatDelay)
                {
                    currentRepeatDelay -= speedUpStep;
                }
            }
            lastNavigationRepeatCheck = Time.frameCount;
            return result;
        }
        public void SubmitElement()
        {
            if (!groupIsActive)
            {
                return;
            }
            if (currentlySelectedElement != null)
            {
                EventSystem eventSystem = EventSystem.current;
                ExecuteEvents.Execute(currentlySelectedElement.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerDownHandler);
                ExecuteEvents.Execute(currentlySelectedElement.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(currentlySelectedElement.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerClickHandler);
                ExecuteEvents.Execute(currentlySelectedElement.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerExitHandler);
            }
        }
        public void RightClickSelectedElement()
        {
            if (!groupIsActive)
            {
                return;
            }
            if (currentlySelectedElement != null)
            {
                EventSystem eventSystem = EventSystem.current;
                PointerEventData pointerEventData = new PointerEventData(eventSystem){ button = PointerEventData.InputButton.Right };
                ExecuteEvents.Execute(currentlySelectedElement.gameObject, pointerEventData, ExecuteEvents.pointerDownHandler);
                ExecuteEvents.Execute(currentlySelectedElement.gameObject, pointerEventData, ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(currentlySelectedElement.gameObject, pointerEventData, ExecuteEvents.pointerClickHandler);
                ExecuteEvents.Execute(currentlySelectedElement.gameObject, pointerEventData, ExecuteEvents.pointerExitHandler);
            }
        }
        public void SubmitElement(Selectable selectable)
        {
            if (!groupIsActive)
            {
                return;
            }
            if (selectable != null)
            {
                EventSystem eventSystem = EventSystem.current;
                ExecuteEvents.Execute(selectable.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerDownHandler);
                ExecuteEvents.Execute(selectable.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerUpHandler);
                ExecuteEvents.Execute(selectable.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerClickHandler);
            }
        }
        public void SubmitElementOrAlternative(Selectable selectable)
        {
            if (!groupIsActive)
            {
                return;
            }
            if (selectable != null)
            {
                if (!selectable.interactable)
                {
                    SelectableWithAlternative selectableWithAlternative = selectable.GetComponent<SelectableWithAlternative>();
                    if (selectableWithAlternative != null)
                    {
                        selectable = selectableWithAlternative.GetAlternativeSelectable();
                    }
                }
                if (selectable != null)
                {
                    EventSystem eventSystem = EventSystem.current;
                    ExecuteEvents.Execute(selectable.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerDownHandler);
                    ExecuteEvents.Execute(selectable.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerUpHandler);
                    ExecuteEvents.Execute(selectable.gameObject, new PointerEventData(eventSystem), ExecuteEvents.pointerClickHandler);
                }

            }
        }
        public Selectable FindSelectable(Selectable selectableToSearchFrom, Vector3 dir)
        {
            dir = selectableToSearchFrom.transform.rotation * dir;
            dir = dir.normalized;
            Vector3 localDir = Quaternion.Inverse(selectableToSearchFrom.transform.rotation) * dir;
            Vector3 pos = selectableToSearchFrom.transform.TransformPoint(GetPointOnRectEdge(selectableToSearchFrom.transform as RectTransform, localDir));
            float maxScore = Mathf.NegativeInfinity;
            Selectable bestPick = null;
            IWindowViewModel menuWindowController = selectableToSearchFrom.GetComponentInParent<IWindowViewModel>();
            NavigationGroup navigationGroup = selectableToSearchFrom.GetComponentInParent<NavigationGroup>();
            Selectable[] allSelectables = Selectable.allSelectablesArray;

            for (int i = 0; i < allSelectables.Length; ++i)
            {
                Selectable sel = allSelectables[i];

                if (sel == null)
                {
                    continue;
                }
                if (sel == this)
                    continue;

                if (sel.navigation.mode == Navigation.Mode.None)
                    continue;
                if (limitNavigationToMenuWindow)
                {
                    IWindowViewModel selectableParentWindow = sel.GetComponentInParent<IWindowViewModel>();
                    if (menuWindowController != selectableParentWindow)
                    {
                        continue;
                    }
                }
                if (limitNavigationToNavGroup)
                {
                    NavigationGroup selNavigationGroup = sel.GetComponentInParent<NavigationGroup>();
                    if (navigationGroup != selNavigationGroup)
                    {
                        continue;
                    }
                }

                var selRect = sel.transform as RectTransform;
                Vector3 selCenter = selRect != null ? (Vector3)selRect.rect.center : Vector3.zero;
                Vector3 myVector = sel.transform.TransformPoint(selCenter) - pos;

                // Value that is the distance out along the direction.
                float dot = Vector3.Dot(dir, myVector);

                // Skip elements that are in the wrong direction or which have zero distance.
                // This also ensures that the scoring formula below will not have a division by zero error.
                if (dot <= 0)
                    continue;

                // This scoring function has two priorities:
                // - Score higher for positions that are closer.
                // - Score higher for positions that are located in the right direction.
                // This scoring function combines both of these criteria.
                // It can be seen as this:
                //   Dot (dir, myVector.normalized) / myVector.magnitude
                // The first part equals 1 if the direction of myVector is the same as dir, and 0 if it's orthogonal.
                // The second part scores lower the greater the distance is by dividing by the distance.
                // The formula below is equivalent but more optimized.
                //
                // If a given score is chosen, the positions that evaluate to that score will form a circle
                // that touches pos and whose center is located along dir. A way to visualize the resulting functionality is this:
                // From the position pos, blow up a circular balloon so it grows in the direction of dir.
                // The first Selectable whose center the circular balloon touches is the one that's chosen.
                float score = dot / myVector.sqrMagnitude;

                if (score > maxScore)
                {
                    maxScore = score;
                    bestPick = sel;
                }
            }
            return bestPick;
        }
        private Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
        {
            if (rect == null)
                return Vector3.zero;
            if (dir != Vector2.zero)
                dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
            return dir;
        }


    }
}