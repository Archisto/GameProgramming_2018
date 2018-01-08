using UnityEngine;

namespace TankGame
{
    public interface IMover
    {
        /// <summary>
        /// Initializes the mover
        /// </summary>
        /// <param name="moveSpeed">move speed (units/s)</param>
        /// <param name="turnSpeed">turn speed (degrees/s)</param>
        void Init(float moveSpeed, float turnSpeed);

        /// <summary>
        /// Moves the mover.
        /// </summary>
        /// <param name="input">input from the user</param>
        void Move(Vector3 input);
    }
}
