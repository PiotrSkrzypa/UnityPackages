using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Runtime.ExceptionServices;

namespace PSkrzypa.EventBus
{
    class EventSubscriber<T> : IEventSubscriber where T : IEventPayload
    {
        public int Id { get; }
        public Type PayloadType { get; }

        private WeakReference _callbackTarget;
        private MethodInfo _callbackMethod;

        private WeakReference _predicateTarget;
        private MethodInfo _predicateMethod;

        private Action<object, T> _callbackInvoker;
        private Func<object, T, bool> _predicateInvoker;

        private ILogger _logger;


        public bool IsAlive
        {
            get
            {
                if (_callbackMethod == null)
                {
                    return false;
                }
                if (_callbackMethod.IsStatic)
                {
                    return true;
                }
                if (_callbackTarget == null ||
                   !_callbackTarget.IsAlive ||
                   _callbackTarget.Target == null)
                {
                    return false;
                }
                return true;
            }
        }

        public EventSubscriber(ILogger logger, Type payloadType, Delegate callback, Delegate predicate = null)
        {
            _logger = logger;
            // validate params
            if (payloadType == null)
            {
                throw new ArgumentNullException(nameof(payloadType));
            }
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            // assign values to vars
            PayloadType = payloadType;
            Id = callback.GetHashCode();
            _callbackMethod = callback.Method;

            // check if callback method is not a static method
            if (!_callbackMethod.IsStatic &&
               callback.Target != null)
            {
                // init weak reference to callback owner
                _callbackTarget = new WeakReference(callback.Target);
            }

            var targetParam = Expression.Parameter(typeof(object), "target");
            var argParam = Expression.Parameter(PayloadType, "arg");

            var call = Expression.Call(
            Expression.Convert(targetParam, _callbackMethod.DeclaringType!),
            _callbackMethod,
            argParam
        );

            var actionType = typeof(Action<,>).MakeGenericType(typeof(object), payloadType);

            _callbackInvoker = Expression.Lambda<Action<object, T>>(call, targetParam, argParam).Compile();
            // --- init predicate ---
            if (predicate == null)
            {
                return;
            }
            _predicateMethod = predicate.Method;

            if (!_predicateMethod.IsStatic &&
               !Equals(predicate.Target, callback.Target))
            {
                _predicateTarget = new WeakReference(predicate.Target);
            }
            var predicateTargetParam = Expression.Parameter(typeof(object), "target");
            var predicateArgParam = Expression.Parameter(PayloadType, "arg");

            var predicateCall = Expression.Call(
            Expression.Convert(predicateTargetParam, _predicateMethod.DeclaringType!),
            _predicateMethod,
            predicateArgParam
        );

            var predicateType = typeof(Func<,,>).MakeGenericType(typeof(object), payloadType, typeof(bool));

            _predicateInvoker = (Func<object, T, bool>)Expression.Lambda(predicateType, predicateCall, predicateTargetParam, predicateArgParam).Compile();

        }
        public void Invoke(T payload)
        {
            // validate callback method info
            if (_callbackMethod == null)
            {
                _logger.LogError($"{nameof(_callbackMethod)} is null.");
                return;
            }
            if (!_callbackMethod.IsStatic &&
               ( _callbackTarget == null ||
                !_callbackTarget.IsAlive ))
            {
                _logger.LogWarning($"{nameof(_callbackMethod)} is not alive.");
                return;
            }

            // get reference to the predicate function owner
            if (_predicateMethod != null)
            {
                object predicateTarget = null;
                if (!_predicateMethod.IsStatic)
                {
                    if (_predicateTarget != null &&
                       _predicateTarget.IsAlive)
                    {
                        predicateTarget = _predicateTarget.Target;
                    }
                    else if (_callbackTarget != null &&
                            _callbackTarget.IsAlive)
                    {
                        predicateTarget = _callbackTarget.Target;
                    }
                }

                // check if predicate returned 'true'
                try
                {


                    var isAccepted = (bool)_predicateInvoker(predicateTarget, payload);
                    //var isAccepted = (bool)_predicateMethod.Invoke(predicateTarget, new object[] {payload});
                    if (!isAccepted)
                    {
                        _logger.LogWarning($"{nameof(_predicateMethod)} prevented calling {nameof(_callbackMethod)}");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    // Preserve the original exception's stack trace when re-throwing
                    Exception actualException = ex.InnerException ?? ex;
                    ExceptionDispatchInfo.Capture(actualException).Throw();
                }
            }

            // invoke callback method
            object callbackTarget = null;
            if (!_callbackMethod.IsStatic &&
               _callbackTarget != null && _callbackTarget.IsAlive)
            {
                callbackTarget = _callbackTarget.Target;
            }

            try
            {
                _callbackInvoker(callbackTarget, payload);
                //_callbackMethod.Invoke(callbackTarget, new object[] { payload });
            }
            catch (Exception ex)
            {
                // Preserve the original exception's stack trace when re-throwing
                Exception actualException = ex.InnerException ?? ex;
                ExceptionDispatchInfo.Capture(actualException).Throw();
            }
        }

        public void Dispose()
        {
            _callbackMethod = null;
            if (_callbackTarget != null)
            {
                _callbackTarget.Target = null;
                _callbackTarget = null;
            }

            _predicateMethod = null;
            if (_predicateTarget != null)
            {
                _predicateTarget.Target = null;
                _predicateTarget = null;
            }

            _callbackInvoker = null;
            _predicateInvoker = null;
        }

        public void Invoke(IEventPayload payload)
        {
            Invoke((T)payload);
        }
    }
}