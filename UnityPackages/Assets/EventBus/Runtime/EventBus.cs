using System;
using System.Collections.Generic;

namespace PSkrzypa.EventBus
{
    public class EventBus<T> where T : IEvent
    {
        private HashSet<IEventListener<T>> _subscribers = new HashSet<IEventListener<T>>();
        public void Register(IEventListener<T> subscriber) => _subscribers.Add(subscriber);
        public void Deregister(IEventListener<T> subscriber) => _subscribers.Remove(subscriber);

        public void Raise(T @event)
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
        public void Clear()
        {
            _subscribers.Clear();
        }
    }
}