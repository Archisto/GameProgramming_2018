using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TankGame
{
    public class CollectableSpawner : MonoBehaviour
    {
        [SerializeField]
        private Collectable collItemPrefab;

        [SerializeField]
        private int poolSize;

        [SerializeField]
        private bool poolShouldGrow;

        [SerializeField, Range(0.1f, 60.0f)]
        private float itemSpawnInterval;

        [SerializeField]
        private Vector3 itemSpawnAreaCorner1;

        [SerializeField]
        private Vector3 itemSpawnAreaCorner2;

        private Pool<Collectable> collItemPool;

        private float elapsedItemSpawnTime = 0;

        private Vector3 ItemSpawnAreaCorner1
        {
            get
            {
                return itemSpawnAreaCorner1;
            }
            set
            {
                itemSpawnAreaCorner1 = value;
            }
        }

        private Vector3 ItemSpawnAreaCorner2
        {
            get
            {
                return itemSpawnAreaCorner2;
            }
            set
            {
                itemSpawnAreaCorner2 = value;
            }
        }

        private void Start()
        {
            collItemPool = new Pool<Collectable>
                (collItemPrefab, poolSize, poolShouldGrow);

            // Makes the item spawn area corners neat and tidy
            float minX = Mathf.Min(ItemSpawnAreaCorner1.x, ItemSpawnAreaCorner2.x);
            float minZ = Mathf.Min(ItemSpawnAreaCorner1.z, ItemSpawnAreaCorner2.z);
            float maxX = Mathf.Max(ItemSpawnAreaCorner1.x, ItemSpawnAreaCorner2.x);
            float maxZ = Mathf.Max(ItemSpawnAreaCorner1.z, ItemSpawnAreaCorner2.z);
            ItemSpawnAreaCorner1 = new Vector3(minX, 0, minZ);
            ItemSpawnAreaCorner2 = new Vector3(maxX, 0, maxZ);
        }

        private void Update()
        {
            UpdateItemSpawnTime();
        }

        private void UpdateItemSpawnTime()
        {
            if ( !GameManager.Instance.GameWon &&
                 !GameManager.Instance.GameLost )
            {
                elapsedItemSpawnTime += Time.deltaTime;
                if (elapsedItemSpawnTime >= itemSpawnInterval)
                {
                    elapsedItemSpawnTime = 0;
                    SpawnItem();
                }
            }
        }

        private void SpawnItem()
        {
            Collectable item = collItemPool.GetPooledObject(true);

            if (item != null)
            {
                float randX = Random.Range(ItemSpawnAreaCorner1.x, ItemSpawnAreaCorner2.x);
                float randZ = Random.Range(ItemSpawnAreaCorner1.z, ItemSpawnAreaCorner2.z);

                item.transform.position = new Vector3(randX, 0, randZ);
                item.InitDefaults();
                item.SetHandler(this);
            }
        }

        public void ReturnItemToPool(Collectable item)
        {
            collItemPool.ReturnObject(item);
        }

        private void OnDrawGizmos()
        {
            DrawSpawnArea();
        }

        private void DrawSpawnArea()
        {
            Gizmos.color = new Color(1, 0, 1, 1);

            Vector3 point1 = ItemSpawnAreaCorner1;
            Vector3 point2 = new Vector3(ItemSpawnAreaCorner1.x, 0, ItemSpawnAreaCorner2.z);
            Vector3 point3 = ItemSpawnAreaCorner2;
            Vector3 point4 = new Vector3(ItemSpawnAreaCorner2.x, 0, ItemSpawnAreaCorner1.z);

            Gizmos.DrawLine(point1, point2);
            Gizmos.DrawLine(point2, point3);
            Gizmos.DrawLine(point3, point4);
            Gizmos.DrawLine(point4, point1);
        }
    }
}
