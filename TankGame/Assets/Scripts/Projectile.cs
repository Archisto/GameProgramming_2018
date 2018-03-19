using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    public class Projectile : MonoBehaviour
    {
        /* SerializeField, Header, ToolTip, Range, HideInInspector, Flags, ExecuteInEditMode */

        [SerializeField, Header("Basic Values")]
        private int damage;

        [SerializeField]
        private float shootingForce;

        [SerializeField, Header("Explosion Values")]
        private float explosionForce;

        [SerializeField]
        private float explosionRadius;

        [SerializeField, HideInInspector]
        private int hitMask;

        //[SerializeField]
        //private ProjectileType type;

        //[Flags]
        //public enum ProjectileType
        //{
        //    None = 0,
        //    Player = 1,
        //    Enemy = 1 << 1,
        //    Neutral = 1 << 2
        //}

        private Weapon weapon;
        private Rigidbody rBody;

        private Hole hole;
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

        private Action<Projectile> PassCollisionInfoToWeapon;
        public void Init(Action<Projectile> collisionCallback)
        {
            PassCollisionInfoToWeapon = collisionCallback;
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
            PassCollisionInfoToWeapon(this);

            if (!holeCreated)
            {
                ShowHole(collision);
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

        public void SetHole(Hole hole)
        {
            this.hole = hole;
        }

        private void ShowHole(Collision collision)
        {
            if (hole != null && !hole.gameObject.activeSelf)
            {
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

                hole.gameObject.SetActive(true);
            }
        }

        //private void CreateHole(Collision collision)
        //{
        //    if (holePrefab != null)
        //    {
        //        Hole hole = Instantiate(holePrefab);
        //        hole.transform.position = transform.position;

        //        Vector3 point = collision.contacts[0].point;
        //        //foreach (ContactPoint contact in collision.contacts)
        //        //{
        //        //    if (Vector3.Distance(transform.position, contact.point) <
        //        //        Vector3.Distance(transform.position, point))
        //        //    {
        //        //        point = contact.point;
        //        //    }
        //        //}

        //        if (line != null)
        //        {
        //            line.from = transform.position;
        //            line.to = point;
        //        }

        //        // TODO: Fix the hole's rotation

        //        hole.transform.rotation = Quaternion.LookRotation(Vector3.forward, transform.position - point);
        //        //hole.transform.rotation = Quaternion.LookRotation(Vector3.up, transform.position - point);

        //        hole.gameObject.SetActive(true);
        //    }
        //}
    }
}
