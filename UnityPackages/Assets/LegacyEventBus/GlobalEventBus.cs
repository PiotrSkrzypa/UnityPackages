using System.Collections.Generic;

namespace PSkrzypa.LegacyEventBus
{
    public static class GlobalEventBus<T> where T : IEvent
    {
        private static HashSet<IEventListener<T>> _subscribers = new HashSet<IEventListener<T>>();
        public static void Register(IEventListener<T> subscriber) => _subscribers.Add(subscriber);
        public static void Deregister(IEventListener<T> subscriber) => _subscribers.Remove(subscriber);

        public static void Raise(T @event)
        {
            var snapshot = new HashSet<IEventListener<T>>(_subscribers);

            foreach (var subscriber in snapshot)
            {
                if (_subscribers.Contains(subscriber))
                {
                    subscriber.OnEvent(@event);
                }
            }
        }
        static void Clear()
        {
            _subscribers.Clear();
        }
    }
}