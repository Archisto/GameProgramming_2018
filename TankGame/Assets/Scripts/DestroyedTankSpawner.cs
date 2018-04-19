using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankGame
{
    /// <summary>
    /// A class which spawns destroyed tanks to the positions of dead unit.
    /// </summary>
    public class DestroyedTankSpawner : MonoBehaviour
    {
        /// <summary>
        /// A model of a destroyed tank
        /// </summary>
        [SerializeField]
        private GameObject destroyedTankPrefab;

        /// <summary>
        /// The size of the destroyed tank pool
        /// </summary>
        [SerializeField]
        private int poolSize;

        /// <summary>
        /// Is a new item created when attempting
        /// to get one from the pool but it's empty
        /// </summary>
        [SerializeField]
        private bool poolShouldGrow;

        private Pool<Transform> destroyedTankPool;

        private List<Transform> destroyedTanks;
        private List<int> unitIDs;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            destroyedTankPool = new Pool<Transform>
                (destroyedTankPrefab.transform, poolSize, poolShouldGrow);

            destroyedTanks = new List<Transform>();
            unitIDs = new List<int>();
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
        /// Removes all destroyed tanks form the world.
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
        /// Returns an item to the item pool.
        /// </summary>
        /// <param name="destroyedTankTransform">
        /// The transform of a destroyed tank object</param>
        public void ReturnItemToPool(Transform destroyedTankTransform)
        {
            destroyedTankPool.ReturnObject(destroyedTankTransform);
        }
    }
}
