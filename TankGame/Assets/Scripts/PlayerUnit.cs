using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    /// <summary>
    /// A tank unit controlled by a player.
    /// </summary>
    public class PlayerUnit : Unit
    {
        /// <summary>
        /// The name of the Horizontal axis input
        /// </summary>
        [SerializeField]
        private string horizontalAxis = "Horizontal";

        /// <summary>
        /// The name of the Vertical axis input
        /// </summary>
        [SerializeField]
        private string verticalAxis = "Vertical";

        /// <summary>
        /// The name of the Cannon Horizontal axis input
        /// </summary>
        [SerializeField]
        private string cannonHorizontalAxis = "Cannon Horizontal";

        /// <summary>
        /// The name of the Cannon Vertical axis input
        /// </summary>
        [SerializeField]
        private string cannonVerticalAxis = "Cannon Vertical";

        private Vector2 input;
        private Vector3 leftTreads;
        private Vector3 rightTreads;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public override void Init()
        {
            // Runs the base classes' implementation of the Init method.
            // Initializes Mover and Weapon.
            base.Init();

            IsPlayerUnit = true;
        }

        /// <summary>
        /// Updates the object each frame.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            // Reads player input if the unit is not dead
            if ( !Health.IsDead )
            {
                HandleInput();
            }
        }

        /// <summary>
        /// Handles player input.
        /// </summary>
        private void HandleInput()
        {
            input = ReadMovementInput();
            Vector3 cannonInput = ReadCannonInput();

            Mover.MoveTank(input);

            ((TransformMover) TankHeadMover).TurnAxis(Vector3.up, cannonInput.x);
            ((TransformMover) BarrelMover).TurnAxis(Vector3.right, -1f * cannonInput.y, -25f, 6.5f);

            if (ReadShootingInput())
            {
                Fire();
            }

            //RecordTreadsPositions();
        }

        /// <summary>
        /// Reads the player's input for moving the unit.
        /// </summary>
        /// <returns>The unit's turning and movement vector</returns>
        private Vector2 ReadMovementInput()
        {
            float turning = Input.GetAxis(horizontalAxis);
            float movement = Input.GetAxis(verticalAxis);

            return new Vector2(turning, movement);
        }

        /// <summary>
        /// Reads the player's input for moving the unit's cannon.
        /// </summary>
        /// <returns>The unit's cannon's turning and tilt vector</returns>
        private Vector3 ReadCannonInput()
        {
            float turning = Input.GetAxis(cannonHorizontalAxis);
            float tilt = Input.GetAxis(cannonVerticalAxis);

            return new Vector3(turning, tilt);
        }

        /// <summary>
        /// Reads the player's input for firing.
        /// </summary>
        /// <returns>Should the cannon be fired</returns>
        private bool ReadShootingInput()
        {
            return Input.GetButtonDown("Fire1");
        }

        /// <summary>
        /// Records the unit's treads' positions in relation to its body
        /// to be used for the tread speed gizmos.
        /// </summary>
        private void RecordTreadsPositions()
        {
            leftTreads = transform.position;
            rightTreads = transform.position;

            var rot = Quaternion.AngleAxis(90, Vector3.up);
            var localDirection = rot * Vector3.forward;
            var worldDirection = transform.TransformDirection(localDirection);

            leftTreads += worldDirection * 0.6f;
            rightTreads -= worldDirection * 0.6f;

            //leftTreads = ((Vector4) transform.localPosition + transform.worldToLocalMatrix * Vector3.left);
            //rightTreads = ((Vector4) transform.localPosition + transform.worldToLocalMatrix * Vector3.right);

            //leftTreads.z = leftTreads.z * -1;
            //rightTreads.z = rightTreads.z * -1;

            leftTreads.y = 0.5f;
            rightTreads.y = 0.5f;
        }

        /// <summary>
        /// Handles spawning if the unit is dead
        /// and the player is not out of lives.
        /// </summary>
        protected override void UpdateRespawn()
        {
            if (GameManager.Instance.PlayerLives > 0)
            {
                base.UpdateRespawn();
            }
        }

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            //DrawTreadSpeeds();
        }

        /// <summary>
        /// Draws lines marking the speeds of each tread.
        /// </summary>
        private void DrawTreadSpeeds()
        {
            // FIXME

            //if (input != Vector3.zero)
            {
                var forwardDir = transform.TransformDirection(Vector3.forward);

                float leftTurning = (input.x > 0 ? input.x : 0);
                float rightTurning = (input.x < 0 ? -1f * input.x : 0);

                if (input.y < 0)
                {
                    leftTurning = -1f * leftTurning;
                    rightTurning = -1f * rightTurning;
                }

                float leftLength = input.y - leftTurning / 2;
                float rightLength = input.y - rightTurning / 2;

                Vector3 leftTarget = leftTreads + leftLength * forwardDir;
                Vector3 rightTarget = rightTreads + rightLength * forwardDir;

                if (leftLength >= 0)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawLine(leftTreads, leftTarget);

                if (rightLength >= 0)
                {
                    Gizmos.color = Color.blue;
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawLine(rightTreads, rightTarget);
            }
        }
    }
}
