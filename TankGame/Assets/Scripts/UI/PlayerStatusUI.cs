using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using L10n = TankGame.Localization.Localization;

namespace TankGame.UI
{
    /// <summary>
    /// A part of the UI which displays the player's status.
    /// </summary>
    public class PlayerStatusUI : MonoBehaviour
    {
        /// <summary>
        /// Localization key for score UI text.
        /// </summary>
        private const string ScoreKey = "score";

        /// <summary>
        /// Localization key for target score UI text.
        /// </summary>
        private const string TargetScoreKey = "targetScore";

        /// <summary>
        /// Localization key for deaths UI text.
        /// </summary>
        private const string DeathsKey = "deaths";

        [SerializeField]
        private Text playerScoreText;

        [SerializeField]
        private Text targetScoreText;

        [SerializeField]
        private Text playerDeathsText;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            Init();
        }

        /// <summary>
        /// Called when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            UnregisterEventListeners();
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public void Init()
        {
            // Registers to listen to the LanguageLoaded event
            L10n.LanguageLoaded += OnLanguageChanged;

            UpdateText();

            Debug.Log("PlayerStatusUI initialized");
        }

        /// <summary>
        /// Called when the language is changed.
        /// </summary>
        private void OnLanguageChanged()
        {
            UpdateText();
        }

        /// <summary>
        /// Updates the UI text.
        /// </summary>
        public void UpdateText()
        {
            SetScoreText(GameManager.Instance.Score);
            SetDeathsText(GameManager.Instance.PlayerDeaths);
        }

        /// <summary>
        /// Unregisters event listeners.
        /// </summary>
        private void UnregisterEventListeners()
        {
            L10n.LanguageLoaded -= OnLanguageChanged;
        }

        /// <summary>
        /// Sets the score text. Uses the current language.
        /// </summary>
        /// <param name="score">Current score</param>
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

        /// <summary>
        /// Sets the deaths text. Uses the current language.
        /// </summary>
        /// <param name="deaths">Current deaths</param>
        public void SetDeathsText(int deaths)
        {
            if (Application.isPlaying)
            {
                string translation = L10n.CurrentLanguage.GetTranslation(DeathsKey);
                playerDeathsText.text = string.Format
                    (translation, deaths, GameManager.Instance.MaxLives);
            }
        }
    }
}
