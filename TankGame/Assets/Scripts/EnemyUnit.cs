using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TankGame.AI;
using TankGame.WaypointSystem;

namespace TankGame
{
    /// <summary>
    /// An AI-controlled enemy unit for the player to fight with.
    /// </summary>
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

        /// <summary>
        /// A list of AI states
        /// </summary>
        private IList<AIStateBase> states = new List<AIStateBase>();

        /// <summary>
        /// The current AI state.
        /// </summary>
        private AIStateBase CurrentState { get; set; }

        /// <summary>
        /// The distance at which an enemy (a player unit) is detected.
        /// </summary>
        public float DetectEnemyDistance { get { return detectEnemyDistance; } }

        /// <summary>
        /// The distance at which the enemy unit starts shooting at its target.
        /// </summary>
        public float ShootingDistance { get { return shootingDistance; } }

        /// <summary>
        /// The target which the enemy unit pursues and attempts to kill.
        /// </summary>
        public PlayerUnit Target { get; set; }

        /// <summary>
        /// A vector from the enemy unit to the target.
        /// </summary>
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

        /// <summary>
        /// Initializes the state system.
        /// </summary>
        private void InitStates()
        {
            PatrolState patrol = new PatrolState(
                this, path, direction, arriveDistance);
            FollowTargetState followTarget = new FollowTargetState(this);
            ShootState shoot = new ShootState(this);
            states.Add(patrol);
            states.Add(followTarget);
            states.Add(shoot);

            // Starts with the patrol state
            CurrentState = patrol;
            CurrentState.Activate();
        }

        /// <summary>
        /// Updates the object each frame.
        /// </summary>
        protected override void Update()
        {
            base.Update();

            if (Health == null || !Health.IsDead)
            {
                CurrentState.Update();
            }
        }

        /// <summary>
        /// Fires a projectile.
        /// </summary>
        public override void Fire()
        {
            if ( !GameManager.Instance.EnemyWeaponsDisabled )
            {
                base.Fire();
            }
        }

        /// <summary>
        /// Changes the AI state.
        /// </summary>
        /// <param name="targetState">AI state type</param>
        /// <returns></returns>
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
                //Debug.Log("Changed state to " + state);
            }

            return result;
        }

        /// <summary>
        /// Gets an AI state by its type.
        /// </summary>
        /// <param name="stateType">A state type</param>
        /// <returns>An AI state</returns>
        private AIStateBase GetStateByType(AIStateType stateType)
        {
            // Returns the first object from the state list whose State property's
            // value equals to stateType. If no object is found, null is returned.
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

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            // If the enemy unit is not dead, draws spheres around it
            // to display its detection and shooting distances
            if (Health == null || !Health.IsDead)
            {
                DrawDetectEnemyDistance();
                DrawShootingDistance(); 
            }
        }

        /// <summary>
        /// Draws a sphere to display the enemy unit's detection distance.
        /// </summary>
        private void DrawDetectEnemyDistance()
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(transform.position, detectEnemyDistance);
        }

        /// <summary>
        /// Draws a sphere to display the enemy unit's shooting distance.
        /// </summary>
        private void DrawShootingDistance()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, shootingDistance);
        }
    }
}
