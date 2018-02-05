using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class LookAt : MonoBehaviour
    {
        [SerializeField]
        private Transform target;

        [SerializeField]
        private bool xAxis = true;

        [SerializeField]
        private bool yAxis = true;

        [SerializeField]
        private bool zAxis = true;

        [SerializeField]
        private float turnSpeed = 1;

        protected Vector3 targetPosition;

        public Transform Target
        {
            get
            {
                return target;
            }
        }

        // Update is called once per frame
        public virtual void Update()
        {
            targetPosition = target.position;

            RotateAtTargetAroundAxis();
        }

        public void RotateAtTarget()
        {
            Vector3 forward = targetPosition - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);
            Quaternion limitedRotation =
                Quaternion.Lerp(transform.rotation,
                                targetRotation,
                                turnSpeed * Time.deltaTime);

            transform.rotation = limitedRotation;
        }

        public void RotateAtTargetAroundAxis()
        {
            Vector3 forward = targetPosition - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(forward, Vector3.up);

            // Limits axes if not all of them are used
            #region AxisLimit:
            if (!(xAxis && yAxis && zAxis))
            {
                Vector3 currentRotation = transform.rotation.eulerAngles;
                float x = currentRotation.x;
                float y = currentRotation.y;
                float z = currentRotation.z;
                if (xAxis)
                {
                    x = targetRotation.eulerAngles.x;
                }
                if (yAxis)
                {
                    y = targetRotation.eulerAngles.y;
                }
                if (zAxis)
                {
                    z = targetRotation.eulerAngles.z;
                }
                targetRotation = Quaternion.Euler(x, y, z);
            }
            #endregion

            Quaternion limitedRotation =
                Quaternion.Lerp(transform.rotation,
                                targetRotation,
                                turnSpeed * Time.deltaTime);

            transform.rotation = limitedRotation;
        }
    }
}
