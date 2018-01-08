using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public abstract class Unit : MonoBehaviour
    {
        public virtual void Init()
        {

        }

        public virtual void Clear()
        {

        }

        /// <summary>
        /// Update is called once per frame.
        /// An abstract method has to be defined
        /// in a non-abstract child class.
        /// </summary>
        protected abstract void Update();
    }
}
