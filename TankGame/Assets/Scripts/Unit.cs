using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Persistence;
using TankGame.Messaging;

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
        private GameObject tankModel;

        [SerializeField]
        private GameObject tankHead;

        [SerializeField]
        private int startingHealth;

        [SerializeField, HideInInspector]
        private int id = -1;

        private TransformMover mover;
        private IMover headMover;
        private IMover barrelMover;

        private Vector3 startPosition;
        private Quaternion startRotation;

        private float remainingRespawnTime = 0;

        public bool IsPlayerUnit { get; protected set; }

        private void OnDestroy()
        {
            // Stops listening to the UnitDied event
            Health.UnitDied -= HandleUnitDied;
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

            startPosition = transform.position;
            startRotation = transform.rotation;

            if (tankModel == null)
            {
                Debug.LogError("Tank model is not set.");
            }
        }

        /// <summary>
        /// Update is called once per frame.
        /// An abstract method has to be defined
        /// in a non-abstract child class.
        /// </summary>
        //protected abstract void Update();

        protected virtual void Update()
        {
            UpdateRespawn();
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

        public float RemainingRespawnTime
        {
            get
            {
                if (Health.IsDead)
                {
                    return remainingRespawnTime;
                }
                else
                {
                    return 0;
                }
            }
            private set
            {
                remainingRespawnTime = value;
            }
        }
        
        protected virtual void UpdateRespawn()
        {
            if (Health != null && Health.IsDead)
            {
                RemainingRespawnTime -= Time.deltaTime;
                if (RemainingRespawnTime <= 0)
                {
                    ResetUnit();
                    // Respawn();
                }
            }
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
            //Debug.Log(name + " died");

            tankModel.SetActive(false);
            RemainingRespawnTime = respawnTime;
            GameManager.Instance.MessageBus.Publish(new UnitDiedMessage(this));
            GameManager.Instance.UnitDied(this, IsPlayerUnit);

            //gameObject.SetActive(false);
        }

        protected virtual void Respawn()
        {
            Health.RestoreToFull();
            RemainingRespawnTime = 0;
            tankModel.SetActive(true);
            GameManager.Instance.UnitRespawned(this);

            //Debug.Log(name + " respawned");
        }

        public void ResetUnit()
        {
            Respawn();
            transform.position = startPosition;
            transform.rotation = startRotation;
            ResetTankHead();
        }

        private void ResetTankHead()
        {
            tankHead.transform.localRotation = Quaternion.Euler(Vector3.zero);
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
                RemainingRespawnTime = RemainingRespawnTime,
                Position = transform.position,
                YRotation = transform.rotation.eulerAngles.y,
                ID = ID
            };
        }

        public void SetUnitData(UnitData data)
        {
            Health.SetHealth(data.Health, true);
            RemainingRespawnTime = data.RemainingRespawnTime;
            transform.position = data.Position;

            Vector3 newRotation = transform.rotation.eulerAngles;
            newRotation.y = data.YRotation;
            transform.rotation = Quaternion.Euler(newRotation);

            ID = data.ID;

            if (Health.IsDead)
            {
                tankModel.SetActive(false);
                GameManager.Instance.SpawnDestroyedTank(this);
            }
            else
            {
                tankModel.SetActive(true);
                ResetTankHead();
            }
        }

        protected virtual void OnDrawGizmos()
        {
            //if (Health != null && Health.IsDead)
            //{
            //    DrawDeathSphere(); 
            //}
        }

        private void DrawDeathSphere()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1.5f);
        }
    }
}
