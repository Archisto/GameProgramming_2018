using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class Hole : MonoBehaviour
    {
        private float lifeTime = 0;
        private float maxLifeTime = 8;

        private void Update()
        {
            lifeTime += Time.deltaTime;

            if (lifeTime >= maxLifeTime)
            {
                Destroy(gameObject);
            }
        }
    }
}
