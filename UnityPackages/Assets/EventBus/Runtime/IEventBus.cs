using System;

namespace PSkrzypa.EventBus
{
    public interface IEventBus
    {
        T GetState<T>() where T : class, IEventPayload;
        void Publish<T>(T payload) where T : IEventPayload;
        void PublishAbstract<T>(T payload) where T : IEventPayload;
        void Subscribe<T>(Action<T> callback, Predicate<T> predicate = null) where T : IEventPayload;
        void Unsubscribe<T>(Action<T> callback) where T : IEventPayload;
    }
}