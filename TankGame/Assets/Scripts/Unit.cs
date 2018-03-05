using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Persistence;

namespace TankGame
{
    [RequireComponent(typeof(IMover))]
    public abstract class Unit : MonoBehaviour, IDamageReceiver
    {
        #region Statics

        private static int idCounter = 0;

        public static int GetNextID()
        {
            var allUnits = FindObjectsOfType<Unit>();
            foreach (var unit in allUnits)
            {
                if (unit.ID >= idCounter)
                {
                    idCounter = unit.ID + 1;
                }
            }

            return idCounter;
        }

        #endregion Statics

        [SerializeField]
        private float moveSpeed = 5f;

        [SerializeField]
        private float turnSpeed = 5f;

        [SerializeField]
        private float respawnTime = 5f;

        [SerializeField]
        private GameObject tankHead;

        [SerializeField]
        private int startingHealth;

        [SerializeField]//, HideInInspector]
        private int id = -1;

        private TransformMover mover;
        private IMover headMover;
        private IMover barrelMover;

        private float timeOfDeath = 0;

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
        //protected abstract void Update();

        protected virtual void Update()
        {
            if (Health != null)
            {
                if (Health.IsDead)
                {
                    if (timeOfDeath == 0)
                    {
                        timeOfDeath = Time.time;
                    }
                    else if (Time.time - timeOfDeath >= respawnTime)
                    {
                        Respawn();
                    }
                }
            }
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

        public int ID
        {
            get { return id; }
            private set { id = value; }
        }

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

        //public virtual void Clear()
        //{

        //}

        public void TakeDamage(int amount)
        {
            Health.TakeDamage(amount);
        }

        protected virtual void HandleUnitDied(Unit unit)
        {
            //gameObject.SetActive(false);
            Debug.Log(name + " died");
        }

        protected virtual void Respawn()
        {
            Health.RestoreToFull();
            timeOfDeath = 0;
            Debug.Log(name + " respawned");
        }

        public void RequestID()
        {
            if (ID < 0)
            {
                ID = GetNextID();
            }
        }

        public UnitData GetUnitData()
        {
            return new UnitData
            {
                Health = Health.CurrentHealth,
                Position = transform.position,
                YRotation = transform.rotation.y,
                ID = ID
            };
        }

        public void SetUnitData(UnitData data)
        {
            Health.SetHealth(data.Health);
            transform.position = data.Position;
            Quaternion newRotation = new Quaternion
                (transform.rotation.x, data.YRotation,
                transform.rotation.z, transform.rotation.w);
            transform.rotation = newRotation;
            ID = data.ID;
        }

        protected virtual void OnDrawGizmos()
        {
            if (Health != null && Health.IsDead)
            {
                DrawDeathSphere(); 
            }
        }

        private void DrawDeathSphere()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1.5f);
        }
    }
}
