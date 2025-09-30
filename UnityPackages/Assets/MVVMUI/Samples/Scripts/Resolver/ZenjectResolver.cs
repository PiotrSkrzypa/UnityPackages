using Zenject;

namespace PSkrzypa.MVVMUI.Samples
{
    public class ZenjectResolver : IResolver
    {
        private readonly DiContainer _container;

        public ZenjectResolver(DiContainer container)
        {
            _container = container;
        }

        public object Resolve(System.Type type) => _container.Resolve(type);
        public T Resolve<T>() => _container.Resolve<T>();
        public object Instantiate(System.Type type, params object[] args) => _container.Instantiate(type, args);
        public T Instantiate<T>(params object[] args) => _container.Instantiate<T>(args);
    }

}