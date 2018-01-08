using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class TransformMover : MonoBehaviour, IMover
    {
        private float moveSpeed, turnSpeed;

        public void Init(float moveSpeed, float turnSpeed)
        {
            this.moveSpeed = moveSpeed;
            this.turnSpeed = turnSpeed;
        }

        public void Move(Vector3 input)
        {
            MoveUnit(input.y);
            TurnUnit(input.x);
        }

        public void MoveUnit(float amount)
        {
            Vector3 position = transform.position;
            Vector3 movement = transform.forward * amount * moveSpeed * Time.deltaTime;
            position += movement;
            transform.position = position;
        }

        public void TurnUnit(float amount)
        {
            float rotation = amount * turnSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotation, Space.Self);
        }

        public void TurnUnitAxis(Vector3 axis, float amount)
        {
            float rotation = amount * turnSpeed * Time.deltaTime;
            transform.Rotate(axis, rotation, Space.Self);
        }

        public void TurnUnitAxis(Vector3 axis, float amount, float min, float max)
        {
            Quaternion oldRotation = transform.rotation;

            float rotation = amount * turnSpeed * Time.deltaTime;
            transform.Rotate(axis, rotation, Space.Self);

            if (!RotationIsValid(axis, min, max))
            {
                transform.rotation = oldRotation;
            }
        }

        private bool RotationIsValid(Vector3 axis, float min, float max)
        {
            float checkedValue = 0;

            if (axis == Vector3.right)
            {
                checkedValue = transform.rotation.eulerAngles.x;
            }
            else if (axis == Vector3.up)
            {
                checkedValue = transform.rotation.eulerAngles.y;
            }
            else if (axis == Vector3.forward)
            {
                checkedValue = transform.rotation.eulerAngles.z;
            }

            // Quick fix for rolling from <0 to <360
            // 0-x => 360-x
            if (checkedValue > 180 && min < 0)
            {
                min = 360 + min;
                max = 360 + max;
            }

            if (checkedValue < min || checkedValue > max)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
