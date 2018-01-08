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

        protected override void Update()
        {
            Vector3 input = ReadMovementInput();
            Vector3 cannonInput = ReadCannonInput();

            Mover.Move(input);

            ((TransformMover) TankHeadMover).TurnUnitAxis(Vector3.up, cannonInput.x);
            ((TransformMover) BarrelMover).TurnUnitAxis(Vector3.right, -1f * cannonInput.y, -25f, 6.5f);

            if (ReadFiringInput())
            {
                Fire();
            }
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
    }
}
