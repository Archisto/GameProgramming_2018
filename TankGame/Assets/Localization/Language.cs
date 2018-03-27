using System;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.Localization
{
    [Serializable]
    public class Language
    {
        [SerializeField]
        private List<string> keys = new List<string>();

        [SerializeField]
        private List<string> values = new List<string>();

        [SerializeField]
        private LangCode langCode;

        public LangCode LanguageCode
        {
            get
            {
                return langCode;
            }
            set
            {
                langCode = value;
            }
        }

        public Language()
        {
            LanguageCode = LangCode.None;
            Debug.Log("Language created but not initialized");
        }

        public Language(LangCode language)
        {
            LanguageCode = language;
            Debug.Log("Language created and initialized");
        }

        public string GetTranslation(string key)
        {
            string result = null;

            // Gets the index of the key (if it exists)
            int index = keys.IndexOf(key);

            // If the index is valid, sets the
            // corresponding value to the result
            if (index >= 0)
            {
                result = values[index];
            }

            return result;
        }

        public Dictionary<string, string> GetValues()
        {
            var result = new Dictionary<string, string>();

            for (int i = 0; i < keys.Count; i++)
            {
                result.Add(keys[i], values[i]);
            }

            return result;
        }

#if UNITY_EDITOR
        public void SetValues(Dictionary<string, string> translations)
        {
            // Clears the lists before adding new values
            Clear();

            foreach (var translation in translations)
            {
                keys.Add(translation.Key);
                values.Add(translation.Value);
            }
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }
#endif
    }
}
