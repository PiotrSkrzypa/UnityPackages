using PSkrzypa.EventBus;

namespace PSkrzypa.MVVMUI.Input.Events
{
	public struct InputDeviceChangedEvent : IEventPayload
	{
		public InputDeviceType inputDeviceType;
	} 
}