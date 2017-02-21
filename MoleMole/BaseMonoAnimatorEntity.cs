namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(Animator))]
    public abstract class BaseMonoAnimatorEntity : BaseMonoAbilityEntity, IFadeOff, IFrameHaltable, IWeaponAttacher
    {
        private Dictionary<int, List<string>> _activeAnimatorEventPatterns;
        private int _addictiveVelocityIndex = -1;
        private Dictionary<int, Vector3> _additiveVelocityDic = new Dictionary<int, Vector3>();
        protected bool _animatePhysicsStarted;
        protected Animator _animator;
        protected HashSet<string> _animEventPredicates;
        private List<ColorAdjuster> _bodyColorAdjusterList;
        private CollisionCallback _collisionCallback;
        private LayerMask _collisionLayerMask;
        private float _curMass;
        private AnimatorEventPatternProcessItem _curProcessItem = new AnimatorEventPatternProcessItem();
        protected string _currentNamedState;
        protected AnimatorStateInfo _currrentAnimatorState;
        private Dictionary<int, MaterialFadeSetting> _fadeMaterialDic = new Dictionary<int, MaterialFadeSetting>();
        protected FrameHaltPlugin _frameHaltPlugin;
        private int _hasAdditiveVelocityCount;
        private int _highspeedMovementCount;
        protected List<MaterialGroup> _instancedMaterialGroups;
        private float _lastTimeScale;
        private List<LayerFader> _layerFaders;
        private MonoLightShadowManager _LightMapCorrectionManager;
        private MonoLightProbManager _lightProbManager;
        private bool _maskAllTriggers;
        protected List<string> _maskedAnimEvents;
        protected List<string> _maskedTriggers;
        private Material[] _materialList;
        protected List<SpecialStateMaterialData> _matListForSpecailState = new List<SpecialStateMaterialData>();
        private bool _muteAdditiveVelocity;
        private bool _needOverrideSteer;
        private bool _needOverrideVelocity;
        private bool _needSteer;
        private Vector3 _nextFaceDir = Vector3.zero;
        private Vector3 _overrideSteer;
        private Vector3 _overrideVelocity;
        private AnimatorStateInfo _prevProcessingStateInfo;
        private AnimatorEventPatternProcessItem _prevProcessItem = new AnimatorEventPatternProcessItem();
        private AnimatorStateInfo _processingStateInfo;
        private float _recoverPosY;
        protected Rigidbody _rigidbody;
        private int _sameFrameExitCount;
        private AnimatorStateInfo[] _sameFrameExitStates = new AnimatorStateInfo[4];
        protected ShaderLerpPlugin _shaderLerpPlugin;
        protected FixedStack<Shader> _shaderStack;
        protected ShaderTransitionPlugin _shaderTransitionPlugin;
        private List<ColorAdjuster> _shadowColorAdjusterList;
        private float _timeScale;
        private FixedStack<float> _timeScaleStack;
        protected Transform _transform;
        protected List<WaitTransitionState> _transitionStates = new List<WaitTransitionState>();
        private float _uniformScale = 1f;
        private List<string> _waitingAudioEvent = new List<string>();
        private bool _waitingForCollision;
        private bool _wasInTransition;
        public ConfigAnimatorEntity animatorConfig;
        [HideInInspector]
        public AttachPoint[] attachPoints = new AttachPoint[0];
        private const string DEFAULT_GROUP_NAME = "DEFAULT_MATERIAL_GROUP";
        private const int FRAME_EXIT_ANIMATOR_STATE_BUFFER_COUNT = 4;
        public MaterialGroup[] materialGroups = new MaterialGroup[0];
        private const int MAX_ADDITIVE_VELOCITY_INDEX = 20;
        protected const float MOVE_SPEED_LIMIT = 340f;
        public Action<AnimatorStateInfo, AnimatorStateInfo> onAnimatorStateChanged;
        public Action<Vector3> onSteerFaceDirectionSet;
        public Action<AnimatorParameterEntry> onUserInputControllerChanged;
        private const string PREDICATE_SPLIT = ":";
        private string[] propNamesForLightProb = new string[] { "_Color", "_MainColor" };
        protected const string RANDOM_PARAM = "Random";
        public Renderer[] renderers;
        public Transform RootNode;
        protected const int SHADER_STACK_SIZE = 5;

        protected BaseMonoAnimatorEntity()
        {
        }

        public override int AddAdditiveVelocity(Vector3 velocity)
        {
            int index = -1;
            if (velocity != Vector3.zero)
            {
                this._addictiveVelocityIndex++;
                if (this._addictiveVelocityIndex > 20)
                {
                    this._addictiveVelocityIndex = 0;
                }
                index = this._addictiveVelocityIndex;
                if (this.HasAdditiveVelocityOfIndex(index))
                {
                    this.SetAdditiveVelocityOfIndex(velocity, index);
                    return index;
                }
                this._additiveVelocityDic.Add(index, velocity);
            }
            return index;
        }

        public void AddAnimatorEventPattern(int stateHash, string eventPattern)
        {
            this.AttachAnimatorEventPattern(stateHash, eventPattern);
        }

        public override void AddAnimEventPredicate(string predicate)
        {
            this._animEventPredicates.Add(predicate);
        }

        public void AddFrameExitedAnimatorStates(AnimatorStateInfo stateInfo)
        {
            this._sameFrameExitStates[this._sameFrameExitCount] = stateInfo;
            this._sameFrameExitCount++;
        }

        protected int AddWaitTransitionState()
        {
            this._transitionStates.Add(WaitTransitionState.Idle);
            return (this._transitionStates.Count - 1);
        }

        public abstract void AnimEventHandler(string animEventID);
        protected virtual void ApplyAnimatorProperties()
        {
            this.SyncAnimatorSpeed();
            this.SyncAnimatorBools();
            this.SyncAnimatorInts();
            this.SyncFadeLayers();
            AnimatorStateInfo currentAnimatorStateInfo = this._animator.GetCurrentAnimatorStateInfo(0);
            if (this.animatorConfig.StateToParamBindMap.ContainsKey(currentAnimatorStateInfo.shortNameHash))
            {
                this.SetStateToParamWithNormalizedTime(currentAnimatorStateInfo);
            }
        }

        private void ApplyLightProb()
        {
            this.InitDataForLightProb();
            LightProbProperties ret = new LightProbProperties();
            if ((this._lightProbManager != null) && this._lightProbManager.Evaluate(this.XZPosition, ref ret))
            {
                for (int i = 0; i < this._instancedMaterialGroups[0].entries.Length; i++)
                {
                    MaterialGroup.RendererMaterials materials = this._instancedMaterialGroups[0].entries[i];
                    for (int k = 0; k < materials.colorModifiers.Length; k++)
                    {
                        materials.colorModifiers[k].Multiply((Color) (ret.bodyColor * 2f));
                    }
                }
                for (int j = 0; j < this._shadowColorAdjusterList.Count; j++)
                {
                    this._shadowColorAdjusterList[j].Apply(ret.shadowColor);
                }
            }
            if ((this._LightMapCorrectionManager != null) && this._LightMapCorrectionManager.Evaluate(this.XZPosition, ref ret))
            {
                for (int m = 0; m < this._instancedMaterialGroups[0].entries.Length; m++)
                {
                    MaterialGroup.RendererMaterials materials2 = this._instancedMaterialGroups[0].entries[m];
                    for (int num5 = 0; num5 < materials2.colorModifiers.Length; num5++)
                    {
                        materials2.colorModifiers[num5].Multiply((Color) (ret.bodyColor * 2f));
                    }
                }
                for (int n = 0; n < this._shadowColorAdjusterList.Count; n++)
                {
                    this._shadowColorAdjusterList[n].Apply(ret.shadowColor);
                }
            }
        }

        protected void AttachAnimatorEventPattern(int animatorStateHash, string eventPattern)
        {
            List<string> list;
            this._activeAnimatorEventPatterns.TryGetValue(animatorStateHash, out list);
            if (list == null)
            {
                list = new List<string>();
                this._activeAnimatorEventPatterns.Add(animatorStateHash, list);
            }
            bool flag = false;
            int num = 0;
            int count = list.Count;
            while (num < count)
            {
                if (eventPattern == list[num])
                {
                    flag = true;
                    break;
                }
                num++;
            }
            if (!flag)
            {
                list.Add(eventPattern);
            }
        }

        public virtual void Awake()
        {
            if (this.RootNode == null)
            {
                throw new Exception("Invalid Type or State!");
            }
            this._rigidbody = base.GetComponent<Rigidbody>();
            this._animator = base.GetComponent<Animator>();
            if (this._animator != null)
            {
                this._animator.logWarnings = false;
            }
            this._transform = base.gameObject.transform;
            this._timeScaleStack = new FixedStack<float>(8, null);
            this._timeScaleStack.Push(1f, true);
            this._timeScale = this._lastTimeScale = this._timeScaleStack.value * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
            this._layerFaders = new List<LayerFader>();
            this._curMass = this._rigidbody.mass;
        }

        public void CastWaitingAudioEvent()
        {
            int num = 0;
            int count = this._waitingAudioEvent.Count;
            while (num < count)
            {
                Singleton<WwiseAudioManager>.Instance.Post(this._waitingAudioEvent[num], base.gameObject, null, null);
                num++;
            }
            this._waitingAudioEvent.Clear();
        }

        private void CheckCollisionHandling(Collision collision)
        {
            ContactPoint point;
            if (this._waitingForCollision && (this._collisionLayerMask.ContainsLayer(collision.gameObject.layer) && this.GetXZContact(collision, out point)))
            {
                Vector3 normal = point.normal;
                normal.y = 0f;
                this._collisionCallback(collision.gameObject.layer, normal);
                this._waitingForCollision = false;
            }
        }

        [AnimationCallback]
        public virtual void CleanOwnedObjects()
        {
            this.StopAllEffects();
            Singleton<AuxObjectManager>.Instance.ClearHitBoxDetectByOwnerEvade(base._runtimeID);
            this.CastWaitingAudioEvent();
        }

        public abstract void ClearAttackTarget();
        public abstract void ClearAttackTriggers();
        public void ClearCheckForCollision()
        {
            this._collisionCallback = null;
            this._waitingForCollision = false;
        }

        private void ClearPreviousParamBindOnTransitionEnd(AnimatorStateInfo fromState, AnimatorStateInfo toState)
        {
            AnimatorStateToParameterConfig config = null;
            AnimatorStateToParameterConfig config2 = null;
            this.animatorConfig.StateToParamBindMap.TryGetValue(fromState.shortNameHash, out config);
            this.animatorConfig.StateToParamBindMap.TryGetValue(toState.shortNameHash, out config2);
            string stateName = (config == null) ? null : config.ParameterID;
            string str2 = (config2 == null) ? null : config2.ParameterID;
            if ((stateName != null) && (stateName != str2))
            {
                this.SetLocomotionBool(stateName, false, false);
                if (config.ParameterIDSub != null)
                {
                    this.SetLocomotionBool(config.ParameterIDSub, false, false);
                }
            }
        }

        public void ClearSkillEffect(string notClearSkillID)
        {
            List<MonoEffect> effectsByOwner = Singleton<EffectManager>.Instance.GetEffectsByOwner(base.GetRuntimeID());
            int num = 0;
            int count = effectsByOwner.Count;
            while (num < count)
            {
                MonoEffect effect = effectsByOwner[num];
                if (!string.IsNullOrEmpty(effect.belongSkillName) && (string.IsNullOrEmpty(notClearSkillID) || (effect.belongSkillName != notClearSkillID)))
                {
                    effect.SetDestroyImmediately();
                }
                num++;
            }
        }

        public override bool ContainAnimEventPredicate(string predicate)
        {
            return this._animEventPredicates.Contains(predicate);
        }

        public abstract void DeadHandler();
        protected void DetachAnimatorEventPattern(int animatorStateHash, string eventPattern)
        {
            if (this._activeAnimatorEventPatterns.ContainsKey(animatorStateHash))
            {
                List<string> list = this._activeAnimatorEventPatterns[animatorStateHash];
                int index = 0;
                int count = list.Count;
                while (index < count)
                {
                    if (eventPattern == list[index])
                    {
                        list.RemoveAt(index);
                        break;
                    }
                    index++;
                }
            }
        }

        public void DisableRootMotionAndCollision()
        {
            this._animator.applyRootMotion = false;
            this._rigidbody.detectCollisions = false;
        }

        public void DisableShadow()
        {
            foreach (Paster paster in base.GetComponentsInChildren<Paster>())
            {
                UnityEngine.Object.Destroy(paster.gameObject);
            }
        }

        public virtual void Enable()
        {
            base.enabled = true;
            base.gameObject.SetActive(true);
        }

        public override void FireEffect(string patternName)
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(patternName, this.XZPosition, this.FaceDirection, (Vector3) (Vector3.one * this._uniformScale), this);
        }

        public override void FireEffect(string patternName, Vector3 initPos, Vector3 initDir)
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(patternName, initPos, initDir, (Vector3) (Vector3.one * this._uniformScale), this);
        }

        public override void FireEffectTo(string patternName, BaseMonoEntity to)
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPatternFromTo(patternName, this.XZPosition, this.FaceDirection, (Vector3) (Vector3.one * this._uniformScale), this, to);
        }

        protected virtual void FixedUpdate()
        {
            bool flag;
            Vector3 zero;
            if (this._needOverrideSteer)
            {
                flag = true;
                zero = this._overrideSteer;
            }
            else if (this._needSteer)
            {
                flag = true;
                zero = this._nextFaceDir;
            }
            else
            {
                flag = false;
                zero = Vector3.zero;
            }
            if (flag)
            {
                float num = Vector3.Angle(this.FaceDirection, zero);
                Quaternion quaternion = Quaternion.AngleAxis(Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(this.FaceDirection, zero))) * num, Vector3.up);
                this._rigidbody.MoveRotation(this._rigidbody.rotation * quaternion);
                this._needSteer = false;
                this._needOverrideSteer = false;
            }
            if (this._needOverrideVelocity)
            {
                this._rigidbody.velocity = this._overrideVelocity;
            }
            if (this._animatePhysicsStarted)
            {
                this.ResetRigidbodyRotation();
            }
            this.FixedUpdatePlugins();
        }

        protected virtual void FixedUpdatePlugins()
        {
        }

        public void FrameHalt(int frameNum)
        {
            if (frameNum > 0)
            {
                this._frameHaltPlugin.FrameHalt(frameNum);
            }
        }

        public Material[] GetAllMaterials()
        {
            if (this._materialList == null)
            {
                this._materialList = this._instancedMaterialGroups[this._instancedMaterialGroups.Count - 1].GetAllMaterials();
            }
            return this._materialList;
        }

        public override Transform GetAttachPoint(string name)
        {
            for (int i = 0; i < this.attachPoints.Length; i++)
            {
                if (this.attachPoints[i].name == name)
                {
                    return this.attachPoints[i].pointTransform;
                }
            }
            return base.transform;
        }

        public string GetCurrentNamedState()
        {
            return this._currentNamedState;
        }

        public override float GetCurrentNormalizedTime()
        {
            if (this._animator.IsInTransition(0))
            {
                return this._animator.GetNextAnimatorStateInfo(0).normalizedTime;
            }
            return this._animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        }

        public bool GetLocomotionBool(string stateName)
        {
            return this._animator.GetBool(stateName);
        }

        public float GetLocomotionFloat(string name)
        {
            return this._animator.GetFloat(name);
        }

        public int GetLocomotionInteger(string stateName)
        {
            return this._animator.GetInteger(stateName);
        }

        public virtual float GetTargetAlpha()
        {
            return 1f;
        }

        private bool GetXZContact(Collision collision, out ContactPoint contact)
        {
            contact = collision.contacts[0];
            for (int i = 0; i < collision.contacts.Length; i++)
            {
                if (Vector3.Angle(collision.contacts[i].normal, Vector3.up) >= 20f)
                {
                    contact = collision.contacts[i];
                    return true;
                }
            }
            return false;
        }

        protected virtual void HandlePhysicsProperty(Collider hitbox, ConfigEntityPhysicsProperty physicsProperty)
        {
        }

        public override bool HasAdditiveVelocityOfIndex(int index)
        {
            return this._additiveVelocityDic.ContainsKey(index);
        }

        public bool HasAttachPoint(string name)
        {
            for (int i = 0; i < this.attachPoints.Length; i++)
            {
                if (this.attachPoints[i].name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public override void Init(uint runtimeID)
        {
            base.Init(runtimeID);
            this._animEventPredicates = new HashSet<string> { "Always" };
            this._activeAnimatorEventPatterns = new Dictionary<int, List<string>>();
            base.RegisterPropertyChangedCallback("Animator_MoveSpeedRatio", new Action(this.SyncAnimatorSpeed));
            base.RegisterPropertyChangedCallback("Animator_OverallSpeedRatio", new Action(this.SyncAnimatorSpeed));
            base.RegisterPropertyChangedCallback("Animator_OverallSpeedRatioMultiplied", new Action(this.SyncAnimatorSpeed));
            base.RegisterPropertyChangedCallback("Entity_MassRatio", new Action(this.SyncMass));
            base.onAnimatorBoolChanged = (Action) Delegate.Combine(base.onAnimatorBoolChanged, new Action(this.SyncAnimatorBools));
            base.onAnimatorIntChanged = (Action) Delegate.Combine(base.onAnimatorIntChanged, new Action(this.SyncAnimatorInts));
            base.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.onCurrentSkillIDChanged, new Action<string, string>(this.OnSkillEffectClear));
            this._highspeedMovementCount = 0;
        }

        private void InitDataForLightProb()
        {
            if (this._lightProbManager == null)
            {
                this._lightProbManager = Singleton<StageManager>.Instance.GetPerpStage().lightProbManager;
            }
            if (this._LightMapCorrectionManager == null)
            {
                this._LightMapCorrectionManager = Singleton<StageManager>.Instance.GetPerpStage().lightMapCorrectManager;
            }
            if ((this._lightProbManager != null) || (this._LightMapCorrectionManager != null))
            {
                if ((this._bodyColorAdjusterList == null) && (this._instancedMaterialGroups.Count > 0))
                {
                    this._bodyColorAdjusterList = new List<ColorAdjuster>();
                    for (int i = 0; i < this._instancedMaterialGroups[0].entries.Length; i++)
                    {
                        MaterialGroup.RendererMaterials materials = this._instancedMaterialGroups[0].entries[i];
                        for (int j = 0; j < materials.materials.Length; j++)
                        {
                            Material mat = materials.materials[j];
                            ColorAdjuster item = new ColorAdjuster(mat, this.propNamesForLightProb);
                            if (!item.IsEmpty())
                            {
                                this._bodyColorAdjusterList.Add(item);
                            }
                        }
                    }
                }
                if (this._shadowColorAdjusterList == null)
                {
                    this._shadowColorAdjusterList = new List<ColorAdjuster>();
                    foreach (Paster paster in base.GetComponentsInChildren<Paster>())
                    {
                        ColorAdjuster adjuster2 = new ColorAdjuster(paster.PasterMaterial, this.propNamesForLightProb);
                        if (!adjuster2.IsEmpty())
                        {
                            this._shadowColorAdjusterList.Add(adjuster2);
                        }
                    }
                }
            }
        }

        private void InitMaterialsForSpecialState()
        {
            MaterialGroup group = this._instancedMaterialGroups[0];
            for (int i = 0; i < group.entries.Length; i++)
            {
                for (int j = 0; j < group.entries[i].materials.Length; j++)
                {
                    Material material = group.entries[i].materials[j];
                    if (material.HasProperty("_SpecialState"))
                    {
                        SpecialStateMaterialData item = new SpecialStateMaterialData {
                            material = material,
                            colorMultiplier = group.entries[i].colorModifiers[j].AddMultiplier(),
                            originalShader = material.shader
                        };
                        this._matListForSpecailState.Add(item);
                    }
                }
            }
        }

        protected virtual void InitPlugins()
        {
            this._frameHaltPlugin = new FrameHaltPlugin(this);
            this._shaderTransitionPlugin = new ShaderTransitionPlugin(this);
            this._shaderLerpPlugin = new ShaderLerpPlugin(this);
        }

        public bool IsFrameHalting()
        {
            return this._frameHaltPlugin.IsActive();
        }

        protected bool IsWaitTransitionUnactive(int stateIx)
        {
            return ((((WaitTransitionState) this._transitionStates[stateIx]) == WaitTransitionState.Idle) || (((WaitTransitionState) this._transitionStates[stateIx]) == WaitTransitionState.TransitionDone));
        }

        protected virtual void LateUpdate()
        {
            this.ProcessAnimatorStates();
        }

        protected void MaskAllTriggers(bool mask)
        {
            this._maskAllTriggers = mask;
        }

        public override void MaskAnimEvent(string animEventID)
        {
            if (this._maskedAnimEvents == null)
            {
                this._maskedAnimEvents = new List<string>();
            }
            this._maskedAnimEvents.Add(animEventID);
        }

        public override void MaskTrigger(string triggerID)
        {
            if (this._maskedTriggers == null)
            {
                this._maskedTriggers = new List<string>();
            }
            this._maskedTriggers.Add(triggerID);
        }

        uint IFadeOff.GetRuntimeID()
        {
            return base.GetRuntimeID();
        }

        uint IFrameHaltable.GetRuntimeID()
        {
            return base.GetRuntimeID();
        }

        GameObject IWeaponAttacher.get_gameObject()
        {
            return base.gameObject;
        }

        public abstract void MultiAnimEventHandler(string multiAnimEventID);
        protected virtual void OnAnimatorMove()
        {
            if (!this._needOverrideVelocity)
            {
                this._rigidbody.velocity = this._animator.velocity;
                if (this.hasAdditiveVelocity)
                {
                    Vector3 zero = Vector3.zero;
                    foreach (Vector3 vector2 in this._additiveVelocityDic.Values)
                    {
                        zero += vector2;
                    }
                    this._rigidbody.velocity += (Vector3) (zero * this.TimeScale);
                }
            }
        }

        protected virtual void OnAnimatorStateChanged(AnimatorStateInfo fromState, AnimatorStateInfo toState)
        {
            if (this.onAnimatorStateChanged != null)
            {
                this.onAnimatorStateChanged(fromState, toState);
            }
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            this.CheckCollisionHandling(collision);
        }

        protected virtual void OnCollisionStay(Collision collision)
        {
            this.CheckCollisionHandling(collision);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.onAnimatorStateChanged = null;
            this.onUserInputControllerChanged = null;
            this.onSteerFaceDirectionSet = null;
            if ((this._instancedMaterialGroups != null) && (this._instancedMaterialGroups[0] != null))
            {
                this._instancedMaterialGroups[0].Dispose();
            }
            for (int i = 0; i < this.materialGroups.Length; i++)
            {
                this.materialGroups[i].Dispose();
            }
        }

        private void OnDrawGizmos()
        {
        }

        protected virtual void OnShaderStackChanged(Shader fromShader, int fromIx, Shader toShader, int toIx)
        {
            if (toIx == -1)
            {
                this.RecoverOriginalShaders();
            }
            else
            {
                for (int i = 0; i < this._matListForSpecailState.Count; i++)
                {
                    this._matListForSpecailState[i].material.shader = toShader;
                }
            }
        }

        protected virtual void OnSkillEffectClear(string oldID, string skillID)
        {
        }

        public virtual void OnTimeScaleChanged(float newTimeScale)
        {
            this._animator.speed = ((1f + this.GetProperty("Animator_OverallSpeedRatio")) * this.GetProperty("Animator_OverallSpeedRatioMultiplied")) * newTimeScale;
        }

        protected Vector3 PickInitPosition(LayerMask mask, Vector3 initPos, float radius)
        {
            int num = 0;
            while (CollisionDetectPattern.SphereOverlapWithEntity(initPos + new Vector3(0f, 1.1f, 0f), radius, mask, base.gameObject) && (num++ < 20))
            {
                Vector2 vector = (Vector2) (UnityEngine.Random.insideUnitCircle * 0.5f);
                initPos.x += vector.x;
                initPos.z += vector.y;
            }
            return initPos;
        }

        public void PlayState(string stateName)
        {
            this._animator.Play(stateName, 0);
        }

        public override void PopHighspeedMovement()
        {
            this._highspeedMovementCount--;
            if (this._highspeedMovementCount == 0)
            {
                this._rigidbody.collisionDetectionMode = !GlobalVars.ENABLE_CONTINUOUS_DETECT_MODE ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous;
            }
        }

        public override void PopMaterialGroup()
        {
            this._materialList = null;
            this._instancedMaterialGroups.RemoveAt(this._instancedMaterialGroups.Count - 1);
            this._instancedMaterialGroups[this._instancedMaterialGroups.Count - 1].ApplyTo(this.renderers);
        }

        public void PopShaderStackByIndex(int index)
        {
            this._shaderStack.Pop(index);
        }

        public override void PopTimeScale(int stackIx)
        {
            if (this._timeScaleStack.IsOccupied(stackIx))
            {
                this._timeScaleStack.Pop(stackIx);
            }
        }

        protected virtual void PostInit()
        {
            this.SetupGraphic();
            this.InitMaterialsForSpecialState();
        }

        public virtual void Preload()
        {
            base.transform.position = InLevelData.CREATE_INIT_POS;
            base.enabled = false;
            base.gameObject.SetActive(false);
        }

        private void ProcessAnimatorPattern()
        {
            if (this._curProcessItem.patterns != null)
            {
                float lastTime = this._curProcessItem.lastTime;
                float normalizedTime = this._processingStateInfo.normalizedTime;
                normalizedTime -= (int) normalizedTime;
                if (normalizedTime >= lastTime)
                {
                    this.ProcessTimeRange(this._curProcessItem.patterns, lastTime, normalizedTime, 0);
                }
                else
                {
                    this.ProcessTimeRange(this._curProcessItem.patterns, lastTime, 1f, 0);
                    this.ProcessTimeRange(this._curProcessItem.patterns, 0f, normalizedTime, 0);
                }
                this._curProcessItem.lastTime = normalizedTime;
            }
            if (this._prevProcessItem.patterns != null)
            {
                if (this._wasInTransition)
                {
                    this.ProcessTimeRange(this._prevProcessItem.patterns, this._prevProcessItem.lastTime, this._prevProcessingStateInfo.normalizedTime, 1);
                    this._prevProcessItem.lastTime = this._prevProcessingStateInfo.normalizedTime;
                }
                else
                {
                    this._prevProcessItem.patterns = null;
                    this._prevProcessItem.lastTime = 0f;
                }
            }
        }

        private void ProcessAnimatorStates()
        {
            AnimatorStateInfo nextAnimatorStateInfo;
            bool flag = this._animator.IsInTransition(0);
            if (flag)
            {
                nextAnimatorStateInfo = this._animator.GetNextAnimatorStateInfo(0);
                this._prevProcessingStateInfo = this._animator.GetCurrentAnimatorStateInfo(0);
            }
            else
            {
                nextAnimatorStateInfo = this._animator.GetCurrentAnimatorStateInfo(0);
            }
            if (this._sameFrameExitCount > 1)
            {
                for (int i = 1; i < this._sameFrameExitCount; i++)
                {
                    if (this._processingStateInfo.fullPathHash != this._sameFrameExitStates[i].fullPathHash)
                    {
                        this.OnAnimatorStateChanged(this._processingStateInfo, this._sameFrameExitStates[i]);
                        this.ClearPreviousParamBindOnTransitionEnd(this._processingStateInfo, this._sameFrameExitStates[i]);
                        this._processingStateInfo = this._sameFrameExitStates[i];
                    }
                }
            }
            this._sameFrameExitCount = 0;
            if (this._wasInTransition && !flag)
            {
                this.ClearPreviousParamBindOnTransitionEnd(this._prevProcessingStateInfo, nextAnimatorStateInfo);
            }
            else if (!flag && (this._processingStateInfo.fullPathHash != nextAnimatorStateInfo.fullPathHash))
            {
                this.ClearPreviousParamBindOnTransitionEnd(this._processingStateInfo, nextAnimatorStateInfo);
            }
            if (this.animatorConfig.StateToParamBindMap.ContainsKey(nextAnimatorStateInfo.shortNameHash))
            {
                this.SetStateToParamWithNormalizedTime(nextAnimatorStateInfo);
            }
            if (nextAnimatorStateInfo.fullPathHash != this._processingStateInfo.fullPathHash)
            {
                this.OnAnimatorStateChanged(this._processingStateInfo, nextAnimatorStateInfo);
                if (this._animator.IsInTransition(0))
                {
                    this._prevProcessItem.patterns = this._curProcessItem.patterns;
                    this._prevProcessItem.lastTime = this._curProcessItem.lastTime;
                }
                else
                {
                    this._prevProcessItem.patterns = null;
                    this._prevProcessItem.lastTime = 0f;
                }
                if (this._curProcessItem.patterns != null)
                {
                    this.ProcessTimeRange(this._curProcessItem.patterns, this._curProcessItem.lastTime, 1f, 2);
                }
                this._curProcessItem.patterns = null;
                this._curProcessItem.lastTime = 0f;
                if (this._activeAnimatorEventPatterns.ContainsKey(nextAnimatorStateInfo.shortNameHash))
                {
                    this._curProcessItem.patterns = new AnimatorEventPattern[this._activeAnimatorEventPatterns[nextAnimatorStateInfo.shortNameHash].Count];
                    this._curProcessItem.lastTime = nextAnimatorStateInfo.normalizedTime;
                    for (int j = 0; j < this._curProcessItem.patterns.Length; j++)
                    {
                        this._curProcessItem.patterns[j] = AnimatorEventData.GetAnimatorEventPattern(this._activeAnimatorEventPatterns[nextAnimatorStateInfo.shortNameHash][j]);
                    }
                    this.ProcessTimeRange(this._curProcessItem.patterns, 0f, this._curProcessItem.lastTime, 1);
                }
            }
            else if (!this._wasInTransition && flag)
            {
                this.OnAnimatorStateChanged(this._processingStateInfo, nextAnimatorStateInfo);
                if (this._curProcessItem.patterns != null)
                {
                    this.ProcessTimeRange(this._curProcessItem.patterns, this._curProcessItem.lastTime, 1f, 2);
                }
                this._prevProcessItem.patterns = this._curProcessItem.patterns;
                this._prevProcessItem.lastTime = this._curProcessItem.lastTime;
                this._curProcessItem.lastTime = nextAnimatorStateInfo.normalizedTime;
                if (this._curProcessItem.patterns != null)
                {
                    this.ProcessTimeRange(this._curProcessItem.patterns, 0f, this._curProcessItem.lastTime, 1);
                }
            }
            this._wasInTransition = flag;
            this._processingStateInfo = nextAnimatorStateInfo;
            this.ProcessAnimatorPattern();
        }

        private void ProcessTimeRange(AnimatorEventPattern[] patterns, float from, float to, int mode = 0)
        {
            for (int i = 0; i < patterns.Length; i++)
            {
                if (patterns[i] != null)
                {
                    this.ProcessTimeRange(patterns[i], from, to, mode);
                }
            }
        }

        private void ProcessTimeRange(AnimatorEventPattern pattern, float from, float to, int mode = 0)
        {
            float num = Mathf.Clamp01(from);
            float num2 = Mathf.Clamp01(to);
            if (num < num2)
            {
                int index = 0;
                int length = pattern.animatorEvents.Length;
                while (index < length)
                {
                    AnimatorEvent event2 = pattern.animatorEvents[index];
                    if ((event2.normalizedTime >= num) && (event2.normalizedTime < num2))
                    {
                        if (mode == 0)
                        {
                            event2.HandleAnimatorEvent(this);
                        }
                        else if (mode == 1)
                        {
                            if (event2.forceTrigger && !event2.forceTriggerOnTransitionOut)
                            {
                                event2.HandleAnimatorEvent(this);
                            }
                        }
                        else if ((mode == 2) && event2.forceTriggerOnTransitionOut)
                        {
                            event2.HandleAnimatorEvent(this);
                        }
                    }
                    index++;
                }
            }
        }

        protected virtual int PushEffectShaderData(Shader shader)
        {
            return this._shaderStack.Push(shader, false);
        }

        public override void PushHighspeedMovement()
        {
            this._highspeedMovementCount++;
            if (this._highspeedMovementCount == 1)
            {
                this._rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
        }

        public override void PushMaterialGroup(string targetGroupname)
        {
            this._materialList = null;
            for (int i = 0; i < this.materialGroups.Length; i++)
            {
                if (this.materialGroups[i].groupName == targetGroupname)
                {
                    this._instancedMaterialGroups.Add(this.materialGroups[i].GetInstancedMaterialGroup());
                    this._instancedMaterialGroups[this._instancedMaterialGroups.Count - 1].ApplyTo(this.renderers);
                    return;
                }
            }
        }

        public override void PushTimeScale(float timescale, int stackIx)
        {
            this._timeScaleStack.Push(stackIx, timescale, false);
        }

        public void RebindAttachPoint(string name, string other)
        {
            Transform attachPoint = this.GetAttachPoint(other);
            for (int i = 0; i < this.attachPoints.Length; i++)
            {
                if (this.attachPoints[i].name == name)
                {
                    this.attachPoints[i].pointTransform = attachPoint;
                    return;
                }
            }
        }

        protected virtual void RecoverOriginalShaders()
        {
            for (int i = 0; i < this._matListForSpecailState.Count; i++)
            {
                this._matListForSpecailState[i].material.shader = this._matListForSpecailState[i].originalShader;
            }
        }

        public void RemoveAnimatorEventPattern(int stateHash, string eventPattern)
        {
            this.DetachAnimatorEventPattern(stateHash, eventPattern);
        }

        public override void RemoveAnimEventPredicate(string predicate)
        {
            this._animEventPredicates.Remove(predicate);
        }

        protected void ResetAllTriggers()
        {
            AnimatorControllerParameter[] parameters = this._animator.parameters;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].type == AnimatorControllerParameterType.Trigger)
                {
                    this._animator.ResetTrigger(parameters[i].nameHash);
                }
            }
        }

        private void ResetAnimatedPhysics()
        {
            this.ResetRigidbodyRotation();
            Vector3 position = this._rigidbody.position;
            position.y = this._recoverPosY;
            this._rigidbody.position = position;
        }

        public void ResetRigidbodyRotation()
        {
            Vector3 eulerAngles = this._rigidbody.rotation.eulerAngles;
            if ((eulerAngles.x != 0f) || (eulerAngles.z != 0f))
            {
                eulerAngles.x = 0f;
                eulerAngles.z = 0f;
                this._rigidbody.rotation = Quaternion.Euler(eulerAngles);
            }
        }

        [DebuggerHidden]
        private IEnumerator ResetRotationIter()
        {
            return new <ResetRotationIter>c__Iterator1B { <>f__this = this };
        }

        public override void ResetTrigger(string name)
        {
            if (base.gameObject.activeInHierarchy)
            {
                this._animator.ResetTrigger(name);
            }
        }

        public override void SetAdditiveVelocity(Vector3 velocity)
        {
            this._additiveVelocityDic.Clear();
            if (velocity != Vector3.zero)
            {
                this._additiveVelocityDic.Add(this._additiveVelocityDic.Count, velocity);
            }
        }

        public override void SetAdditiveVelocityOfIndex(Vector3 velocity, int index)
        {
            if (this.HasAdditiveVelocityOfIndex(index))
            {
                if (velocity == Vector3.zero)
                {
                    this._additiveVelocityDic.Remove(index);
                }
                else
                {
                    this._additiveVelocityDic[index] = velocity;
                }
            }
        }

        public void SetCheckForCollision(LayerMask collisionMask, CollisionCallback callback)
        {
            this._collisionCallback = callback;
            this._collisionLayerMask = collisionMask;
            this._waitingForCollision = true;
        }

        public override void SetHasAdditiveVelocity(bool value)
        {
            bool hasAdditiveVelocity = this.hasAdditiveVelocity;
            this._hasAdditiveVelocityCount += !value ? -1 : 1;
            this._hasAdditiveVelocityCount = (this._hasAdditiveVelocityCount >= 0) ? this._hasAdditiveVelocityCount : 0;
            if ((hasAdditiveVelocity != value) && (base.onHasAdditiveVelocityChanged != null))
            {
                base.onHasAdditiveVelocityChanged(value);
            }
        }

        public void SetLocomotionBool(int stateHash, bool value, bool isUserInput = false)
        {
            if ((isUserInput && (this.onUserInputControllerChanged != null)) && (value != this._animator.GetBool(stateHash)))
            {
                AnimatorParameterEntry entry = new AnimatorParameterEntry {
                    stateHash = stateHash,
                    type = AnimatorControllerParameterType.Bool,
                    boolValue = value
                };
                this.onUserInputControllerChanged(entry);
            }
            this._animator.SetBool(stateHash, value);
        }

        public void SetLocomotionBool(string stateName, bool value, bool isUserInput = false)
        {
            this.SetLocomotionBool(Animator.StringToHash(stateName), value, isUserInput);
        }

        public void SetLocomotionFloat(int stateHash, float value, bool isUserInput = false)
        {
            if ((isUserInput && (this.onUserInputControllerChanged != null)) && (value != this._animator.GetFloat(stateHash)))
            {
                AnimatorParameterEntry entry = new AnimatorParameterEntry {
                    stateHash = stateHash,
                    type = AnimatorControllerParameterType.Float,
                    floatValue = value
                };
                this.onUserInputControllerChanged(entry);
            }
            this._animator.SetFloat(stateHash, value);
        }

        public void SetLocomotionFloat(string stateName, float value, bool isUserInput = false)
        {
            this.SetLocomotionFloat(Animator.StringToHash(stateName), value, isUserInput);
        }

        public void SetLocomotionInteger(int stateHash, int value, bool isUserInput = false)
        {
            if ((isUserInput && (this.onUserInputControllerChanged != null)) && (value != this._animator.GetInteger(stateHash)))
            {
                AnimatorParameterEntry entry = new AnimatorParameterEntry {
                    stateHash = stateHash,
                    type = AnimatorControllerParameterType.Bool,
                    intValue = value
                };
                this.onUserInputControllerChanged(entry);
            }
            this._animator.SetInteger(stateHash, value);
        }

        public void SetLocomotionInteger(string stateName, int value, bool isUserInput = false)
        {
            this.SetLocomotionInteger(Animator.StringToHash(stateName), value, isUserInput);
        }

        public void SetLocomotionRandom(int n)
        {
            int num = UnityEngine.Random.Range(0, n);
            this._animator.SetInteger("Random", num);
        }

        protected void SetMass(float mass)
        {
            this._curMass = mass;
            this.SyncMass();
        }

        public void SetMonsterMaterialFadeEnabled(bool enable)
        {
            SkinnedMeshRenderer[] componentsInChildren = base.GetComponentsInChildren<SkinnedMeshRenderer>();
            if ((componentsInChildren != null) && (componentsInChildren.Length > 0))
            {
                for (int i = 0; i < componentsInChildren.Length; i++)
                {
                    if (enable)
                    {
                        foreach (Material material in componentsInChildren[i].sharedMaterials)
                        {
                            int instanceID = material.GetInstanceID();
                            if (this._fadeMaterialDic.ContainsKey(instanceID) && this._fadeMaterialDic[instanceID].recorded)
                            {
                                material.SetFloat("_FadeDistance", this._fadeMaterialDic[instanceID].fadeDistance);
                                material.SetFloat("_FadeOffset", this._fadeMaterialDic[instanceID].fadeOffset);
                            }
                        }
                    }
                    else
                    {
                        foreach (Material material2 in componentsInChildren[i].sharedMaterials)
                        {
                            if (!this._fadeMaterialDic.ContainsKey(material2.GetInstanceID()))
                            {
                                float @float = material2.GetFloat("_FadeDistance");
                                float offset = material2.GetFloat("_FadeOffset");
                                this._fadeMaterialDic.Add(material2.GetInstanceID(), new MaterialFadeSetting(@float, offset));
                                material2.SetFloat("_FadeDistance", 0f);
                                material2.SetFloat("_FadeOffset", 0f);
                            }
                        }
                    }
                }
                if (enable)
                {
                    this._fadeMaterialDic.Clear();
                }
            }
        }

        public override void SetNeedOverrideVelocity(bool needOverrideVelocity)
        {
            this._needOverrideVelocity = needOverrideVelocity;
        }

        public virtual void SetOverrideSteerFaceDirectionFrame(Vector3 overrideSteer)
        {
            this._needOverrideSteer = true;
            this._overrideSteer = overrideSteer;
        }

        public override void SetOverrideVelocity(Vector3 velocity)
        {
            this._overrideVelocity = velocity;
        }

        public virtual void SetPause(bool pause)
        {
        }

        public override void SetShaderData(E_ShaderData dataType, bool bEnable)
        {
            MonoBuffShader_SpecialTransition buffShaderData = Singleton<ShaderDataManager>.Instance.GetBuffShaderData<MonoBuffShader_SpecialTransition>(dataType);
            this._shaderTransitionPlugin.StartTransition(this._matListForSpecailState, buffShaderData, bEnable);
        }

        public override void SetShaderDataLerp(E_ShaderData dataType, bool bEnable, float enableDuration = -1f, float disableDuration = -1f, bool bUseNewTexture = false)
        {
            MonoBuffShader_Lerp buffShaderData = Singleton<ShaderDataManager>.Instance.GetBuffShaderData<MonoBuffShader_Lerp>(dataType);
            int shaderIx = -1;
            if (bEnable && !string.IsNullOrEmpty(buffShaderData.NewShaderName))
            {
                this.TryInitShaderStack();
                Shader shader = Shader.Find(buffShaderData.NewShaderName);
                shaderIx = this.PushEffectShaderData(shader);
            }
            if (buffShaderData.Keyword != string.Empty)
            {
                for (int i = 0; i < this._matListForSpecailState.Count; i++)
                {
                    Material material = this._matListForSpecailState[i].material;
                    if (buffShaderData.Keyword == "DISTORTION")
                    {
                        material.SetOverrideTag("Distortion", "Character");
                    }
                    else
                    {
                        material.EnableKeyword(buffShaderData.Keyword);
                    }
                }
            }
            if (bUseNewTexture)
            {
                for (int j = 0; j < this._matListForSpecailState.Count; j++)
                {
                    this._matListForSpecailState[j].material.SetTexture(buffShaderData.TexturePropertyName, buffShaderData.NewTexture);
                }
            }
            if (enableDuration > 0f)
            {
                buffShaderData.EnableDuration = enableDuration;
            }
            if (disableDuration > 0f)
            {
                buffShaderData.DisableDuration = disableDuration;
            }
            this._shaderLerpPlugin.StartLerp(dataType, this._matListForSpecailState, buffShaderData, bEnable, shaderIx);
        }

        private void SetStateToParamWithNormalizedTime(AnimatorStateInfo curState)
        {
            AnimatorStateToParameterConfig config = this.animatorConfig.StateToParamBindMap[curState.shortNameHash];
            float num = curState.normalizedTime - Mathf.Floor(curState.normalizedTime);
            if ((num >= config.NormalizedTimeStart) && (num < config.NormalizedTimeStop))
            {
                if (!this.GetLocomotionBool(config.ParameterID))
                {
                    this.SetLocomotionBool(config.ParameterID, true, false);
                    if (config.ParameterIDSub != null)
                    {
                        this.SetLocomotionBool(config.ParameterIDSub, true, false);
                    }
                }
            }
            else if (this.GetLocomotionBool(config.ParameterID))
            {
                this.SetLocomotionBool(config.ParameterID, false, false);
                if (config.ParameterIDSub != null)
                {
                    this.SetLocomotionBool(config.ParameterIDSub, false, false);
                }
            }
        }

        public override void SetTimeScale(float timescale, int stackIx)
        {
            this._timeScaleStack.Set(stackIx, timescale, false);
        }

        public override void SetTrigger(string name)
        {
            if ((base.gameObject.activeInHierarchy && !this._maskAllTriggers) && ((this._maskedTriggers == null) || !this._maskedTriggers.Contains(name)))
            {
                this._animator.SetTrigger(name);
            }
        }

        public void SetUniformScale(float uniformScale)
        {
            this._uniformScale = uniformScale;
            this._transform.localScale = (Vector3) (this._transform.localScale * uniformScale);
        }

        protected virtual void SetupGraphic()
        {
            this._instancedMaterialGroups = new List<MaterialGroup>();
            this._instancedMaterialGroups.Add(new MaterialGroup("DEFAULT_MATERIAL_GROUP", this.renderers).GetInstancedMaterialGroup());
            this._instancedMaterialGroups[0].ApplyTo(this.renderers);
        }

        public abstract void SetUseLocalController(bool enabled);
        private void Start()
        {
        }

        protected void StartAnimatePhysics()
        {
            this._recoverPosY = this._rigidbody.position.y;
            this._animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
            this._animatePhysicsStarted = true;
            this._rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        public void StartFadeAnimatorLayerWeight(int layer, float weight, float duration)
        {
            LayerFader fader = null;
            for (int i = 0; i < this._layerFaders.Count; i++)
            {
                if ((this._layerFaders[i] != null) && (this._layerFaders[i].layer == layer))
                {
                    fader = this._layerFaders[i];
                    fader.fromWeight = this._animator.GetLayerWeight(layer);
                    fader.toWeight = weight;
                    fader.duration = duration;
                    fader.t = 0f;
                    fader.isDone = false;
                    break;
                }
            }
            if (fader == null)
            {
                fader = new LayerFader {
                    layer = layer,
                    fromWeight = this._animator.GetLayerWeight(layer),
                    toWeight = weight,
                    duration = duration
                };
                int num2 = this._layerFaders.SeekAddPosition<LayerFader>();
                this._layerFaders[num2] = fader;
            }
        }

        protected void StartWaitTransitionState(int stateIx)
        {
            this._transitionStates[stateIx] = !this._animator.IsInTransition(0) ? WaitTransitionState.WaitForTransition : WaitTransitionState.DuringTransition;
        }

        public override void SteerFaceDirectionTo(Vector3 forward)
        {
            this._nextFaceDir = forward;
            this._nextFaceDir.y = 0f;
            this._needSteer = true;
            if (this.onSteerFaceDirectionSet != null)
            {
                this.onSteerFaceDirectionSet(forward);
            }
        }

        [AnimationCallback]
        public void StopAllEffects()
        {
            Singleton<EffectManager>.Instance.ClearEffectsByOwner(base._runtimeID);
        }

        [AnimationCallback]
        public void StopAllEffectsImmediately()
        {
            Singleton<EffectManager>.Instance.ClearEffectsByOwnerImmediately(base._runtimeID);
        }

        protected void StopAnimatePhysics()
        {
            this._animator.updateMode = AnimatorUpdateMode.Normal;
            this._rigidbody.collisionDetectionMode = !GlobalVars.ENABLE_CONTINUOUS_DETECT_MODE ? CollisionDetectionMode.Discrete : CollisionDetectionMode.Continuous;
            if (base.gameObject.activeInHierarchy)
            {
                base.StartCoroutine(this.ResetRotationIter());
            }
            else
            {
                this.ResetAnimatedPhysics();
            }
            this._animatePhysicsStarted = false;
        }

        public virtual void StopAudioPattern(string name)
        {
        }

        private void SyncAnimatorBools()
        {
            if (base.gameObject.activeInHierarchy)
            {
                foreach (KeyValuePair<string, bool> pair in base._animatorBoolParams)
                {
                    this._animator.SetBool(pair.Key, pair.Value);
                }
            }
        }

        private void SyncAnimatorInts()
        {
            if (base.gameObject.activeInHierarchy)
            {
                foreach (KeyValuePair<string, int> pair in base._animatorIntParams)
                {
                    this._animator.SetInteger(pair.Key, pair.Value);
                }
            }
        }

        private void SyncAnimatorSpeed()
        {
            if (base.gameObject.activeInHierarchy)
            {
                this._animator.speed = ((1f + this.GetProperty("Animator_OverallSpeedRatio")) * this.GetProperty("Animator_OverallSpeedRatioMultiplied")) * this.TimeScale;
            }
        }

        public void SyncAnimatorState(int stateHash, float normalizedTime)
        {
            if (normalizedTime == 0f)
            {
                this._animator.CrossFade(stateHash, 0.1f, 0, 0.1f);
            }
            else
            {
                this._animator.Play(stateHash, 0, normalizedTime);
            }
        }

        private void SyncFadeLayers()
        {
            if (base.gameObject.activeInHierarchy)
            {
                for (int i = 0; i < this._layerFaders.Count; i++)
                {
                    if ((this._layerFaders[i] != null) && this._layerFaders[i].isDone)
                    {
                        this._animator.SetLayerWeight(this._layerFaders[i].layer, this._layerFaders[i].toWeight);
                    }
                }
            }
        }

        private void SyncMass()
        {
            this._rigidbody.mass = Mathf.Min((float) 200f, (float) (this._curMass * (1f + this.GetProperty("Entity_MassRatio"))));
        }

        public void SyncPosition(Vector3 targetPosition)
        {
            this._rigidbody.MovePosition(targetPosition);
        }

        [AnimationCallback]
        private void TimeSlowTriggerForce(float time)
        {
            Singleton<LevelManager>.Instance.levelActor.TimeSlow(time);
        }

        [AnimationCallback]
        public virtual void TriggerAudioPattern(string patternName)
        {
            char[] separator = new char[] { ':' };
            string[] strArray = patternName.Split(separator);
            if (strArray.Length == 2)
            {
                if (!this._animEventPredicates.Contains(strArray[1]))
                {
                    return;
                }
                patternName = strArray[0];
            }
            char[] chArray2 = new char[] { '#' };
            strArray = patternName.Split(chArray2);
            patternName = strArray[0];
            string item = null;
            if (strArray.Length > 1)
            {
                item = strArray[1];
            }
            if (!patternName.StartsWith("VO_L") || (base.GetRuntimeID() == Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID()))
            {
                Singleton<WwiseAudioManager>.Instance.Post(patternName, base.gameObject, null, null);
                if (this._waitingAudioEvent.Contains(patternName))
                {
                    this._waitingAudioEvent.Remove(patternName);
                }
                if ((item != null) && !this._waitingAudioEvent.Contains(item))
                {
                    this._waitingAudioEvent.Add(item);
                }
            }
        }

        [AnimationCallback]
        private void TriggerCameraShake(string arg)
        {
            char[] separator = new char[] { ',' };
            string[] strArray = arg.Split(separator);
            float range = float.Parse(strArray[0].Trim());
            float time = float.Parse(strArray[1].Trim());
            Singleton<CameraManager>.Instance.GetMainCamera().ActShakeEffect(time, range, 0f, 2, false, false);
        }

        [AnimationCallback]
        public void TriggerEffectPattern(string patternName)
        {
            char[] separator = new char[] { ':' };
            string[] strArray = patternName.Split(separator);
            if (strArray.Length == 2)
            {
                this.TriggerEffectPattern(strArray[0], strArray[1], null);
            }
            else
            {
                this.TriggerEffectPattern(patternName, null, null);
            }
        }

        public virtual void TriggerEffectPattern(string patternName, string predicate1, string predicate2)
        {
            if (((predicate1 == null) || this._animEventPredicates.Contains(predicate1)) && ((predicate2 == null) || this._animEventPredicates.Contains(predicate2)))
            {
                List<MonoEffect> list = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue(patternName, this.XZPosition, this.FaceDirection, (Vector3) (Vector3.one * this._uniformScale), this);
                int num = 0;
                int count = list.Count;
                while (num < count)
                {
                    MonoEffect effect = list[num];
                    effect.belongSkillName = this.CurrentSkillID;
                    num++;
                }
            }
        }

        [AnimationCallback]
        private void TriggerExposure(float time)
        {
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            if (mainCamera != null)
            {
                mainCamera.ActExposureEffect(time, 0f, time, 10f);
            }
        }

        protected void TriggerTint(string renderDataName, float duration, float transitDuration)
        {
            Singleton<StageManager>.Instance.GetPerpStage().TriggerTint(renderDataName, duration, transitDuration);
        }

        protected virtual void TryInitShaderStack()
        {
            if (this._shaderStack == null)
            {
                this._shaderStack = new FixedStack<Shader>(5, new Action<Shader, int, Shader, int>(this.OnShaderStackChanged));
            }
        }

        public override void UnmaskAnimEvent(string animEventID)
        {
            this._maskedAnimEvents.Remove(animEventID);
        }

        public override void UnmaskTrigger(string triggerID)
        {
            this._maskedTriggers.Remove(triggerID);
        }

        protected virtual void Update()
        {
            this.UpdatePlugins();
            this._timeScale = (Singleton<LevelManager>.Instance.levelEntity.TimeScale * this._timeScaleStack.value) * (1f + this.GetProperty("Entity_TimeScaleDelta"));
            if (this._lastTimeScale != this.TimeScale)
            {
                this.OnTimeScaleChanged(this.TimeScale);
            }
            this._lastTimeScale = this.TimeScale;
            bool flag = this._animator.IsInTransition(0);
            for (int i = 0; i < this._transitionStates.Count; i++)
            {
                if (((WaitTransitionState) this._transitionStates[i]) == WaitTransitionState.WaitForTransition)
                {
                    if (flag)
                    {
                        this._transitionStates[i] = WaitTransitionState.DuringTransition;
                    }
                }
                else if ((((WaitTransitionState) this._transitionStates[i]) == WaitTransitionState.DuringTransition) && !flag)
                {
                    this._transitionStates[i] = WaitTransitionState.TransitionDone;
                }
            }
            this.UpdateLayerFading();
            this.ApplyLightProb();
            this._instancedMaterialGroups[0].ApplyColorModifiers();
        }

        private void UpdateLayerFading()
        {
            float num = Time.deltaTime * this.TimeScale;
            for (int i = 0; i < this._layerFaders.Count; i++)
            {
                LayerFader fader = this._layerFaders[i];
                if ((fader != null) && !fader.isDone)
                {
                    fader.t += num;
                    if (fader.t > fader.duration)
                    {
                        this._animator.SetLayerWeight(fader.layer, fader.toWeight);
                        this._layerFaders[i].isDone = true;
                    }
                    else
                    {
                        this._animator.SetLayerWeight(fader.layer, Mathf.Lerp(fader.fromWeight, fader.toWeight, fader.t / fader.duration));
                    }
                }
            }
        }

        protected virtual void UpdatePlugins()
        {
            this._frameHaltPlugin.Core();
            this._shaderTransitionPlugin.Core();
            this._shaderLerpPlugin.Core();
        }

        public Vector3 FaceDirection
        {
            get
            {
                return this._transform.forward;
            }
        }

        public bool hasAdditiveVelocity
        {
            get
            {
                if (this._muteAdditiveVelocity)
                {
                    return false;
                }
                return (this._hasAdditiveVelocityCount > 0);
            }
        }

        public bool MuteAdditiveVelocity
        {
            get
            {
                return this._muteAdditiveVelocity;
            }
            set
            {
                this._muteAdditiveVelocity = value;
            }
        }

        public Vector3 RootNodePosition
        {
            get
            {
                return this.RootNode.position;
            }
        }

        public override float TimeScale
        {
            get
            {
                return this._timeScale;
            }
        }

        public FixedStack<float> timeScaleStack
        {
            get
            {
                return this._timeScaleStack;
            }
        }

        public override Vector3 XZPosition
        {
            get
            {
                if (this._transform != null)
                {
                    return new Vector3(this._transform.position.x, 0f, this._transform.position.z);
                }
                if (base.transform == null)
                {
                    return Vector3.zero;
                }
                return new Vector3(base.transform.position.x, 0f, base.transform.position.z);
            }
        }

        [CompilerGenerated]
        private sealed class <ResetRotationIter>c__Iterator1B : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal BaseMonoAnimatorEntity <>f__this;

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
                        this.$current = null;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.ResetAnimatedPhysics();
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

        private class AnimatorEventPatternProcessItem
        {
            public float lastTime;
            public AnimatorEventPattern[] patterns;
        }

        public delegate void CollisionCallback(int layer, Vector3 forward);

        private class ColorAdjuster
        {
            public Material material;
            public List<PropIdOrigColorPair> propIdOrigColorList;

            public ColorAdjuster(Material mat, string[] propNames)
            {
                this.material = mat;
                this.propIdOrigColorList = new List<PropIdOrigColorPair>();
                foreach (string str in propNames)
                {
                    if (mat.HasProperty(str))
                    {
                        PropIdOrigColorPair item = new PropIdOrigColorPair {
                            propId = Shader.PropertyToID(str),
                            origColor = mat.GetColor(str)
                        };
                        this.propIdOrigColorList.Add(item);
                    }
                }
            }

            public void Apply(Color adjustColor)
            {
                for (int i = 0; i < this.propIdOrigColorList.Count; i++)
                {
                    PropIdOrigColorPair pair = this.propIdOrigColorList[i];
                    if (this.material != null)
                    {
                        this.material.SetColor(pair.propId, pair.origColor * adjustColor);
                    }
                }
            }

            public bool IsEmpty()
            {
                return (this.propIdOrigColorList.Count == 0);
            }

            public class PropIdOrigColorPair
            {
                public Color origColor;
                public int propId;
            }
        }

        private class LayerFader
        {
            public float duration;
            public float fromWeight;
            public bool isDone;
            public int layer;
            public float t;
            public float toWeight;
        }

        public class MaterialFadeSetting
        {
            public float fadeDistance;
            public float fadeOffset;
            public bool recorded;

            public MaterialFadeSetting(float distance, float offset)
            {
                this.fadeDistance = distance;
                this.fadeOffset = offset;
                this.recorded = true;
            }
        }

        public class SpecialStateMaterialData
        {
            public MaterialColorModifier.Multiplier colorMultiplier;
            public Material material;
            public Shader originalShader;
        }

        protected enum WaitTransitionState
        {
            Idle,
            WaitForTransition,
            DuringTransition,
            TransitionDone
        }
    }
}

