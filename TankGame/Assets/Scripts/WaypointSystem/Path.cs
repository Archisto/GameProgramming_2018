using UnityEngine;
using System.Collections.Generic;
using System;

namespace TankGame.WaypointSystem
{
    public enum PathType
    {
        Loop, // After reaching the last waypoint, user moves to the first
        PingPong // User changes direction after reaching the last waypoint
    }

    public enum Direction
    {
        Forward,
        Backward
    }

    public class Path : MonoBehaviour
    {
        [SerializeField]
        private PathType _pathType;

        private List<Waypoint> _waypoints;

        // Dictionary which defines a color for each path type.
        private readonly Dictionary<PathType, Color> _pathColors =
            new Dictionary<PathType, Color>()
            {
                { PathType.Loop, Color.green },
                { PathType.PingPong, Color.red }
            };

        public List<Waypoint> Waypoints
        {
            get
            {
                // Populates the waypoints if not done that yet and in editor every time
                if (_waypoints == null ||
                    _waypoints.Count == 0 ||
                    !Application.isPlaying)
                {
                    PopulateWaypoints();
                }
                return _waypoints;
            }
        }

        public Waypoint GetClosestWaypoint(Vector3 position)
        {
            float smallestSqrMagnitude = float.PositiveInfinity;
            Waypoint closest = null;
            for (int i = 0; i < Waypoints.Count; i++)
            {
                Waypoint waypoint = Waypoints[i];
                Vector3 toWaypointVector = waypoint.Position - position;
                float currentSqrMagnitude = toWaypointVector.sqrMagnitude;
                if (currentSqrMagnitude < smallestSqrMagnitude)
                {
                    closest = waypoint;
                    smallestSqrMagnitude = currentSqrMagnitude;
                }
            }

            return closest;
        }

        public Waypoint GetNextWaypoint(Waypoint currentWaypoint,
            ref Direction direction)
        {
            Waypoint nextWaypoint = null;
            for (int i = 0; i < Waypoints.Count; i++)
            {
                if (Waypoints[i] == currentWaypoint)
                {
                    switch (_pathType)
                    {
                        case PathType.Loop:
                        nextWaypoint = GetNextWaypointLoop(i, direction);
                        break;
                        case PathType.PingPong:
                        nextWaypoint = GetNextWaypointPingPong(i, ref direction);
                        break;
                    }
                    break;
                }
            }
            return nextWaypoint;
        }

        private Waypoint GetNextWaypointPingPong(int currentWaypointIndex,
            ref Direction direction)
        {
            Waypoint nextWaypoint = null;
            switch (direction)
            {
                case Direction.Forward:
                if (currentWaypointIndex < Waypoints.Count - 1)
                {
                    nextWaypoint = Waypoints[currentWaypointIndex + 1];
                }
                else
                {
                    nextWaypoint = Waypoints[currentWaypointIndex - 1];
                    direction = Direction.Backward;
                }
                break;
                case Direction.Backward:
                if (currentWaypointIndex > 0)
                {
                    nextWaypoint = Waypoints[currentWaypointIndex - 1];
                }
                else
                {
                    nextWaypoint = Waypoints[1];
                    direction = Direction.Forward;
                }
                break;
            }
            return nextWaypoint;
        }

        private Waypoint GetNextWaypointLoop(int currentWaypointIndex,
            Direction direction)
        {
            Waypoint nextWaypoint = direction == Direction.Forward
                ? Waypoints[++currentWaypointIndex % Waypoints.Count]
                : Waypoints[((--currentWaypointIndex >= 0) ? currentWaypointIndex : Waypoints.Count - 1) % Waypoints.Count];
            return nextWaypoint;
        }

        private void PopulateWaypoints()
        {
            int childCount = transform.childCount;
            _waypoints = new List<Waypoint>(childCount);
            for (int i = 0; i < childCount; i++)
            {
                Transform waypointTransform = transform.GetChild(i);
                Waypoint waypoint = waypointTransform.GetComponent<Waypoint>();
                if (waypoint != null)
                {
                    _waypoints.Add(waypoint);
                }
            }
        }

        /// <summary>
        /// Draws lines between waypoints
        /// </summary>
        protected void OnDrawGizmos()
        {
            Gizmos.color = _pathColors[_pathType];
            if (Waypoints.Count > 1)
            {
                for (int i = 1; i < Waypoints.Count; i++)
                {
                    // Draw line from previous waypoint to current.
                    Gizmos.DrawLine(Waypoints[i - 1].Position, Waypoints[i].Position);
                }
                if (_pathType == PathType.Loop)
                {
                    // From last waypoint to first 
                    Gizmos.DrawLine(Waypoints[Waypoints.Count - 1].Position,
                        Waypoints[0].Position);
                }
            }
        }
    }
}
