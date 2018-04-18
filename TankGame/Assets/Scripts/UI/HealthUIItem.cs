using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TankGame.Messaging;
using L10n = TankGame.Localization.Localization;

namespace TankGame.UI
{
    public class HealthUIItem : MonoBehaviour
    {
        private const string HealthKey = "health";
        private const string RespawnKey = "respawns";
        private const string OutOfLivesKey = "outOfLives";

        /// <summary>
        /// A reference to to the unit the health of which is drawn to the UI
        /// </summary>
        private Unit unit;

        /// <summary>
        /// The component which draws the text to the UI
        /// </summary>
        private Text text;

        private ISubscription<UnitDiedMessage> unitDiedSubscription;
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

        private bool isDead;
        private bool stopUpdating;
        private int respawnCurrentSecond = 0;

        private void OnDestroy()
        {
            UnregisterEventListeners();
        }

        private void Update()
        {
            if (!stopUpdating && isDead)
            {
                if (!IsEnemy && GameManager.Instance.GameLost)
                {
                    SetPlayerOutOfLivesText();
                    stopUpdating = true;
                }
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

            unit.Health.HealthChanged += OnUnitHealthChanged;
            //unit.Health.UnitDied += OnUnitDied;

            unitDiedSubscription = GameManager.Instance.
                MessageBus.Subscribe<UnitDiedMessage>(OnUnitDied);
            gameResetSubscription = GameManager.Instance.
                MessageBus.Subscribe<GameResetMessage>(OnGameReset);

            SetText(unit.Health.CurrentHealth);
        }

        private void OnUnitHealthChanged(Unit unit, int health)
        {
            if (isDead && health > 0)
            {
                isDead = false;
            }

            SetText(health);
        }

        public void OnUnitDied(UnitDiedMessage msg)
        {
            // Only if the unit of the message is the
            // unit to which this HealthUIItem belongs,
            // handle unit death
            if (msg.DeadUnit == unit)
            {
                //msg.PrintMessage();

                isDead = true;
                respawnCurrentSecond = (int) unit.RemainingRespawnTime + 1;

                //UnregisterEventListeners();
            }
        }

        //public void OnUnitDied(Unit unit)
        //{
        //    isDead = true;
        //    respawnCurrentSecond = (int) unit.RemainingRespawnTime + 1;

        //    //UnregisterEventListeners();
        //}

        private void OnLanguageChanged()
        {
            SetText(unit.Health.CurrentHealth);
        }

        private void UnregisterEventListeners()
        {
            unit.Health.HealthChanged -= OnUnitHealthChanged;

            if (!GameManager.IsClosing)
            {
                GameManager.Instance.MessageBus.Unsubscribe(unitDiedSubscription);
                GameManager.Instance.MessageBus.Unsubscribe(gameResetSubscription);
            }
            //unit.Health.UnitDied -= OnUnitDied;

            L10n.LanguageLoaded -= OnLanguageChanged;
        }

        private void SetText(int health)
        {
            string translation = "";

            string unitKey = IsEnemy ? "enemy" : "player";
            string unitTranslation = L10n.CurrentLanguage.GetTranslation(unitKey);

            if (health > 0)
            {
                translation = L10n.CurrentLanguage.GetTranslation(HealthKey);
                text.text = string.Format(translation, unitTranslation, health);

                // C# 6 syntax
                //text.text = $"{unit.name} health: {health}";
            }
            else
            {
                if (!IsEnemy && GameManager.Instance.GameLost)
                {
                    translation = L10n.CurrentLanguage.GetTranslation(OutOfLivesKey);
                    text.text = string.Format(translation, unitTranslation);
                }
                else
                {
                    // Rounds the respawn time number down and adds 1
                    // to display seconds correctly
                    respawnCurrentSecond = (int) unit.RemainingRespawnTime + 1;

                    translation = L10n.CurrentLanguage.GetTranslation(RespawnKey);
                    text.text = string.Format(translation, unitTranslation, respawnCurrentSecond);
                }
            }
        }

        private void SetPlayerOutOfLivesText()
        {
            string translation =
                L10n.CurrentLanguage.GetTranslation(OutOfLivesKey);
            string unitTranslation =
                L10n.CurrentLanguage.GetTranslation("player");

            text.text = string.Format(translation, unitTranslation);
        }

        private void OnGameReset(GameResetMessage msg)
        {
            stopUpdating = false;
        }
    }
}
