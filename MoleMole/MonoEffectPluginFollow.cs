namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginFollow : BaseMonoEffectPlugin
    {
        private Vector3 _additionalOffset = Vector3.zero;
        private bool _firstFrameUpdated;
        private Transform _followTarget;
        private float _initRotationYOffset;
        [Header("Activate Game Objectg On Start")]
        public GameObject ActivateOnStart;
        [Header("Follow by init pos")]
        public bool FollowByInitPos;
        [Header("Follow rotation")]
        public bool FollowRotation;
        [Header("Empty path means the most outer transform.")]
        public string FollowTargetPath;
        [Header("Follow Target's ***root***'s Y axis rotation")]
        public bool FollowYRotation;
        [Header("Is NOT using attach point, treat Follow Target Path as transform path.")]
        public bool IsNotAttachPoint;
        [Header("Don't destory when follow target becomes null")]
        public bool NoFollowDestory;
        [Header("Only do follow on first frame")]
        public bool OnlyFirstFrame;

        protected override void Awake()
        {
            base.Awake();
            if (this.ActivateOnStart != null)
            {
                this.ActivateOnStart.SetActive(false);
            }
        }

        private void FollowPosition()
        {
            if (this.FollowRotation)
            {
                base.transform.rotation = this._followTarget.rotation;
            }
            if (this.FollowYRotation)
            {
                base.transform.rotation = Quaternion.Euler(base.transform.rotation.eulerAngles.x, this._followTarget.root.eulerAngles.y - this._initRotationYOffset, base.transform.rotation.eulerAngles.z);
            }
            base.transform.position = (this._followTarget.position + Vector3.Scale(base.transform.TransformDirection(base._effect.OffsetVec3), base._effect.transform.localScale)) + this._followTarget.TransformDirection(this._additionalOffset);
        }

        public override bool IsToBeRemove()
        {
            if (this.NoFollowDestory)
            {
                return false;
            }
            return (this._followTarget == null);
        }

        public void LateUpdate()
        {
            if (!this.OnlyFirstFrame || !this._firstFrameUpdated)
            {
                this._firstFrameUpdated = true;
                if ((this._followTarget != null) && !this.IsToBeRemove())
                {
                    this.FollowPosition();
                }
            }
        }

        private void OnDisable()
        {
            if (this.ActivateOnStart != null)
            {
                this.ActivateOnStart.SetActive(false);
            }
        }

        public override void SetDestroy()
        {
        }

        public void SetFollowParentTarget(Transform parent)
        {
            if (string.IsNullOrEmpty(this.FollowTargetPath) || this.IsNotAttachPoint)
            {
                this._followTarget = parent.Find(this.FollowTargetPath);
            }
            else
            {
                this._followTarget = parent.GetComponent<BaseMonoEntity>().GetAttachPoint(this.FollowTargetPath);
            }
            if (this.FollowByInitPos)
            {
                this._additionalOffset = this._followTarget.InverseTransformDirection(base.transform.position - this._followTarget.position);
            }
            this._initRotationYOffset = this._followTarget.root.eulerAngles.y - base.transform.rotation.eulerAngles.y;
            this.FollowPosition();
            this._firstFrameUpdated = false;
        }

        public override void Setup()
        {
            if (this.ActivateOnStart != null)
            {
                this.ActivateOnStart.SetActive(true);
            }
        }
    }
}

