﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField]
        private int damage;

        [SerializeField]
        private float shootingForce;

        [SerializeField]
        private float explosionForce;

        [SerializeField]
        private float explosionRadius;

        [SerializeField, HideInInspector]
        private int hitMask;

        [SerializeField]
        private GameObject holePrefab;

        private Weapon weapon;
        private Rigidbody rBody;

        private bool holeCreated;
        private DebugLine line;

        /// <summary>
        /// Self-initializing property. Gets the reference to
        /// the Rigidbody component when used the first time.
        /// </summary>
        public Rigidbody Rigidbody
        {
            get
            {
                if (rBody == null)
                {
                    rBody = gameObject.GetOrAddComponent<Rigidbody>();
                }
                return rBody;
            }
        }

        //public void Init(Weapon weapon)
        //{
        //    this.weapon = weapon;
        //    line = FindObjectOfType<DebugLine>();
        //}

        private Action<Projectile> collisionCallback;
        public void Init(Action<Projectile> collisionCallback)
        {
            this.collisionCallback = collisionCallback;
            line = FindObjectOfType<DebugLine>();
        }

        public void Launch(Vector3 direction)
        {
            // TODO: Add particle effects

            Rigidbody.AddForce(direction.normalized * shootingForce, ForceMode.Impulse);

            holeCreated = false;
        }

        protected void OnCollisionEnter(Collision collision)
        {
            // TODO: Add particle effects

            ApplyDamage();

            Rigidbody.velocity = Vector3.zero;

            // Passes collision information to the weapon
            //weapon.ProjectileHit(this);
            collisionCallback(this);

            if (!holeCreated)
            {
                CreateHole(collision);
                holeCreated = true;
            }
        }

        private void ApplyDamage()
        {
            List<IDamageReceiver> alreadyDamaged = new List<IDamageReceiver>();

            Collider[] damageReceivers = Physics.OverlapSphere(
                transform.position, explosionRadius, hitMask);

            for (int i = 0; i < damageReceivers.Length; i++)
            {
                IDamageReceiver damageReceiver =
                    damageReceivers[i].GetComponentInParent<IDamageReceiver>();
                if (damageReceiver != null &&
                    !alreadyDamaged.Contains(damageReceiver))
                {
                    damageReceiver.TakeDamage(damage);
                    alreadyDamaged.Add(damageReceiver);
                    // TODO: Apply explosion force
                }
            }
        }

        private void CreateHole(Collision collision)
        {
            if (holePrefab != null)
            {
                GameObject hole = Instantiate(holePrefab);

                hole.transform.position = transform.position;

                Vector3 point = collision.contacts[0].point;
                //foreach (ContactPoint contact in collision.contacts)
                //{
                //    if (Vector3.Distance(transform.position, contact.point) <
                //        Vector3.Distance(transform.position, point))
                //    {
                //        point = contact.point;
                //    }
                //}

                if (line != null)
                {
                    line.from = transform.position;
                    line.to = point;
                }

                // TODO: Fix the hole's rotation

                hole.transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - point);
                //hole.transform.rotation = Quaternion.LookRotation(Vector3.up, transform.position - point);
            }
        }
    }
}
