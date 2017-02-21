namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class BaseMonoDynamicObject : BaseMonoEntity
    {
        private float _lastTimeScale;
        protected float _timeScale = 1f;
        [NonSerialized]
        public BaseMonoEntity owner;
        [NonSerialized]
        public uint ownerID;

        protected BaseMonoDynamicObject()
        {
        }

        public override Transform GetAttachPoint(string name)
        {
            return base.transform;
        }

        public virtual void Init(uint runtimeID, uint ownerID)
        {
            base._runtimeID = runtimeID;
            this.ownerID = ownerID;
            this.owner = Singleton<EventManager>.Instance.GetEntity(ownerID);
            this._lastTimeScale = this.TimeScale;
            this.dynamicType = DynamicType.Default;
        }

        public bool IsOwnerStaticInScene()
        {
            if (this.owner is BaseMonoMonster)
            {
                BaseMonoMonster owner = this.owner as BaseMonoMonster;
                if ((owner != null) && owner.isStaticInScene)
                {
                    return true;
                }
            }
            return false;
        }

        protected virtual void OnTimeScaleChanged(float newTimescale)
        {
        }

        public virtual void SetDied()
        {
        }

        protected virtual void Start()
        {
            Singleton<EventManager>.Instance.FireEvent(new EvtDynamicObjectCreated(this.owner.GetRuntimeID(), base._runtimeID, this.dynamicType), MPEventDispatchMode.Normal);
        }

        protected virtual void Update()
        {
            if (this._lastTimeScale != this.TimeScale)
            {
                this.OnTimeScaleChanged(this.TimeScale);
            }
            this._lastTimeScale = this.TimeScale;
        }

        public DynamicType dynamicType { get; set; }

        public override float TimeScale
        {
            get
            {
                return (!this.owner.IsActive() ? (Singleton<LevelManager>.Instance.levelEntity.TimeScale * this._timeScale) : (this.owner.TimeScale * this._timeScale));
            }
        }

        public override Vector3 XZPosition
        {
            get
            {
                return new Vector3(base.transform.position.x, 0f, base.transform.position.z);
            }
        }

        public enum DynamicType
        {
            Default,
            Barrier,
            NavigationArrow,
            EvadeDummy
        }
    }
}

