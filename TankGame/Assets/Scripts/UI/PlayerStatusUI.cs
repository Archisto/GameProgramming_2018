using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using L10n = TankGame.Localization.Localization;

namespace TankGame.UI
{
    public class PlayerStatusUI : MonoBehaviour
    {
        private const string ScoreKey = "score";
        private const string TargetScoreKey = "targetScore";
        private const string DeathsKey = "deaths";

        [SerializeField]
        private Text playerScoreText;

        [SerializeField]
        private Text targetScoreText;

        [SerializeField]
        private Text playerDeathsText;

        private void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            UnregisterEventListeners();
        }

        public void Init()
        {
            L10n.LanguageLoaded += OnLanguageChanged;

            UpdateText();

            Debug.Log("ScoreUI initialized");
        }

        private void OnLanguageChanged()
        {
            UpdateText();
        }

        public void UpdateText()
        {
            SetScoreText(GameManager.Instance.Score);
            SetDeathsText(GameManager.Instance.PlayerDeaths);
        }

        private void UnregisterEventListeners()
        {
            L10n.LanguageLoaded -= OnLanguageChanged;
        }

        public void SetScoreText(int score)
        {
            if (Application.isPlaying)
            {
                string translation = L10n.CurrentLanguage.GetTranslation(ScoreKey);
                playerScoreText.text = string.Format(translation, score);

                translation = L10n.CurrentLanguage.GetTranslation(TargetScoreKey);
                targetScoreText.text = string.Format
                    (translation, GameManager.Instance.TargetScore);
            }
        }

        public void SetDeathsText(int deaths)
        {
            if (Application.isPlaying)
            {
                string translation = L10n.CurrentLanguage.GetTranslation(DeathsKey);
                playerDeathsText.text = string.Format
                    (translation, deaths, GameManager.Instance.StartingLives);
            }
        }
    }
}
