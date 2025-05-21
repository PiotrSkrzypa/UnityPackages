using System;
using System.Collections.Generic;

namespace PSkrzypa
{
    public interface ILocalizationService
    {
        Action OnLocalizeEvent { get; set; }

        string GetTranslation(string key);

        List<string> GetAllLanguagesCode();

        void SetLanguage(string languageCode);

    }
}