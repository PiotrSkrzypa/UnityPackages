using System;

namespace PSkrzypa.EventBus
{
    public interface IThreadDispatcher
    {
        int ThreadId { get; }
        void Dispatch(Delegate action, object[] payload);
    }
}