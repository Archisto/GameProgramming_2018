using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankGame.Messaging;
using Random = UnityEngine.Random;

namespace TankGame
{
    /// <summary>
    /// A class which spawns collectables to random positions within an area.
    /// </summary>
    public class CollectableSpawner : MonoBehaviour
    {
        /// <summary>
        /// The spawned collectable
        /// </summary>
        [SerializeField]
        private Collectable collItemPrefab;

        /// <summary>
        /// Is a new item created when attempting
        /// to get one from the pool but it's empty
        /// </summary>
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
        private List<Collectable> collectables;

        private float elapsedItemSpawnTime = 0;

        /// <summary>
        /// Message bus subscription to the game resetting
        /// </summary>
        private ISubscription<GameResetMessage> gameResetSubscription;

        /// <summary>
        /// A corner of the rectangular collectable spawn area.
        /// </summary>
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

        /// <summary>
        /// A corner of the rectangular collectable spawn area.
        /// </summary>
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

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            collItemPool = new Pool<Collectable>
                (collItemPrefab, poolSize, poolShouldGrow);

            collectables = new List<Collectable>();

            gameResetSubscription = GameManager.Instance.
                MessageBus.Subscribe<GameResetMessage>(OnGameReset);

            // Makes the item spawn area corners neat and tidy
            float minX = Mathf.Min(ItemSpawnAreaCorner1.x, ItemSpawnAreaCorner2.x);
            float minZ = Mathf.Min(ItemSpawnAreaCorner1.z, ItemSpawnAreaCorner2.z);
            float maxX = Mathf.Max(ItemSpawnAreaCorner1.x, ItemSpawnAreaCorner2.x);
            float maxZ = Mathf.Max(ItemSpawnAreaCorner1.z, ItemSpawnAreaCorner2.z);
            ItemSpawnAreaCorner1 = new Vector3(minX, 0, minZ);
            ItemSpawnAreaCorner2 = new Vector3(maxX, 0, maxZ);
        }

        /// <summary>
        /// Updates the object each frame.
        /// </summary>
        private void Update()
        {
            UpdateItemSpawnTime();
        }

        /// <summary>
        /// Spawns collectables at set intervals
        /// if the game is not over.
        /// </summary>
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

        /// <summary>
        /// Spawns a collectable at a random position within the spawn area.
        /// </summary>
        private void SpawnItem()
        {
            Collectable item = collItemPool.GetPooledObject(true);

            if (item != null)
            {
                // Gets random x- and z-coordinates
                float randX = Random.Range(ItemSpawnAreaCorner1.x, ItemSpawnAreaCorner2.x);
                float randZ = Random.Range(ItemSpawnAreaCorner1.z, ItemSpawnAreaCorner2.z);

                item.transform.position = new Vector3(randX, 0, randZ);
                item.InitDefaults();
                item.SetHandler(this);

                collectables.Add(item);
            }
        }

        /// <summary>
        /// Removes all collectables from the world.
        /// </summary>
        public void DespawnAllItems()
        {
            foreach (Collectable collectable in collectables)
            {
                if (collectable.gameObject.activeSelf)
                {
                    ReturnItemToPool(collectable);
                }
            }

            collectables.Clear();
        }

        /// <summary>
        /// Returns a collectable to the pool.
        /// </summary>
        /// <param name="item">A collectable</param>
        public void ReturnItemToPool(Collectable item)
        {
            collItemPool.ReturnObject(item);
        }

        /// <summary>
        /// Removes all collectables from the world if the game is reset.
        /// </summary>
        /// <param name="msg">A game reset message</param>
        private void OnGameReset(GameResetMessage msg)
        {
            DespawnAllItems();
        }

        /// <summary>
        /// Draws debug gizmos.
        /// </summary>
        private void OnDrawGizmos()
        {
            DrawSpawnArea();
        }

        /// <summary>
        /// Draws the borders of the collectable spawn area.
        /// </summary>
        private void DrawSpawnArea()
        {
            Gizmos.color = Color.black;

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
