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
        /// <param name="position">The position of a target object</param>
        public void Move(Vector3 position)
        {
            Move(moveSpeed);
            Turn(position, true);
        }

        /// <summary>
        /// Moves the game object.
        /// </summary>
        /// <param name="amount">Movement amount</param>
        public void Move(float amount)
        {
            // FIXME: amount * moveSpeed cubes the moveSpeed, not supposed to
            // Can't fix right away, results in slow units

            Vector3 position = transform.position;
            Vector3 movement = transform.forward * amount * moveSpeed * Time.deltaTime;
            position += movement;
            transform.position = position;
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
        /// <param name="amount">Turning amount</param>
        public void Turn(float amount)
        {
            float rotation = amount * turnSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotation, Space.Self);
        }

        /// <summary>
        /// Turns the game object on an axis.
        /// </summary>
        /// <param name="axis">An axis</param>
        /// <param name="amount">Movement amount</param>
        public void TurnAxis(Vector3 axis, float amount)
        {
            float rotation = amount * turnSpeed * Time.deltaTime;
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
            // TODO: Check if the rotation is valid before assigning it to the game object.
            // Transforms automatically clamp angles between 0 and 360.
            
            Quaternion oldRotation = transform.rotation;

            float rotation = amount * turnSpeed * Time.deltaTime;
            transform.Rotate(axis, rotation, Space.Self);

            if (!RotationIsValid(axis, min, max))
            {
                transform.rotation = oldRotation;
            }
        }

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
