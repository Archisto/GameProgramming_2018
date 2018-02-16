using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    [RequireComponent(typeof(IMover))]
    public abstract class Unit : MonoBehaviour, IDamageReceiver
    {
        [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField]
        private float turnSpeed = 5f;

        [SerializeField]
        private GameObject tankHead;

        [SerializeField]
        private int startingHealth;

        private TransformMover mover;
        private IMover headMover;
        private IMover barrelMover;

        private void Awake()
        {
            Init();
        }

        private void OnDestroy()
        {
            // Stops listening to the UnitDied event
            Health.UnitDied -= HandleUnitDied;
        }

        /// <summary>
        /// Update is called once per frame.
        /// An abstract method has to be defined
        /// in a non-abstract child class.
        /// </summary>
        protected abstract void Update();

        public virtual void Init()
        {
            Weapon = GetComponentInChildren<Weapon>();
            if (Weapon != null)
            {
                Weapon.Init(this);
            }

            mover = gameObject.GetOrAddComponent<TransformMover>();
            mover.Init(moveSpeed, turnSpeed);

            TransformMover[] headMovers =
                tankHead.GetComponentsInChildren<TransformMover>();
            headMover = headMovers[0];
            barrelMover = headMovers[1];

            headMover.Init(0f, turnSpeed);
            barrelMover.Init(0f, turnSpeed);

            Health = new Health(this, startingHealth);

            // Registering to listen to the UnitDied event.
            // The method must return void and have one parameter.
            Health.UnitDied += HandleUnitDied;
        }

        public Weapon Weapon { get; protected set; }

        public TransformMover Mover
        {
            get
            {
                return mover;
            }
        }

        protected IMover TankHeadMover
        {
            get
            {
                return headMover;
            }
        }

        protected IMover BarrelMover
        {
            get
            {
                return barrelMover;
            }
        }

        public Health Health { get; protected set; }

        public virtual void Fire()
        {
            Weapon.Fire();

            //// Calculates the firing direction:
            //// from the barrel's base to its tip in world space
            //Vector3 firingDirection = barrelTip.transform.position - ((TransformMover) barrelMover).transform.position;

            //// Changes the firing direction vector's distance to 1
            //firingDirection.Normalize();

            //// Creates a new projectile
            //Projectile newProjectile = Instantiate(projectilePrefab);
            //newProjectile.Init(barrelTip.transform.position, firingDirection, 20);
        }

        public virtual void Clear()
        {

        }

        public void TakeDamage(int amount)
        {
            Health.TakeDamage(amount);
        }

        protected virtual void HandleUnitDied(Unit unit)
        {
            gameObject.SetActive(false);
        }
    }
}
