using UnityEngine;

namespace TankGame
{
    /// <summary>
    /// Makes the Camera object follow a target transform.
    /// </summary>
    public class CameraFollow : MonoBehaviour, ICameraFollow
    {
        /// <summary>
        /// The camera's angle
        /// </summary>
        [SerializeField]
        private float angle;

        /// <summary>
        /// The camera's distance from the target transform
        /// </summary>
        [SerializeField]
        private float distance;

        /// <summary>
        /// The target transform at which the camera looks
        /// </summary>
        [SerializeField]
        private Transform target;

        /// <summary>
        /// Updates the object after all normal updating has been done.
        /// To prevent shaking, the camera has to be moved after the
        /// followed object has moved.
        /// </summary>
        private void LateUpdate()
        {
            if (target != null)
            {
                UpdatePositionAndRotation();
            }
        }

        /// <summary>
        /// Updates the camera's position and rotation.
        /// </summary>
        private void UpdatePositionAndRotation()
        {
            Quaternion newRotation;
            Vector3 newPosition;

            // Converts the angle to radians
            float angleRad = Mathf.Deg2Rad * angle;

            // Calculates the new position
            float forward = Mathf.Sin(angleRad) * distance;
            float up = Mathf.Cos(angleRad) * distance;
            newPosition = target.position +
                (target.forward * forward * -1) +
                (Vector3.up * up);

            // Calculates the new rotation with these rules:
            // - increasing the angle in the x-axis raises the camera's view
            // - when the angle in the x-axis is 0, the camera looks down
            // - the camera looks at the same direction as the target in
            //   the y-axis
            float angleX = -1 * angle + 90;
            newRotation =
                Quaternion.Euler(angleX, target.rotation.eulerAngles.y, 0);

            // Sets the new position and rotation to the camera
            transform.position = newPosition;
            transform.rotation = newRotation;
        }

        /// <summary>
        /// Sets the camera's angle.
        /// </summary>
        /// <param name="angle">an angle for the camera
        /// (0 = down, 90 = straight forward)</param>
        public void SetAngle(float angle)
        {
            this.angle = Mathf.Clamp(angle, 0, 180);
        }

        /// <summary>
        /// Sets the camera's distance from the target transform.
        /// </summary>
        /// <param name="distance">a distance</param>
        public void SetDistance(float distance)
        {
            this.distance = Mathf.Clamp(distance, 0, 80);
        }

        /// <summary>
        /// Sets the target transform at which the camera looks.
        /// </summary>
        /// <param name="targetTransform">a target transform</param>
        public void SetTarget(Transform targetTransform)
        {
            target = targetTransform;
        }
    }
}
