using UnityEngine;
using System.Collections;

namespace TankGame.WaypointSystem
{
    public interface IPathUser
    {
        Waypoint CurrentWaypoint { get; }
        Direction Direction { get; set; }
        Vector3 Position { get; set; }
    }
}
