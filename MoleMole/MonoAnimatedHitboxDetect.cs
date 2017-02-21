namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody))]
    public class MonoAnimatedHitboxDetect : MonoAuxObject
    {
        private Animation _animation;
        private LayerMask _collisionMask;
        private bool _enableHitWall;
        protected HashSet<uint> _enteredIDs;
        private bool _follow;
        private bool _followOwnerTimeScale;
        private Transform _followTarget;
        private Vector3 _hittingWallPosition;
        private bool _hitWallDestroy;
        private string _hitWallDestroyEffect;
        private bool _ignoreTimeScale;
        private Vector3 _lastFramePosition;
        private bool _lastFrameRecordRunning;
        private float _lastTimeScale;
        private bool _onHittingWallTriggered;
        private string _overrideAnimEventID;
        private RaycastHit _raycastWallHit;
        private Rigidbody _rigidbody;
        private bool _stopOnFirstContact;
        [Header("Hitpoint will be on the line of this and other collided transform")]
        public Transform collideCenterTransform;
        public Action<MonoAnimatedHitboxDetect> destroyCallback;
        public bool dontDestroyWhenOwnerEvade;
        public Action<MonoAnimatedHitboxDetect, Collider> enemyEnterCallback;
        [Header("Fixed retreat direction")]
        public Vector3 fixedRetreatDirection;
        [NonSerialized]
        public BaseMonoEntity owner;
        public Action<Collider, string> triggerEnterCallback;
        [Header("Use fixed retreat directioon")]
        public bool useFixedReteatDirection;
        [Header("Use owner center for retreat direction")]
        public bool useOwnerCenterForRetreatDirection;

        protected virtual void Awake()
        {
            this._enteredIDs = new HashSet<uint>();
            this._animation = base.GetComponent<Animation>();
            this._rigidbody = base.GetComponent<Rigidbody>();
            this._rigidbody.detectCollisions = false;
        }

        public virtual Vector3 CalculateFixedRetreatDirection(Vector3 hitPoint)
        {
            return base.transform.TransformDirection(this.fixedRetreatDirection);
        }

        [AnimationCallback]
        private void CreateLevelProp(string propName)
        {
            ConfigPropObject propObjectConfig = PropObjectData.GetPropObjectConfig(propName);
            float hP = propObjectConfig.PropArguments.HP;
            float attack = propObjectConfig.PropArguments.Attack;
            Vector3 position = this.collideCenterTransform.position;
            position.y = 0f;
            Singleton<PropObjectManager>.Instance.CreatePropObject(this.owner.GetRuntimeID(), propName, hP, attack, position, this.collideCenterTransform.forward, false);
        }

        public void EnableOnOwnerBeHitCanceledDestroySelf()
        {
            BaseMonoAbilityEntity owner = this.owner as BaseMonoAbilityEntity;
            if (owner != null)
            {
                owner.onBeHitCanceled = (Action<string>) Delegate.Combine(owner.onBeHitCanceled, new Action<string>(this.OnOwnerBeHitCancelCallback));
            }
        }

        [AnimationCallback]
        public void EnableWallHitCheck()
        {
            this._enableHitWall = true;
        }

        [AnimationCallback]
        public void EnableWallHitDestroy(string hitWallDestroyEffect)
        {
            this._enableHitWall = true;
            this._hitWallDestroy = true;
            this._hitWallDestroyEffect = hitWallDestroyEffect;
        }

        protected virtual void FireTriggerCallback(Collider other, BaseMonoEntity entity)
        {
            if ((other != null) && (this.owner != null))
            {
                this.triggerEnterCallback(other, this._overrideAnimEventID);
                if (this.enemyEnterCallback != null)
                {
                    this.enemyEnterCallback(this, other);
                }
            }
        }

        public void Init(BaseMonoEntity owner, LayerMask mask, Transform followTarget, bool follow, bool stopOnFirstContact)
        {
            this.owner = owner;
            this._follow = follow;
            this._stopOnFirstContact = stopOnFirstContact;
            this._collisionMask = mask;
            this._lastTimeScale = owner.TimeScale;
            this._onHittingWallTriggered = false;
            this._lastFrameRecordRunning = false;
            this._enableHitWall = false;
            this._hitWallDestroy = false;
            this._ignoreTimeScale = false;
            this._followOwnerTimeScale = false;
            this._followTarget = (followTarget == null) ? owner.transform : followTarget;
            MonoEffect componentInChildren = base.GetComponentInChildren<MonoEffect>();
            if (componentInChildren != null)
            {
                componentInChildren.SetOwner(owner);
            }
        }

        protected virtual void LateUpdate()
        {
            if ((this.owner != null) && this.owner.IsActive())
            {
                if (!this._ignoreTimeScale)
                {
                    if (this._followOwnerTimeScale)
                    {
                        this._animation[this._animation.clip.name].speed = this.owner.TimeScale;
                    }
                    else if (this._lastTimeScale != this.owner.TimeScale)
                    {
                        this._animation[this._animation.clip.name].speed = this.owner.TimeScale;
                    }
                }
                this._lastTimeScale = this.owner.TimeScale;
                if (this._follow)
                {
                    this._rigidbody.position = this._followTarget.transform.position;
                    this._rigidbody.rotation = this._followTarget.rotation;
                }
            }
            if (this._enableHitWall)
            {
                this.WallHitCheck();
            }
            if (this._onHittingWallTriggered)
            {
                this.collideCenterTransform.transform.SetPositionX(this._hittingWallPosition.x);
                this.collideCenterTransform.transform.SetPositionZ(this._hittingWallPosition.z);
                this.collideCenterTransform.transform.SetLocalEulerAnglesX(0f);
                this.collideCenterTransform.transform.SetLocalEulerAnglesY(0f);
                this.collideCenterTransform.transform.SetLocalEulerAnglesZ(0f);
                if (this._hitWallDestroy)
                {
                    this._animation.Stop();
                }
            }
            this.RecordLastFramePosition();
            if (!this._animation.isPlaying)
            {
                if (this.destroyCallback != null)
                {
                    this.destroyCallback(this);
                }
                base.SetDestroy();
            }
        }

        private void OnDestroy()
        {
            BaseMonoAbilityEntity owner = this.owner as BaseMonoAbilityEntity;
            if (owner != null)
            {
                owner.onBeHitCanceled = (Action<string>) Delegate.Remove(owner.onBeHitCanceled, new Action<string>(this.OnOwnerBeHitCancelCallback));
            }
        }

        [Conditional("UNITY_EDITOR"), Conditional("NG_HSOD_DEBUG")]
        protected virtual void OnDrawGizmos()
        {
            if (SuperDebug.DEBUG_SWITCH[1])
            {
                Gizmos.color = !this._onHittingWallTriggered ? Color.green : Color.red;
                Vector3 vector = this.collideCenterTransform.transform.position - this._lastFramePosition;
                vector.Normalize();
                Gizmos.DrawLine(this._lastFramePosition, this._lastFramePosition + ((Vector3) (vector * 1f)));
                foreach (Collider collider in base.GetComponentsInChildren<Collider>())
                {
                    if (collider.enabled)
                    {
                        Matrix4x4 matrix = Gizmos.matrix;
                        Gizmos.matrix = collider.transform.localToWorldMatrix;
                        Gizmos.color = Color.blue;
                        if (collider is BoxCollider)
                        {
                            BoxCollider collider2 = (BoxCollider) collider;
                            Gizmos.DrawWireCube(collider2.center, collider2.size);
                        }
                        else if (collider is MeshCollider)
                        {
                            MeshCollider collider3 = (MeshCollider) collider;
                            Gizmos.DrawWireMesh(collider3.sharedMesh);
                        }
                        else if (collider is CapsuleCollider)
                        {
                            CapsuleCollider collider4 = (CapsuleCollider) collider;
                            Gizmos.DrawWireSphere(collider4.center, collider4.radius);
                        }
                        Gizmos.matrix = matrix;
                    }
                }
            }
        }

        protected virtual void OnEnteredReset()
        {
        }

        protected virtual void OnEntityEntered(Collider other, BaseMonoEntity entity)
        {
        }

        private void OnOwnerBeHitCancelCallback(string skillID)
        {
            base.SetDestroy();
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((this._collisionMask.value & (((int) 1) << other.gameObject.layer)) != 0)
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
                if (componentInParent is MonoDummyDynamicObject)
                {
                    BaseMonoDynamicObject obj2 = (BaseMonoDynamicObject) componentInParent;
                    if ((obj2.dynamicType == BaseMonoDynamicObject.DynamicType.EvadeDummy) && (obj2.owner != null))
                    {
                        this._enteredIDs.Add(obj2.owner.GetRuntimeID());
                    }
                }
                this._enteredIDs.Add(componentInParent.GetRuntimeID());
                this.OnEntityEntered(other, componentInParent);
                if (this._stopOnFirstContact)
                {
                    this._animation.Stop();
                    this._rigidbody.detectCollisions = false;
                }
                if (this.triggerEnterCallback != null)
                {
                    if (this._follow)
                    {
                        base.StartCoroutine(this.WaitEndOfFrameTriggerHit(other, componentInParent));
                    }
                    else
                    {
                        this.FireTriggerCallback(other, componentInParent);
                    }
                }
            }
        }

        private void OnValidate()
        {
            if (this.useFixedReteatDirection)
            {
            }
        }

        private void RecordHittingWallPosAndRoate()
        {
            this._hittingWallPosition = this.collideCenterTransform.transform.position;
        }

        private void RecordLastFramePosition()
        {
            this._lastFrameRecordRunning = true;
            this._lastFramePosition = this.collideCenterTransform.transform.position;
        }

        [AnimationCallback]
        private void ResetNewTrigger(string animEventID)
        {
            this._overrideAnimEventID = animEventID;
        }

        [AnimationCallback]
        private void ResetNewTriggerByMultiAnimEventID(string multiAnimEventID)
        {
            BaseMonoAvatar owner = this.owner as BaseMonoAvatar;
            ConfigMultiAnimEvent event2 = owner.config.MultiAnimEvents[multiAnimEventID];
            for (int i = 0; i < event2.AnimEventNames.Length; i++)
            {
                if (owner.CheckAnimEventPredicate(event2.AnimEventNames[i]))
                {
                    this.ResetNewTrigger(event2.AnimEventNames[i]);
                    return;
                }
            }
        }

        [AnimationCallback]
        private void ResetTriggerWithoutResetInside()
        {
            this._enteredIDs.Clear();
            this.OnEnteredReset();
        }

        [AnimationCallback]
        private void ResetTriggerWithResetInside()
        {
            this._enteredIDs.Clear();
            this.OnEnteredReset();
            this._rigidbody.detectCollisions = false;
            this._rigidbody.detectCollisions = true;
        }

        public void SetFollowOwnerTimeScale(bool followOwnerTimeScale)
        {
            this._followOwnerTimeScale = followOwnerTimeScale;
        }

        public void SetIgnoreTimeScale(bool ignoreTimeScale)
        {
            this._ignoreTimeScale = ignoreTimeScale;
        }

        private void Start()
        {
            this._animation.Play();
            base.StartCoroutine(this.WaitEndOfFrameEnableCollision());
        }

        [AnimationCallback]
        private void TriggerAudioPattern(string name)
        {
            Singleton<WwiseAudioManager>.Instance.Post(name, base.gameObject, null, null);
        }

        [AnimationCallback]
        private void TriggerEffectPattern(string patternName)
        {
            if (this.owner != null)
            {
                List<MonoEffect> list;
                Singleton<EffectManager>.Instance.TriggerEntityEffectPatternRaw(patternName, base.transform.position, base.transform.forward, Vector3.one, this.owner, out list);
                for (int i = 0; i < list.Count; i++)
                {
                    MonoEffect effect = list[i];
                    effect.SetOwner(this.owner);
                    if (effect.GetComponent<MonoEffectPluginFollow>() != null)
                    {
                        effect.GetComponent<MonoEffectPluginFollow>().SetFollowParentTarget(base.transform);
                    }
                    effect.SetupOverride(this.owner);
                    effect.dontDestroyWhenOwnerEvade = this.dontDestroyWhenOwnerEvade;
                }
            }
        }

        [AnimationCallback]
        private void TriggerOwnerAnimEvent(string animEventID)
        {
            BaseMonoAnimatorEntity owner = this.owner as BaseMonoAnimatorEntity;
            if ((owner != null) && owner.IsActive())
            {
                owner.AnimEventHandler(animEventID);
            }
        }

        public void TriggerOwnerAnimEventIgnoreOwnerActive(string animEventID)
        {
            BaseMonoAnimatorEntity owner = this.owner as BaseMonoAnimatorEntity;
            if (owner != null)
            {
                owner.AnimEventHandler(animEventID);
            }
        }

        [DebuggerHidden]
        private IEnumerator WaitEndOfFrameEnableCollision()
        {
            return new <WaitEndOfFrameEnableCollision>c__Iterator2E { <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator WaitEndOfFrameTriggerHit(Collider other, BaseMonoEntity entity)
        {
            return new <WaitEndOfFrameTriggerHit>c__Iterator2D { other = other, entity = entity, <$>other = other, <$>entity = entity, <>f__this = this };
        }

        private void WallHitCheck()
        {
            if (this._lastFrameRecordRunning && this._enableHitWall)
            {
                Vector3 direction = this.collideCenterTransform.transform.position - this._lastFramePosition;
                direction.Normalize();
                float magnitude = direction.magnitude;
                if ((Physics.Raycast(this._lastFramePosition, direction, out this._raycastWallHit, magnitude, ((int) 1) << InLevelData.STAGE_COLLIDER_LAYER) && (Vector3.Angle(this._raycastWallHit.normal, Vector3.up) >= 20f)) && !this._onHittingWallTriggered)
                {
                    this._onHittingWallTriggered = true;
                    this.RecordHittingWallPosAndRoate();
                    if (this._hitWallDestroy && !string.IsNullOrEmpty(this._hitWallDestroyEffect))
                    {
                        this.TriggerEffectPattern(this._hitWallDestroyEffect);
                    }
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WaitEndOfFrameEnableCollision>c__Iterator2E : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoAnimatedHitboxDetect <>f__this;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this._rigidbody.detectCollisions = true;
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <WaitEndOfFrameTriggerHit>c__Iterator2D : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal BaseMonoEntity <$>entity;
            internal Collider <$>other;
            internal MonoAnimatedHitboxDetect <>f__this;
            internal BaseMonoEntity entity;
            internal Collider other;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.FireTriggerCallback(this.other, this.entity);
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

