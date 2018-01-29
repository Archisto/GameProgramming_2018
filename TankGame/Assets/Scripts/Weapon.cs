using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        private Projectile projectilePrefab;

        [SerializeField, Tooltip("Ammo / second")]
        private float firingRate = 1 / 3f;

        [SerializeField]
        private Transform shootingPoint;

        private Pool<Projectile> projectiles;
        private Unit owner;
        private bool canFire = true;
        private float firingTimer = 0;

        public void Init(Unit owner)
        {
            this.owner = owner;

            projectiles = new Pool<Projectile>(projectilePrefab, 4, false,
                //item => item.Init(this)); // Lambda parameter - no return value and one parameter (the item)
            InitProjectile);
            // Func<bool>);

            /* Lambda parameter:
            
            (Arg arg1, Arg arg2) =>
            {
                arg1.Method1(Param param1);
                arg2.Method2(Param param2);
                Method3();
            }
            */

            //projectiles = new Pool<Projectile>(
            //    projectilePrefab, 4, false, item => InitItem(item));
        }

        /// <summary>
        /// Alt lambda parameter method
        /// </summary>
        /// <param name="projectile"></param>
        private void InitProjectile(Projectile projectile)
        {
            //projectile.Init(this);
            projectile.Init(ProjectileHit);
        }

        protected virtual void Update()
        {
            UpdateFiringTimer();
        }

        private void UpdateFiringTimer()
        {
            if (!canFire)
            {
                firingTimer += Time.deltaTime;
            }

            if (firingTimer >= firingRate)
            {
                canFire = true;
                firingTimer = 0;
            }
        }

        public bool Fire()
        {
            if (!canFire)
            {
                return false;
            }

            // Takes a projectile from the pool and launches it
            Projectile projectile = projectiles.GetPooledObject();

            if (projectile != null)
            {
                canFire = false;

                // Calculates the firing direction:
                // from the barrel's base to its tip in world space
                Vector3 firingDirection = shootingPoint.position - transform.position;

                // Changes the firing direction vector's distance to 1
                firingDirection.Normalize();

                projectile.transform.position = shootingPoint.position;
                projectile.Launch(firingDirection);

                return true;
            }

            // Creates a new projectile
            //Projectile newProjectile = Instantiate(projectilePrefab);
            //newProjectile.Init(barrelTipPos, firingDirection, 20);

            return false;
        }

        private void ProjectileHit(Projectile projectile)
        {
            if (!projectiles.ReturnObject(projectile))
            {
                Debug.LogError("Could not return the projectile back to the pool.");
            }
        }
    }
}
