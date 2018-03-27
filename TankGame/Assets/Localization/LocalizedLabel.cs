using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TankGame.Localization;

namespace TankGame.UI
{
    public class LocalizedLabel : MonoBehaviour
    {
        [SerializeField]
        private Text text;

        [SerializeField]
        private string key;

        private void Awake()
        {
            Localization.Localization.LanguageLoaded += SetText;
        }

        private void Start()
        {
            text = GetComponent<Text>();
            SetText();
        }

        private void SetText()
        {
            // FIXME

            if (text != null)
            {
                text.text = Localization.Localization.
                    CurrentLanguage.GetTranslation(key);
            }
        }
    }
}
