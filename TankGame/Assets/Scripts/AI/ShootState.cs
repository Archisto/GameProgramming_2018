using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.AI
{
    public class ShootState : AIStateBase
    {
        public ShootState(EnemyUnit owner)
            : base(owner, AIStateType.Shoot)
        {
            AddTransition(AIStateType.Patrol);
            AddTransition(AIStateType.FollowTarget);
        }

        public override void Activate()
        {
            base.Activate();
        }

        public override void Update()
        {
            if ( !ChangeState() )
            {
                // Shoot at the target
                Owner.Fire();

                // Rotate at the target
                Owner.Mover.Turn(Owner.Target.transform.position, true);
            }
        }

        protected override bool ChangeState()
        {
            // Did the player die?
            // If yes, go to patrol state
            if (Owner.Target.Health.IsDead)
            {
                return Owner.PerformTransition(AIStateType.Patrol);
            }

            // Did the player get outside of the shooting range?
            // If yes, go to follow target state

            Vector3 toPlayerVector = Owner.transform.position - Owner.Target.transform.position;
            float sqrDistanceToPlayer = toPlayerVector.sqrMagnitude;
            if (sqrDistanceToPlayer > SqrShootingDistance)
            {
                return Owner.PerformTransition(AIStateType.FollowTarget);
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

            //    if (Vector3.Distance(Owner.transform.position, player.transform.position) > Owner.ShootingDistance)
            //    {
            //        Owner.PerformTransition(AIStateType.FollowTarget);
            //        return true;
            //    }
            //}
            //else
            //{
            //    Owner.Target = null;
            //    Owner.PerformTransition(AIStateType.Patrol);
            //    return true;
            //}

            return false;
        }
    }
}
