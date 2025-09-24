using System;

namespace PSkrzypa.EventBus
{
    internal interface IEventSubscriber
    {
        int Id { get; }
        bool IsAlive { get; }
        Type PayloadType { get; }

        void Dispose();
        void Invoke(IEventPayload payload);
    }
}