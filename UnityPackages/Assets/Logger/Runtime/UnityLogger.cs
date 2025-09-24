using UnityEngine;

namespace PSkrzypa
{
    public class UnityLogger : ILogger
    {
        public void Log(string text)
        {
            Debug.Log(text);
        }

        public void LogWarning(string v)
        {
            Debug.LogWarning(v);
        }

        public void LogError(string v)
        {
            Debug.LogError(v);
        }

    }
}