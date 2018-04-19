using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    /// <summary>
    /// Draws a line from 'from' to 'to'.
    /// </summary>
    public class DebugLine : MonoBehaviour
    {
        public Color color = Color.white;
        public Vector3 from;
        public Vector3 to;

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawLine(from, to);
        }
    }
}
