namespace PSkrzypa.EventBus
{
	public interface IEvent { }

	public struct TestEvent : IEvent { }

	public struct TestEventWithArgs : IEvent
	{
		public int health;
		public int mana;
	} 
}