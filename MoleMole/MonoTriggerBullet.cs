namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class MonoTriggerBullet : BaseMonoDynamicObject
    {
        private float _aliveDuration = -1f;
        private EntityTimer _aliveTimer;
        private bool _collisionEnabled = true;
        private LayerMask _collisionMask;
        private HashSet<uint> _enteredIDs;
        private bool _ignoreTimeScale;
        private bool _isToBeRemoved;
        private Vector3 _originalPosition;
        private bool _passBy;
        private Vector3 _placingPosition;
        private EntityTimer _placingTimer;
        private EntityTimer _resetTimer;
        private float _resumeSpeed;
        private Rigidbody _rigidbody;
        private BulletState _state;
        private Vector3 _targetPosition;
        public float _traceLerpCoef;
        public float _traceLerpCoefAcc;
        public float acceleration;
        [Header("Follow Rotation")]
        public bool followRotation;
        [Header("Is NOT Attach Point")]
        public bool isNotAttachPoint;
        [Header("Addtional offset from launch point")]
        public Vector3 offset;
        [Header("Parent Attach Point/Transform Path")]
        public string parentTransform;
        public float speed;
        public Vector3 speedAdd;
        public float targetReachThreshold;

        protected void Awake()
        {
            this._collisionMask = -1;
            this._rigidbody = base.GetComponent<Rigidbody>();
            this._enteredIDs = new HashSet<uint>();
            this._aliveTimer = new EntityTimer(this._aliveDuration);
        }

        private Vector3 CreateHitForward()
        {
            Vector3 velocity = this._rigidbody.velocity;
            if (velocity == Vector3.zero)
            {
                velocity = base.transform.forward;
            }
            velocity.Normalize();
            return velocity;
        }

        private float GetBulletTimeScale()
        {
            if (this._ignoreTimeScale)
            {
                return base._timeScale;
            }
            return this.TimeScale;
        }

        public override void Init(uint runtimeID, uint ownerID)
        {
            Transform attachPoint;
            base.Init(runtimeID, ownerID);
            if (string.IsNullOrEmpty(this.parentTransform) || this.isNotAttachPoint)
            {
                attachPoint = base.owner.transform.Find(this.parentTransform);
            }
            else
            {
                attachPoint = base.owner.GetAttachPoint(this.parentTransform);
            }
            base.transform.position = attachPoint.TransformPoint(this.offset);
            if (this.followRotation)
            {
                base.transform.rotation = attachPoint.rotation;
            }
            this._aliveTimer.Reset(false);
        }

        public override bool IsActive()
        {
            return !this._isToBeRemoved;
        }

        public override bool IsToBeRemove()
        {
            return this._isToBeRemoved;
        }

        protected override void OnTimeScaleChanged(float newTimescale)
        {
            if (Time.timeScale < 1f)
            {
                this._rigidbody.velocity = (Vector3) (base.transform.forward * 2f);
            }
            else
            {
                this._rigidbody.velocity = (Vector3) (newTimescale * ((this.speed * base.transform.forward) + this.speedAdd));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (((this._collisionMask.value & (((int) 1) << other.gameObject.layer)) != 0) && this._collisionEnabled)
            {
                BaseMonoEntity componentInParent = other.GetComponentInParent<BaseMonoEntity>();
                if (Singleton<RuntimeIDManager>.Instance.ParseCategory(componentInParent.GetRuntimeID()) == 4)
                {
                    if (this._enteredIDs.Contains(componentInParent.GetRuntimeID()))
                    {
                        return;
                    }
                }
                else if (!componentInParent.IsActive() || this._enteredIDs.Contains(componentInParent.GetRuntimeID()))
                {
                    return;
                }
                BaseMonoEntity owner = componentInParent;
                if (componentInParent is BaseMonoDynamicObject)
                {
                    BaseMonoDynamicObject obj2 = (BaseMonoDynamicObject) componentInParent;
                    if ((obj2.dynamicType == BaseMonoDynamicObject.DynamicType.EvadeDummy) && (obj2.owner != null))
                    {
                        this._enteredIDs.Add(obj2.owner.GetRuntimeID());
                    }
                }
                else if (componentInParent is MonoBodyPartEntity)
                {
                    owner = ((MonoBodyPartEntity) componentInParent).owner;
                }
                if (!(owner is BaseMonoAbilityEntity) || !((BaseMonoAbilityEntity) owner).isGhost)
                {
                    this._enteredIDs.Add(owner.GetRuntimeID());
                    EvtBulletHit evt = new EvtBulletHit(base._runtimeID, owner.GetRuntimeID()) {
                        ownerID = base.ownerID
                    };
                    Vector3 position = base.transform.position - ((Vector3) ((Time.deltaTime * this.BulletTimeScale) * this._rigidbody.velocity));
                    AttackResult.HitCollsion collsion = new AttackResult.HitCollsion {
                        hitPoint = other.ClosestPointOnBounds(position),
                        hitDir = this.CreateHitForward()
                    };
                    evt.hitCollision = collsion;
                    Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
                }
            }
        }

        public void ResetInside(float resetTime = 0.4f)
        {
            if (this._resetTimer == null)
            {
                this._resetTimer = new EntityTimer(resetTime, this);
            }
            this._resetTimer.Reset(true);
        }

        public void SetCollisionEnabled(bool _enabled = true)
        {
            this._collisionEnabled = _enabled;
        }

        public void SetCollisionMask(LayerMask mask)
        {
            this._collisionMask = mask;
        }

        public override void SetDied()
        {
            base.SetDied();
            this._isToBeRemoved = true;
            Singleton<EffectManager>.Instance.ClearEffectsByOwner(base._runtimeID);
        }

        public void SetupAtReset()
        {
            this._rigidbody.velocity = Vector3.zero;
            this._state = BulletState.None;
        }

        public void SetupLinear()
        {
            this._rigidbody.velocity = (Vector3) (((base.transform.forward * this.speed) + this.speedAdd) * this.BulletTimeScale);
            this._state = BulletState.Linear;
        }

        public void SetupPositioning(Vector3 originalPosition, Vector3 targetPosition, float duration, float resumeSpeed, float steerCoef, float steerCoefAcc, Vector3 tracingPosition, bool passBy = false)
        {
            this._state = BulletState.Placing;
            this._placingTimer = new EntityTimer(duration);
            this._placingTimer.Reset(true);
            this._originalPosition = originalPosition;
            this._placingPosition = targetPosition;
            this._resumeSpeed = resumeSpeed;
            this._traceLerpCoef = steerCoef;
            this._traceLerpCoefAcc = steerCoefAcc;
            this._targetPosition = tracingPosition;
            this._passBy = passBy;
        }

        public void SetupTracing()
        {
            this._state = BulletState.TracePosition;
            this._targetPosition = new Vector3(0f, 100f, 0f);
        }

        public void SetupTracing(Vector3 targetPosition, float steerCoef, float steerCoefAcc, bool passBy = false)
        {
            this._state = BulletState.TracePosition;
            this._targetPosition = targetPosition;
            this._traceLerpCoef = steerCoef;
            this._traceLerpCoefAcc = steerCoefAcc;
            this._passBy = passBy;
        }

        protected override void Start()
        {
            base.Start();
            this._rigidbody.velocity = (Vector3) (((base.transform.forward * this.speed) + this.speedAdd) * this.BulletTimeScale);
            if (this._state == BulletState.None)
            {
                this._state = BulletState.Linear;
            }
            if (this._aliveDuration > 0f)
            {
                this._aliveTimer.Reset(true);
            }
        }

        protected override void Update()
        {
            AttackResult.HitCollsion collsion;
            base.Update();
            if (this._resetTimer != null)
            {
                this._resetTimer.Core(1f);
                if (this._resetTimer.isTimeUp)
                {
                    this._enteredIDs.Clear();
                    this._resetTimer.Reset(false);
                }
            }
            if (this._aliveTimer.isActive)
            {
                this._aliveTimer.Core(this.BulletTimeScale);
                if (this._aliveTimer.isTimeUp)
                {
                    EvtBulletHit evt = new EvtBulletHit(base._runtimeID) {
                        ownerID = base.ownerID
                    };
                    collsion = new AttackResult.HitCollsion {
                        hitDir = this.CreateHitForward(),
                        hitPoint = base.transform.position
                    };
                    evt.hitCollision = collsion;
                    evt.hitGround = true;
                    evt.selfExplode = true;
                    Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
                    this._aliveTimer.Reset(false);
                }
            }
            if (InLevelData.IsOutOfStage(this.XZPosition))
            {
                EvtBulletHit hit2 = new EvtBulletHit(base._runtimeID) {
                    ownerID = base.ownerID
                };
                collsion = new AttackResult.HitCollsion {
                    hitDir = this.CreateHitForward(),
                    hitPoint = base.transform.position
                };
                hit2.hitCollision = collsion;
                Singleton<EventManager>.Instance.FireEvent(hit2, MPEventDispatchMode.Normal);
            }
            else if ((base.transform.position.y < 0.05f) && this._collisionEnabled)
            {
                EvtBulletHit hit3 = new EvtBulletHit(base._runtimeID) {
                    ownerID = base.ownerID
                };
                collsion = new AttackResult.HitCollsion {
                    hitDir = this.CreateHitForward(),
                    hitPoint = base.transform.position
                };
                hit3.hitCollision = collsion;
                hit3.hitGround = true;
                Singleton<EventManager>.Instance.FireEvent(hit3, MPEventDispatchMode.Normal);
            }
            else if (this._state == BulletState.Linear)
            {
                this.speed += (this.acceleration * Time.deltaTime) * this.BulletTimeScale;
                this._rigidbody.velocity = (Vector3) ((this.speed * base.transform.forward) * this.BulletTimeScale);
            }
            else if (this._state == BulletState.TracePosition)
            {
                this._traceLerpCoef += (this._traceLerpCoefAcc * Time.deltaTime) * this.BulletTimeScale;
                Vector3 forward = base.transform.forward;
                Vector3 vector3 = this._targetPosition - this._rigidbody.position;
                if (vector3.magnitude >= this.targetReachThreshold)
                {
                    forward = Vector3.Normalize(this._targetPosition - this._rigidbody.position);
                    float num = Vector3.Angle(forward, base.transform.forward);
                    if (!this._passBy || (num < 90.0))
                    {
                        base.transform.forward = Vector3.Slerp(base.transform.forward, forward, (Time.deltaTime * this.BulletTimeScale) * this._traceLerpCoef);
                    }
                    this.speed += (this.acceleration * Time.deltaTime) * this.BulletTimeScale;
                    this._rigidbody.velocity = (Vector3) (((this.speed * base.transform.forward) + this.speedAdd) * this.BulletTimeScale);
                }
                else
                {
                    this._rigidbody.velocity = (Vector3) (base.transform.forward * 0f);
                    if (this._collisionEnabled)
                    {
                        EvtBulletHit hit4 = new EvtBulletHit(base._runtimeID) {
                            ownerID = base.ownerID
                        };
                        collsion = new AttackResult.HitCollsion {
                            hitDir = this.CreateHitForward(),
                            hitPoint = base.transform.position
                        };
                        hit4.hitCollision = collsion;
                        hit4.hitGround = true;
                        Singleton<EventManager>.Instance.FireEvent(hit4, MPEventDispatchMode.Normal);
                    }
                }
            }
            else if (((this._state == BulletState.Placing) && (this._placingTimer != null)) && this._placingTimer.isActive)
            {
                this._placingTimer.Core(this.BulletTimeScale);
                if (!this._placingTimer.isTimeUp)
                {
                    base.transform.position = Vector3.Slerp(this._originalPosition, this._placingPosition, this._placingTimer.timer / this._placingTimer.timespan);
                    this._collisionEnabled = false;
                    this._collisionMask = -1;
                    this._rigidbody.velocity = (Vector3) ((this.speed * base.transform.forward) * this.BulletTimeScale);
                }
                else
                {
                    this._state = BulletState.TracePosition;
                    this._placingTimer.Reset(false);
                    this.speed = this._resumeSpeed;
                    this._collisionEnabled = true;
                }
            }
        }

        public float AliveDuration
        {
            get
            {
                return this._aliveDuration;
            }
            set
            {
                this._aliveDuration = value;
                if (this._aliveTimer != null)
                {
                    this._aliveTimer.timespan = value;
                }
            }
        }

        public float BulletTimeScale
        {
            get
            {
                if (this._ignoreTimeScale)
                {
                    return base._timeScale;
                }
                return this.TimeScale;
            }
        }

        public bool IgnoreTimeScale
        {
            get
            {
                return this._ignoreTimeScale;
            }
            set
            {
                this._ignoreTimeScale = value;
            }
        }

        public Vector3 RigidbodyPos
        {
            get
            {
                return this._rigidbody.position;
            }
            set
            {
                this._rigidbody.position = value;
            }
        }

        public override float TimeScale
        {
            get
            {
                return (!base.owner.IsActive() ? ((Singleton<LevelManager>.Instance.levelEntity.TimeScale * Singleton<LevelManager>.Instance.levelEntity.AuxTimeScale) * base._timeScale) : ((base.owner.TimeScale * Singleton<LevelManager>.Instance.levelEntity.AuxTimeScale) * base._timeScale));
            }
        }

        private enum BulletState
        {
            None,
            Linear,
            TracePosition,
            Placing
        }
    }
}

