using UnityEngine;

namespace TankGame
{
    [RequireComponent(typeof(Collider))]
    public class Collectable : MonoBehaviour
    {
        /// <summary>
        /// The collectable item's model, or, if it has
        /// not been set, this component's game object
        /// </summary>
        [SerializeField]
        private GameObject _collectableObject;

        [SerializeField]
        private bool _removedWhenCollected;

        [SerializeField]
        private bool _respawnsAfterRest;

        [SerializeField]
        private int _score;

        [SerializeField]
        private float _motionSpeed;

        [SerializeField]
        private float _motionTime = 1;

        [SerializeField]
        private float _particleTime = 1;

        [SerializeField]
        private float _restingTime;

        private Vector3 _defaultPosition;
        private Quaternion _defaultRotation;

        private ParticleSystem _particles;
        private CollectableSpawner _handler;

        private bool _active = false;
        private bool _motionCompleted = false;
        private float _activeTime = 0;
        private float _elapsedActiveTime = 0;

        private bool _resting = false;
        private float _elapsedRestingTime = 0;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start()
        {
            if (_collectableObject == null)
            {
                _collectableObject = gameObject;
            }

            InitDefaults();

            _particles = GetComponent<ParticleSystem>();

            if (_particles != null)
            {
                ParticleSystem.MainModule psMain = _particles.main;
                psMain.duration = _particleTime;

                _activeTime = Mathf.Max(_motionTime, _particleTime);
            }
            else
            {
                _activeTime = _motionTime;
            }
        }

        public void InitDefaults()
        {
            _defaultPosition = _collectableObject.transform.position;
            _defaultRotation = _collectableObject.transform.rotation;
        }

        private void ResetToDefaults()
        {
            _collectableObject.transform.position = _defaultPosition;
            _collectableObject.transform.rotation = _defaultRotation;
        }

        public void SetHandler(CollectableSpawner handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Update is called once per frame.
        /// </summary>
        protected virtual void Update()
        {
            if (_active)
            {
                UpdateActivity();
            }
            else if (_resting)
            {
                UpdateRest();
            }
            else
            {
                UpdateIdleMotion();
            }
        }

        protected virtual void UpdateActivity()
        {
            UpdateActiveMotion();

            _elapsedActiveTime += Time.deltaTime;

            if (_elapsedActiveTime > _activeTime)
            {
                _active = false;
                _motionCompleted = false;
                _elapsedActiveTime = 0;

                if (_removedWhenCollected)
                {
                    Remove();
                }

            }
            else
            {
                if (!_motionCompleted &&
                    _elapsedActiveTime > _motionTime)
                {
                    _motionCompleted = true;

                    if (_removedWhenCollected)
                    {
                        ShowCollectableObject(false);
                    }
                    else
                    {
                        ResetToDefaults();
                    }
                }
                else if (_elapsedActiveTime > _particleTime)
                {
                    // Does nothing
                }
            }
        }

        protected virtual void UpdateRest()
        {
            _elapsedRestingTime += Time.deltaTime;

            if (_elapsedRestingTime > _restingTime)
            {
                _elapsedRestingTime = 0;
                _resting = false;

                if (_respawnsAfterRest)
                {
                    Respawn();
                }
            }
        }

        protected virtual void UpdateActiveMotion()
        {
            Vector3 newPosition = _collectableObject.transform.position;
            newPosition.y += _motionSpeed * Time.deltaTime;

            Vector3 newRotation = _collectableObject.transform.rotation.eulerAngles;
            newRotation.y += 200 * _motionSpeed * Time.deltaTime;

            _collectableObject.transform.position = newPosition;
            _collectableObject.transform.rotation = Quaternion.Euler(newRotation);
        }

        protected virtual void UpdateIdleMotion()
        {
            Vector3 newRotation = _collectableObject.transform.rotation.eulerAngles;
            newRotation.y += 75 * _motionSpeed * Time.deltaTime;

            _collectableObject.transform.rotation = Quaternion.Euler(newRotation);
        }

        public virtual void Activate()
        {
            // TODO: What happens when collected?

            // Returns if already collected
            if (!_collectableObject.activeSelf)
            {
                return;
            }

            _active = true;

            GameManager.Instance.AddScore(_score);

            if (_particles != null)
            {
                _particles.Play();
            }

            //Debug.Log(name + " collected");
        }

        public virtual void Respawn()
        {
            ResetToDefaults();
            ShowCollectableObject(true);
        }

        public virtual void Remove()
        {
            ResetToDefaults();
            _handler.ReturnItemToPool(this);

            // If _cO is this.gameObject, respawning doesn't work
            // (Update is not called)
            //_collectableObject.SetActive(false);
        }

        protected virtual void ShowCollectableObject(bool show)
        {
            _collectableObject.SetActive(show);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (!_active && !_resting && _collectableObject.activeSelf)
            {
                PlayerUnit playerUnit = other.GetComponentInParent<PlayerUnit>();
                if (playerUnit != null && !playerUnit.Health.IsDead)
                {
                    Activate();

                    if (!_removedWhenCollected || _respawnsAfterRest)
                    {
                        _resting = true;
                    }
                }
            }
        }
    }
}
