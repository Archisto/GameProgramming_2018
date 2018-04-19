using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.UI
{
    /// <summary>
    /// The user interface.
    /// </summary>
    public class UI : MonoBehaviour
    {
        /// <summary>
        /// Message that is displayed when the player wins the game
        /// </summary>
        [SerializeField]
        private GameObject winMessage;

        /// <summary>
        /// Message that is displayed when the player loses the game
        /// </summary>
        [SerializeField]
        private GameObject loseMessage;

        /// <summary>
        /// The current UI.
        /// </summary>
        public static UI Current { get; private set; }

        /// <summary>
        /// The unit health UI
        /// </summary>
        public HealthUI HealthUI { get; private set; }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public void Init()
        {
            // Makes this the current UI which can be accessed by any class
            Current = this;

            HealthUI = GetComponentInChildren<HealthUI>();
            HealthUI.Init();
        }

        /// <summary>
        /// Resets the UI.
        /// </summary>
        public void ResetUI()
        {
            DisplayWinMessage(false);
            DisplayLoseMessage(false);
        }

        /// <summary>
        /// Displays or hides the win message.
        /// </summary>
        /// <param name="display">Should the message be displayed</param>
        public void DisplayWinMessage(bool display)
        {
            winMessage.SetActive(display);
        }

        /// <summary>
        /// Displays or hides the lose message.
        /// </summary>
        /// <param name="display">Should the message be displayed</param>
        public void DisplayLoseMessage(bool display)
        {
            loseMessage.SetActive(display);
        }
    }
}
