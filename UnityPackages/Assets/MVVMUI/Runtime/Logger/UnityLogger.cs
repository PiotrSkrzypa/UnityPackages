using UnityEngine;

namespace PSkrzypa
{
    public class UnityLogger : ILogger
    {
        public void Log(string text)
        {
            Debug.Log(text);
        }
    }
}