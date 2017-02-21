namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class BaseMonoEffect : BaseMonoEntity
    {
        protected Transform _effectTrans;
        [NonSerialized]
        public bool isFromEffectPool;
        public Vector3 OffsetVec3;
        public float RotateX;

        protected BaseMonoEffect()
        {
        }

        protected virtual void Awake()
        {
        }

        public override Transform GetAttachPoint(string name)
        {
            return base.transform;
        }

        public virtual void Init(string effectPath, uint runtimeID, Vector3 initPos, Vector3 faceDir, Vector3 initScale, bool isFromEffectPool)
        {
            this.EffectTypeName = effectPath;
            base._runtimeID = runtimeID;
            this._effectTrans = base.gameObject.transform;
            float angle = Vector3.Angle(faceDir, Vector3.forward);
            if (Vector3.Cross(faceDir, Vector3.forward).y >= 0.0)
            {
                angle = -angle;
            }
            initPos += Quaternion.AngleAxis(angle, Vector3.up) * Vector3.Scale(this.OffsetVec3, initScale);
            this.FaceDirection = faceDir;
            this._effectTrans.position = initPos;
            this._effectTrans.localEulerAngles = new Vector3(this.RotateX, this._effectTrans.localEulerAngles.y, this._effectTrans.localEulerAngles.z);
            this._effectTrans.localScale = initScale;
            this.isFromEffectPool = isFromEffectPool;
        }

        protected override void OnDestroy()
        {
        }

        public virtual void Setup()
        {
        }

        public virtual void Start()
        {
        }

        public virtual void Update()
        {
        }

        public uint EffectTypeID { get; private set; }

        public string EffectTypeName { get; private set; }

        public Vector3 FaceDirection
        {
            get
            {
                return this._effectTrans.forward;
            }
            protected set
            {
                value.Normalize();
                this._effectTrans.forward = value;
            }
        }

        public override Vector3 XZPosition
        {
            get
            {
                return new Vector3(this._effectTrans.position.x, 0f, this._effectTrans.position.z);
            }
        }
    }
}

