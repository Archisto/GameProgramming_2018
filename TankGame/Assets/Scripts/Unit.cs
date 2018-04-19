using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Persistence;
using TankGame.Messaging;

namespace TankGame
{
    /// <summary>
    /// A tank unit that moves and fires.
    /// </summary>
    [RequireComponent(typeof(IMover))]
    public abstract class Unit : MonoBehaviour, IDamageReceiver
    {
        #region Statics

        private static int idCounter = 0;

        /// <summary>
        /// Gets the next unused unit ID.
        /// </summary>
        /// <returns>A unused unit ID</returns>
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

        /// <summary>
        /// The unit ID for saving and loading
        /// </summary>
        [SerializeField, HideInInspector]
        private int id = -1;

        private TransformMover mover;
        private IMover headMover;
        private IMover barrelMover;

        private Vector3 startPosition;
        private Quaternion startRotation;

        private float remainingRespawnTime = 0;

        /// <summary>
        /// Is the unit controlled by a player.
        /// </summary>
        public bool IsPlayerUnit { get; protected set; }

        /// <summary>
        /// Called when the object is destroyed.
        /// </summary>
        private void OnDestroy()
        {
            // Stops listening to the UnitDied event
            Health.UnitDied -= OnUnitDied;
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        public virtual void Init()
        {
            // Initializes the weapon
            Weapon = GetComponentInChildren<Weapon>();
            if (Weapon != null)
            {
                Weapon.Init(this);
            }

            // Initializes moving
            mover = gameObject.GetOrAddComponent<TransformMover>();
            mover.Init(moveSpeed, turnSpeed);

            // Initializes turning the cannon
            TransformMover[] headMovers =
                tankHead.GetComponentsInChildren<TransformMover>();
            headMover = headMovers[0];
            barrelMover = headMovers[1];
            headMover.Init(0f, turnSpeed);
            barrelMover.Init(0f, turnSpeed);

            // Initializes health
            Health = new Health(this, startingHealth);

            // Registers to listen to the UnitDied event
            Health.UnitDied += OnUnitDied;

            // Sets spawn position and rotation
            startPosition = transform.position;
            startRotation = transform.rotation;

            if (tankModel == null)
            {
                Debug.LogError("Tank model is not set.");
            }
        }

        /// <summary>
        /// Updates the object each frame.
        /// </summary>
        protected virtual void Update()
        {
            UpdateRespawn();
        }

        /// <summary>
        /// The unit's weapon.
        /// </summary>
        public Weapon Weapon { get; protected set; }

        /// <summary>
        /// The unit's mover.
        /// </summary>
        public TransformMover Mover
        {
            get
            {
                return mover;
            }
        }

        /// <summary>
        /// The unit's cannon's horizontal mover.
        /// </summary>
        protected IMover TankHeadMover
        {
            get
            {
                return headMover;
            }
        }

        /// <summary>
        /// The unit's cannon's vertical mover.
        /// </summary>
        protected IMover BarrelMover
        {
            get
            {
                return barrelMover;
            }
        }

        /// <summary>
        /// The unit's health.
        /// </summary>
        public Health Health { get; protected set; }

        /// <summary>
        /// The unit's ID.
        /// </summary>
        public int ID
        {
            get { return id; }
            private set { id = value; }
        }

        /// <summary>
        /// The remaining respawn time. Always 0 if the unit is not dead.
        /// </summary>
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
        
        /// <summary>
        /// Handles spawning if the unit is dead.
        /// </summary>
        protected virtual void UpdateRespawn()
        {
            if (Health != null && Health.IsDead)
            {
                RemainingRespawnTime -= Time.deltaTime;
                if (RemainingRespawnTime <= 0)
                {
                    Respawn();
                }
            }
        }

        /// <summary>
        /// Fires a projectile at the direction of the cannon.
        /// </summary>
        public virtual void Fire()
        {
            Weapon.Fire();
        }

        /// <summary>
        /// Deals damage to the unit.
        /// </summary>
        /// <param name="amount">Amount of damage</param>
        public void TakeDamage(int amount)
        {
            Health.TakeDamage(amount);
        }

        /// <summary>
        /// Called when the unit dies.
        /// </summary>
        /// <param name="unit">The dead unit (this)</param>
        protected virtual void OnUnitDied(Unit unit)
        {
            tankModel.SetActive(false);
            RemainingRespawnTime = respawnTime;
            GameManager.Instance.MessageBus.Publish(new UnitDiedMessage(this));
            GameManager.Instance.UnitDied(this, IsPlayerUnit);

            //Debug.Log(name + " died");
        }

        /// <summary>
        /// Respawns the unit to its original position.
        /// </summary>
        public virtual void Respawn()
        {
            Health.RestoreToFull();
            RemainingRespawnTime = 0;
            tankModel.SetActive(true);
            transform.position = startPosition;
            transform.rotation = startRotation;
            ResetTankHead();
            GameManager.Instance.UnitRespawned(this);

            //Debug.Log(name + " respawned");
        }

        /// <summary>
        /// Makes the cannon point forward.
        /// </summary>
        private void ResetTankHead()
        {
            tankHead.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        /// <summary>
        /// Sets new ID if the current one is invalid.
        /// </summary>
        public void RequestID()
        {
            if (ID < 0)
            {
                ID = GetNextID();
            }
        }

        /// <summary>
        /// Gets data of the unit for saving.
        /// </summary>
        /// <returns>The unit's data</returns>
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

        /// <summary>
        /// Sets loaded data to this unit.
        /// </summary>
        /// <param name="data">Unit data</param>
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

        /// <summary>
        /// Draws gizmos.
        /// </summary>
        protected virtual void OnDrawGizmos()
        {
            //if (Health != null && Health.IsDead)
            //{
            //    DrawDeathSphere(); 
            //}
        }

        /// <summary>
        /// Draws a red sphere around the unit if is dead.
        /// </summary>
        private void DrawDeathSphere()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1.5f);
        }
    }
}
