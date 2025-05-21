using System;
using System.Collections.Generic;
using UnityEngine;

namespace PSkrzypa
{
    public class MockLocalizationService : ILocalizationService
    {
        public Action OnLocalizeEvent { get; set; }

        string currentLanguageCode;

        public MockLocalizationService()
        {
            Debug.Log("Initialize localization service");
        }

        public List<string> GetAllLanguagesCode()
        {
            return new List<string>() { "PL", "EN" };
        }

        public string GetTranslation(string key)
        {
            return key + $"_{currentLanguageCode}";
        }

        public void SetLanguage(string languageCode)
        {
            currentLanguageCode = languageCode;
            OnLocalizeEvent?.Invoke();
        }
    }
}