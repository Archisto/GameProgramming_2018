using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.WaypointSystem;
using TankGame.Systems;

namespace TankGame.AI
{
    public class PatrolState : AIStateBase
    {
        private Path _path;
        private Direction _direction;
        private float _arriveDistance;

        public Waypoint CurrentWaypoint { get; private set; }

        public PatrolState(EnemyUnit owner, Path path, Direction direction,
                           float arriveDistance)
            : base()
        {
            State = AIStateType.Patrol;
            Owner = owner;
            _path = path;
            _direction = direction;
            _arriveDistance = arriveDistance;
            AddTransition(AIStateType.FollowTarget);
        }

        public override void Activate()
        {
            base.Activate();
            CurrentWaypoint = _path.GetClosestWaypoint(Owner.transform.position);
        }

        public override void Update()
        {
            // Should we change the state?
            //    If yes, change state and return

            if ( !ChangeState() )
            {
                // Are we close enough to the current waypoint?
                //    If yes, get the next waypoint
                // Move towards the current waypoint
                // Rotate -||-
                CurrentWaypoint = GetWaypoint();
                Owner.Mover.Move(CurrentWaypoint.Position);

                Debug.Log("Moving");
            }
        }

        private bool ChangeState()
        {
            int mask = LayerMask.GetMask("Player");
            Collider[] players = Physics.OverlapSphere(Owner.transform.position,
                Owner.DetectEnemyDistance, mask);

            if (players.Length > 0)
            {
                PlayerUnit player = players[0].gameObject.GetComponent<PlayerUnit>();
                Owner.TargetPlayer = player;
                Owner.PerformTransition(AIStateType.FollowTarget);

                Debug.Log("Changed state");

                return true;
            }

            return false;
        }

        private Waypoint GetWaypoint()
        {
            Waypoint result = CurrentWaypoint;
            Vector3 toWaypointVector = CurrentWaypoint.Position - Owner.transform.position;
            float toWaypointSqr = toWaypointVector.sqrMagnitude;
            float sqrArriveDistance = _arriveDistance * _arriveDistance;

            if (toWaypointSqr <= sqrArriveDistance)
            {
                result = _path.GetNextWaypoint(CurrentWaypoint, ref _direction);
            }

            return result;
        }
    }
}
