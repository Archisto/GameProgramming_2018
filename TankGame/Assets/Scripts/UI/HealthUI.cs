using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.UI
{
    /// <summary>
    /// A part of the UI which displays each unit's health.
    /// </summary>
    public class HealthUI : MonoBehaviour
    {
        /// <summary>
        /// A health UI item
        /// </summary>
        [SerializeField]
        private HealthUIItem healthUIItemPrefab;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public void Init()
        {
            Debug.Log("HealthUI initialized");
        }

        /// <summary>
        /// Adds a health UI item for a unit.
        /// </summary>
        /// <param name="unit">A unit</param>
        public void AddUnit(Unit unit)
        {
            var healthItem = Instantiate(healthUIItemPrefab, transform);
            healthItem.Init(unit);
            healthItem.gameObject.SetActive(true);
        }
    }
}
