namespace PSkrzypa
{
    public interface ILogger
    {
        void Log(string text);
        void LogWarning(string v);
        void LogError(string v);
    }
}