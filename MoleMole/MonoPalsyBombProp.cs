namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class MonoPalsyBombProp : MonoTriggerProp
    {
        private Vector3 _bornAcceleration;
        private Vector3 _bornVelocity;
        private float _currentSpeed;
        private float _height = 1f;
        private Rigidbody _rigidbody;
        private float _selfRotateSpeed = 100f;
        private PropState _state;

        protected override void Awake()
        {
            base.Awake();
            this._rigidbody = base.GetComponent<Rigidbody>();
            this._rigidbody.velocity = Vector3.zero;
            this._state = PropState.Idle;
        }

        public override void Init(uint runtimeID)
        {
            base.Init(runtimeID);
            base._transform.position += new Vector3(0f, this._height, 0f);
        }

        protected override void OnEffectiveTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == InLevelData.STAGE_COLLIDER_LAYER)
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtPropObjectForceKilled(base._runtimeID), MPEventDispatchMode.Normal);
            }
            else
            {
                base.OnEffectiveTriggerEnter(other);
            }
        }

        public override void SetDied(KillEffect killEffect)
        {
            base.SetDied(killEffect);
            if (base.config.PropArguments.OnKillEffectPattern != null)
            {
                this.FireEffect(base.config.PropArguments.OnKillEffectPattern, base.transform.position, base.transform.forward);
            }
        }

        public void StartParabolaBorn(Vector3 bornPosition, Vector3 bornVelocity, Vector3 bornAcceleration)
        {
            this._state = PropState.Born;
            this._rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
            base.transform.position = bornPosition;
            float num = Vector3.Dot(Vector3.up, bornVelocity);
            num = Vector3.Dot(Vector3.down, bornAcceleration);
            base.transform.position = bornPosition;
            this._bornVelocity = bornVelocity;
            this._bornAcceleration = bornAcceleration;
        }

        protected override void Update()
        {
            base.Update();
            float angle = (Time.deltaTime * this._selfRotateSpeed) * this.TimeScale;
            base.transform.Rotate(base.transform.up, angle);
            if (this._state == PropState.Born)
            {
                this._bornVelocity += (Vector3) ((this._bornAcceleration * this.TimeScale) * Time.deltaTime);
                this._rigidbody.velocity = this._bornVelocity;
                if ((Vector3.Dot(Vector3.down, this._bornVelocity) > 0f) && (base.transform.position.y < this._height))
                {
                    this._rigidbody.constraints |= RigidbodyConstraints.FreezePositionY;
                    this._state = PropState.Idle;
                }
            }
            else
            {
                BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                if (localAvatar != null)
                {
                    if (this._state == PropState.Idle)
                    {
                        this._currentSpeed = 0f;
                        this._rigidbody.velocity = Vector3.zero;
                        float warningRange = base.config.PropArguments.WarningRange;
                        if (Vector3.Distance(localAvatar.XZPosition, this.XZPosition) < warningRange)
                        {
                            this._state = PropState.Chase;
                        }
                    }
                    else if (this._state == PropState.Chase)
                    {
                        if (this._currentSpeed < base.config.PropArguments.MaxMoveSpeed)
                        {
                            this._currentSpeed += (base.config.PropArguments.Acceleration * this.TimeScale) * Time.deltaTime;
                            if (this._currentSpeed > base.config.PropArguments.MaxMoveSpeed)
                            {
                                this._currentSpeed = base.config.PropArguments.MaxMoveSpeed;
                            }
                        }
                        float escapeRange = base.config.PropArguments.EscapeRange;
                        if (Vector3.Distance(localAvatar.XZPosition, this.XZPosition) > escapeRange)
                        {
                            this._state = PropState.Idle;
                        }
                        Vector3 xZPosition = localAvatar.XZPosition;
                        if (GlobalVars.USE_GET_PATH_SWITCH && Singleton<DetourManager>.Instance.GetTargetPosition(this, this.XZPosition, localAvatar.XZPosition, ref xZPosition))
                        {
                            Debug.DrawLine(this.XZPosition, xZPosition, Color.yellow, 0.1f);
                        }
                        Vector3 vector3 = xZPosition - this.XZPosition;
                        this._rigidbody.velocity = (Vector3) (vector3.normalized * this._currentSpeed);
                    }
                    else if (this._state == PropState.Died)
                    {
                    }
                    base.transform.position = new Vector3(base.transform.position.x, this._height, base.transform.position.z);
                }
            }
        }

        private enum PropState
        {
            Born,
            Idle,
            Chase,
            Died
        }
    }
}

