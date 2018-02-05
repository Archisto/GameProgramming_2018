using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame.AI
{
    public enum AIStateType
    {
        Error = 0,
        Patrol = 1,
        FollowTarget = 2,
        Shoot = 3
    }

    public abstract class AIStateBase
    {
        /// <summary>
        /// The state related to this object
        /// </summary>
        public AIStateType State { get; protected set; }

        /// <summary>
        /// The target states to which we can transition from this state
        /// </summary>
        public IList<AIStateType> TargetStates { get; protected set; }

        /// <summary>
        /// The owner Unit of this state (Unit is the state controller class)
        /// </summary>
        public EnemyUnit Owner { get; protected set; }

        /// <summary>
        /// Constructor method.
        /// </summary>
        public AIStateBase()
        {
            TargetStates = new List<AIStateType>();
        }

        public abstract void Update();

        /// <summary>
        /// Adds a new target state to the target state
        /// list if it isn't on the list already.
        /// </summary>
        /// <param name="targetState">A target state to
        /// be added to the target state list</param>
        /// <returns>Was the state added to the list</returns>
        public bool AddTransition(AIStateType targetState)
        {
            // Use the extension method AddUnique to add a target state.
            // Will return false if the state was already added.
            return TargetStates.AddUnique(targetState);
        }

        /// <summary>
        /// Removes a target state from the target
        /// state list if it is on the list.
        /// </summary>
        /// <param name="targetState">A target state to
        /// be removed from the target state list</param>
        /// <returns>Was the state removed from the list</returns>
        public bool RemoveTransition(AIStateType targetState)
        {
            return TargetStates.Remove(targetState);
        }

        /// <summary>
        /// Checks if the target state is on the target state list.
        /// </summary>
        /// <param name="targetState">A target state</param>
        /// <returns>Is the traget state on the target state list</returns>
        public virtual bool CheckTransition(AIStateType targetState)
        {
            return TargetStates.Contains(targetState);
        }

        public virtual void Activate()
        {

        }

        public virtual void StartDeactivating()
        {

        }
    }
}
