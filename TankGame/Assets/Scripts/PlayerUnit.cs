using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    [RequireComponent(typeof(TransformMover))]
    public class PlayerUnit : Unit
    {
        [SerializeField]
        private string horizontalAxis = "Horizontal";

        [SerializeField]
        private string verticalAxis = "Vertical";

        [SerializeField]
        private string cannonHorizontalAxis = "Cannon Horizontal";

        [SerializeField]
        private string cannonVerticalAxis = "Cannon Vertical";

        private Vector3 input;
        private Vector3 leftTreads;
        private Vector3 rightTreads;

        protected override void Update()
        {
            input = ReadMovementInput();
            Vector3 cannonInput = ReadCannonInput();

            Mover.Move(input);

            ((TransformMover) TankHeadMover).TurnAxis(Vector3.up, cannonInput.x);
            ((TransformMover) BarrelMover).TurnAxis(Vector3.right, -1f * cannonInput.y, -25f, 6.5f);

            if (ReadShootingInput())
            {
                Fire();
            }

            UpdateTreadsPositions();
        }

        private Vector3 ReadMovementInput()
        {
            float turning = Input.GetAxis(horizontalAxis);
            float movement = Input.GetAxis(verticalAxis);

            return new Vector3(turning, movement);
        }

        private Vector3 ReadCannonInput()
        {
            float turning = Input.GetAxis(cannonHorizontalAxis);
            float tilt = Input.GetAxis(cannonVerticalAxis);

            return new Vector3(turning, tilt);
        }

        private bool ReadShootingInput()
        {
            return Input.GetButtonDown("Fire1");
        }

        private void UpdateTreadsPositions()
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

        private void OnDrawGizmos()
        {
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
