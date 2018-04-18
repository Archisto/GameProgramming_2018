using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankGame
{
    public class DestroyedTankSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject destroyedTankPrefab;

        [SerializeField]
        private int poolSize;

        [SerializeField]
        private bool poolShouldGrow;

        private Pool<Transform> destroyedTankPool;

        private List<Transform> destroyedTanks;
        private List<int> unitIDs;

        private void Start()
        {
            destroyedTankPool = new Pool<Transform>
                (destroyedTankPrefab.transform, poolSize, poolShouldGrow);

            destroyedTanks = new List<Transform>();
            unitIDs = new List<int>();
        }

        public void SpawnDestroyedTank(Unit unit)
        {
            Transform dTankTransform = destroyedTankPool.GetPooledObject(true);

            if (dTankTransform != null)
            {
                dTankTransform.position = unit.transform.position;
                dTankTransform.rotation = unit.transform.rotation;

                destroyedTanks.Add(dTankTransform);
                unitIDs.Add(unit.ID);
            }
        }

        public void DespawnDestroyedTank(Unit unit)
        {
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

        public void DespawnAllDestroyedTanks()
        {
            foreach (Transform dTankTransform in destroyedTanks)
            {
                ReturnItemToPool(dTankTransform);
            }

            destroyedTanks.Clear();
            unitIDs.Clear();
        }

        public void ReturnItemToPool(Transform destroyedTankTransform)
        {
            destroyedTankPool.ReturnObject(destroyedTankTransform);
        }
    }
}
