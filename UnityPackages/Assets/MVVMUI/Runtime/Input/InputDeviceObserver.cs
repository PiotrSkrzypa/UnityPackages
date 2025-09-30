using System.Linq;
using PSkrzypa.EventBus;
using PSkrzypa.MVVMUI.Input.Events;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace PSkrzypa.MVVMUI.Input
{
    public class InputDeviceObserver
    {
        private const string steamdeckControllerName = "XInputControllerWindows";
        private const string dualSenseControllerName = "dualsense";
        private const string dualShockControllerName = "dualshock";

        public InputDeviceType ActiveDevice { get => activeDevice; }
        InputDeviceType activeDevice;
        string[] connectedGamepads;
        UnityEngine.InputSystem.InputDevice currentDevice;
        IEventBus _eventBus;

        public InputDeviceObserver(IEventBus eventBus)
        {
            _eventBus = eventBus;
            Initialize();
        }

        public void Initialize()
        {
            connectedGamepads = UnityEngine.Input.GetJoystickNames();
            UnityEngine.InputSystem.InputSystem.onAnyButtonPress.Call(OnAnyButtonPressed);
            _eventBus.Publish(new InputDeviceChangedEvent() { inputDeviceType = activeDevice });
        }
        
        private void OnAnyButtonPressed(UnityEngine.InputSystem.InputControl ctrl)
        {
            if (currentDevice == ctrl.device)
            {
                return;
            }
            currentDevice = ctrl.device;
            connectedGamepads = UnityEngine.Input.GetJoystickNames();
            if (ctrl.device is UnityEngine.InputSystem.Gamepad gamepad || ctrl.device is UnityEngine.InputSystem.Joystick joystick)
            {
                if (connectedGamepads == null || connectedGamepads.All(x => string.IsNullOrEmpty(x)))
                {
                    return;
                }
                if (ctrl.device.name == steamdeckControllerName)
                {
                    //if (SteamManager.Initialized && Steamworks.SteamUtils.IsSteamRunningOnSteamDeck())
                    //{
                    //    if (activeDevice != InputDeviceType.Steamdeck)
                    //    {
                    //        SwitchInputDevice(InputDeviceType.Steamdeck);
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        return;
                    //    }
                    //}
                    if (activeDevice != InputDeviceType.XBoxGamepad)
                    {
                        SwitchInputDevice(InputDeviceType.XBoxGamepad);
                        return;
                    }
                }
                else if (ctrl.device.name.ToLower().Contains(dualSenseControllerName) || ctrl.device.name.ToLower().Contains(dualShockControllerName))
                {
                    if (activeDevice != InputDeviceType.DualSense)
                    {
                        SwitchInputDevice(InputDeviceType.DualSense);
                        return;
                    }
                }
                else
                {
                    if (activeDevice != InputDeviceType.XBoxGamepad)
                    {
                        SwitchInputDevice(InputDeviceType.XBoxGamepad);
                        return;
                    }
                }
            }
            if (ctrl.device is UnityEngine.InputSystem.Mouse mouse || ctrl.device is UnityEngine.InputSystem.Keyboard keyboard)
            {
                if (activeDevice != InputDeviceType.MouseAndKeyboard)
                {
                    SwitchInputDevice(InputDeviceType.MouseAndKeyboard);
                    return;
                }
            }
        }
        void SwitchInputDevice(InputDeviceType inputDeviceType)
        {
            activeDevice = inputDeviceType;
            Debug.Log($"Input device changed to {activeDevice}");
            _eventBus.Publish(new InputDeviceChangedEvent() { inputDeviceType = activeDevice });
        }
    }
    public enum InputDeviceType { MouseAndKeyboard = 0, XBoxGamepad = 1, Steamdeck = 2, DualSense = 3 }
}