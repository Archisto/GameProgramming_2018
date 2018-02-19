using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class Projectile_old : MonoBehaviour
    {
        [SerializeField]
        private GameObject holePrefab;

        private Vector3 direction;
        private float speed;

        private float gravity = 1f;

        private bool holeCreated;

        public void Init(Vector3 position, Vector3 direction, float speed)
        {
            transform.position = position;
            this.direction = direction;
            this.speed = speed;
        }

        private void Update()
        {
            transform.position += direction * speed * Time.deltaTime;

            direction.y -= gravity * Time.deltaTime;

            if (!holeCreated && transform.position.y < 0)
            {
                CreateHole();
                holeCreated = true;
            }

            if (transform.position.y < -1)
            {
                Clear();
            }
        }

        private void CreateHole()
        {
            if (holePrefab != null)
            {
                GameObject hole = Instantiate(holePrefab);

                Vector3 holePosition = transform.position;
                holePosition.y = 0.02f;
                hole.transform.position = holePosition;
            }
        }

        public void Clear()
        {
            Destroy(gameObject);
        }
    }
}
