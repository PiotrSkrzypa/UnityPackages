using System;

namespace PSkrzypa.EventBus
{
    public interface IEventBinding<T>
    {
        public Action<T> OnEvent { get; set; }
        public Action OnEventNoArgs { get; set; }
        public void Register();
        public void Deregister();
    }

    public class EventBinding<T> : IEventBinding<T>, IEventListener<T> where T : IEvent
    {
        Action<T> onEvent = _ => { };
        Action onEventNoArgs = () => { };

        Action<T> IEventBinding<T>.OnEvent
        {
            get => onEvent;
            set => onEvent = value;
        }

        Action IEventBinding<T>.OnEventNoArgs
        {
            get => onEventNoArgs;
            set => onEventNoArgs = value;
        }

        public EventBinding(Action<T> onEvent) => this.onEvent = onEvent;
        public EventBinding(Action onEventNoArgs) => this.onEventNoArgs = onEventNoArgs;

        public void Add(Action onEvent) => onEventNoArgs += onEvent;
        public void Remove(Action onEvent) => onEventNoArgs -= onEvent;

        public void Add(Action<T> onEvent) => this.onEvent += onEvent;
        public void Remove(Action<T> onEvent) => this.onEvent -= onEvent;

        public void Register()
        {
            GlobalEventBus<T>.Register(this);
        }
        public void Deregister()
        {
            GlobalEventBus<T>.Deregister(this);
        }

        public void OnEvent(T @event)
        {
            onEvent.Invoke(@event);
            onEventNoArgs.Invoke();
        }
    }
}