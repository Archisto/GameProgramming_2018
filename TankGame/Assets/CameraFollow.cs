using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private float angle;

        [SerializeField]
        private float distance;

        [SerializeField]
        private Transform target;

        private void LateUpdate()
        {
            if (target != null)
            {
                GoToPosition();
            }
        }

        private void GoToPosition()
        {
            Quaternion newRotation = transform.rotation;
            Vector3 newPosition = target.position;

            float angleRad = Mathf.Deg2Rad * angle;

            //Vector3 targetPos = target.position;
            //float angle2 = 180 - 90 - angle;

            float a = Mathf.Sin(angleRad) * distance;
            float b = Mathf.Cos(angleRad) * distance;

            newPosition -= target.forward * a;
            newPosition += Vector3.up * b;

            float angleX = -1 * angle + 90;

            newRotation = Quaternion.Euler(angleX, target.rotation.eulerAngles.y, 0);

            //Vector3 targetToCamera = targetPos

            //newRotation.SetLookRotation();

            transform.position = newPosition;
            transform.rotation = newRotation;
        }
    }
}
