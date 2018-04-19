using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    /// <summary>
    /// Moves the game object of which this a component.
    /// </summary>
    public class TransformMover : MonoBehaviour, IMover
    {
        private float moveSpeed, turnSpeed;

        /// <summary>
        /// Initializes speeds.
        /// </summary>
        /// <param name="moveSpeed">Moving speed</param>
        /// <param name="turnSpeed">Turning speed</param>
        public void Init(float moveSpeed, float turnSpeed)
        {
            this.moveSpeed = moveSpeed;
            this.turnSpeed = turnSpeed;
        }

        /// <summary>
        /// Moves the game object.
        /// </summary>
        /// <param name="amount">Movement speed factor</param>
        public void Move(float amount)
        {
            Vector3 position = transform.position;
            Vector3 movement = transform.forward * moveSpeed * amount * Time.deltaTime;
            position += movement;
            transform.position = position;
        }

        /// <summary>
        /// Moves the game object.
        /// </summary>
        /// <param name="position">The position of a target object</param>
        public void Move(Vector3 position)
        {
            Move(1);
            Turn(position, true);
        }

        /// <summary>
        /// Moves the game object using tank controls
        /// (x = turning, y = moving).
        /// </summary>
        /// <param name="direction"></param>
        public void MoveTank(Vector2 direction)
        {
            Move(direction.y);
            Turn(direction.x);
        }

        /// <summary>
        /// Turns the game object towards a target.
        /// </summary>
        /// <param name="target">A target object</param>
        /// <param name="keepSameY">Is the y-axis left unchanged</param>
        public void Turn(Vector3 target, bool keepSameY)
        {
            Vector3 direction = target - transform.position;

            if (keepSameY)
            {
                direction.y = transform.position.y;
            }

            direction = direction.normalized;

            float turnSpeedRad = Mathf.Deg2Rad * turnSpeed * Time.deltaTime;
            Vector3 rotation = Vector3.RotateTowards(transform.forward, direction, turnSpeedRad, 0);
            transform.rotation = Quaternion.LookRotation(rotation, transform.up);
        }

        /// <summary>
        /// Turns the game object.
        /// </summary>
        /// <param name="amount">Turning speed factor</param>
        public void Turn(float amount)
        {
            TurnAxis(Vector3.up, amount);
        }

        /// <summary>
        /// Turns the game object on an axis.
        /// </summary>
        /// <param name="axis">An axis</param>
        /// <param name="amount">Turning speed factor</param>
        public void TurnAxis(Vector3 axis, float amount)
        {
            float rotation = turnSpeed * amount * Time.deltaTime;
            transform.Rotate(axis, rotation, Space.Self);
        }

        /// <summary>
        /// Turns the game object on an axis.
        /// </summary>
        /// <param name="axis">An axis</param>
        /// <param name="amount">Movement amount</param>
        /// <param name="min">Minimum angle (degrees)</param>
        /// <param name="max">Maximum angle (degrees)</param>
        public void TurnAxis(Vector3 axis, float amount, float min, float max)
        {
            //Vector3 newRotation = ValidVector
            //    (transform.rotation.eulerAngles, axis, amount, min, max);
            //transform.rotation = Quaternion.Euler(newRotation);

            Quaternion oldRotation = transform.rotation;

            float rotation = amount * turnSpeed * Time.deltaTime;
            transform.Rotate(axis, rotation, Space.Self);

            if (!RotationIsValid(axis, min, max))
            {
                transform.rotation = oldRotation;
            }
        }

        //private Vector3 ValidVector(Vector3 unalteredVector, Vector3 axis, float amount, float min, float max)
        //{
        //    Vector3 newVector = unalteredVector;
        //    newVector += axis * amount;

        //    // The final value on the given axis
        //    float axisValue = 0;

        //    if (axis.x > 1)
        //    {
        //        axisValue = newVector.x;
        //    }
        //    else if (axis.y > 1)
        //    {
        //        axisValue = newVector.y;
        //    }
        //    else if (axis.z > 1)
        //    {
        //        axisValue = newVector.z;
        //    }

        //    if (axisValue >= min && axisValue <= max)
        //    {
        //        return newVector;
        //    }
        //    else
        //    {
        //        return unalteredVector;
        //    }
        //}

        /// <summary>
        /// Checks if the game object's rotation on an axis is valid.
        /// </summary>
        /// <param name="axis">An axis</param>
        /// <param name="min">Minimum angle (degrees)</param>
        /// <param name="max">Maximum angle (degrees)</param>
        /// <returns></returns>
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
