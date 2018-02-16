using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TankGame.AI;
using TankGame.WaypointSystem;

namespace TankGame
{
    public class EnemyUnit : Unit
    {
        [SerializeField]
        private float detectEnemyDistance;

        [SerializeField]
        private float shootingDistance;

        [SerializeField]
        private Path path;

        [SerializeField]
        private float arriveDistance;

        [SerializeField]
        private Direction direction = Direction.Forward;

        private IList<AIStateBase> states = new List<AIStateBase>();
        private AIStateBase CurrentState { get; set; }

        public float DetectEnemyDistance { get { return detectEnemyDistance; } }
        public float ShootingDistance { get { return shootingDistance; } }
        public PlayerUnit Target { get; set; }

        public Vector3? ToTargetVector
        {
            get
            {
                if (Target != null)
                {
                    return Target.transform.position - transform.position;
                }

                return null;
            }
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public override void Init()
        {
            // Runs the base classes' implementation of the Init method.
            // Initializes Mover and Weapon.
            base.Init();

            // Initializes the state system
            InitStates();
        }
        private void InitStates()
        {
            PatrolState patrol = new PatrolState(
                this, path, direction, arriveDistance);
            FollowTargetState followTarget = new FollowTargetState(this);
            ShootState shoot = new ShootState(this);
            states.Add(patrol);
            states.Add(followTarget);
            states.Add(shoot);

            CurrentState = patrol;
            CurrentState.Activate();
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        protected override void Update()
        {
            CurrentState.Update();
        }

        public bool PerformTransition(AIStateType targetState)
        {
            if ( !CurrentState.CheckTransition(targetState) )
            {
                Debug.Log("State change failed");
                return false;
            }

            bool result = false;

            AIStateBase state = GetStateByType(targetState);
            if (state != null)
            {
                CurrentState.StartDeactivating();
                CurrentState = state;
                CurrentState.Activate();
                result = true;
                Debug.Log("Changed state to " + state);
            }

            return result;
        }

        private AIStateBase GetStateByType(AIStateType stateType)
        {
            // Returns the first object from the state list whose State property's
            // value equals to stateType. If no object was found, null is returned.
            return states.FirstOrDefault( state => state.State == stateType );

            // Does the same as the previous, single line
            //foreach (AIStateBase state in states)
            //{
            //    if (state.State == stateType)
            //    {
            //        return state;
            //    }
            //}

            //return null;
        }

        private void OnDrawGizmos()
        {
            DrawDetectEnemyDistance();
            DrawShootingDistance();
        }

        private void DrawDetectEnemyDistance()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, detectEnemyDistance);
        }

        private void DrawShootingDistance()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, shootingDistance);
        }
    }
}
