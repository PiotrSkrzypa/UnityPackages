using System;
using System.Collections.Concurrent;
using System.Threading;
using Zenject;

namespace PSkrzypa.EventBus
{
    public class MainThreadDispatcher : IThreadDispatcher, ITickable
    {
        private readonly ConcurrentQueue<DispatcherTask> _tasks = new ConcurrentQueue<DispatcherTask>();

        public int ThreadId
        {
            get;
            private set;
        }

        public int TasksCount => _tasks.Count;

        private ILogger _logger;

        public MainThreadDispatcher(ILogger logger)
        {
            _logger = logger;
            ThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        public void Dispatch(Delegate action, object[] payload)
        {
            _tasks.Enqueue(new DispatcherTask(action, payload));
        }

        public void Tick()
        {
            while (_tasks.Count > 0)
            {
                if (!_tasks.TryDequeue(out var task))
                {
                    continue;
                }
                _logger?.Log($"(Queue.Count: {_tasks.Count}) Dispatching task {task.Action}");

                task.Invoke();
                task.Dispose();
            }
        }
    }
}