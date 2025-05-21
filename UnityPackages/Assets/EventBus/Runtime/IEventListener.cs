namespace PSkrzypa.EventBus
{
	/// <summary>
	/// Event listener basic interface
	/// </summary>
	public interface IEventListenerBase { };

	/// <summary>
	/// A public interface you'll need to implement for each type of event you want to listen to.
	/// </summary>
	public interface IEventListener<IEvent> : IEventListenerBase
	{
		void OnEvent(IEvent @event);
	}

}