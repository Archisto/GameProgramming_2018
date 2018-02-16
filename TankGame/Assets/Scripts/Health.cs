using System;
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
            CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, CurrentHealth);

            bool outOfHealth = (CurrentHealth == 0);
            if (outOfHealth && UnitDied != null)
            {
                UnitDied(Owner);
            }

            return outOfHealth;
        }
    }
}
