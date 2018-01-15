using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        Projectile projectilePrefab;

        private Pool<Projectile> projectiles;

        private Unit owner;

        public void Init(Unit owner)
        {
            this.owner = owner;
            projectiles = new Pool<Projectile>(projectilePrefab, 4, false);
        }
    }
}
