using System;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Localization;

namespace TankGame.UI
{
    public class LocalizationUI : MonoBehaviour
    {
        public void SetEnglish()
        {
            Localization.Localization.LoadLanguage(LangCode.EN);
        }

        public void SetFinnish()
        {
            Localization.Localization.LoadLanguage(LangCode.FI);
        }
    }
}
