using PSkrzypa.EventBus;

namespace PSkrzypa.MVVMUI.Input.Events
{
	public struct InputDeviceChangedEvent : IEvent
	{
		public MVVMUI.Input.InputDeviceType inputDeviceType;
	} 
}