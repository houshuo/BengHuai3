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

    public abstract class BaseMonoAvatar : BaseMonoAnimatorEntity, IAIEntity, IFadeOff, IFrameHaltable, IRetreatable, IAttacker
    {
        private string _activeCameraAnimName;
        private Vector3 _activeEnterSteerClampStart;
        private SkillEnterSteerOption _activeEnterSteerOption;
        private SkillEnterSteerState _activeEnterSteerState;
        protected BaseAvatarAIController _aiController;
        private List<int> _attachedEffects = new List<int>();
        private int _attackMoveRatioIx;
        private AttackSpeedState _attackSpeedState;
        private int _attackSpeedTimeScaleIx;
        private BaseMonoEntity _attackTarget;
        protected Action<BaseMonoAvatar> _attackTargetSelectAction;
        private float _baseMassRatio = 1f;
        private float _clearAttackTargetTimer;
        private AvatarControlData _controlData;
        private string _currentSkillID;
        private bool _delayedSwapOutTriggered;
        private DynamicBone[] _dynamicBones;
        private int _equipedWeaponID = -1;
        private FaceAnimation _faceAnimation;
        private bool _hasGotParameterHodeMode;
        private bool _hasUpdatedControlThisFrame;
        protected BaseAvatarInputController _inputController;
        private bool _isAlive;
        private bool _isDeadAlready;
        private bool _isFromAttackOrSkill;
        private bool _isHodeMode;
        private bool _isLockDirection;
        private bool _isShadowColorAdjusted;
        private bool _isToBeRemoved;
        private AvatarControlData _lastFrameControlData;
        private float _moveSpeedRatio;
        private bool _muteAnimRetarget;
        private int _muteControlCount;
        private int _muteLockAttackTargetIx;
        private int _muteSteerIx;
        private int _noCollisionCount;
        private AtlasMatInfoProvider _providerL;
        private AtlasMatInfoProvider _providerM;
        private AtlasMatInfoProvider _providerR;
        private List<ShadowColorAdjuster> _shadowColorAdjusterList;
        private float _skillMassRatio = 1f;
        private float _steerLerpRatio = 1f;
        private List<BaseMonoEntity> _subAttackTargetList;
        private Coroutine _waitMoveSoundCoroutine;
        private const float AVATAR_DASH_MASS_RATIO = 1f;
        private const float AVATAR_ONMOVE_MASS_RATIO = 1f;
        private const float AVATAR_STABLE_MASS_RATIO = 100f;
        protected const string COMBAT_TO_STANDBY_CD_PARAM = "CombatToStandByCD";
        public ConfigAvatar config;
        protected const string DAMAGE_RATIO_PARAM = "DamageRatio";
        protected const string DIE_PARAM = "IsDead";
        public Collider hitbox;
        public Renderer leftEyeRenderer;
        public Renderer mouthRenderer;
        protected const string MOVE_SPEED_PARAM = "MoveSpeed";
        [Header("During these skillIDs dynamic bone animation ")]
        public string[] muteDynamicBonesSkillIDs;
        public Action<BaseMonoEntity> onAttackTargetChanged;
        public Action<BaseMonoAvatar> onDie;
        public Action<bool> onLockDirectionChanged;
        protected const string ORDER_MOVE_PARAM = "IsMove";
        public Renderer rightEyeRenderer;
        protected const string RUN_STEP_ON_RIGHT_PARAM = "RunStepOnRight";
        protected const string TRIGGER_APPEAR = "TriggerAppear";
        protected const string TRIGGER_ATTACK_PARAM = "TriggerAttack";
        protected const string TRIGGER_HIT_PARAM = "TriggerHit";
        protected const string TRIGGER_HOLD_ATTACK_PARAM = "TriggerHoldAttack";
        protected const string TRIGGER_KNOCK_DOWN_PARAM = "TriggerKnockDown";
        protected const string TRIGGER_SKILL_PARAM = "TriggerSkill_";
        protected const string TRIGGER_SWITCH_IN = "TriggerSwitchIn";
        protected const string TRIGGER_SWITCH_OUT = "TriggerSwitchOut";
        protected const string TRIGGER_WEAPON_PARAM = "TriggerWeapon";

        public event AnimatedHitBoxCreatedHandler onAnimatedHitBoxCreatedCallBack;

        protected BaseMonoAvatar()
        {
        }

        public void AddTargetToSubAttackList(BaseMonoEntity target)
        {
            if ((target != null) && !this._subAttackTargetList.Contains(target))
            {
                this._subAttackTargetList.Add(target);
            }
        }

        private void AdjustShadowColors()
        {
            Camera main = Camera.main;
            if (main != null)
            {
                PostFXBase component = main.GetComponent<PostFXBase>();
                if (component != null)
                {
                    float avatarShadowAdjust = component.AvatarShadowAdjust;
                    for (int i = 0; i < this._shadowColorAdjusterList.Count; i++)
                    {
                        this._shadowColorAdjusterList[i].Apply(avatarShadowAdjust);
                    }
                    this._isShadowColorAdjusted = true;
                }
            }
        }

        [AnimationCallback]
        public override void AnimEventHandler(string animEventID)
        {
            ConfigAvatarAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(this.config, animEventID);
            if (((event2 != null) && ((base._maskedAnimEvents == null) || !base._maskedAnimEvents.Contains(animEventID))) && (base._animEventPredicates.Contains(event2.Predicate) && base._animEventPredicates.Contains(event2.Predicate2)))
            {
                if (((event2.CameraShake != null) && event2.CameraShake.ShakeOnNotHit) && Singleton<AvatarManager>.Instance.IsLocalAvatar(base._runtimeID))
                {
                    AttackPattern.ActCameraShake(event2.CameraShake);
                }
                if (event2.AttackPattern != null)
                {
                    event2.AttackPattern.patternMethod(animEventID, event2.AttackPattern, this, AttackPattern.GetLayerMask(this));
                }
                if (event2.TriggerAbility != null)
                {
                    EvtAbilityStart evt = new EvtAbilityStart(base._runtimeID, null) {
                        abilityID = event2.TriggerAbility.ID,
                        abilityName = event2.TriggerAbility.Name,
                        abilityArgument = event2.TriggerAbility.Argument
                    };
                    Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
                }
                if ((event2.PhysicsProperty != null) && event2.PhysicsProperty.IsFreezeDirection)
                {
                    this.MuteLockAttackTargetTillNextState();
                }
                if ((event2.CameraAction != null) && Singleton<AvatarManager>.Instance.IsLocalAvatar(base._runtimeID))
                {
                    this.DoCameraAction(event2.CameraAction);
                }
                if (((event2.TimeSlow != null) && Singleton<AvatarManager>.Instance.IsLocalAvatar(base._runtimeID)) && (event2.TimeSlow.Force || (((this.AttackTarget != null) && this.AttackTarget.IsActive()) && (Vector3.Distance(this.XZPosition, this.AttackTarget.XZPosition) < 2f))))
                {
                    Singleton<LevelManager>.Instance.levelActor.TimeSlow(event2.TimeSlow.Duration, event2.TimeSlow.SlowRatio, null);
                }
                if (event2.TriggerEffectPattern != null)
                {
                    base.TriggerEffectPattern(event2.TriggerEffectPattern.EffectPattern);
                }
                if (event2.TriggerTintCamera != null)
                {
                    base.TriggerTint(event2.TriggerTintCamera.RenderDataName, event2.TriggerTintCamera.Duration, event2.TriggerTintCamera.TransitDuration);
                }
            }
        }

        protected override void ApplyAnimatorProperties()
        {
            base.ApplyAnimatorProperties();
            this.SyncAnimatorMoveSpeed();
        }

        public override int AttachEffect(string effectPattern)
        {
            int patternIx = base.AttachEffect(effectPattern);
            if (!base.gameObject.activeSelf)
            {
                Singleton<EffectManager>.Instance.SetIndexedEntityEffectPatternActive(patternIx, false);
            }
            int num2 = this._attachedEffects.SeekAddPosition();
            this._attachedEffects[num2] = patternIx;
            return patternIx;
        }

        private void AttachEffectOverrides()
        {
            MonoEffectOverride component = base.GetComponent<MonoEffectOverride>();
            if (component == null)
            {
                component = base.gameObject.AddComponent<MonoEffectOverride>();
            }
            component.effectPredicates.AddRange(this.config.CommonArguments.EffectPredicates);
        }

        public virtual void BeHit(int frameHalt, AttackResult.AnimatorHitEffect hitEffect, AttackResult.AnimatorHitEffectAux hitEffectAux, KillEffect killEffect, BeHitEffect beHitEffect, float aniDamageRatio, Vector3 hitForward, float retreatVelocity, uint sourceID, bool targetLockSource, bool doSteerToHitForward)
        {
            if (this.IsActive())
            {
                if ((hitEffect == AttackResult.AnimatorHitEffect.ThrowDown) || (hitEffect == AttackResult.AnimatorHitEffect.ThrowBlow))
                {
                    hitEffect = AttackResult.AnimatorHitEffect.KnockDown;
                }
                base._animator.ResetTrigger("TriggerKnockDown");
                base._animator.SetFloat("DamageRatio", aniDamageRatio);
                if (hitEffect > AttackResult.AnimatorHitEffect.Light)
                {
                    base._animator.SetTrigger("TriggerHit");
                    base.ClearSkillEffect(null);
                    this.ClearAttackTriggers();
                    base.CastWaitingAudioEvent();
                    if (hitEffect == AttackResult.AnimatorHitEffect.KnockDown)
                    {
                        base._animator.SetTrigger("TriggerKnockDown");
                    }
                    else if (hitEffect == AttackResult.AnimatorHitEffect.FaceAttacker)
                    {
                        doSteerToHitForward = true;
                    }
                    if (base.onBeHitCanceled != null)
                    {
                        base.onBeHitCanceled(this.CurrentSkillID);
                    }
                }
                if (doSteerToHitForward)
                {
                    this.SteerFaceDirectionTo(-hitForward);
                    this.MuteSteerTillNextState();
                }
                if (targetLockSource)
                {
                    BaseMonoEntity newTarget = Singleton<EventManager>.Instance.GetEntity(sourceID);
                    if (((newTarget != null) && newTarget.IsActive()) && (newTarget is BaseMonoMonster))
                    {
                        this.SetAttackTarget(newTarget);
                        this.ClearAttackTargetTimed(0.5f);
                    }
                }
                this.FrameHalt(frameHalt);
            }
        }

        private Vector3 CalculateSteerTargetForwardWithOption(Vector3 targetForward, Vector3 clampBaseForward, SkillEnterSteerOption option)
        {
            Vector3 vector = (Vector3) (Quaternion.AngleAxis(Mathf.Clamp(Miscs.AngleFromToIgnoreY(clampBaseForward, targetForward), -option.MaxSteeringAngle, option.MaxSteeringAngle), Vector3.up) * clampBaseForward);
            return vector.normalized;
        }

        private bool CanUseSkill(string skillName)
        {
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(base.GetRuntimeID());
            if (actor == null)
            {
                return false;
            }
            return actor.CanUseSkill(skillName);
        }

        public bool CheckAnimEventPredicate(string animEventID)
        {
            ConfigAvatarAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(this.config, animEventID);
            return (((event2 != null) && ((base._maskedAnimEvents == null) || !base._maskedAnimEvents.Contains(animEventID))) && (base._animEventPredicates.Contains(event2.Predicate) && base._animEventPredicates.Contains(event2.Predicate2)));
        }

        private void CheckDynamicBoneMute(string oldSkillID, string newSkillID)
        {
            if (GlobalVars.AVATAR_USE_DYNAMIC_BONE)
            {
                bool flag2 = false;
                bool flag3 = false;
                for (int i = 0; i < this.muteDynamicBonesSkillIDs.Length; i++)
                {
                    if (this.muteDynamicBonesSkillIDs[i] == oldSkillID)
                    {
                        flag2 = true;
                    }
                    if (this.muteDynamicBonesSkillIDs[i] == newSkillID)
                    {
                        flag3 = true;
                    }
                }
                if (!flag2 && flag3)
                {
                    for (int j = 0; j < this._dynamicBones.Length; j++)
                    {
                        this._dynamicBones[j].SetWeight(0f);
                    }
                }
                else if (flag2 && !flag3)
                {
                    for (int k = 0; k < this._dynamicBones.Length; k++)
                    {
                        this._dynamicBones[k].SetWeight(1f);
                    }
                }
            }
        }

        [AnimationCallback]
        public override void ClearAttackTarget()
        {
            this.ClearAttackTriggers();
            this.SetAttackTarget(null);
        }

        public void ClearAttackTargetTimed(float duration = 0.5f)
        {
            if (this.AttackTarget != null)
            {
                this._clearAttackTargetTimer = duration;
            }
        }

        public override void ClearAttackTriggers()
        {
            if (this.IsActive())
            {
                base._animator.ResetTrigger("TriggerAttack");
                base._animator.ResetTrigger("TriggerHoldAttack");
                this.ClearSkillTriggers();
            }
        }

        private void ClearFaceAnimation()
        {
            if (this._providerL != null)
            {
                if (this._providerL.ReleaseReference())
                {
                    Resources.UnloadAsset(this._providerL);
                }
                this._providerL = null;
            }
            if (this._providerR != null)
            {
                if (this._providerR.ReleaseReference())
                {
                    Resources.UnloadAsset(this._providerR);
                }
                this._providerR = null;
            }
            if (this._providerM != null)
            {
                if (this._providerM.ReleaseReference())
                {
                    Resources.UnloadAsset(this._providerM);
                }
                this._providerM = null;
            }
        }

        public void ClearHitTrigger()
        {
            base._animator.ResetTrigger("TriggerHit");
        }

        public void ClearSkillTriggers()
        {
            if (this.IsActive())
            {
                if (!this._delayedSwapOutTriggered)
                {
                    base._animator.ResetTrigger("TriggerSwitchOut");
                }
                for (int i = 1; i <= 3; i++)
                {
                    string skillTriggerBySkillNum = this.GetSkillTriggerBySkillNum(i);
                    base._animator.ResetTrigger(skillTriggerBySkillNum);
                }
            }
        }

        public void ClearSubAttackList()
        {
            this._subAttackTargetList.Clear();
        }

        [AnimationCallback]
        public override void DeadHandler()
        {
            if (!this._isToBeRemoved)
            {
                if (this.CurrentSkillID != null)
                {
                    this.SetCurrentSkillID(null);
                }
                if (this.onDie != null)
                {
                    this.onDie(this);
                }
                this._isDeadAlready = true;
            }
        }

        public void DebugSetControllableAI()
        {
            BTreeAvatarAIController activeAIController = this.GetActiveAIController() as BTreeAvatarAIController;
            activeAIController.ChangeBehavior("test/AvatarAutoBattleBehavior_Attack_Test");
            activeAIController.SetBehaviorVariable("DoAttack", true);
            activeAIController.SetActive(true);
            this._inputController.SetActive(true);
        }

        public override void DetachEffect(int patternIx)
        {
            base.DetachEffect(patternIx);
            for (int i = 0; i < this._attachedEffects.Count; i++)
            {
                if (this._attachedEffects[i] == patternIx)
                {
                    this._attachedEffects[i] = -1;
                    break;
                }
            }
        }

        public void DetachWeapon()
        {
            ConfigWeapon weaponConfig = WeaponData.GetWeaponConfig(this._equipedWeaponID);
            weaponConfig.Attach.GetDetachHandler()(weaponConfig.Attach, this, this.AvatarTypeName);
        }

        private void DisableAttachedEffectOnActiveChanged(bool active)
        {
            if (active)
            {
                for (int i = 0; i < this._attachedEffects.Count; i++)
                {
                    if ((this._attachedEffects[i] != -1) && (Singleton<EffectManager>.Instance.GetIndexedEntityEffectPattern(this._attachedEffects[i]) != null))
                    {
                        Singleton<EffectManager>.Instance.SetIndexedEntityEffectPatternActive(this._attachedEffects[i], true);
                    }
                }
            }
            else
            {
                for (int j = 0; j < this._attachedEffects.Count; j++)
                {
                    if ((this._attachedEffects[j] != -1) && ((Singleton<EffectManager>.Instance != null) && (Singleton<EffectManager>.Instance.GetIndexedEntityEffectPattern(this._attachedEffects[j]) != null)))
                    {
                        Singleton<EffectManager>.Instance.SetIndexedEntityEffectPatternActive(this._attachedEffects[j], false);
                    }
                }
            }
        }

        public void DoCameraAction(ConfigAvatarCameraAction actionConfig)
        {
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            ConfigAvatarCameraAction action = actionConfig;
            if (action is SetCameraDistance)
            {
                SetCameraDistance distance = (SetCameraDistance) action;
                mainCamera.SetTimedPullZ(distance.RadiusRatio, distance.Elevation, distance.CenterY, distance.FOVOffset, distance.Time, distance.LerpTime, distance.LerpCurve, false);
            }
            else if (action is MoleMole.Config.PlayAvatarCameraAnimation)
            {
                MoleMole.Config.PlayAvatarCameraAnimation animation = (MoleMole.Config.PlayAvatarCameraAnimation) action;
                this.PlayAvatarCameraAnimation(animation.CameraAnimName, animation.EnterPolarMode, animation.ExitTransitionLerp);
            }
            else if (action is SuddenRecover)
            {
                Singleton<CameraManager>.Instance.GetMainCamera().SuddenRecover();
            }
        }

        private void EnterSteerFaceDirectionWithSteerOption(Vector3 targetForward, SkillEnterSteerOption option, bool isFreeSteer)
        {
            if (option == null)
            {
                if (!isFreeSteer || this.GetActiveControlData().hasSteer)
                {
                    this.SteerFaceDirectionTo(targetForward);
                }
            }
            else if (option.SteerType == SkillEnterSteerOption.EnterSteerType.Instant)
            {
                if (!isFreeSteer || this.GetActiveControlData().hasSteer)
                {
                    this.SteerFaceDirectionTo(this.CalculateSteerTargetForwardWithOption(targetForward, base.FaceDirection, option));
                }
            }
            else if ((option.MuteSteerWhenNoEnemy && (this._subAttackTargetList.Count <= 0)) && (this._attackTarget == null))
            {
                this.MuteSteerTillNextState();
            }
            else
            {
                this._activeEnterSteerOption = option;
                this._activeEnterSteerClampStart = base.FaceDirection;
                this._activeEnterSteerState = SkillEnterSteerState.WaitingForStart;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        [AnimationCallback]
        private void ForceSteerBack()
        {
            this.SteerFaceDirectionTo(-base.FaceDirection);
        }

        public void ForceUseAIController()
        {
            this._inputController.SetActive(false);
            this._aiController.SetActive(true);
        }

        public IAIController GetActiveAIController()
        {
            return this._aiController;
        }

        public AvatarControlData GetActiveControlData()
        {
            if (this._muteControlCount > 0)
            {
                return AvatarControlData.emptyControlData;
            }
            return (!this._hasUpdatedControlThisFrame ? this._lastFrameControlData : this._controlData);
        }

        public override BaseMonoEntity GetAttackTarget()
        {
            return this.AttackTarget;
        }

        public Transform GetFollowTransform(uint followMode)
        {
            if (followMode != 1)
            {
                throw new Exception("Invalid Type or State!");
            }
            return base._transform;
        }

        public BaseAvatarInputController GetInputController()
        {
            return this._inputController;
        }

        private string GetInstantTriggerEventName(string instantSkillID)
        {
            if (this.config.Skills.ContainsKey(instantSkillID))
            {
                return this.config.Skills[instantSkillID].InstantTriggerEvent;
            }
            return string.Empty;
        }

        private string GetSkillIDBySkillNum(int skillNum)
        {
            switch (skillNum)
            {
                case 1:
                    return "SKL01";

                case 2:
                    return "SKL02";

                case 3:
                    return "SKL_WEAPON";
            }
            throw new Exception("Invalid Type or State!");
        }

        private string GetSkillTriggerBySkillNum(int skillNum)
        {
            return ((skillNum != 3) ? ("TriggerSkill_" + skillNum) : "TriggerWeapon");
        }

        [AnimationCallback]
        private void HideCloseUpPanel()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.MonsterCloseUpEnd, null));
        }

        public void Init(bool isLocal, uint runtimeID, string avatarTypeName, int weaponID, Vector3 initPos, Vector3 initForward, bool isLeader)
        {
            this.AvatarTypeName = avatarTypeName;
            this.AvatarTypeID = AvatarData.GetAvatarTypeIDByName(this.AvatarTypeName);
            this.config = AvatarData.GetAvatarConfig(this.AvatarTypeName);
            base.animatorConfig = this.config;
            base.commonConfig = this.config.CommonConfig;
            base.Init(runtimeID);
            this._isAlive = true;
            this.isLeader = isLeader;
            this._attackTargetSelectAction = this.config.AttackTargetSelectPattern.selectMethod;
            this.MoveSpeedRatio = 1f;
            initPos.y += this.config.CommonArguments.CreatePosYOffset;
            LayerMask mask = (((int) 1) << InLevelData.AVATAR_LAYER) | (((int) 1) << InLevelData.MONSTER_LAYER);
            initPos = base.PickInitPosition(mask, initPos, this.config.CommonArguments.CollisionRadius);
            base.transform.position = initPos;
            initForward.y = 0f;
            base.transform.forward = initForward;
            base._rigidbody.rotation = base.transform.rotation;
            UnityEngine.Debug.DrawLine(this.XZPosition, this.XZPosition + ((Vector3) (base.transform.forward * 5f)), Color.cyan, 2f);
            base._animEventPredicates.Add(this.config.CommonArguments.DefaultAnimEventPredicate);
            this.InitController();
            this.InitSkillAnimatorEventPattern();
            this.InitPlugins();
            object[] objArray1 = new object[] { "Avatar_", this.AvatarTypeName, "_", runtimeID };
            base.gameObject.name = string.Concat(objArray1);
            this._muteSteerIx = base.AddWaitTransitionState();
            this._muteLockAttackTargetIx = base.AddWaitTransitionState();
            this._attackSpeedState = AttackSpeedState.Idle;
            this._attackSpeedTimeScaleIx = this.PushProperty("Animator_OverallSpeedRatio", 0f);
            base.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.onCurrentSkillIDChanged, new Action<string, string>(this.OnSkillIDChanged));
            base.onActiveChanged = (Action<bool>) Delegate.Combine(base.onActiveChanged, new Action<bool>(this.DisableAttachedEffectOnActiveChanged));
            base.onIsGhostChanged = (Action<bool>) Delegate.Combine(base.onIsGhostChanged, new Action<bool>(this.OnIsGhostChanged));
            base.RegisterPropertyChangedCallback("Entity_AttackSpeed", new Action(this.OnAttackSpeedChanged));
            this._attackMoveRatioIx = this.PushProperty("Animator_RigidBodyVelocityRatio", 0f);
            base.RegisterPropertyChangedCallback("Entity_AttackMoveRatio", new Action(this.OnAttackMoveChanged));
            this.AttachEffectOverrides();
            this._equipedWeaponID = weaponID;
            this._subAttackTargetList = new List<BaseMonoEntity>();
            this.UploadFaceTexture();
            this.InitFaceAnimation();
            this.PostInit();
            if (GlobalVars.ENABLE_CONTINUOUS_DETECT_MODE)
            {
                base._rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
        }

        protected void InitController()
        {
            this._inputController = new KianaInputController(this);
            this._aiController = new BTreeAvatarAIController(this);
            this._controlData = AvatarControlData.emptyControlData;
            this._lastFrameControlData = new AvatarControlData();
        }

        private void InitDynamicBone()
        {
            bool flag = GlobalVars.AVATAR_USE_DYNAMIC_BONE;
            this._dynamicBones = base.gameObject.GetComponentsInChildren<DynamicBone>();
            foreach (DynamicBone bone in this._dynamicBones)
            {
                bone.enabled = flag;
            }
        }

        private void InitFaceAnimation()
        {
            if (((this.leftEyeRenderer != null) && (this.rightEyeRenderer != null)) && ((this.mouthRenderer != null) && (this._faceAnimation == null)))
            {
                string name = this.AvatarTypeName.Substring(0, this.AvatarTypeName.IndexOf("_"));
                ConfigFaceAnimation faceAnimation = FaceAnimationData.GetFaceAnimation(name);
                if (faceAnimation != null)
                {
                    this._faceAnimation = new FaceAnimation();
                    string path = "FaceAtlas/" + name + "/Eye/Atlas";
                    string str3 = "FaceAtlas/" + name + "/Mouth/Atlas";
                    if (this._providerL == null)
                    {
                        this._providerL = Resources.Load<AtlasMatInfoProvider>(path);
                        this._providerL.RetainReference();
                    }
                    if (this._providerR == null)
                    {
                        this._providerR = Resources.Load<AtlasMatInfoProvider>(path);
                        this._providerR.RetainReference();
                    }
                    if (this._providerM == null)
                    {
                        this._providerM = Resources.Load<AtlasMatInfoProvider>(str3);
                        this._providerM.RetainReference();
                    }
                    if (((this._providerL != null) && (this._providerR != null)) && (this._providerM != null))
                    {
                        FacePartControl leftEye = new FacePartControl();
                        leftEye.Init(this._providerL, this.leftEyeRenderer);
                        FacePartControl rightEye = new FacePartControl();
                        rightEye.Init(this._providerR, this.rightEyeRenderer);
                        FacePartControl mouth = new FacePartControl();
                        mouth.Init(this._providerM, this.mouthRenderer);
                        this._faceAnimation.Setup(faceAnimation, leftEye, rightEye, mouth);
                    }
                }
            }
        }

        private void InitMaterials()
        {
            this.InitOriginalShadowColorList();
            this.AdjustShadowColors();
        }

        private void InitOriginalShadowColorList()
        {
            this._shadowColorAdjusterList = new List<ShadowColorAdjuster>();
            if (base._instancedMaterialGroups.Count > 0)
            {
                foreach (MaterialGroup.RendererMaterials materials in base._instancedMaterialGroups[0].entries)
                {
                    foreach (Material material in materials.materials)
                    {
                        if (material.HasProperty("_FirstShadowMultColor"))
                        {
                            this._shadowColorAdjusterList.Add(new ShadowColorAdjuster(material));
                        }
                    }
                }
            }
        }

        private void InitSkillAnimatorEventPattern()
        {
            foreach (string str in this.config.Skills.Keys)
            {
                ConfigAvatarSkill skill = this.config.Skills[str];
                if (skill.AnimatorEventPattern != null)
                {
                    int index = 0;
                    int length = skill.AnimatorStateNames.Length;
                    while (index < length)
                    {
                        base.AttachAnimatorEventPattern(Animator.StringToHash(skill.AnimatorStateNames[index]), skill.AnimatorEventPattern);
                        index++;
                    }
                }
            }
        }

        public override bool IsActive()
        {
            return (this._isAlive && base.gameObject.activeSelf);
        }

        public bool IsAIActive()
        {
            return this._aiController.active;
        }

        public bool IsAlive()
        {
            return this._isAlive;
        }

        public bool IsAnimatorInTag(AvatarData.AvatarTagGroup tagGroup)
        {
            return this.IsAnimatorInTag(tagGroup, base._animator.GetCurrentAnimatorStateInfo(0));
        }

        public bool IsAnimatorInTag(AvatarData.AvatarTagGroup tagGroup, AnimatorStateInfo stateInfo)
        {
            return AvatarData.AVATAR_TAG_GROUPS[(int) tagGroup].Contains(stateInfo.tagHash);
        }

        public bool IsAttackHoldMode()
        {
            if (!this._hasGotParameterHodeMode && (base._animator != null))
            {
                this._isHodeMode = base._animator.HasParameter("_IsHoldMode");
                this._hasGotParameterHodeMode = true;
            }
            return ((this._isHodeMode && (base._animator != null)) && base._animator.GetBool("_IsHoldMode"));
        }

        public bool IsControlMuted()
        {
            return (this._muteControlCount > 0);
        }

        public virtual bool IsDeadAlready()
        {
            return this._isDeadAlready;
        }

        private bool IsSkillInstantTrigger(string skillID)
        {
            bool isInstantTrigger = false;
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(base.GetRuntimeID());
            if (actor != null)
            {
                if (skillID == "SKL_WEAPON")
                {
                    return EquipmentSkillData.GetEquipmentSkillConfig(actor.GetSkillInfo("SKL_WEAPON").avatarSkillID).IsInstantTrigger;
                }
                if (actor.config.Skills.ContainsKey(skillID))
                {
                    isInstantTrigger = actor.config.Skills[skillID].IsInstantTrigger;
                }
            }
            return isInstantTrigger;
        }

        public bool IsSwitchOutTriggerSet()
        {
            return base._animator.GetBool("TriggerSwitchOut");
        }

        public override bool IsToBeRemove()
        {
            return this._isToBeRemoved;
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            this._lastFrameControlData.CopyFrom(this._controlData);
            this._hasUpdatedControlThisFrame = false;
            this._controlData = AvatarControlData.emptyControlData;
            if (this._inputController.active)
            {
                this._inputController.controlData.FrameReset();
            }
            if (this._aiController.active)
            {
                this._aiController.controlData.FrameReset();
            }
        }

        Vector3 IAIEntity.get_FaceDirection()
        {
            return base.FaceDirection;
        }

        Vector3 IAIEntity.get_RootNodePosition()
        {
            return base.RootNodePosition;
        }

        Transform IAIEntity.get_transform()
        {
            return base.transform;
        }

        uint IAIEntity.GetRuntimeID()
        {
            return base.GetRuntimeID();
        }

        float IAttacker.Evaluate(DynamicFloat target)
        {
            return base.Evaluate(target);
        }

        int IAttacker.Evaluate(DynamicInt target)
        {
            return base.Evaluate(target);
        }

        Vector3 IAttacker.get_FaceDirection()
        {
            return base.FaceDirection;
        }

        Transform IAttacker.get_transform()
        {
            return base.transform;
        }

        uint IAttacker.GetRuntimeID()
        {
            return base.GetRuntimeID();
        }

        string IRetreatable.GetCurrentNamedState()
        {
            return base.GetCurrentNamedState();
        }

        uint IRetreatable.GetRuntimeID()
        {
            return base.GetRuntimeID();
        }

        [AnimationCallback]
        public override void MultiAnimEventHandler(string multiAnimEventID)
        {
            ConfigMultiAnimEvent event2 = this.config.MultiAnimEvents[multiAnimEventID];
            for (int i = 0; i < event2.AnimEventNames.Length; i++)
            {
                this.AnimEventHandler(event2.AnimEventNames[i]);
            }
        }

        protected void MuteLockAttackTargetTillNextState()
        {
            base.StartWaitTransitionState(this._muteLockAttackTargetIx);
        }

        protected void MuteSteerTillNextState()
        {
            base.StartWaitTransitionState(this._muteSteerIx);
        }

        private void NamedStateChanged(string fromNamedState, string toNamedState)
        {
            base._currentNamedState = toNamedState;
            ConfigNamedState state = null;
            ConfigNamedState state2 = null;
            if (fromNamedState != null)
            {
                state = this.config.NamedStates[fromNamedState];
            }
            if (toNamedState != null)
            {
                state2 = this.config.NamedStates[toNamedState];
            }
            if ((state2 != null) && state2.HighSpeedMovement)
            {
                this.PushHighspeedMovement();
            }
            if ((state != null) && state.HighSpeedMovement)
            {
                this.PopHighspeedMovement();
            }
        }

        public void onAnimatedHitBoxCreated(MonoAnimatedHitboxDetect hitBox, ConfigEntityAttackPattern attackPattern)
        {
            if (this.onAnimatedHitBoxCreatedCallBack != null)
            {
                this.onAnimatedHitBoxCreatedCallBack(hitBox, attackPattern);
            }
        }

        protected override void OnAnimatorMove()
        {
            base.OnAnimatorMove();
            if (this.IsAnimatorInTag(AvatarData.AvatarTagGroup.Movement))
            {
                base._rigidbody.velocity = (Vector3) (base._rigidbody.velocity * (this._moveSpeedRatio + this.GetProperty("Animator_MoveSpeedRatio")));
            }
            base._rigidbody.velocity = (Vector3) (base._rigidbody.velocity * (1f + this.GetProperty("Animator_RigidBodyVelocityRatio")));
        }

        protected override void OnAnimatorStateChanged(AnimatorStateInfo fromState, AnimatorStateInfo toState)
        {
            string str;
            string str2;
            string str3;
            string str4;
            base.OnAnimatorStateChanged(fromState, toState);
            this.config.StateToNamedStateMap.TryGetValue(fromState.shortNameHash, out str);
            this.config.StateToNamedStateMap.TryGetValue(toState.shortNameHash, out str2);
            if ((str != null) || (str2 != null))
            {
                this.NamedStateChanged(str, str2);
            }
            this.config.StateToSkillIDMap.TryGetValue(fromState.shortNameHash, out str3);
            this.config.StateToSkillIDMap.TryGetValue(toState.shortNameHash, out str4);
            if ((str3 != null) || (str4 != null))
            {
                this.SkillIDChanged(str3, fromState, str4, toState);
            }
            this.SetAvatarMassByAnimatorTag(toState);
            this.SwitchAnimatorStateChanged(fromState, toState);
            this.UpdatePlaySoundOnAnimatorStateChanged(fromState, toState);
            base.ResetRigidbodyRotation();
        }

        private void OnAttackMoveChanged()
        {
            if (this._attackSpeedState == AttackSpeedState.DuringAttackTime)
            {
                this.SetPropertyByStackIndex("Animator_RigidBodyVelocityRatio", this._attackMoveRatioIx, this.GetProperty("Entity_AttackMoveRatio"));
            }
        }

        private void OnAttackSpeedChanged()
        {
            if (this._attackSpeedState == AttackSpeedState.DuringAttackTime)
            {
                this.SetPropertyByStackIndex("Animator_OverallSpeedRatio", this._attackSpeedTimeScaleIx, this.GetProperty("Entity_AttackSpeed"));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.onAttackTargetChanged = null;
            this.onLockDirectionChanged = null;
            this.onDie = null;
            this.ClearFaceAnimation();
            if (this._aiController != null)
            {
                ((BTreeAvatarAIController) this._aiController).DisableBehavior();
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.switchState = AvatarSwitchState.OffStage;
            this._waitMoveSoundCoroutine = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.switchState = AvatarSwitchState.OnStage;
            if (this.config != null)
            {
                this.ApplyAnimatorProperties();
            }
        }

        private void OnIsGhostChanged(bool isGhost)
        {
            if (this._isAlive)
            {
                this.hitbox.enabled = !isGhost;
            }
        }

        protected override void OnSkillEffectClear(string oldID, string skillID)
        {
            if (skillID != null)
            {
                ConfigAvatarSkill skill = this.config.Skills[skillID];
                if (skill.NeedClearEffect)
                {
                    base.ClearSkillEffect(skillID);
                }
            }
        }

        private void OnSkillIDChanged(string oldID, string skillID)
        {
            if ((this._attackSpeedState == AttackSpeedState.DuringAttackTime) || (skillID == null))
            {
                this.SetPropertyByStackIndex("Animator_OverallSpeedRatio", this._attackSpeedTimeScaleIx, 0f);
                this._attackSpeedState = AttackSpeedState.Idle;
            }
            else if (this.config.Skills[skillID].AttackNormalizedTimeStop != 0f)
            {
                this._attackSpeedState = AttackSpeedState.WaitingAttackTimeStart;
            }
            else
            {
                this._attackSpeedState = AttackSpeedState.Idle;
            }
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(base._runtimeID))
            {
                if ((oldID != null) && this.config.Skills[oldID].MuteCameraControl)
                {
                    Singleton<CameraManager>.Instance.GetMainCamera().SetMuteManualCameraControl(false);
                }
                if ((skillID != null) && this.config.Skills[skillID].MuteCameraControl)
                {
                    Singleton<CameraManager>.Instance.GetMainCamera().SetMuteManualCameraControl(true);
                }
            }
            if (skillID == null)
            {
                this._skillMassRatio = 1f;
            }
            else if (this.config.Skills[skillID].MassRatio != 1f)
            {
                this._skillMassRatio = this.config.Skills[skillID].MassRatio;
            }
            Singleton<LevelManager>.Instance.levelActor.SetLevelComboTimerState(LevelActor.ComboTimerState.Running);
            this.CheckDynamicBoneMute(oldID, skillID);
        }

        public virtual void PickHPMedic(uint HPMedicRuntimeID)
        {
            if (base.gameObject.activeSelf)
            {
                this.FireEffect("Ability_HealHP_Pick");
            }
            MonoGoods goods = (MonoGoods) Singleton<DynamicObjectManager>.Instance.TryGetDynamicObjectByRuntimeID(HPMedicRuntimeID);
            MonoEntityAudio component = base.GetComponent<MonoEntityAudio>();
            if (((component != null) && !goods.muteSound) && (base.GetRuntimeID() == Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID()))
            {
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(base.GetRuntimeID());
                if (((double) actor.HP) > (((double) actor.maxHP) * 0.5))
                {
                    component.PostPickupHPHigh();
                }
                else
                {
                    component.PostPickupHPLow();
                }
            }
        }

        public virtual void PickupCoin(uint coinRuntimeID)
        {
            if (base.gameObject.activeSelf)
            {
                this.FireEffect("Ability_GetCoin");
            }
            MonoGoods goods = (MonoGoods) Singleton<DynamicObjectManager>.Instance.TryGetDynamicObjectByRuntimeID(coinRuntimeID);
            MonoEntityAudio component = base.GetComponent<MonoEntityAudio>();
            if (((component != null) && !goods.muteSound) && (base.GetRuntimeID() == Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID()))
            {
                component.PostPickupCoin();
            }
        }

        public virtual void PickupEquipItem(int rarity, uint equipItemRuntimeID)
        {
            List<MonoEffect> list = Singleton<EffectManager>.Instance.TriggerEntityEffectPatternReturnValue("Ability_GetEquipItem", this, false);
            int num = 0;
            int count = list.Count;
            while (num < count)
            {
                Singleton<DynamicObjectManager>.Instance.SetParticleColorByRarity(list[num].gameObject, rarity);
                num++;
            }
            MonoGoods goods = (MonoGoods) Singleton<DynamicObjectManager>.Instance.TryGetDynamicObjectByRuntimeID(equipItemRuntimeID);
            MonoEntityAudio component = base.GetComponent<MonoEntityAudio>();
            if (((component != null) && !goods.muteSound) && (base.GetRuntimeID() == Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID()))
            {
                component.PostPickupEquipItem();
            }
        }

        private void PlayAvatarCameraAnimation(string cameraAnimName, MainCameraFollowState.EnterPolarMode enterPolarMode, bool exitTransitionLerp)
        {
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(base._runtimeID))
            {
                MonoAuxObject obj2 = Singleton<AuxObjectManager>.Instance.CreateSimpleAuxObject(cameraAnimName, base._runtimeID);
                this._activeCameraAnimName = cameraAnimName;
                obj2.transform.parent = base._transform;
                Singleton<CameraManager>.Instance.GetMainCamera().PlayAvatarCameraAnimationThenTransitToFollow(obj2.GetComponent<Animation>(), this, enterPolarMode, exitTransitionLerp);
            }
        }

        public override void PopNoCollision()
        {
            this._noCollisionCount--;
            if (this._noCollisionCount == 0)
            {
                base.gameObject.layer = InLevelData.AVATAR_LAYER;
            }
        }

        protected override void PostInit()
        {
            base.PostInit();
            this.InitMaterials();
            WeaponData.WeaponModelAndEffectAttach(this._equipedWeaponID, this.AvatarTypeName, this);
            this.InitDynamicBone();
        }

        public override void PushNoCollision()
        {
            this._noCollisionCount++;
            if (this._noCollisionCount == 1)
            {
                base.gameObject.layer = InLevelData.INACTIVE_ENTITY_LAYER;
            }
        }

        public void RefreshController()
        {
            this._inputController.controlData.FrameReset();
            this._aiController.controlData.FrameReset();
            bool isActive = Singleton<AvatarManager>.Instance.IsLocalAvatar(base.GetRuntimeID());
            bool isAutoBattle = Singleton<AvatarManager>.Instance.isAutoBattle;
            LevelActor actor = (LevelActor) Singleton<EventManager>.Instance.GetActor(0x21800001);
            if (actor.levelMode == LevelActor.Mode.Single)
            {
                this._inputController.SetActive(isActive);
                this._aiController.SetActive(isAutoBattle);
            }
            else if (actor.levelMode == LevelActor.Mode.Multi)
            {
                this._inputController.SetActive(isActive);
                this._aiController.SetActive(isAutoBattle || !isActive);
            }
            else if (actor.levelMode == LevelActor.Mode.MultiRemote)
            {
                this._inputController.SetActive(isActive);
                this._aiController.SetActive(false);
            }
            else if (actor.levelMode == LevelActor.Mode.NetworkedMP)
            {
                this._inputController.SetActive(isActive);
                this._aiController.SetActive(false);
            }
            this.ClearAttackTriggers();
        }

        public void ResetTriggerSwitchOut()
        {
            base._animator.ResetTrigger("TriggerSwitchOut");
            this._delayedSwapOutTriggered = false;
            this.switchState = AvatarSwitchState.OnStage;
        }

        public virtual void Revive(Vector3 revivePosition)
        {
            this._isAlive = true;
            this._isDeadAlready = false;
            base._transform.position = revivePosition;
            this.hitbox.enabled = true;
        }

        public virtual void RunBSStart()
        {
            if (this.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackOrSkill))
            {
                this._isFromAttackOrSkill = true;
                this._steerLerpRatio = 1.3f;
            }
        }

        public virtual void RunBSStop()
        {
            if (!this.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackOrSkill) && this._isFromAttackOrSkill)
            {
                this._isFromAttackOrSkill = false;
                this._steerLerpRatio = 1f;
            }
        }

        [AnimationCallback]
        protected void RunOnLeftFoot()
        {
            base._animator.SetFloat("RunStepOnRight", 0f);
        }

        [AnimationCallback]
        protected void RunOnRightFoot()
        {
            base._animator.SetFloat("RunStepOnRight", 1f);
        }

        public void SelectTarget()
        {
            this._clearAttackTargetTimer = 0f;
            this._attackTargetSelectAction(this);
        }

        public void SetAttackSelectMethod(Action<BaseMonoAvatar> selector)
        {
            this._attackTargetSelectAction = selector;
        }

        public override void SetAttackTarget(BaseMonoEntity newTarget)
        {
            bool flag = false;
            if (this._attackTarget != newTarget)
            {
                flag = true;
            }
            this._attackTarget = newTarget;
            if ((this.onAttackTargetChanged != null) && flag)
            {
                this.onAttackTargetChanged(newTarget);
            }
        }

        private void SetAvatarMassByAnimatorTag(AnimatorStateInfo toState)
        {
            int tagHash = toState.tagHash;
            if (((tagHash == AvatarData.AVATAR_APPEAR_TAG) || (tagHash == AvatarData.AVATAR_IDLESUB_TAG)) || (tagHash == AvatarData.AVATAR_DIE_TAG))
            {
                this._baseMassRatio = 100f;
            }
            else if ((tagHash == AvatarData.AVATAR_SKL_TAG) || (tagHash == AvatarData.AVATAR_SKL_NO_TARGET_TAG))
            {
                this._baseMassRatio = 1f;
            }
            else
            {
                this._baseMassRatio = 1f;
            }
            base.SetMass((1f * this._baseMassRatio) * this._skillMassRatio);
        }

        public void SetCountedMuteControl(bool mute)
        {
            this._muteControlCount += !mute ? -1 : 1;
            if (this._muteControlCount < 0)
            {
                this._muteControlCount = 0;
            }
        }

        private void SetCurrentSkillID(string value)
        {
            if (base.onCurrentSkillIDChanged != null)
            {
                base.onCurrentSkillIDChanged(this._currentSkillID, value);
            }
            this._currentSkillID = value;
        }

        public override void SetDied(KillEffect killEffect)
        {
            this._isAlive = false;
            this.CleanOwnedObjects();
            base.CastWaitingAudioEvent();
            if (killEffect == KillEffect.KillNow)
            {
                this.hitbox.enabled = false;
                base._animator.SetBool("IsDead", true);
            }
            else if (killEffect == KillEffect.KillImmediately)
            {
                this._isToBeRemoved = true;
                this.DeadHandler();
            }
        }

        public void SetMuteAnimRetarget(bool mute)
        {
            this._muteAnimRetarget = mute;
        }

        public override void SetUseLocalController(bool enabled)
        {
            if (enabled)
            {
                this.RefreshController();
            }
            else
            {
                this._inputController.SetActive(false);
                this._aiController.SetActive(false);
            }
        }

        [AnimationCallback]
        private void ShowCloseUpPanel(string name)
        {
            Singleton<MainUIManager>.Instance.ShowPage(new MonsterCloseUpPageContext(name), UIType.Page);
        }

        private void SkillIDChanged(string fromSkillID, AnimatorStateInfo fromState, string toSkillID, AnimatorStateInfo toState)
        {
            ConfigAvatarSkill skill = null;
            ConfigAvatarSkill skill2 = null;
            if (fromSkillID != null)
            {
                skill = this.config.Skills[fromSkillID];
            }
            if (toSkillID != null)
            {
                skill2 = this.config.Skills[toSkillID];
            }
            if (this._activeCameraAnimName != null)
            {
                MonoAuxObject auxObject = Singleton<AuxObjectManager>.Instance.GetAuxObject(base._runtimeID, this._activeCameraAnimName);
                if (auxObject != null)
                {
                    auxObject.SetDestroy();
                    this._activeCameraAnimName = null;
                }
            }
            this._activeEnterSteerState = SkillEnterSteerState.Idle;
            this._activeEnterSteerOption = null;
            if ((skill2 != null) && skill2.HighSpeedMovement)
            {
                this.PushHighspeedMovement();
            }
            if ((skill != null) && skill.HighSpeedMovement)
            {
                this.PopHighspeedMovement();
            }
            if (this.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackOrSkill, fromState) && !this.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackOrSkill, toState))
            {
                if (!this._muteAnimRetarget)
                {
                    if (AvatarData.RUN_CLEAR_ATTACK_TARGET && this.IsAnimatorInTag(AvatarData.AvatarTagGroup.Movement, toState))
                    {
                        this.SetAttackTarget(null);
                    }
                    else
                    {
                        this.ClearAttackTargetTimed(0.5f);
                    }
                }
                this.SetPropertyByStackIndex("Animator_RigidBodyVelocityRatio", this._attackMoveRatioIx, 0f);
            }
            else if (toSkillID != null)
            {
                switch (skill2.SkillType)
                {
                    case AvatarSkillType.AttackStart:
                        Singleton<EventManager>.Instance.FireEvent(new EvtAttackStart(base.GetRuntimeID(), toSkillID), MPEventDispatchMode.Normal);
                        break;

                    case AvatarSkillType.SkillStart:
                        Singleton<EventManager>.Instance.FireEvent(new EvtSkillStart(base.GetRuntimeID(), toSkillID), MPEventDispatchMode.Normal);
                        break;
                }
                if (skill2.ForceMuteSteer && base._animator.IsInTransition(0))
                {
                    this.MuteSteerTillNextState();
                }
                if (this.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackWithNoTarget, toState))
                {
                    if (AvatarData.NO_TARGET_SKILL_CLEAR_AVATAR_TARGET && !this._muteAnimRetarget)
                    {
                        this.SetAttackTarget(null);
                    }
                    if (((skill2.EnterSteer != SkillEnterSetting.MuteFreeSteer) && (skill2.EnterSteer != SkillEnterSetting.MuteRetarget)) && (skill2.EnterSteer != SkillEnterSetting.OnlyRetargetWhenNoTarget))
                    {
                        this.EnterSteerFaceDirectionWithSteerOption(this.GetActiveControlData().steerDirection, skill2.EnterSteerOption, true);
                    }
                }
                else if ((this.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackTargetLeadDirection, toState) || this.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackSteerOnEnter, toState)) && (skill2.EnterSteer != SkillEnterSetting.MuteRetarget))
                {
                    if (!this._muteAnimRetarget && (((this.AttackTarget == null) || !this.AttackTarget.IsActive()) || (this.GetActiveControlData().hasSteer && (skill2.EnterSteer != SkillEnterSetting.OnlyRetargetWhenNoTarget))))
                    {
                        this.SelectTarget();
                    }
                    if ((this.AttackTarget == null) || !this.AttackTarget.IsActive())
                    {
                        if (((skill2.EnterSteer != SkillEnterSetting.MuteFreeSteer) && (skill2.EnterSteer != SkillEnterSetting.MuteRetarget)) && (skill2.EnterSteer != SkillEnterSetting.OnlyRetargetWhenNoTarget))
                        {
                            this.EnterSteerFaceDirectionWithSteerOption(this.GetActiveControlData().steerDirection, skill2.EnterSteerOption, true);
                        }
                    }
                    else
                    {
                        Vector3 targetForward = this.AttackTarget.XZPosition - this.XZPosition;
                        this.EnterSteerFaceDirectionWithSteerOption(targetForward, skill2.EnterSteerOption, false);
                        if (base._animator.IsInTransition(0))
                        {
                            this.MuteSteerTillNextState();
                        }
                    }
                }
                else if ((this.AttackTarget != null) && this.AttackTarget.IsActive())
                {
                    this._clearAttackTargetTimer = 0f;
                }
                if (this.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackTargetLeadDirection, toState))
                {
                    this.SetPropertyByStackIndex("Animator_RigidBodyVelocityRatio", this._attackMoveRatioIx, this.GetProperty("Entity_AttackMoveRatio"));
                }
                else
                {
                    this.SetPropertyByStackIndex("Animator_RigidBodyVelocityRatio", this._attackMoveRatioIx, 0f);
                }
            }
            this.SetCurrentSkillID(toSkillID);
        }

        private void StartWeaponSkillAbility()
        {
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(base.GetRuntimeID());
            if (actor != null)
            {
                ConfigEquipmentSkillEntry equipmentSkillConfig = EquipmentSkillData.GetEquipmentSkillConfig(actor.GetSkillInfo("SKL_WEAPON").avatarSkillID);
                EvtAbilityStart evt = new EvtAbilityStart(base.GetRuntimeID(), null) {
                    abilityName = equipmentSkillConfig.AbilityName
                };
                Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
            }
        }

        private void StopMoveSoundCoroutine()
        {
            if (this._waitMoveSoundCoroutine != null)
            {
                base.StopCoroutine(this._waitMoveSoundCoroutine);
                this._waitMoveSoundCoroutine = null;
            }
        }

        private void SwitchAnimatorStateChanged(AnimatorStateInfo from, AnimatorStateInfo to)
        {
            if ((from.shortNameHash != this.config.StateMachinePattern.SwitchInAnimatorStateHash) || (to.shortNameHash != this.config.StateMachinePattern.SwitchOutAnimatorStateHash))
            {
                if ((to.shortNameHash == this.config.StateMachinePattern.SwitchInAnimatorStateHash) || (to.shortNameHash == this.config.StateMachinePattern.SwitchOutAnimatorStateHash))
                {
                    this.SetNeedOverrideVelocity(true);
                    this.SetOverrideVelocity(Vector3.zero);
                    base._rigidbody.detectCollisions = false;
                    base._animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                }
                else if ((from.shortNameHash == this.config.StateMachinePattern.SwitchInAnimatorStateHash) || (from.shortNameHash == this.config.StateMachinePattern.SwitchOutAnimatorStateHash))
                {
                    this.SetNeedOverrideVelocity(false);
                    base._rigidbody.detectCollisions = true;
                    base._animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
                }
            }
            if (to.shortNameHash == this.config.StateMachinePattern.SwitchOutAnimatorStateHash)
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtAvatarSwapOutStart(base._runtimeID), MPEventDispatchMode.Normal);
                this.switchState = AvatarSwitchState.SwitchingOut;
                if (this._delayedSwapOutTriggered)
                {
                    Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Avatar_SwitchRoleOut", this.XZPosition, base.FaceDirection, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
                    this._delayedSwapOutTriggered = false;
                }
            }
            else if (from.shortNameHash == this.config.StateMachinePattern.SwitchOutAnimatorStateHash)
            {
                if (to.shortNameHash == this.config.StateMachinePattern.SwitchInAnimatorStateHash)
                {
                    this.SwitchOutFinishHandle(false);
                    this.switchState = AvatarSwitchState.OnStage;
                }
                else
                {
                    this.SwitchOutFinishHandle(true);
                }
            }
            if (from.shortNameHash == this.config.StateMachinePattern.SwitchInAnimatorStateHash)
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtAvatarSwapInEnd(base._runtimeID), MPEventDispatchMode.Normal);
            }
        }

        private void SwitchOutFinishHandle(bool setInactive)
        {
            this.ClearAttackTriggers();
            this.OrderMove = false;
            if (this.CurrentSkillID != null)
            {
                this.SetCurrentSkillID(null);
            }
            if (setInactive)
            {
                base.gameObject.SetActive(false);
            }
        }

        private void SyncAnimatorMoveSpeed()
        {
            float num = (this._moveSpeedRatio * (1f + this.GetProperty("Animator_MoveSpeedRatio"))) - 1f;
            base._animator.SetFloat("MoveSpeed", 1f + (num * 0.35f));
        }

        [AnimationCallback]
        private void TimeSlowTrigger(float time)
        {
            if ((Singleton<AvatarManager>.Instance.IsLocalAvatar(base._runtimeID) && (this.AttackTarget != null)) && (this.AttackTarget.IsActive() && (Vector3.Distance(this.XZPosition, this.AttackTarget.XZPosition) < 2f)))
            {
                Singleton<LevelManager>.Instance.levelActor.TimeSlow(time);
            }
        }

        [AnimationCallback]
        private void TrggerCameraRotateToFaceDirection()
        {
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(base._runtimeID))
            {
                Singleton<CameraManager>.Instance.GetMainCamera().SetRotateToFaceDirection();
            }
        }

        public virtual void TriggerAppear()
        {
            base._animator.SetTrigger("TriggerAppear");
        }

        public void TriggerAttack()
        {
            this.SetTrigger("TriggerAttack");
            this.ResetTrigger("TriggerWeapon");
        }

        public override void TriggerAttackPattern(string animEventID, LayerMask layerMask)
        {
            ConfigAvatarAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(this.config, animEventID);
            if ((event2.CameraShake != null) && event2.CameraShake.ShakeOnNotHit)
            {
                AttackPattern.ActCameraShake(event2.CameraShake);
            }
            if (event2.AttackPattern != null)
            {
                event2.AttackPattern.patternMethod(animEventID, event2.AttackPattern, this, layerMask);
            }
        }

        [AnimationCallback]
        public void TriggerAttackScreenShake(string attackName)
        {
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(base._runtimeID))
            {
                AttackPattern.ActCameraShake(SharedAnimEventData.ResolveAnimEvent(this.config, attackName).CameraShake);
            }
        }

        private void TriggerAvatarInstantSkill(string skillID)
        {
            ConfigAvatarSkill skill = this.config.Skills[skillID];
            string instantTriggerEventName = this.GetInstantTriggerEventName(skillID);
            if ((skill != null) && !string.IsNullOrEmpty(instantTriggerEventName))
            {
                this.AnimEventHandler(instantTriggerEventName);
                switch (skill.SkillType)
                {
                    case AvatarSkillType.AttackStart:
                        Singleton<EventManager>.Instance.FireEvent(new EvtAttackStart(base.GetRuntimeID(), skillID), MPEventDispatchMode.Normal);
                        break;

                    case AvatarSkillType.SkillStart:
                        Singleton<EventManager>.Instance.FireEvent(new EvtSkillStart(base.GetRuntimeID(), skillID), MPEventDispatchMode.Normal);
                        break;
                }
            }
        }

        [AnimationCallback]
        public void TriggerCameraAnimation(string cameraAnimName)
        {
            this.PlayAvatarCameraAnimation(cameraAnimName, MainCameraFollowState.EnterPolarMode.NearestPointOnSphere, true);
        }

        [AnimationCallback]
        private void TriggerCameraPullFar(float time)
        {
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(base._runtimeID))
            {
                Singleton<CameraManager>.Instance.GetMainCamera().SetTimedPullZ(1.3f, 0f, 0f, 0f, time, 0f, string.Empty, false);
            }
        }

        [AnimationCallback]
        private void TriggerCameraPullFurther(float time)
        {
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(base._runtimeID))
            {
                Singleton<CameraManager>.Instance.GetMainCamera().SetTimedPullZ(1.9f, 0f, 0f, 0f, time, 0f, string.Empty, false);
            }
        }

        [AnimationCallback]
        private void TriggerCameraPushNear(float time)
        {
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(base._runtimeID))
            {
                Singleton<CameraManager>.Instance.GetMainCamera().SetTimedPullZ(0.8f, 0f, 0f, 0f, time, 0f, string.Empty, false);
            }
        }

        [AnimationCallback]
        protected void TriggerFaceAnimation(string name)
        {
            if (this._faceAnimation != null)
            {
                this._faceAnimation.PlayFaceAnimation(name, FaceAnimationPlayMode.Clamp);
            }
        }

        public void TriggerHoldAttack()
        {
            base._animator.SetTrigger("TriggerHoldAttack");
            this.ResetTrigger("TriggerWeapon");
        }

        public virtual void TriggerSkill(int skillNum)
        {
            base._animator.ResetTrigger("TriggerAttack");
            string skillIDBySkillNum = this.GetSkillIDBySkillNum(skillNum);
            if (this.CanUseSkill(skillIDBySkillNum))
            {
                if (this.IsSkillInstantTrigger(skillIDBySkillNum))
                {
                    if (skillIDBySkillNum == "SKL_WEAPON")
                    {
                        this.TriggerWeaponInstantSkill(skillIDBySkillNum);
                    }
                    else
                    {
                        this.TriggerAvatarInstantSkill(skillIDBySkillNum);
                    }
                }
                else
                {
                    string skillTriggerBySkillNum = this.GetSkillTriggerBySkillNum(skillNum);
                    this.SetTrigger(skillTriggerBySkillNum);
                    if (skillTriggerBySkillNum == "TriggerWeapon")
                    {
                        bool flag = true;
                        if ((this.CurrentSkillID != null) && (this.config.Skills[this.CurrentSkillID].SkillType == AvatarSkillType.AttackStart))
                        {
                            flag = false;
                        }
                        if (flag)
                        {
                            this.SetTrigger("TriggerAttack");
                        }
                    }
                }
            }
        }

        public virtual void TriggerSwitchIn()
        {
            if (this.switchState == AvatarSwitchState.SwitchingOut)
            {
                base._animator.Play(this.config.StateMachinePattern.SwitchInAnimatorStateHash, 0);
            }
            else
            {
                base._animator.SetTrigger("TriggerSwitchIn");
            }
        }

        public virtual void TriggerSwitchOut(AvatarSwapOutType swapOutType)
        {
            if (swapOutType == AvatarSwapOutType.Force)
            {
                base._animator.Play(this.config.StateMachinePattern.SwitchOutAnimatorStateHash, 0);
            }
            else if (swapOutType == AvatarSwapOutType.Normal)
            {
                base._animator.SetTrigger("TriggerSwitchOut");
            }
            else if (swapOutType == AvatarSwapOutType.Delayed)
            {
                base._animator.SetTrigger("TriggerSwitchOut");
                this._delayedSwapOutTriggered = true;
            }
        }

        [AnimationCallback]
        private void TriggerTintCamera(float duration)
        {
            base.TriggerTint("Effect_Tint", duration, 0.5f);
        }

        private void TriggerWeaponInstantSkill(string skillID)
        {
            this.StartWeaponSkillAbility();
            Singleton<EventManager>.Instance.FireEvent(new EvtSkillStart(base.GetRuntimeID(), skillID), MPEventDispatchMode.Normal);
        }

        protected override void Update()
        {
            base.Update();
            this.UpdateControl();
            this.UpdateAttackSpeed();
            this.UpdateFaceAnimation();
            if (!this._isShadowColorAdjusted || Application.isEditor)
            {
                this.AdjustShadowColors();
            }
        }

        private void UpdateAttackSpeed()
        {
            if (this._attackSpeedState == AttackSpeedState.WaitingAttackTimeStart)
            {
                if (this.GetCurrentNormalizedTime() > this.config.Skills[this.CurrentSkillID].AttackNormalizedTimeStart)
                {
                    this.SetPropertyByStackIndex("Animator_OverallSpeedRatio", this._attackSpeedTimeScaleIx, this.GetProperty("Entity_AttackSpeed"));
                    this._attackSpeedState = AttackSpeedState.DuringAttackTime;
                }
            }
            else if ((this._attackSpeedState == AttackSpeedState.DuringAttackTime) && (this.GetCurrentNormalizedTime() > this.config.Skills[this.CurrentSkillID].AttackNormalizedTimeStop))
            {
                this.SetPropertyByStackIndex("Animator_OverallSpeedRatio", this._attackSpeedTimeScaleIx, 0f);
                this._attackSpeedState = AttackSpeedState.AttackTimeEnded;
            }
        }

        protected virtual void UpdateControl()
        {
            if (this._inputController.active)
            {
                this._inputController.Core();
                this._controlData = this._inputController.controlData;
            }
            if (this._aiController.active)
            {
                this._aiController.Core();
                if (!this._controlData.hasAnyControl)
                {
                    this._controlData = this._aiController.controlData;
                }
            }
            this._hasUpdatedControlThisFrame = true;
            if (this._muteControlCount <= 0)
            {
                if (this._controlData.hasOrderMove)
                {
                    this.OrderMove = this._controlData.orderMove;
                }
                if (this._controlData.hasSetAttackTarget)
                {
                    this.SetAttackTarget(this._controlData.attackTarget);
                }
                if ((this._controlData.hasSteer && !this.IsAnimatorInTag(AvatarData.AvatarTagGroup.MuteJoyStickInput)) && (!this.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackTargetLeadDirection) && base.IsWaitTransitionUnactive(this._muteSteerIx)))
                {
                    this.SteerFaceDirectionTo(Vector3.Lerp(base.FaceDirection, this._controlData.steerDirection, ((this._steerLerpRatio * this._controlData.lerpRatio) * Time.deltaTime) * this.TimeScale));
                }
                if (this.IsAnimatorInTag(AvatarData.AvatarTagGroup.AllowTriggerInput))
                {
                    if (this._controlData.useAttack)
                    {
                        this.TriggerAttack();
                    }
                    if (this._controlData.useHoldAttack)
                    {
                        this.TriggerHoldAttack();
                    }
                    for (int i = 1; i < this._controlData.useSkills.Length; i++)
                    {
                        if (this._controlData.useSkills[i])
                        {
                            this.TriggerSkill(i);
                        }
                    }
                }
                if (((base.IsWaitTransitionUnactive(this._muteSteerIx) && base.IsWaitTransitionUnactive(this._muteLockAttackTargetIx)) && (this.IsAnimatorInTag(AvatarData.AvatarTagGroup.AttackTargetLeadDirection) && (this.AttackTarget != null))) && this.AttackTarget.IsActive())
                {
                    Vector3 forward = this.AttackTarget.XZPosition - this.XZPosition;
                    base.SteerFaceDirectionTo(forward);
                }
                else if (this._activeEnterSteerState > SkillEnterSteerState.Idle)
                {
                    if (this._activeEnterSteerState == SkillEnterSteerState.WaitingForStart)
                    {
                        float currentNormalizedTime = this.GetCurrentNormalizedTime();
                        if (currentNormalizedTime > this._activeEnterSteerOption.MaxSteerNormalizedTimeEnd)
                        {
                            this._activeEnterSteerState = SkillEnterSteerState.Idle;
                            this._activeEnterSteerOption = null;
                        }
                        else if (currentNormalizedTime > this._activeEnterSteerOption.MaxSteerNormalizedTimeStart)
                        {
                            this._activeEnterSteerState = SkillEnterSteerState.Steering;
                        }
                    }
                    else if (this._activeEnterSteerState == SkillEnterSteerState.Steering)
                    {
                        if (this.GetCurrentNormalizedTime() > this._activeEnterSteerOption.MaxSteerNormalizedTimeEnd)
                        {
                            this._activeEnterSteerState = SkillEnterSteerState.Idle;
                            this._activeEnterSteerOption = null;
                        }
                        else
                        {
                            Vector3 normalized;
                            if ((this.AttackTarget != null) && this.AttackTarget.IsActive())
                            {
                                Vector3 vector3 = this.AttackTarget.XZPosition - this.XZPosition;
                                normalized = vector3.normalized;
                                this.SteerFaceDirectionTo(Vector3.Slerp(base.FaceDirection, normalized, (this._activeEnterSteerOption.SteerLerpRatio * this.TimeScale) * Time.deltaTime));
                            }
                            else if (this._controlData.hasSteer)
                            {
                                normalized = this.CalculateSteerTargetForwardWithOption(this._controlData.steerDirection, this._activeEnterSteerClampStart, this._activeEnterSteerOption);
                                this.SteerFaceDirectionTo(Vector3.Slerp(base.FaceDirection, normalized, (this._activeEnterSteerOption.SteerLerpRatio * this.TimeScale) * Time.deltaTime));
                            }
                        }
                    }
                }
                if ((this.AttackTarget != null) && !this.AttackTarget.IsActive())
                {
                    this.SetAttackTarget(null);
                }
                else if (this._clearAttackTargetTimer > 0f)
                {
                    this._clearAttackTargetTimer -= Time.deltaTime * this.TimeScale;
                    if (this._clearAttackTargetTimer <= 0f)
                    {
                        this.SetAttackTarget(null);
                    }
                }
            }
        }

        private void UpdateFaceAnimation()
        {
            if (this._faceAnimation != null)
            {
                this._faceAnimation.Process(Time.deltaTime);
            }
        }

        private void UpdatePlaySoundOnAnimatorStateChanged(AnimatorStateInfo fromState, AnimatorStateInfo toState)
        {
            if (((fromState.tagHash != AvatarData.AVATAR_MOVESUB_TAG) && (toState.tagHash == AvatarData.AVATAR_MOVESUB_TAG)) && ((this._waitMoveSoundCoroutine == null) && base.gameObject.activeInHierarchy))
            {
                this._waitMoveSoundCoroutine = base.StartCoroutine(this.WaitPlayMoveSound());
            }
            if (((fromState.tagHash == AvatarData.AVATAR_MOVESUB_TAG) && (toState.tagHash != AvatarData.AVATAR_MOVESUB_TAG)) && ((this._waitMoveSoundCoroutine != null) && base.gameObject.activeInHierarchy))
            {
                this.StopMoveSoundCoroutine();
            }
        }

        private void UploadFaceTexture()
        {
            if (this.leftEyeRenderer != null)
            {
                this.leftEyeRenderer.sharedMaterial.mainTexture = this.leftEyeRenderer.sharedMaterial.mainTexture;
            }
            if (this.rightEyeRenderer != null)
            {
                this.rightEyeRenderer.sharedMaterial.mainTexture = this.rightEyeRenderer.sharedMaterial.mainTexture;
            }
            if (this.mouthRenderer != null)
            {
                this.mouthRenderer.sharedMaterial.mainTexture = this.mouthRenderer.sharedMaterial.mainTexture;
            }
        }

        [DebuggerHidden]
        private IEnumerator WaitPlayMoveSound()
        {
            return new <WaitPlayMoveSound>c__Iterator1C { <>f__this = this };
        }

        public BaseMonoEntity AttackTarget
        {
            get
            {
                return this._attackTarget;
            }
        }

        public uint AvatarTypeID { get; private set; }

        public string AvatarTypeName { get; private set; }

        public override string CurrentSkillID
        {
            get
            {
                return this._currentSkillID;
            }
        }

        public int EquipedWeaponID
        {
            get
            {
                return this._equipedWeaponID;
            }
        }

        public bool isLeader { get; set; }

        public bool IsLockDirection
        {
            get
            {
                return this._isLockDirection;
            }
            set
            {
                bool flag = false;
                if (value != this._isLockDirection)
                {
                    flag = true;
                }
                this._isLockDirection = value;
                if ((this.onLockDirectionChanged != null) && flag)
                {
                    this.onLockDirectionChanged(value);
                }
            }
        }

        public float MoveSpeedRatio
        {
            set
            {
                float aniMinSpeedRatio = this.config.StateMachinePattern.AniMinSpeedRatio;
                float aniMaxSpeedRatio = this.config.StateMachinePattern.AniMaxSpeedRatio;
                this._moveSpeedRatio = Mathf.Clamp(value, aniMinSpeedRatio, aniMaxSpeedRatio);
                this.SyncAnimatorMoveSpeed();
            }
        }

        public bool OrderMove
        {
            get
            {
                return base.GetLocomotionBool("IsMove");
            }
            set
            {
                base.SetLocomotionBool("IsMove", value, true);
            }
        }

        public List<BaseMonoEntity> SubAttackTargetList
        {
            get
            {
                return this._subAttackTargetList;
            }
        }

        public AvatarSwitchState switchState { get; private set; }

        public override float TimeScale
        {
            get
            {
                if (base.gameObject.activeSelf)
                {
                    return base.TimeScale;
                }
                return Singleton<LevelManager>.Instance.levelEntity.TimeScale;
            }
        }

        [CompilerGenerated]
        private sealed class <WaitPlayMoveSound>c__Iterator1C : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal BaseMonoAvatar <>f__this;
            internal float <beginTime>__1;
            internal MonoEntityAudio <entityAudio>__2;
            internal int <random>__0;

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
                        this.<random>__0 = UnityEngine.Random.Range(3, 11);
                        this.<beginTime>__1 = 0f;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00C2;
                }
                if (this.<beginTime>__1 <= this.<random>__0)
                {
                    this.<beginTime>__1 += Time.deltaTime * this.<>f__this.TimeScale;
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.<entityAudio>__2 = this.<>f__this.GetComponent<MonoEntityAudio>();
                if (this.<entityAudio>__2 != null)
                {
                    this.<entityAudio>__2.PostMove();
                }
                this.<>f__this._waitMoveSoundCoroutine = null;
                this.$PC = -1;
            Label_00C2:
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

        private enum AttackSpeedState
        {
            Idle,
            WaitingAttackTimeStart,
            DuringAttackTime,
            AttackTimeEnded
        }

        public enum AvatarSwapOutType
        {
            Normal,
            Force,
            Delayed
        }

        public enum AvatarSwitchState
        {
            OnStage,
            OffStage,
            SwitchingOut
        }

        private class ShadowColorAdjuster
        {
            private Color _first;
            private Material _material;
            private Color _second;

            public ShadowColorAdjuster(Material material)
            {
                this._material = material;
                this._first = material.GetColor("_FirstShadowMultColor");
                this._second = material.GetColor("_SecondShadowMultColor");
            }

            public void Apply(float factor)
            {
                this._material.SetColor("_FirstShadowMultColor", Color.Lerp(this._first, Color.white, factor));
                this._material.SetColor("_SecondShadowMultColor", Color.Lerp(this._second, Color.white, factor));
            }
        }

        private enum SkillEnterSteerState
        {
            Idle,
            WaitingForStart,
            Steering
        }
    }
}

