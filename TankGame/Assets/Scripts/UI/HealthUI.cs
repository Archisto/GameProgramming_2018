using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.UI
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField]
        private HealthUIItem healthUIItemPrefab;

        public void Init()
        {
            Debug.Log("HealthUI initialized");
        }

        public void AddUnit(Unit unit)
        {
            var healthItem = Instantiate(healthUIItemPrefab, transform);
            healthItem.Init(unit);
            healthItem.gameObject.SetActive(true);
        }
    }
}
