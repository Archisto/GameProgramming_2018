using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TankGame
{
    /// <summary>
    /// A projectile which deals damage to opposing units.
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        [SerializeField, Header("Basic Values")]
        private int damage;

        [SerializeField]
        private float shootingForce;

        [SerializeField, Header("Explosion Values")]
        private float explosionForce;

        [SerializeField]
        private float explosionRadius;

        /// <summary>
        /// A mask of targets the projectile can hit
        /// </summary>
        [SerializeField, HideInInspector]
        private int hitMask;

        /// <summary>
        /// An action which is passed from weapon and
        /// fired when the projectile hits something.
        /// </summary>
        private Action<Projectile> PassCollisionInfoToWeapon;

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
        private ParticleSystem hitParticles;

        /// <summary>
        /// The hole the projectile creates on collision
        /// </summary>
        private Hole hole;
        private bool holeCreated;

        /// <summary>
        /// A line which shows the projectile's hit angle. Debugging purposes only.
        /// </summary>
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

        /// <summary>
        /// Initializes the object.
        /// </summary>
        /// <param name="collisionCallback">A method which is called when
        /// the projectile hit ssomething</param>
        public void Init(Action<Projectile> collisionCallback)
        {
            PassCollisionInfoToWeapon = collisionCallback;
            hitParticles = GetComponent<ParticleSystem>();
            line = FindObjectOfType<DebugLine>();
        }

        /// <summary>
        /// Launches the projectile at the given direction.
        /// </summary>
        /// <param name="direction">Firing direction</param>
        public void Launch(Vector3 direction)
        {

            Rigidbody.AddForce(direction.normalized * shootingForce, ForceMode.Impulse);
            //holeCreated = false;
        }

        /// <summary>
        /// Called when the projectile collides with something.
        /// </summary>
        /// <param name="collision">Collision info</param>
        protected void OnCollisionEnter(Collision collision)
        {
            ApplyDamage();

            // Stops the projectile
            Rigidbody.velocity = Vector3.zero;

            // Passes collision information to the weapon
            PassCollisionInfoToWeapon(this);

            // Plays particle effects
            if (hitParticles != null)
            {
                hitParticles.Play();
            }

            // Creates a hole at the point of impact
            //if (!holeCreated)
            //{
            //    CreateHole(collision);
            //    holeCreated = true;
            //}
        }

        /// <summary>
        /// Deals damage to the hit target.
        /// </summary>
        private void ApplyDamage()
        {
            List<IDamageReceiver> alreadyDamaged = new List<IDamageReceiver>();

            // Gets all damage receiving colliders in
            // an area within the explosion radius
            Collider[] damageReceivers = Physics.OverlapSphere(
                transform.position, explosionRadius, hitMask);

            // Deals damage to the damage receivers if they are valid
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

        /// <summary>
        /// Sets the hole this projectile creates on collision.
        /// </summary>
        /// <param name="hole"></param>
        public void SetHole(Hole hole)
        {
            this.hole = hole;
        }

        /// <summary>
        /// Creates a hole at the point of collision.
        /// </summary>
        /// <param name="collision">Collision info</param>
        private void CreateHole(Collision collision)
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
