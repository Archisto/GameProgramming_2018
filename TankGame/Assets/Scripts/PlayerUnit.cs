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

            if (ReadFiringInput())
            {
                Fire();
            }

            leftTreads = (transform.localPosition + 2 * Vector3.left);
            rightTreads = (transform.localPosition + 2 * Vector3.right);
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

        private bool ReadFiringInput()
        {
            return Input.GetButtonDown("Fire1");
        }

        private void OnDrawGizmos()
        {
            if (input != null)
            {
                Vector3 leftTarget = leftTreads;
                Vector3 rightTarget = rightTreads;

                if (input.x < 0)
                {
                    rightTarget += Vector3.forward;
                }
                else if (input.x > 0)
                {
                    leftTarget += Vector3.forward;
                }
                else
                {
                    leftTarget += Vector3.forward;
                    rightTarget += Vector3.forward;
                }

                Gizmos.color = Color.red;

                Gizmos.DrawLine(transform.localToWorldMatrix * leftTreads, transform.localToWorldMatrix * leftTarget);
                Gizmos.DrawLine(transform.localToWorldMatrix * rightTreads, transform.localToWorldMatrix * rightTarget);
            }
        }
    }
}
