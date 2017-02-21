namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class BaseMonoCamera : BaseMonoEntity
    {
        protected Transform _cameraTrans;

        protected BaseMonoCamera()
        {
        }

        public virtual void Awake()
        {
        }

        public override Transform GetAttachPoint(string name)
        {
            return base.transform;
        }

        protected void Init(uint cameraType, uint runtimeID)
        {
            this.CameraType = cameraType;
            base._runtimeID = runtimeID;
            this._cameraTrans = base.gameObject.transform;
        }

        public override bool IsActive()
        {
            return true;
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        public uint CameraType { get; private set; }

        public Vector3 Forward
        {
            get
            {
                return this._cameraTrans.forward;
            }
            private set
            {
            }
        }

        public override float TimeScale
        {
            get
            {
                return 1f;
            }
        }

        public override Vector3 XZPosition
        {
            get
            {
                return new Vector3(this._cameraTrans.position.x, 0f, this._cameraTrans.position.z);
            }
        }
    }
}

