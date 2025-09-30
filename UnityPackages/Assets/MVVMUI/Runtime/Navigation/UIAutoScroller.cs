using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PSkrzypa.MVVMUI.Navigation
{
    /// <summary>
    /// Component that automatically scrolls a ScrollRect to ensure the currently selected UI element is visible.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class UIAutoScroller : MonoBehaviour
    {
        [SerializeField]
        private float scrollSpeed = 10f;
        [SerializeField]
        private int selectableItemsDepht = 0;
        private ScrollType scrollDirection;

        protected RectTransform LayoutListGroup
        {
            get { return TargetScrollRect != null ? TargetScrollRect.content : null; }
        }

        protected ScrollType ScrollDirection
        {
            get { return scrollDirection; }
        }
        protected float ScrollSpeed
        {
            get { return scrollSpeed; }
        }

        protected RectTransform ScrollWindow { get; set; }
        protected ScrollRect TargetScrollRect { get; set; }

        protected EventSystem CurrentEventSystem
        {
            get { return EventSystem.current; }
        }
        protected GameObject LastCheckedGameObject { get; set; }
        protected GameObject CurrentSelectedGameObject
        {
            get { return EventSystem.current.currentSelectedGameObject; }
        }
        protected RectTransform CurrentTargetRectTransform { get; set; }
        protected bool IsManualScrollingAvailable { get; set; }

        protected virtual void Awake()
        {
            TargetScrollRect = GetComponent<ScrollRect>();
            scrollDirection = ScrollType.BOTH;
            if (!TargetScrollRect.horizontal)
            {
                scrollDirection = ScrollType.VERTICAL;
            }
            if (!TargetScrollRect.vertical)
            {
                scrollDirection = ScrollType.HORIZONTAL;
            }
            ScrollWindow = TargetScrollRect.GetComponent<RectTransform>();
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            UpdateReferences();
            ScrollRectToLevelSelection();
        }

        private void UpdateReferences()
        {
            if (CurrentSelectedGameObject != LastCheckedGameObject)
            {
                CurrentTargetRectTransform = ( CurrentSelectedGameObject != null ) ?
                    CurrentSelectedGameObject.GetComponent<RectTransform>() :
                    null;

                if (CurrentSelectedGameObject != null &&
                    CurrentSelectedGameObject.transform.parent == LayoutListGroup.transform)
                {
                    IsManualScrollingAvailable = false;
                }
            }

            LastCheckedGameObject = CurrentSelectedGameObject;
        }

        public void CancelAutoscroll()
        {
            if (IsManualScrollingAvailable == true)
            {
                return;
            }

            IsManualScrollingAvailable = true;
        }

        private void ScrollRectToLevelSelection()
        {
            bool referencesAreIncorrect = (TargetScrollRect == null || LayoutListGroup == null || ScrollWindow == null);

            if (referencesAreIncorrect == true || IsManualScrollingAvailable == true)
            {
                return;
            }

            RectTransform selection = CurrentTargetRectTransform;

            bool selectionIsCorrect = selection!=null;
            if (!selectionIsCorrect)
            {
                return;
            }
            else
            {
                selectionIsCorrect &= selection.transform.parent == LayoutListGroup.transform;
                if (!selectionIsCorrect && selectableItemsDepht > 0)
                {
                    for (int depth = 0; depth < selectableItemsDepht; depth++)
                    {
                        selection = selection.transform.parent.transform as RectTransform;
                    }
                    selectionIsCorrect = selection.transform.parent == LayoutListGroup.transform;
                }
            }
            if (!selectionIsCorrect)
            {
                return;
            }
            switch (ScrollDirection)
            {
                case ScrollType.VERTICAL:
                    UpdateVerticalScrollPosition(selection);
                    break;
                case ScrollType.HORIZONTAL:
                    UpdateHorizontalScrollPosition(selection);
                    break;
                case ScrollType.BOTH:
                    UpdateVerticalScrollPosition(selection);
                    UpdateHorizontalScrollPosition(selection);
                    break;
            }
        }

        private void UpdateVerticalScrollPosition(RectTransform selection)
        {
            float selectionPosition = -selection.anchoredPosition.y - (selection.rect.height * (1 - selection.pivot.y));

            float elementHeight = selection.rect.height;
            float maskHeight = ScrollWindow.rect.height;
            float listAnchorPosition = LayoutListGroup.anchoredPosition.y;

            float offlimitsValue = GetScrollOffset(selectionPosition, listAnchorPosition, elementHeight, maskHeight);

            TargetScrollRect.verticalNormalizedPosition +=
                ( offlimitsValue / LayoutListGroup.rect.height ) * Time.unscaledDeltaTime * scrollSpeed;
            TargetScrollRect.verticalNormalizedPosition = Mathf.Clamp01(TargetScrollRect.verticalNormalizedPosition);
        }

        private void UpdateHorizontalScrollPosition(RectTransform selection)
        {
            float selectionPosition = -selection.anchoredPosition.x - (selection.rect.width * (1 - selection.pivot.x));

            float elementWidth = selection.rect.width;
            float maskWidth = ScrollWindow.rect.width;
            float listAnchorPosition = -LayoutListGroup.anchoredPosition.x;

            float offlimitsValue = -GetScrollOffset(selectionPosition, listAnchorPosition, elementWidth, maskWidth);

            TargetScrollRect.horizontalNormalizedPosition +=
                ( offlimitsValue / LayoutListGroup.rect.width ) * Time.unscaledDeltaTime * scrollSpeed;
            TargetScrollRect.horizontalNormalizedPosition = Mathf.Clamp01(TargetScrollRect.horizontalNormalizedPosition);
        }

        private float GetScrollOffset(float position, float listAnchorPosition, float targetLength, float maskLength)
        {
            if (position < listAnchorPosition + ( targetLength / 2 ))
            {
                return ( listAnchorPosition + maskLength ) - ( position - targetLength );
            }
            else if (position + targetLength > listAnchorPosition + maskLength)
            {
                return ( listAnchorPosition + maskLength ) - ( position + targetLength );
            }

            return 0;
        }

        public enum ScrollType
        {
            VERTICAL,
            HORIZONTAL,
            BOTH
        }
    }
}