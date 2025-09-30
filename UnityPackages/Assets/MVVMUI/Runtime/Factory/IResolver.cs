namespace PSkrzypa.MVVMUI
{
    public interface IResolver
    {
        object Resolve(System.Type type);
        T Resolve<T>();
        object Instantiate(System.Type type, params object[] args);
        T Instantiate<T>(params object[] args);
    }
}