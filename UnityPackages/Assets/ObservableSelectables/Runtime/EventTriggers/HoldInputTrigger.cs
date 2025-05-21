using System.Collections.Generic;
using Alchemy.Inspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PSkrzypa.ObservableSelectables.EventTriggers
{
    public class HoldInputTrigger : MonoBehaviour
    {
        [SerializeField] float requiredHoldDuration;
        [SerializeField] bool resetInstantly;
        [HideIf("resetInstantly")][SerializeField] float resetTime;
        [SerializeField] float minimumIntervalBetweenActivations;
        [SerializeField] List<EventToTrigger> eventsToTrigger;
        [SerializeField] List<EventToTrigger> eventsOnUpdate;
        [SerializeField] Image progressIndicator;
        float currentDuration;
        bool inputDetectedThisFrame;
        float lastActivationTime;
        bool interactable = true;

        public bool Interactable { get => interactable; set => interactable = value; }

        private void Awake()
        {
            enabled = false;
        }
        public void OnInputDetected()
        {
            if (!interactable)
            {
                return;
            }
            if (Time.time - lastActivationTime < minimumIntervalBetweenActivations)
            {
                return;
            }
            inputDetectedThisFrame = true;
            currentDuration += Time.deltaTime;
            if (!enabled)
            {
                enabled = true;
            }
            if (currentDuration >= requiredHoldDuration)
            {
                lastActivationTime = Time.time;
                ResetTrigger();
                TriggerEvents();
            }
        }
        void Update()
        {
            if (inputDetectedThisFrame)
            {
                inputDetectedThisFrame = false;
                TriggerUpdateEvents();
                return;
            }
            if (resetInstantly)
            {
                ResetTrigger();
                return;
            }
            currentDuration -= ( 1f / resetTime ) * Time.deltaTime;
            TriggerUpdateEvents();
            if (currentDuration <= 0f)
            {
                ResetTrigger();
                return;
            }
        }

        private void ResetTrigger()
        {
            currentDuration = 0;
            enabled = false;
            TriggerUpdateEvents();
        }

        void TriggerEvents()
        {
            BaseEventData eventData = new BaseEventData(EventSystem.current);
            if (eventsToTrigger != null)
            {
                for (int i = 0; i < eventsToTrigger.Count; i++)
                {
                    eventsToTrigger[i]?.Invoke(eventData);
                }
            }
        }
        void TriggerUpdateEvents()
        {
            BaseEventData eventData = new BaseEventData(EventSystem.current);
            if (eventsOnUpdate != null)
            {
                for (int i = 0; i < eventsOnUpdate.Count; i++)
                {
                    eventsOnUpdate[i]?.Invoke(eventData);
                }
            }
            if (progressIndicator != null)
            {
                progressIndicator.fillAmount = currentDuration / requiredHoldDuration;
            }
        }


    }
}