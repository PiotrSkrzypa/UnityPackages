using System;
using System.Diagnostics;
using Alchemy.Inspector;
using PSkrzypa.EventBus;
using UnityEngine;
using Zenject;
using Debug = UnityEngine.Debug;

namespace PSkrzypa.MVVMUI
{
    public class MenuControllerViewer : MonoBehaviour
    {
        [Inject] [NonSerialized] [ShowInInspector][ReadOnly] IMenuController menuController;
        [Inject] IEventBus eventBus;
        string testString = "test";
        TestSubscriber[] subscribers;

        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                int iterationCount = 100;
                Subscribe(iterationCount);
                stopwatch.Stop();
                Debug.Log($"Subscribe: {stopwatch.ElapsedMilliseconds} ms for {iterationCount} subscriptions");

            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                int iterationCount = 1000;
                Stopwatch stopwatch = Stopwatch.StartNew();
                Publish(iterationCount);
                stopwatch.Stop();
                Debug.Log($"Publish event: {stopwatch.ElapsedMilliseconds} ms for {iterationCount} calls");

            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Backspace))
            {
                ClearSubscribers();
                Debug.Log("Cleared subscribers");
            }
        }

        private void Subscribe(int iterationCount)
        {
            subscribers = new TestSubscriber[iterationCount];
            for (int i = 0; i < iterationCount; i++)
            {
                TestSubscriber subscriber = new TestSubscriber(eventBus);
                subscribers[i] = subscriber;
            }
        }
        private void Publish(int iterationCount)
        {
            for (int i = 0; i < iterationCount; i++)
            {
                eventBus.Publish(new TestPayload() { TestString = testString });
            }
        }
        private void ClearSubscribers()
        {
            if (subscribers != null)
            {
                Array.Clear(subscribers, 0, subscribers.Length);
                subscribers = null;
                GC.Collect();
            }
        }
    }

    class TestSubscriber
    {
        public TestSubscriber(IEventBus eventBus)
        {
            eventBus.Subscribe<TestPayload>(OnTestEvent);
        }
        void OnTestEvent(TestPayload payload)
        {
            //Debug.Log($"Received TestPayload with message: {payload.TestString}");
        }
    }
    public struct TestPayload : IEventPayload
    {
        public string TestString;
    }
}
