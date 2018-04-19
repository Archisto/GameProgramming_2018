using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TankGame.Messaging;
using L10n = TankGame.Localization.Localization;

namespace TankGame.UI
{
    /// <summary>
    /// Part of the UI which displays a unit's health and respawn timer.
    /// </summary>
    public class HealthUIItem : MonoBehaviour
    {
        /// <summary>
        /// Localization key for health UI text.
        /// </summary>
        private const string HealthKey = "health";

        /// <summary>
        /// Localization key for respawn timer UI text.
        /// </summary>
        private const string RespawnKey = "respawns";

        /// <summary>
        /// Localization key for out-of-lives UI text.
        /// </summary>
        private const string OutOfLivesKey = "outOfLives";

        /// <summary>
        /// A reference to to the unit the health of which is drawn to the UI
        /// </summary>
        private Unit unit;

        /// <summary>
        /// The component which draws the text to the UI
        /// </summary>
        private Text text;

        private bool isDead;
        private bool stopUpdating;
        private int respawnCurrentSecond = 0;

        /// <summary>
        /// Message bus subscription to a unit dying
        /// </summary>
        private ISubscription<UnitDiedMessage> unitDiedSubscription;

        /// <summary>
        /// Message bus subscription to the game resetting
        /// </summary>
        private ISubscription<GameResetMessage> gameResetSubscription;

        /// <summary>
        /// Returns whether the unit is an enemy unit.
        /// </summary>
        public bool IsEnemy
        {
            get
            {
                return
                    unit != null &&
                    unit is EnemyUnit;
            }
        }

        /// <summary>
        /// Called when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            UnregisterEventListeners();
        }

        /// <summary>
        /// Updates the object each frame.
        /// </summary>
        private void Update()
        {
            if (!stopUpdating && isDead)
            {
                // If the player is out of lives, sets the text to say so
                // and stops updating because nothing can change
                // without reseting the game
                if (!IsEnemy && GameManager.Instance.PlayerLives == 0)
                {
                    SetPlayerOutOfLivesText();
                    stopUpdating = true;
                }
                // Sets the text if the respawn timer's second changes
                if (unit.RemainingRespawnTime + 1 != respawnCurrentSecond)
                {
                    SetText(0);
                }
            }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// <param name="unit">A referenced unit</param>
        public void Init(Unit unit)
        {
            // Registers to listen to the LanguageLoaded event
            L10n.LanguageLoaded += OnLanguageChanged;

            this.unit = unit;
            text = GetComponentInChildren<Text>();

            if (text == null)
            {
                Debug.LogError("Text component not found in children");
                return;
            }

            // Draws the health text with different colors 
            // depending on if the unit is an enemy unit or not.
            // Red for enemy, dark green for player.
            text.color = (IsEnemy ? Color.red : new Color(0, 0.6f, 0, 1));

            // Registers to listen to the unit's HealthChanged event
            unit.Health.HealthChanged += OnUnitHealthChanged;

            // Subcribes to listen to messages
            unitDiedSubscription = GameManager.Instance.
                MessageBus.Subscribe<UnitDiedMessage>(OnUnitDied);
            gameResetSubscription = GameManager.Instance.
                MessageBus.Subscribe<GameResetMessage>(OnGameReset);

            SetText(unit.Health.CurrentHealth);
        }

        /// <summary>
        /// Sets the text on the UI.
        /// </summary>
        /// <param name="health">The unit's current health</param>
        private void SetText(int health)
        {
            string translation = "";

            string unitKey = IsEnemy ? "enemy" : "player";
            string unitTranslation =
                L10n.CurrentLanguage.GetTranslation(unitKey);

            // Displays the unit's health
            if (health > 0)
            {
                translation = L10n.CurrentLanguage.GetTranslation(HealthKey);
                text.text = string.Format(translation, unitTranslation, health);

                // C# 6 syntax
                //text.text = $"{unit.name} health: {health}";
            }
            // Displays the unit's respawn timer
            else
            {
                // Rounds the respawn time number down and adds 1
                // to display seconds correctly
                respawnCurrentSecond = (int) unit.RemainingRespawnTime + 1;

                translation = L10n.CurrentLanguage.GetTranslation(RespawnKey);
                text.text = string.Format
                    (translation, unitTranslation, respawnCurrentSecond);
            }
        }

        /// <summary>
        /// Sets the text on the UI to say the player is out of lives.
        /// </summary>
        private void SetPlayerOutOfLivesText()
        {
            string translation =
                L10n.CurrentLanguage.GetTranslation(OutOfLivesKey);
            string unitTranslation =
                L10n.CurrentLanguage.GetTranslation("player");

            text.text = string.Format(translation, unitTranslation);
        }

        /// <summary>
        /// Called when the referenced unit's health changes.
        /// </summary>
        /// <param name="unit">Referenced unit</param>
        /// <param name="health">The unit's current health</param>
        private void OnUnitHealthChanged(Unit unit, int health)
        {
            if (isDead && health > 0)
            {
                isDead = false;
            }

            SetText(health);
        }

        /// <summary>
        /// Called when a unit dies.
        /// </summary>
        /// <param name="msg">A unit died message</param>
        public void OnUnitDied(UnitDiedMessage msg)
        {
            // Only if the unit of the message is the
            // referenced unit, handle unit death
            if (msg.DeadUnit == unit)
            {
                //msg.PrintMessage();

                isDead = true;
                respawnCurrentSecond = (int) unit.RemainingRespawnTime + 1;
            }
        }

        /// <summary>
        /// Called when the game is reset.
        /// </summary>
        /// <param name="msg">A game reset message</param>
        private void OnGameReset(GameResetMessage msg)
        {
            stopUpdating = false;
        }

        /// <summary>
        /// Called when the current language changes.
        /// </summary>
        private void OnLanguageChanged()
        {
            if (!IsEnemy && GameManager.Instance.PlayerLives == 0)
            {
                SetPlayerOutOfLivesText();
            }
            else
            {
                SetText(unit.Health.CurrentHealth);
            }
        }

        /// <summary>
        /// Unregisters and unsubscribes listening to any events.
        /// </summary>
        private void UnregisterEventListeners()
        {
            L10n.LanguageLoaded -= OnLanguageChanged;
            unit.Health.HealthChanged -= OnUnitHealthChanged;

            if (!GameManager.IsClosing)
            {
                GameManager.Instance.MessageBus.Unsubscribe(unitDiedSubscription);
                GameManager.Instance.MessageBus.Unsubscribe(gameResetSubscription);
            }
        }
    }
}
