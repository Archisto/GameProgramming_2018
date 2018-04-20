using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankGame
{
    /// <summary>
    /// A class which spawns explosions to where projectiles hit and
    /// destroyed tanks to the positions of dead unit.
    /// </summary>
    public class CombatSpawner : MonoBehaviour
    {
        /// <summary>
        /// A model of a destroyed tank
        /// </summary>
        [SerializeField]
        private GameObject destroyedTankPrefab;

        /// <summary>
        /// An explosion effect
        /// </summary>
        [SerializeField]
        private ParticleSystem explosionPrefab;

        /// <summary>
        /// The size of the destroyed tank pool
        /// </summary>
        [SerializeField]
        private int destroyedTankPoolSize;

        /// <summary>
        /// The size of the explosion pool
        /// </summary>
        [SerializeField]
        private int explosionPoolSize;

        /// <summary>
        /// Is a new item created when attempting
        /// to get one from a pool but it's empty
        /// </summary>
        [SerializeField]
        private bool poolsShouldGrow;

        private Pool<Transform> destroyedTankPool;
        private Pool<ParticleSystem> explosionPool;

        private List<Transform> destroyedTanks;
        private List<int> unitIDs;
        private List<ParticleSystem> explosions;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            destroyedTankPool = new Pool<Transform>
                (destroyedTankPrefab.transform, destroyedTankPoolSize, poolsShouldGrow);
            explosionPool = new Pool<ParticleSystem>
                (explosionPrefab, explosionPoolSize, poolsShouldGrow);

            destroyedTanks = new List<Transform>();
            unitIDs = new List<int>();
            explosions = new List<ParticleSystem>();
        }

        private void Update()
        {
            UpdateExplosions();
        }

        /// <summary>
        /// Despawns explosions when they expire.
        /// </summary>
        public void UpdateExplosions()
        {
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                if ( !explosions[i].isPlaying )
                {
                    ReturnItemToPool(explosions[i]);
                    explosions.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Spawns a destroyed tank to the position of a dead unit.
        /// </summary>
        /// <param name="unit">A dead unit</param>
        public void SpawnDestroyedTank(Unit unit)
        {
            Transform dTankTransform = destroyedTankPool.GetPooledObject(true);

            if (dTankTransform != null)
            {
                dTankTransform.position = unit.transform.position;
                dTankTransform.rotation = unit.transform.rotation;

                // Adds the destroyed tank's transform and the dead unit's ID to
                // lists so that removing the correct destroyed tank is possible
                destroyedTanks.Add(dTankTransform);
                unitIDs.Add(unit.ID);
            }
        }

        /// <summary>
        /// Spawns an explosion to the given position.
        /// </summary>
        /// <param name="position">A position</param>
        public void SpawnExplosion(Vector3 position)
        {
            ParticleSystem explosion = explosionPool.GetPooledObject(true);

            if (explosion != null)
            {
                explosion.transform.position = position;
                explosions.Add(explosion);
                explosion.Play();
            }
        }

        /// <summary>
        /// Removes a destroyed tank from the position where the unit died.
        /// </summary>
        /// <param name="unit">A dead or respawning unit</param>
        public void DespawnDestroyedTank(Unit unit)
        {
            // Loops through recorded unit IDs and removes the destroyed
            // tank which corresponds with the given unit's ID
            for (int i = 0; i < unitIDs.Count; i++)
            {
                if (unitIDs[i] == unit.ID)
                {
                    ReturnItemToPool(destroyedTanks[i]);
                    destroyedTanks.RemoveAt(i);
                    unitIDs.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary>
        /// Removes all destroyed tanks from the world.
        /// </summary>
        public void DespawnAllDestroyedTanks()
        {
            foreach (Transform dTankTransform in destroyedTanks)
            {
                ReturnItemToPool(dTankTransform);
            }

            destroyedTanks.Clear();
            unitIDs.Clear();
        }

        /// <summary>
        /// Removes all explosions from the world.
        /// </summary>
        public void DespawnAllExplosions()
        {
            foreach (ParticleSystem explosion in explosions)
            {
                ReturnItemToPool(explosion);
            }

            explosions.Clear();
        }

        /// <summary>
        /// Returns an item to the item pool.
        /// </summary>
        /// <param name="destroyedTankTransform">
        /// The transform of a destroyed tank object</param>
        public void ReturnItemToPool(Transform destroyedTankTransform)
        {
            destroyedTankPool.ReturnObject(destroyedTankTransform);
        }

        /// <summary>
        /// Returns an item to the item pool.
        /// </summary>
        /// <param name="explosion">An explosion object</param>
        public void ReturnItemToPool(ParticleSystem explosion)
        {
            explosionPool.ReturnObject(explosion);
        }
    }
}
