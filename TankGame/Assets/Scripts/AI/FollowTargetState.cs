using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.AI
{
    public class FollowTargetState : AIStateBase
    {
        public FollowTargetState(EnemyUnit owner)
            : base(owner, AIStateType.FollowTarget)
        {
            AddTransition(AIStateType.Patrol);
            AddTransition(AIStateType.Shoot);
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Update()
        {
            if ( !ChangeState() )
            {
                // Move towards the follow target
                // Rotate -||-
                Owner.Mover.Move(Owner.Target.transform.position);
            }
        }

        protected override bool ChangeState()
        {
            // Are we at shooting range?
            // If yes, go to shoot state

            Vector3 toPlayerVector = Owner.transform.position - Owner.Target.transform.position;
            float sqrDistanceToPlayer = toPlayerVector.sqrMagnitude;
            if (sqrDistanceToPlayer < SqrShootingDistance)
            {
                return Owner.PerformTransition(AIStateType.Shoot);
            }

            // Did the player get away?
            // If yes, go to patrol state

            else if (sqrDistanceToPlayer > SqrDetectEnemyDistance)
            {
                return Owner.PerformTransition(AIStateType.Patrol);
            }


            //int mask = LayerMask.GetMask("Player");

            //Collider[] players = Physics.OverlapSphere(Owner.transform.position,
            //    Owner.DetectEnemyDistance, mask);

            //if (players.Length > 0)
            //{
            //    PlayerUnit player = players[0].gameObject.GetComponentInHierarchy<PlayerUnit>();

            //    if (Vector3.Distance(Owner.transform.position, player.transform.position) <= Owner.ShootingDistance)
            //    {
            //        Owner.PerformTransition(AIStateType.Shoot);

            //        Debug.Log("Changed state to Shoot");

            //        return true;
            //    }
            //}
            //else
            //{
            //    Owner.Target = null;
            //    Owner.PerformTransition(AIStateType.Patrol);

            //    Debug.Log("Changed state to Patrol");

            //    return true;
            //}

            return false;
        }
    }
}
