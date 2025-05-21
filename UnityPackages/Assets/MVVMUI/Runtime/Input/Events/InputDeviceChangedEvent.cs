using PSkrzypa.EventBus;

namespace PSkrzypa.MMVMUI.Input.Events
{
	public struct InputDeviceChangedEvent : IEvent
	{
		public MVVMUI.Input.InputDeviceType inputDeviceType;
	} 
}