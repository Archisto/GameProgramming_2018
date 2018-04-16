using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.UI
{
    public class UI : MonoBehaviour
    {
        [SerializeField]
        private GameObject winMessage;

        [SerializeField]
        private GameObject loseMessage;

        public static UI Current { get; private set; }

        public HealthUI HealthUI { get; private set; }

        public void Init()
        {
            Current = this;
            HealthUI = GetComponentInChildren<HealthUI>();
            HealthUI.Init();
        }

        public void ResetUI()
        {
            DisplayWinMessage(false);
            DisplayLoseMessage(false);
        }

        public void DisplayWinMessage(bool display)
        {
            winMessage.SetActive(display);
        }

        public void DisplayLoseMessage(bool display)
        {
            loseMessage.SetActive(display);
        }
    }
}
