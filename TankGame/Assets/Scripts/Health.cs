﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class Health
    {
        /// <summary>
        /// An event which is triggered when the owner Unit dies
        /// </summary>
        public event Action<Unit> UnitDied;

        private int maxHealth;

        public int CurrentHealth { get; private set; }

        public Unit Owner { get; private set; }

        public Health(Unit owner, int startingHealth)
        {
            Owner = owner;
            maxHealth = startingHealth;
            RestoreToFull();
        }

        /// <summary>
        /// Returns whether or not the Unit is dead.
        /// </summary>
        /// <returns>is the Unit dead</returns>
        public bool IsDead
        {
            get
            {
                return (CurrentHealth == 0);
            }
        }

        public void RestoreToFull()
        {
            CurrentHealth = maxHealth;
        }

        /// <summary>
        /// Applies damage to the Unit.
        /// </summary>
        /// <param name="damage">amount of damage</param>
        /// <returns>does the Unit die</returns>
        public bool TakeDamage(int damage)
        {
            if (!IsDead)
            {
                // Deals damage
                CurrentHealth =
                    Mathf.Clamp(CurrentHealth - damage, 0, CurrentHealth);

                // If the Unit died, an event is triggered
                if (IsDead)
                {
                    RaiseUnitDiedEvent();
                    return true;
                }
            }

            return false;
        }

        public void SetHealth(int health)
        {
            bool wasDead = IsDead;

            CurrentHealth = Mathf.Clamp(health, 0, maxHealth);

            // If the unit's health was set to 0,
            // an event is triggered
            if (!wasDead && IsDead)
            {
                RaiseUnitDiedEvent();
            }
        }

        protected void RaiseUnitDiedEvent()
        {
            // An event can't be fired from any other class than
            // the one in which it is declared. Instead call
            // a method (like this) which raises the event.

            //UnitDied?.Invoke(Owner);
            if (UnitDied != null)
            {
                UnitDied(Owner);
            }
        }
    }
}
