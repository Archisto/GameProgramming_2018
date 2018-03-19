using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TankGame.Messaging;

namespace TankGame.UI
{
    public class HealthUIItem : MonoBehaviour
    {
        /// <summary>
        /// A reference to to the unit the health of which is drawn to the UI
        /// </summary>
        private Unit unit;

        /// <summary>
        /// The component which draws the text to the UI
        /// </summary>
        private Text text;

        private ISubscription<UnitDiedMessage> unitDiedSubscription;

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
        private int respawnCurrentSecond = 0;

        private void OnDestroy()
        {
            UnregisterEventListeners();
        }

        private void Update()
        {
            if (isDead &&
                unit.RemainingRespawnTime + 1 < respawnCurrentSecond)
            {
                SetText(0);
            }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// <param name="unit">A referenced unit</param>
        public void Init(Unit unit)
        {
            this.unit = unit;
            text = GetComponentInChildren<Text>();

            if (text == null)
            {
                Debug.LogError("Text component not found in children");
                return;
            }

            // Draws the health text with different colors 
            // depending on if the unit is an enemy unit or not
            text.color = (IsEnemy ? Color.red : Color.green);

            unit.Health.HealthChanged += OnUnitHealthChanged;
            //unit.Health.UnitDied += OnUnitDied;
            unitDiedSubscription =
                GameManager.Instance.MessageBus.Subscribe<UnitDiedMessage>(OnUnitDied);
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
                msg.PrintMessage();

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

        private void UnregisterEventListeners()
        {
            unit.Health.HealthChanged -= OnUnitHealthChanged;

            if (!GameManager.IsClosing)
            {
                GameManager.Instance.MessageBus.Unsubscribe(unitDiedSubscription);
            }
            //unit.Health.UnitDied -= OnUnitDied;
        }

        private void SetText(int health)
        {
            if (health > 0)
            {
                text.text = string.Format("{0} health: {1}", unit.name, health);

                // C# 6 syntax for the same thing
                //text.text = $"{unit.name} health: {health}";
            }
            else
            {
                respawnCurrentSecond = (int) unit.RemainingRespawnTime + 1;
                text.text = string.Format("{0} respawns in: {1}", unit.name, respawnCurrentSecond);
            }
        }
    }
}
