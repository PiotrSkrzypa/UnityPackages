namespace PSkrzypa.LegacyEventBus
{
	public interface IEvent { }

	public struct TestEvent : IEvent { }

	public struct TestEventWithArgs : IEvent
	{
		public int health;
		public int mana;
	} 
}