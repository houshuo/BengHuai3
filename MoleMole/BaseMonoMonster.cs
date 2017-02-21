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

    public abstract class BaseMonoMonster : BaseMonoAnimatorEntity, IAIEntity, IFadeOff, IFrameHaltable, IRetreatable, IAttacker
    {
        protected BaseMonsterAIController _aiController;
        private AttackSpeedState _attackSpeedState;
        private int _attackSpeedTimeScaleIx;
        private BaseMonoEntity _attackTarget;
        private bool _canMoveHorizontal;
        private bool _checkOutsideWall;
        private Coroutine _checkOutsideWallCoroutine;
        private string _currentSkillID;
        private Coroutine _fastDieCoroutine;
        private bool _hasSteeredThisFrame;
        private float _inactiveTimer = -1f;
        private bool _isAlive;
        private bool _isToBeRemove;
        private Transform _mainCameraTrans;
        private int _monsterTagID;
        private float _moveSpeedRatio;
        private int _muteControlCount;
        private int _muteSteerIx;
        private int _noCollisionCount;
        private float _originalMass;
        private Dictionary<int, string> _patternMap = new Dictionary<int, string>();
        private bool _preloaded;
        protected RetreatPlugin _retreatPlugin;
        protected float _standBySteerTime;
        private bool _usingThrowMass;
        private bool _usingTransparentShader;
        private Coroutine _waitHitDieCoroutine;
        private int _walkSteerStateHash;
        protected const string ABS_MOVE_SPEED_PARAM = "AbsMoveSpeed";
        public MonoBodyPartEntity[] bodyParts;
        public ConfigMonster config;
        protected const string DAMAGE_RATIO_PARAM = "DamageRatio";
        public DestroyMode destroyMode;
        protected const string DIE_PARAM = "IsDead";
        protected const string HIT_EFFECT_AUX_PARAM = "HitEffectAux";
        protected const string HIT_TIME_OFFSET_RATIO_PARAM = "HitTimeOffsetRatio";
        public Collider hitbox;
        protected const string IS_HEAVY_RETREAT = "IsHeavyRetreat";
        protected const string IS_STANDBY_WALK_STEER_PARAM = "IsStandByWalkSteer";
        [NonSerialized]
        public bool isStaticInScene;
        protected const int MONSTER_BASE_SHADER_IX = 0;
        protected const int MONSTER_ELITE_SHADER_IX = 1;
        protected const string MOVE_HORIZONTAL_PARAM = "IsMoveHorizontal";
        protected const string MOVE_SPEED_PARAM = "MoveSpeed";
        public Action<BaseMonoEntity> onAttackTargetChanged;
        public Action<BaseMonoMonster> onDie;
        public Action<BaseMonoMonster, bool, bool> onHitStateChanged;
        protected const string ORDER_MOVE_PARAM = "IsMove";
        protected const string TRIGGER_HIT_PARAM = "HitTrigger";
        protected const string TRIGGER_LEVEL_END_LOSE = "LevelLose";
        protected const string TRIGGER_LEVEL_END_WIN = "LevelWin";
        protected const string TRIGGER_LIGHT_HIT_PARAM = "LightHitTrigger";
        protected const string TRIGGER_THROW_BLOW_PARAM = "ThrowBlowTrigger";
        protected const string TRIGGER_THROW_DOWN_PARAM = "ThrowDownTrigger";
        protected const string TRIGGER_THROW_PARAM = "ThrowTrigger";

        public event AnimatedHitBoxCreatedHandler onAnimatedHitBoxCreatedCallBack;

        protected BaseMonoMonster()
        {
        }

        [AnimationCallback]
        public override void AnimEventHandler(string animEventID)
        {
            ConfigMonsterAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(this.config, animEventID);
            if (((event2 != null) && ((base._maskedAnimEvents == null) || !base._maskedAnimEvents.Contains(animEventID))) && (base._animEventPredicates.Contains(event2.Predicate) && base._animEventPredicates.Contains(event2.Predicate2)))
            {
                if (event2.TriggerAbility != null)
                {
                    EvtAbilityStart evt = new EvtAbilityStart(base._runtimeID, null) {
                        abilityID = event2.TriggerAbility.ID,
                        abilityName = event2.TriggerAbility.Name,
                        abilityArgument = event2.TriggerAbility.Argument
                    };
                    Singleton<EventManager>.Instance.FireEvent(evt, MPEventDispatchMode.Normal);
                }
                if ((event2.CameraShake != null) && event2.CameraShake.ShakeOnNotHit)
                {
                    AttackPattern.ActCameraShake(event2.CameraShake);
                }
                if (event2.AttackPattern != null)
                {
                    if ((event2.AttackProperty == null) || (event2.AttackProperty.AttackTargetting == MixinTargetting.Enemy))
                    {
                        event2.AttackPattern.patternMethod(animEventID, event2.AttackPattern, this, AttackPattern.GetLayerMask(this));
                    }
                    else
                    {
                        event2.AttackPattern.patternMethod(animEventID, event2.AttackPattern, this, Singleton<EventManager>.Instance.GetAbilityHitboxTargettingMask(base._runtimeID, event2.AttackProperty.AttackTargetting));
                    }
                }
                if (event2.AttackHint != null)
                {
                    this.HandleAttackHint(event2.AttackHint);
                }
                if (event2.PhysicsProperty != null)
                {
                    ConfigEntityPhysicsProperty physicsProperty = event2.PhysicsProperty;
                    this.HandlePhysicsProperty(this.hitbox, physicsProperty);
                }
                if (event2.TriggerEffectPattern != null)
                {
                    base.TriggerEffectPattern(event2.TriggerEffectPattern.EffectPattern);
                }
                if (event2.TriggerTintCamera != null)
                {
                    base.TriggerTint(event2.TriggerTintCamera.RenderDataName, event2.TriggerTintCamera.Duration, event2.TriggerTintCamera.TransitDuration);
                }
                if ((event2.TimeSlow != null) && (event2.TimeSlow.Force || (((this.AttackTarget != null) && this.AttackTarget.IsActive()) && (Vector3.Distance(this.XZPosition, this.AttackTarget.XZPosition) < 2f))))
                {
                    Singleton<LevelManager>.Instance.levelActor.TimeSlow(event2.TimeSlow.Duration, event2.TimeSlow.SlowRatio, null);
                }
            }
        }

        private void ApplyAnimatorConfig()
        {
            foreach (string str in this.config.AnimatorConfig.Keys)
            {
                bool flag = this.config.AnimatorConfig[str];
                this.SetPersistentAnimatorBool(str, flag);
            }
        }

        protected override void ApplyAnimatorProperties()
        {
            base.ApplyAnimatorProperties();
            this.SyncAnimatorMoveSpeed();
        }

        private void AttachEffectOverrides()
        {
            if ((this.config.CommonArguments.EffectPredicates != null) && (this.config.CommonArguments.EffectPredicates.Length != 0))
            {
                MonoEffectOverride component = base.GetComponent<MonoEffectOverride>();
                if (component == null)
                {
                    component = base.gameObject.AddComponent<MonoEffectOverride>();
                }
                component.effectPredicates.AddRange(this.config.CommonArguments.EffectPredicates);
            }
        }

        public override void Awake()
        {
            base.Awake();
            this._originalMass = base._rigidbody.mass;
        }

        public virtual void BeHit(int frameHalt, AttackResult.AnimatorHitEffect hitEffect, AttackResult.AnimatorHitEffectAux hitEffectAux, KillEffect killEffect, BeHitEffect beHitEffect, float aniDamageRatio, Vector3 hitForward, float retreatVelocity)
        {
            bool flag;
            if (beHitEffect == BeHitEffect.KillingBeHit)
            {
                if (this.config.StateMachinePattern.FastDieEffectPattern == null)
                {
                    killEffect = KillEffect.KillNow;
                }
                if (killEffect == KillEffect.KillNow)
                {
                    if ((this.config.StateMachinePattern.ThrowBlowDieNamedState != null) && (((hitEffect == AttackResult.AnimatorHitEffect.ThrowUp) || (hitEffect == AttackResult.AnimatorHitEffect.ThrowUpBlow)) || (hitEffect == AttackResult.AnimatorHitEffect.ThrowBlow)))
                    {
                        base.SetLocomotionBool("IsHeavyRetreat", true, false);
                        this.BlowVelocityScaledRetreat(hitForward, retreatVelocity, this.config.StateMachinePattern.ThrowBlowDieNamedState);
                    }
                    flag = false;
                }
                else if (killEffect == KillEffect.KillFastWithNormalAnim)
                {
                    flag = true;
                }
                else if (killEffect == KillEffect.KillTillHitAnimationEnd)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else if (beHitEffect == BeHitEffect.OverkillBeHit)
            {
                if (killEffect == KillEffect.KillNow)
                {
                    if ((this.config.StateMachinePattern.ThrowBlowDieNamedState != null) && (((hitEffect == AttackResult.AnimatorHitEffect.ThrowUp) || (hitEffect == AttackResult.AnimatorHitEffect.ThrowUpBlow)) || (hitEffect == AttackResult.AnimatorHitEffect.ThrowBlow)))
                    {
                        base.SetLocomotionBool("IsHeavyRetreat", true, false);
                        this.BlowVelocityScaledRetreat(hitForward, retreatVelocity, this.config.StateMachinePattern.ThrowBlowDieNamedState);
                    }
                    flag = false;
                    this.KillDead(killEffect);
                }
                else if (killEffect == KillEffect.KillTillHitAnimationEnd)
                {
                    flag = true;
                    if (hitEffect == AttackResult.AnimatorHitEffect.Light)
                    {
                        hitEffect = AttackResult.AnimatorHitEffect.Normal;
                    }
                }
                else
                {
                    flag = false;
                    this.KillDead(killEffect);
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                if (((beHitEffect == BeHitEffect.KillingBeHit) && (beHitEffect == BeHitEffect.OverkillBeHit)) && (hitEffect <= AttackResult.AnimatorHitEffect.Light))
                {
                    hitEffect = AttackResult.AnimatorHitEffect.Normal;
                }
                if (hitEffect != AttackResult.AnimatorHitEffect.Mute)
                {
                    if (hitEffect == AttackResult.AnimatorHitEffect.Light)
                    {
                        this.SetTrigger("LightHitTrigger");
                        this.FrameHalt(frameHalt);
                    }
                    else if (hitEffect > AttackResult.AnimatorHitEffect.Light)
                    {
                        if (base.onBeHitCanceled != null)
                        {
                            base.onBeHitCanceled(this.CurrentSkillID);
                        }
                        bool flag2 = retreatVelocity > this.config.StateMachinePattern.HeavyRetreatThreshold;
                        base.SetLocomotionBool("IsHeavyRetreat", flag2, false);
                        base.SetLocomotionFloat("DamageRatio", aniDamageRatio, false);
                        base.SetLocomotionFloat("HitTimeOffsetRatio", UnityEngine.Random.value, false);
                        int num = (int) hitEffectAux;
                        if (this.config.StateMachinePattern.UseRandomLeftRightHitEffectAsNormal && (hitEffectAux == AttackResult.AnimatorHitEffectAux.Normal))
                        {
                            num = UnityEngine.Random.Range(1, 3);
                        }
                        if (this.config.StateMachinePattern.UseBackHitAngleCheck && (Vector3.Angle(base.FaceDirection, hitForward) < this.config.StateMachinePattern.BackHitDegreeThreshold))
                        {
                            num = 5;
                        }
                        if (this.config.StateMachinePattern.UseLeftRightHitAngleCheck)
                        {
                            bool flag3 = true;
                            if (Vector3.Angle(base.transform.right, hitForward) < this.config.StateMachinePattern.LeftRightHitAngleRange)
                            {
                                num = 7;
                                flag3 = false;
                            }
                            if (flag3 && (Vector3.Angle(-base.transform.right, hitForward) < this.config.StateMachinePattern.LeftRightHitAngleRange))
                            {
                                num = 6;
                            }
                        }
                        base._animator.SetInteger("HitEffectAux", num);
                        this.ResetTrigger("LightHitTrigger");
                        this.ResetTrigger("HitTrigger");
                        this.ResetTrigger("ThrowTrigger");
                        this.ResetTrigger("ThrowBlowTrigger");
                        this.ResetTrigger("ThrowDownTrigger");
                        this.SetTrigger("HitTrigger");
                        this.FrameHalt(frameHalt);
                        base.ClearSkillEffect(null);
                        base.CastWaitingAudioEvent();
                        Singleton<AuxObjectManager>.Instance.ClearAuxObjects<MonoAttackHint>(base.GetRuntimeID());
                        if (hitEffect == AttackResult.AnimatorHitEffect.ThrowUp)
                        {
                            this.SetTrigger("ThrowTrigger");
                            this.StandRetreat(hitForward, retreatVelocity);
                        }
                        else if (hitEffect == AttackResult.AnimatorHitEffect.ThrowUpBlow)
                        {
                            this.SetTrigger("ThrowTrigger");
                            if (this.config.StateMachinePattern.ThrowUpNamedState != null)
                            {
                                this.BlowDecelerateRetreat(hitForward, retreatVelocity, this.config.StateMachinePattern.ThrowUpNamedState, this.config.StateMachinePattern.ThrowUpNamedStateRetreatStopNormalizedTime);
                            }
                            else
                            {
                                this.StandRetreat(hitForward, retreatVelocity);
                            }
                        }
                        else if (hitEffect == AttackResult.AnimatorHitEffect.ThrowDown)
                        {
                            this.SetTrigger("ThrowDownTrigger");
                            this.StandRetreat(hitForward, retreatVelocity);
                        }
                        else if (hitEffect == AttackResult.AnimatorHitEffect.ThrowBlow)
                        {
                            if (this.config.StateMachinePattern.ThrowBlowNamedState != null)
                            {
                                this.SetTrigger("ThrowBlowTrigger");
                                this.BlowVelocityScaledRetreat(hitForward, retreatVelocity, this.config.StateMachinePattern.ThrowBlowNamedState);
                            }
                            else
                            {
                                this.StandRetreat(hitForward, retreatVelocity);
                            }
                        }
                        else if (hitEffect == AttackResult.AnimatorHitEffect.ThrowAirBlow)
                        {
                            if ((this.config.StateMachinePattern.ThrowBlowAirNamedState != null) && this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
                            {
                                this.SetTrigger("ThrowBlowTrigger");
                                this.BlowDecelerateRetreat(hitForward, retreatVelocity, this.config.StateMachinePattern.ThrowBlowAirNamedState, this.config.StateMachinePattern.ThrowBlowAirNamedStateRetreatStopNormalizedTime);
                            }
                            else
                            {
                                this.StandRetreat(hitForward, retreatVelocity);
                            }
                        }
                        else if ((hitEffect == AttackResult.AnimatorHitEffect.KnockDown) && (this is BaseMonoDarkAvatar))
                        {
                            this.SetTrigger("TriggerKnockDown");
                            this.StandRetreat(hitForward, retreatVelocity);
                        }
                        else if (hitEffect == AttackResult.AnimatorHitEffect.FaceAttacker)
                        {
                            this.SetOverrideSteerFaceDirectionFrame(-hitForward);
                            this.StandRetreat(hitForward, retreatVelocity);
                        }
                        else
                        {
                            this.StandRetreat(hitForward, retreatVelocity);
                        }
                    }
                }
            }
        }

        protected virtual void BlowDecelerateRetreat(Vector3 retreatDir, float retreatVelocity, string namedState, float endNormalizedTime)
        {
            this.SetOverrideSteerFaceDirectionFrame(-retreatDir);
            if (retreatVelocity != 0f)
            {
                this._retreatPlugin.BlowDecelerateRetreat(retreatDir, retreatVelocity, namedState, this.config.StateMachinePattern.RetreatBlowVelocityRatio, endNormalizedTime);
            }
        }

        protected virtual void BlowVelocityScaledRetreat(Vector3 retreatDir, float retreatVelocity, string namedState)
        {
            this.SetOverrideSteerFaceDirectionFrame(-retreatDir);
            if (retreatVelocity != 0f)
            {
                this._retreatPlugin.BlowVelocityScaledRetreat(retreatDir, retreatVelocity, namedState, retreatVelocity * this.config.StateMachinePattern.RetreatToVelocityScaleRatio);
            }
        }

        private void CheckHitStateChanged(AnimatorStateInfo fromState, AnimatorStateInfo toState)
        {
            bool flag = this.IsAnimatorInTag(MonsterData.MonsterTagGroup.ShowHit, fromState) || this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw, fromState);
            bool flag2 = this.IsAnimatorInTag(MonsterData.MonsterTagGroup.ShowHit, toState) || this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw, toState);
            if ((flag != flag2) && (this.onHitStateChanged != null))
            {
                this.onHitStateChanged(this, flag, flag2);
            }
        }

        [DebuggerHidden]
        private IEnumerator CheckOutsideWall()
        {
            return new <CheckOutsideWall>c__Iterator27 { <>f__this = this };
        }

        public override void CleanOwnedObjects()
        {
            base.CleanOwnedObjects();
            Singleton<AuxObjectManager>.Instance.ClearAuxObjects<MonoAttackHint>(base.GetRuntimeID());
        }

        public override void ClearAttackTarget()
        {
            this.ClearAttackTriggers();
            this.SetAttackTarget(null);
        }

        public override void ClearAttackTriggers()
        {
            if (this.IsActive())
            {
                foreach (string str in this.config.Skills.Keys)
                {
                    base._animator.ResetTrigger(str);
                }
            }
        }

        public void ClearHitTrigger()
        {
            base._animator.ResetTrigger("HitTrigger");
        }

        public void ClearSoleAnimatorEventPattern(int stateHash)
        {
            if (this._patternMap.ContainsKey(stateHash))
            {
                base.DetachAnimatorEventPattern(stateHash, this._patternMap[stateHash]);
                this._patternMap.Remove(stateHash);
            }
        }

        public void ClearSoleSkillAnimatorEventPattern(string skillName)
        {
            if (this.config.Skills.ContainsKey(skillName))
            {
                string[] animatorStateNames = this.config.Skills[skillName].AnimatorStateNames;
                if (animatorStateNames != null)
                {
                    int index = 0;
                    int length = animatorStateNames.Length;
                    while (index < length)
                    {
                        this.ClearSoleAnimatorEventPattern(Animator.StringToHash(animatorStateNames[index]));
                        index++;
                    }
                }
            }
        }

        private BaseMonsterAIController CreateAIController()
        {
            object[] args = new object[] { this };
            return (BaseMonsterAIController) Activator.CreateInstance(System.Type.GetType("MoleMole." + this.AIModeName + "MonsterAIController"), args);
        }

        public override void DeadHandler()
        {
            if (this.config.StateMachinePattern.DieAnimEventID != null)
            {
                this.AnimEventHandler(this.config.StateMachinePattern.DieAnimEventID);
            }
            this.SetDestroy();
        }

        private void DieNow(KillEffect killEffect)
        {
            if (this.onDie != null)
            {
                this.onDie(this);
            }
            if (killEffect != KillEffect.KillImmediately)
            {
                float killFastDuration = 0f;
                MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(base.GetRuntimeID());
                AbilityState none = AbilityState.None;
                if (this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw) && (this.config.StateMachinePattern.ThrowDieEffectPattern != null))
                {
                    killEffect = KillEffect.KillFastWithNormalAnim;
                    killFastDuration = 0.1f;
                }
                else if ((actor != null) && actor.abilityState.ContainsState(AbilityState.Frozen))
                {
                    killEffect = KillEffect.KillFastWithNormalAnim;
                    killFastDuration = 0.1f;
                    none = actor.abilityState;
                }
                else
                {
                    killFastDuration = this.config.StateMachinePattern.FastDieAnimationWaitDuration;
                }
                if (killEffect == KillEffect.KillNow)
                {
                    base.ResetAllTriggers();
                    this.SetTrigger("IsDead");
                    base.MaskAllTriggers(true);
                }
                else if (killEffect == KillEffect.KillFastWithDieAnim)
                {
                    base.ResetAllTriggers();
                    this.SetTrigger("IsDead");
                    base.MaskAllTriggers(true);
                    this._fastDieCoroutine = base.StartCoroutine(this.FastDieIter(killFastDuration, none));
                }
                else if (killEffect == KillEffect.KillFastWithNormalAnim)
                {
                    this._fastDieCoroutine = base.StartCoroutine(this.FastDieIter(killFastDuration, none));
                }
                else if (killEffect == KillEffect.KillFastImmediately)
                {
                    this._fastDieCoroutine = base.StartCoroutine(this.FastDieIter(0f, none));
                }
                this.hitbox.enabled = false;
                for (int i = 0; i < this.bodyParts.Length; i++)
                {
                    this.bodyParts[i].hitbox.enabled = false;
                }
                if (this.config.CommonArguments.HitboxInactiveDelay > 0f)
                {
                    this._inactiveTimer = this.config.CommonArguments.HitboxInactiveDelay;
                }
                else
                {
                    base.gameObject.layer = InLevelData.INACTIVE_ENTITY_LAYER;
                }
            }
        }

        private void FadeInHandler(float duration)
        {
        }

        private void FadeOutHandler(float duration)
        {
        }

        [DebuggerHidden]
        private IEnumerator FastDieIter(float killFastDuration, AbilityState abilityState = 0)
        {
            return new <FastDieIter>c__Iterator25 { killFastDuration = killFastDuration, abilityState = abilityState, <$>killFastDuration = killFastDuration, <$>abilityState = abilityState, <>f__this = this };
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        protected override void FixedUpdatePlugins()
        {
            this._retreatPlugin.FixedCore();
        }

        public IAIController GetActiveAIController()
        {
            return this._aiController;
        }

        public List<BaseMonoAbilityEntity> GetAllHitboxEnabledBodyParts()
        {
            List<BaseMonoAbilityEntity> list = new List<BaseMonoAbilityEntity>();
            foreach (MonoBodyPartEntity entity in this.bodyParts)
            {
                if (entity.hitbox.enabled)
                {
                    list.Add(entity);
                }
            }
            return list;
        }

        public int GetAnimatorTag()
        {
            return base._animator.GetCurrentAnimatorStateInfo(0).tagHash;
        }

        public override BaseMonoEntity GetAttackTarget()
        {
            return this.AttackTarget;
        }

        public Vector3 GetDropPosition()
        {
            return ((base.RootNodePosition.y <= 1f) ? new Vector3(base.RootNodePosition.x, 1f, base.RootNodePosition.z) : new Vector3(base.RootNodePosition.x, base.RootNodePosition.y, base.RootNodePosition.z));
        }

        public float GetOriginMoveSpeed(string moveSpeedKey)
        {
            float moveSpeedRatio = 1f;
            if (this.uniqueMonsterID != 0)
            {
                moveSpeedRatio = MonsterData.GetUniqueMonsterMetaData(this.uniqueMonsterID).moveSpeedRatio;
            }
            return (((float) this.config.DynamicArguments[moveSpeedKey]) * moveSpeedRatio);
        }

        private void HandleAttackHint(ConfigMonsterAttackHint attackHintConfig)
        {
            string name = string.Empty;
            if (attackHintConfig is RectAttackHint)
            {
                name = "RectAttackHint";
            }
            else if (attackHintConfig is CircleAttackHint)
            {
                name = "CircleAttackHint";
            }
            else
            {
                if (!(attackHintConfig is SectorAttackHint))
                {
                    throw new Exception("Invalid Type or State!");
                }
                name = "SectorAttackHint";
            }
            Singleton<AuxObjectManager>.Instance.CreateAuxObject<MonoAttackHint>(name, base.GetRuntimeID()).Init(this, this.AttackTarget as BaseMonoAnimatorEntity, attackHintConfig);
        }

        public void Init(string monsterName, string typeName, uint runtimeID, Vector3 initPos, uint uniqueMonsterID = 0, string overrideAIName = null, bool checkOutsideWall = true, bool isElite = false, bool disableBehaviorWhenInit = false, int monsterTagID = 0)
        {
            if (!this._preloaded)
            {
                this.MonsterName = monsterName;
                this.TypeName = typeName;
                this.uniqueMonsterID = uniqueMonsterID;
                string configType = string.Empty;
                if (uniqueMonsterID != 0)
                {
                    configType = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID).configType;
                }
                this.config = MonsterData.GetMonsterConfig(monsterName, typeName, configType);
                base.animatorConfig = this.config;
                base.commonConfig = this.config.CommonConfig;
                base.Init(runtimeID);
            }
            else
            {
                base._runtimeID = runtimeID;
            }
            this._isAlive = true;
            this._monsterTagID = monsterTagID;
            initPos.y += this.config.CommonArguments.CreatePosYOffset;
            LayerMask mask = (((int) 1) << InLevelData.AVATAR_LAYER) | (((int) 1) << InLevelData.MONSTER_LAYER);
            initPos = base.PickInitPosition(mask, initPos, this.config.CommonArguments.CollisionRadius);
            base.transform.position = initPos;
            base._animEventPredicates.Add(this.config.CommonArguments.DefaultAnimEventPredicate);
            if (!this._preloaded)
            {
                string aIName = string.Empty;
                if (overrideAIName != null)
                {
                    aIName = overrideAIName;
                }
                else if (uniqueMonsterID != 0)
                {
                    aIName = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID).AIName;
                }
                else
                {
                    aIName = MonsterData.GetMonsterConfigMetaData(monsterName, typeName).AIName;
                }
                this.InitController(aIName, disableBehaviorWhenInit);
                this.InitPlugins();
                this.InitSkillAnimatorEventPattern();
                this.AttachEffectOverrides();
            }
            this.InitBodyParts();
            this.MoveSpeedRatio = 1f;
            this._attackSpeedState = AttackSpeedState.Idle;
            this._attackSpeedTimeScaleIx = this.PushProperty("Animator_OverallSpeedRatio", 0f);
            base.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(base.onCurrentSkillIDChanged, new Action<string, string>(this.OnSkillIDChanged));
            base.onIsGhostChanged = (Action<bool>) Delegate.Combine(base.onIsGhostChanged, new Action<bool>(this.OnIsGhostChanged));
            base.RegisterPropertyChangedCallback("Entity_AttackSpeed", new Action(this.OnAttackSpeedChanged));
            this._muteSteerIx = base.AddWaitTransitionState();
            this._canMoveHorizontal = base._animator.HasParameter("IsMoveHorizontal");
            this.PostInit();
            this.InitUniqueMonsterAndElite(isElite);
            this.ApplyAnimatorConfig();
            this._mainCameraTrans = Singleton<CameraManager>.Instance.GetMainCamera().transform;
            if (GlobalVars.ENABLE_CONTINUOUS_DETECT_MODE)
            {
                base._rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            }
            this.InitBornSound();
            this._checkOutsideWall = checkOutsideWall;
            if (this._checkOutsideWall)
            {
                base.StartCoroutine(this.CheckOutsideWall());
            }
        }

        private void InitBodyParts()
        {
            foreach (MonoBodyPartEntity entity in this.bodyParts)
            {
                entity.Init(Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(4), this);
            }
        }

        private void InitBornSound()
        {
            if ((this.uniqueMonsterID == 0) && !(this is BaseMonoBoss))
            {
                MonoEntityAudio component = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetComponent<MonoEntityAudio>();
                if (component != null)
                {
                    component.PostMonsterBorn();
                }
            }
        }

        protected void InitController(string AIName, bool disableBehaviorWhenInit)
        {
            this._aiController = new BTreeMonsterAIController(this, AIName, disableBehaviorWhenInit);
            if (!disableBehaviorWhenInit)
            {
                ((BTreeMonsterAIController) this._aiController).EnableBehavior();
            }
            if (this.uniqueMonsterID != 0)
            {
                UniqueMonsterMetaData uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(this.uniqueMonsterID);
                List<string> attackCDNames = uniqueMonsterMetaData.attackCDNames;
                List<float> attackCDs = uniqueMonsterMetaData.attackCDs;
                BTreeMonsterAIController controller = (BTreeMonsterAIController) this._aiController;
                for (int i = 0; i < attackCDNames.Count; i++)
                {
                    controller.SetBehaviorVariable<float>(attackCDNames[i], attackCDs[i]);
                }
            }
        }

        private void InitDynamicBone()
        {
            bool flag = GlobalVars.MONSTER_USE_DYNAMIC_BONE;
            foreach (DynamicBone bone in base.gameObject.GetComponentsInChildren<DynamicBone>())
            {
                bone.enabled = flag;
            }
        }

        protected override void InitPlugins()
        {
            base.InitPlugins();
            this._retreatPlugin = new RetreatPlugin(this);
        }

        private void InitSkillAnimatorEventPattern()
        {
            foreach (string str in this.config.Skills.Keys)
            {
                ConfigMonsterSkill skill = this.config.Skills[str];
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

        private void InitUniqueMonsterAndElite(bool isElite)
        {
            if (this.uniqueMonsterID != 0)
            {
                List<float> scale = MonsterData.GetUniqueMonsterMetaData(this.uniqueMonsterID).scale;
                if (scale.Count == 3)
                {
                    base.SetUniformScale(scale[0]);
                }
            }
            else if (isElite)
            {
                base.SetUniformScale(1.1f);
            }
            if (this.config.CommonArguments.UseEliteShader && ((this.uniqueMonsterID != 0) || isElite))
            {
                this.SetEliteShader();
                this.PostSetEliteMat();
            }
        }

        public override bool IsActive()
        {
            return (this._isAlive && base.gameObject.activeSelf);
        }

        public bool IsAIControllerActive()
        {
            return this._aiController.active;
        }

        public bool IsAnimatorInTag(MonsterData.MonsterTagGroup tagGroup)
        {
            return this.IsAnimatorInTag(tagGroup, base._animator.GetCurrentAnimatorStateInfo(0));
        }

        public bool IsAnimatorInTag(MonsterData.MonsterTagGroup tagGroup, AnimatorStateInfo stateInfo)
        {
            return MonsterData.MONSTER_TAG_GROUPS[(int) tagGroup].Contains(stateInfo.tagHash);
        }

        public bool isGoingToAttack(float deltaTime)
        {
            if (this.CurrentSkillID != null)
            {
                AnimatorStateInfo info = !base._animator.IsInTransition(0) ? base._animator.GetCurrentAnimatorStateInfo(0) : base._animator.GetNextAnimatorStateInfo(0);
                AnimationClip clip = !base._animator.IsInTransition(0) ? base._animator.GetCurrentAnimatorClipInfo(0)[0].clip : base._animator.GetNextAnimatorClipInfo(0)[0].clip;
                AnimationEvent[] events = clip.events;
                if (events.Length == 0)
                {
                    return false;
                }
                for (int i = 0; i < events.Length; i++)
                {
                    if ((events[i].functionName == "AnimEventHandler") && (events[i].stringParameter.IndexOf("Hint") < 0))
                    {
                        float num2 = clip.length * info.normalizedTime;
                        if ((num2 > (events[i].time - deltaTime)) && (num2 < events[i].time))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool IsMuteControl()
        {
            return (this._muteControlCount > 0);
        }

        public bool IsRetreating()
        {
            return this._retreatPlugin.IsActive();
        }

        public override bool IsToBeRemove()
        {
            return this._isToBeRemove;
        }

        public void KillDead(KillEffect killEffect)
        {
            if (this._waitHitDieCoroutine != null)
            {
                base.StopCoroutine(this._waitHitDieCoroutine);
                this._waitHitDieCoroutine = null;
                this.DieNow(killEffect);
            }
            else if (this._fastDieCoroutine != null)
            {
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();
            this._hasSteeredThisFrame = false;
            if (this.config.CommonArguments.UseSwitchShader && ((base._shaderStack == null) || (base._shaderStack.GetRealTopIndex() == 0)))
            {
                this.LateUpdateShader();
            }
        }

        protected void LateUpdateShader()
        {
            Vector3 lhs = base.transform.position - this._mainCameraTrans.position;
            float num = Vector3.Dot(lhs, this._mainCameraTrans.forward);
            if (this._usingTransparentShader)
            {
                if (num > this.config.CommonArguments.UseTransparentShaderDistanceThreshold)
                {
                    this.SwitchTransparentShader(false);
                    this._usingTransparentShader = false;
                }
            }
            else if (num < this.config.CommonArguments.UseTransparentShaderDistanceThreshold)
            {
                this.SwitchTransparentShader(true);
                this._usingTransparentShader = true;
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
            base._rigidbody.velocity = (Vector3) (base._rigidbody.velocity * (1f + this.GetProperty("Animator_RigidBodyVelocityRatio")));
        }

        protected override void OnAnimatorStateChanged(AnimatorStateInfo fromState, AnimatorStateInfo toState)
        {
            string str;
            string str2;
            string str3;
            string str4;
            base.OnAnimatorStateChanged(fromState, toState);
            this.CheckHitStateChanged(fromState, toState);
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
            if (this.config.StateMachinePattern.UseStandByWalkSteer)
            {
                if ((fromState.shortNameHash != this._walkSteerStateHash) && (toState.shortNameHash == this._walkSteerStateHash))
                {
                    this.SetNeedOverrideVelocity(true);
                    this.SetOverrideVelocity(Vector3.zero);
                }
                else if ((toState.shortNameHash != this._walkSteerStateHash) && (fromState.shortNameHash == this._walkSteerStateHash))
                {
                    this.SetNeedOverrideVelocity(false);
                }
            }
            base.ResetRigidbodyRotation();
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
            this.StopCheckOutsideWallCoroutine();
            base.OnDestroy();
            this.onAttackTargetChanged = null;
            this.onDie = null;
            if (this._aiController != null)
            {
                ((BTreeMonsterAIController) this._aiController).DisableBehavior();
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (this.config != null)
            {
                this.ApplyAnimatorProperties();
            }
        }

        private void OnIsGhostChanged(bool isGhost)
        {
            this.hitbox.enabled = !isGhost;
        }

        protected override void OnShaderStackChanged(Shader fromShader, int fromIx, Shader toShader, int toIx)
        {
            if (toIx == 0)
            {
                this.RecoverOriginalShaders();
            }
            else
            {
                base.OnShaderStackChanged(fromShader, fromIx, toShader, toIx);
            }
        }

        protected override void OnSkillEffectClear(string oldID, string skillID)
        {
            if (skillID != null)
            {
                ConfigMonsterSkill skill = this.config.Skills[skillID];
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
            if (skillID == null)
            {
                base.SetMass(this._originalMass);
            }
            else
            {
                base.SetMass(this._originalMass * this.config.Skills[skillID].MassRatio);
            }
        }

        public override void PopNoCollision()
        {
            this._noCollisionCount--;
            if (this._noCollisionCount == 0)
            {
                base.gameObject.layer = InLevelData.MONSTER_LAYER;
            }
        }

        protected override void PostInit()
        {
            if (!this._preloaded)
            {
                base.PostInit();
                this.InitDynamicBone();
            }
            if (this.config.StateMachinePattern.UseStandByWalkSteer)
            {
                this._walkSteerStateHash = Animator.StringToHash(this.config.StateMachinePattern.WalkSteerAnimatorStateName);
            }
        }

        public virtual void PostSetEliteMat()
        {
            for (int i = 0; i < base._matListForSpecailState.Count; i++)
            {
                Material material = base._matListForSpecailState[i].material;
                material.SetColor("_EliteColor1", this.config.EliteArguments.EliteColor1);
                material.SetColor("_EliteColor2", this.config.EliteArguments.EliteColor2);
                material.SetFloat("_EliteEmissionScaler1", this.config.EliteArguments.EliteEmissionScaler1);
                material.SetFloat("_EliteEmissionScaler2", this.config.EliteArguments.EliteEmissionScaler2);
                material.SetFloat("_EliteNormalDisplacement1", this.config.EliteArguments.EliteNormalDisplacement1);
                material.SetFloat("_EliteNormalDisplacement2", this.config.EliteArguments.EliteNormalDisplacement2);
            }
        }

        public void PreInit(string monsterName, string typeName, uint uniqueMonsterID = 0, bool disableBehaviorWhenInit = false)
        {
            this.MonsterName = monsterName;
            this.TypeName = typeName;
            this.uniqueMonsterID = uniqueMonsterID;
            string configType = string.Empty;
            UniqueMonsterMetaData uniqueMonsterMetaData = null;
            if (uniqueMonsterID != 0)
            {
                uniqueMonsterMetaData = MonsterData.GetUniqueMonsterMetaData(uniqueMonsterID);
                configType = uniqueMonsterMetaData.configType;
            }
            this.config = MonsterData.GetMonsterConfig(monsterName, typeName, configType);
            base.animatorConfig = this.config;
            base.commonConfig = this.config.CommonConfig;
            base.Init(0);
            string aIName = string.Empty;
            if (uniqueMonsterMetaData != null)
            {
                aIName = uniqueMonsterMetaData.AIName;
            }
            else
            {
                aIName = MonsterData.GetMonsterConfigMetaData(monsterName, typeName).AIName;
            }
            this.InitController(aIName, disableBehaviorWhenInit);
            this.InitPlugins();
            this.InitSkillAnimatorEventPattern();
            this.AttachEffectOverrides();
            base.PostInit();
            this.InitDynamicBone();
            this._preloaded = true;
        }

        protected override int PushEffectShaderData(Shader shader)
        {
            return base._shaderStack.PushAbove(1, shader, false);
        }

        public override void PushNoCollision()
        {
            this._noCollisionCount++;
            if (this._noCollisionCount == 1)
            {
                base.gameObject.layer = InLevelData.INACTIVE_ENTITY_LAYER;
            }
        }

        protected override void RecoverOriginalShaders()
        {
            if (this.config.CommonArguments.UseSwitchShader)
            {
                this.SwitchTransparentShader(this._usingTransparentShader);
            }
            else
            {
                base.RecoverOriginalShaders();
            }
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

        public void SetCountedMuteControl(bool mute)
        {
            this._muteControlCount += !mute ? -1 : 1;
        }

        private void SetCurrentSKillID(string value)
        {
            if (base.onCurrentSkillIDChanged != null)
            {
                base.onCurrentSkillIDChanged(this._currentSkillID, value);
            }
            this._currentSkillID = value;
        }

        public void SetDestroy()
        {
            if (this.destroyMode == DestroyMode.SetToBeRemoved)
            {
                this._isToBeRemove = true;
            }
            else if (this.destroyMode == DestroyMode.DeactivateOnly)
            {
                base.gameObject.SetActive(false);
            }
        }

        public override void SetDied(KillEffect killEffect)
        {
            if (!this._isAlive && (killEffect == KillEffect.KillImmediately))
            {
                if (this._fastDieCoroutine != null)
                {
                    base.StopCoroutine(this._fastDieCoroutine);
                    this._fastDieCoroutine = null;
                }
                if (this._waitHitDieCoroutine != null)
                {
                    base.StopCoroutine(this._waitHitDieCoroutine);
                    this._waitHitDieCoroutine = null;
                }
                this.SetDestroy();
            }
            else
            {
                this._isAlive = false;
                this._aiController.SetActive(false);
                if ((((killEffect == KillEffect.KillFastImmediately) || (killEffect == KillEffect.KillFastWithDieAnim)) || (killEffect == KillEffect.KillFastWithNormalAnim)) && (this.config.StateMachinePattern.FastDieEffectPattern == null))
                {
                    killEffect = KillEffect.KillNow;
                }
                this.CleanOwnedObjects();
                base.CastWaitingAudioEvent();
                if (this._aiController != null)
                {
                    ((BTreeMonsterAIController) this._aiController).DisableBehavior();
                }
                if (((killEffect == KillEffect.KillNow) || (killEffect == KillEffect.KillFastWithDieAnim)) || (killEffect == KillEffect.KillFastWithNormalAnim))
                {
                    this.DieNow(killEffect);
                }
                else if (killEffect == KillEffect.KillImmediately)
                {
                    this.SetDestroy();
                    this.DieNow(killEffect);
                }
                else if (killEffect == KillEffect.KillTillHitAnimationEnd)
                {
                    this._waitHitDieCoroutine = base.StartCoroutine(this.WaitHitAnimationFinishIter());
                }
            }
        }

        public virtual void SetEliteShader()
        {
            this.TryInitShaderStack();
            base._shaderStack.Push(1, MonsterData.MONSTER_ELITE_SHADER, false);
        }

        public void SetSoleAnimatorEventPattern(int stateHash, string animatorEventPatternName)
        {
            if (this._patternMap.ContainsKey(stateHash))
            {
                if (this._patternMap[stateHash] == animatorEventPatternName)
                {
                    return;
                }
                base.DetachAnimatorEventPattern(stateHash, this._patternMap[stateHash]);
            }
            base.AttachAnimatorEventPattern(stateHash, animatorEventPatternName);
            this._patternMap[stateHash] = animatorEventPatternName;
        }

        public void SetSoleSkillAnimatorEventPattern(string skillName, string animatorEventPatternName)
        {
            if (this.config.Skills.ContainsKey(skillName))
            {
                string[] animatorStateNames = this.config.Skills[skillName].AnimatorStateNames;
                if (animatorStateNames != null)
                {
                    int index = 0;
                    int length = animatorStateNames.Length;
                    while (index < length)
                    {
                        this.SetSoleAnimatorEventPattern(Animator.StringToHash(animatorStateNames[index]), animatorEventPatternName);
                        index++;
                    }
                }
            }
        }

        public void SetUseAIController(bool isUse)
        {
            this._aiController.SetActive(isUse);
        }

        public override void SetUseLocalController(bool enabled)
        {
            this.SetUseAIController(enabled);
        }

        private void SkillIDChanged(string fromSkillID, AnimatorStateInfo fromState, string toSkillID, AnimatorStateInfo toState)
        {
            ConfigMonsterSkill skill = null;
            ConfigMonsterSkill skill2 = null;
            if (fromSkillID != null)
            {
                skill = this.config.Skills[fromSkillID];
            }
            if (toSkillID != null)
            {
                skill2 = this.config.Skills[toSkillID];
            }
            if ((skill2 != null) && skill2.HighSpeedMovement)
            {
                this.PushHighspeedMovement();
            }
            if ((skill != null) && skill.HighSpeedMovement)
            {
                this.PopHighspeedMovement();
            }
            if ((skill2 != null) && skill2.Unselectable)
            {
                base.SetCountedDenySelect(true, false);
            }
            if ((skill != null) && skill.Unselectable)
            {
                base.SetCountedDenySelect(false, false);
            }
            if (((toSkillID != null) || (fromSkillID == null)) && (toSkillID != null))
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtAttackStart(base.GetRuntimeID(), toSkillID), MPEventDispatchMode.Normal);
                if ((skill2.SteerToTargetOnEnter && (this.AttackTarget != null)) && this.AttackTarget.IsActive())
                {
                    Vector3 dir = this.AttackTarget.XZPosition - this.XZPosition;
                    this.SteerFaceDirectionTo(dir);
                    if (base._animator.IsInTransition(0))
                    {
                        this.MuteSteerTillNextState();
                    }
                }
            }
            this.SetCurrentSKillID(toSkillID);
        }

        protected virtual void StandRetreat(Vector3 retreatDir, float retreatVelocity)
        {
            if (retreatVelocity != 0f)
            {
                this._retreatPlugin.StandRetreat(retreatDir, retreatVelocity);
            }
        }

        public override void SteerFaceDirectionTo(Vector3 dir)
        {
            if (base.IsWaitTransitionUnactive(this._muteSteerIx))
            {
                base.SteerFaceDirectionTo(dir);
                this._hasSteeredThisFrame = true;
            }
        }

        private void StopCheckOutsideWallCoroutine()
        {
            if (this._checkOutsideWallCoroutine != null)
            {
                base.StopCoroutine(this._checkOutsideWallCoroutine);
                this._checkOutsideWallCoroutine = null;
            }
        }

        public virtual void SwitchEliteShader(bool enable)
        {
            if (base._shaderStack != null)
            {
                if (enable && !base._shaderStack.IsOccupied(1))
                {
                    base._shaderStack.Push(1, MonsterData.MONSTER_ELITE_SHADER, false);
                }
                else if (!enable && base._shaderStack.IsOccupied(1))
                {
                    base._shaderStack.Pop(1);
                }
            }
        }

        protected void SwitchTransparentShader(bool useTransparent)
        {
            Material[] allMaterials = this.GetAllMaterials();
            for (int i = 0; i < allMaterials.Length; i++)
            {
                if (useTransparent)
                {
                    allMaterials[i].shader = MonsterData.MONSTER_TRANSPARENT_SHADER;
                }
                else
                {
                    allMaterials[i].shader = MonsterData.MONSTER_OPAQUE_SHADER;
                }
            }
        }

        private void SyncAnimatorMoveSpeed()
        {
            float num = this._moveSpeedRatio * (1f + this.GetProperty("Animator_MoveSpeedRatio"));
            base._animator.SetFloat("MoveSpeed", num);
            if (this.config.StateMachinePattern.UseAbsMoveSpeed)
            {
                base._animator.SetFloat("AbsMoveSpeed", Mathf.Abs(num));
            }
        }

        public override void TriggerAttackPattern(string animEventID, LayerMask layerMask)
        {
            ConfigMonsterAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(this.config, animEventID);
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
        private void TriggerAttackScreenShake(string attackName)
        {
            AttackPattern.ActCameraShake(SharedAnimEventData.ResolveAnimEvent(this.config, attackName).CameraShake);
        }

        protected override void TryInitShaderStack()
        {
            if (base._shaderStack == null)
            {
                base.TryInitShaderStack();
                base._shaderStack.Push(MonsterData.MONSTER_OPAQUE_SHADER, true);
            }
        }

        protected override void Update()
        {
            base.Update();
            this._aiController.Core();
            this.UpdateAttackSpeed();
            if (!this._usingThrowMass)
            {
                if (this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
                {
                    this._usingThrowMass = true;
                    base.SetMass((this._originalMass * 0.1f) * (1f + this.GetProperty("Entity_MassRatio")));
                }
            }
            else if (!this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
            {
                this._usingThrowMass = false;
                base.SetMass(this._originalMass * (1f + this.GetProperty("Entity_MassRatio")));
            }
            if (this.config.StateMachinePattern.UseStandByWalkSteer && this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Idle))
            {
                if (this._hasSteeredThisFrame)
                {
                    this._standBySteerTime += Time.deltaTime * this.TimeScale;
                    if (!base.GetLocomotionBool("IsStandByWalkSteer") && (this._standBySteerTime > this.config.StateMachinePattern.WalkSteerTimeThreshold))
                    {
                        base.SetLocomotionBool("IsStandByWalkSteer", true, false);
                    }
                }
                else
                {
                    if (base.GetLocomotionBool("IsStandByWalkSteer"))
                    {
                        base.SetLocomotionBool("IsStandByWalkSteer", false, false);
                    }
                    this._standBySteerTime = 0f;
                }
            }
            if (this.config.StateMachinePattern.KeepHitboxStanding)
            {
                Transform transform = this.hitbox.transform;
                Vector3 eulerAngles = transform.eulerAngles;
                eulerAngles.x = -90f;
                eulerAngles.y = 0f;
                eulerAngles.z = 0f;
                transform.eulerAngles = eulerAngles;
                if (base.RootNode.position.y < this.config.StateMachinePattern.KeepHitboxStandingMinHeight)
                {
                    Vector3 xZPosition = this.XZPosition;
                    xZPosition.y = this.config.StateMachinePattern.KeepHitboxStandingMinHeight;
                    transform.position = xZPosition;
                }
                else
                {
                    transform.localPosition = Vector3.zero;
                }
            }
            if (this._inactiveTimer > 0f)
            {
                this._inactiveTimer -= Time.deltaTime * this.TimeScale;
                if (this._inactiveTimer <= 0f)
                {
                    base.gameObject.layer = InLevelData.INACTIVE_ENTITY_LAYER;
                }
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
        }

        protected override void UpdatePlugins()
        {
            base._frameHaltPlugin.Core();
            this._retreatPlugin.Core();
            base._shaderTransitionPlugin.Core();
            base._shaderLerpPlugin.Core();
        }

        [DebuggerHidden]
        private IEnumerator WaitHitAnimationFinishIter()
        {
            return new <WaitHitAnimationFinishIter>c__Iterator26 { <>f__this = this };
        }

        public string AIModeName
        {
            get
            {
                return this.config.StateMachinePattern.AIMode;
            }
        }

        public BaseMonoEntity AttackTarget
        {
            get
            {
                return this._attackTarget;
            }
        }

        public override string CurrentSkillID
        {
            get
            {
                return this._currentSkillID;
            }
        }

        public bool hasArmor { get; set; }

        public string MonsterName { get; private set; }

        public int MonsterTagID
        {
            get
            {
                return this._monsterTagID;
            }
        }

        public bool MoveHorizontal
        {
            get
            {
                return (this._canMoveHorizontal && base.GetLocomotionBool("IsMoveHorizontal"));
            }
            set
            {
                if (this._canMoveHorizontal)
                {
                    base.SetLocomotionBool("IsMoveHorizontal", value, true);
                }
            }
        }

        public float MoveSpeedRatio
        {
            set
            {
                float num = Mathf.Abs(value);
                float num2 = Mathf.Sign(value);
                float aniMinSpeedRatio = this.config.StateMachinePattern.AniMinSpeedRatio;
                float aniMaxSpeedRatio = this.config.StateMachinePattern.AniMaxSpeedRatio;
                this._moveSpeedRatio = num2 * Mathf.Clamp(num, aniMinSpeedRatio, aniMaxSpeedRatio);
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

        public string TypeName { get; private set; }

        public uint uniqueMonsterID { get; private set; }

        [CompilerGenerated]
        private sealed class <CheckOutsideWall>c__Iterator27 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal BaseMonoMonster <>f__this;

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
                    case 1:
                        Miscs.CheckOutsideWallAndDrag(this.<>f__this.transform);
                        this.$current = new WaitForSeconds(0.2f);
                        this.$PC = 1;
                        return true;

                    default:
                        break;
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
        private sealed class <FastDieIter>c__Iterator25 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal AbilityState <$>abilityState;
            internal float <$>killFastDuration;
            internal BaseMonoMonster <>f__this;
            internal string <effectPattern>__1;
            internal float <timer>__0;
            internal AbilityState abilityState;
            internal float killFastDuration;

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
                        this.<timer>__0 = this.killFastDuration;
                        break;

                    case 1:
                        break;

                    case 2:
                        goto Label_00FE;

                    case 3:
                        goto Label_01DC;

                    default:
                        goto Label_020A;
                }
                if (this.<timer>__0 > 0f)
                {
                    this.<timer>__0 -= Time.deltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_020C;
                }
                this.<>f__this._animator.speed = 0f;
                if (this.<>f__this._retreatPlugin.IsActive())
                {
                    this.<>f__this._retreatPlugin.CancelActiveRetreat();
                }
                this.<timer>__0 = 0.3f;
            Label_00FE:
                while (this.<timer>__0 > 0.24f)
                {
                    this.<timer>__0 -= Time.deltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_020C;
                }
                this.<effectPattern>__1 = null;
                if (this.abilityState.ContainsState(AbilityState.Frozen))
                {
                    this.<effectPattern>__1 = "Frozen_Die";
                }
                else if (this.<>f__this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
                {
                    this.<effectPattern>__1 = this.<>f__this.config.StateMachinePattern.ThrowDieEffectPattern;
                }
                else
                {
                    this.<effectPattern>__1 = this.<>f__this.config.StateMachinePattern.FastDieEffectPattern;
                }
                if (this.<effectPattern>__1 != null)
                {
                    this.<>f__this.TriggerEffectPattern(this.<effectPattern>__1);
                }
            Label_01DC:
                while (this.<timer>__0 > 0f)
                {
                    this.<timer>__0 -= Time.deltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale;
                    this.$current = null;
                    this.$PC = 3;
                    goto Label_020C;
                }
                this.<>f__this._fastDieCoroutine = null;
                this.<>f__this.DeadHandler();
                this.$PC = -1;
            Label_020A:
                return false;
            Label_020C:
                return true;
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
        private sealed class <WaitHitAnimationFinishIter>c__Iterator26 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal BaseMonoMonster <>f__this;
            internal float <timer>__0;

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
                        this.<timer>__0 = 0.2f;
                        break;

                    case 1:
                        break;

                    case 2:
                        goto Label_00E8;

                    case 3:
                        goto Label_0133;

                    default:
                        goto Label_0189;
                }
                if ((!this.<>f__this.IsAnimatorInTag(MonsterData.MonsterTagGroup.ShowHit) && !this.<>f__this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw)) && (this.<timer>__0 > 0f))
                {
                    this.<timer>__0 -= !this.<>f__this._frameHaltPlugin.IsActive() ? (Time.deltaTime * Singleton<LevelManager>.Instance.levelEntity.TimeScale) : 0f;
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_018B;
                }
                if (!this.<>f__this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
                {
                    if (!this.<>f__this.IsAnimatorInTag(MonsterData.MonsterTagGroup.ShowHit))
                    {
                        this.<>f__this.DieNow(KillEffect.KillFastImmediately);
                        goto Label_0176;
                    }
                    goto Label_0133;
                }
            Label_00E8:
                while (this.<>f__this.IsAnimatorInTag(MonsterData.MonsterTagGroup.Throw))
                {
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_018B;
                }
                this.<>f__this.DieNow(KillEffect.KillFastImmediately);
                goto Label_0176;
            Label_0133:
                while (this.<>f__this.IsAnimatorInTag(MonsterData.MonsterTagGroup.ShowHit) && (this.<>f__this.GetCurrentNormalizedTime() < 0.7f))
                {
                    this.$current = null;
                    this.$PC = 3;
                    goto Label_018B;
                }
                this.<>f__this.DieNow(KillEffect.KillNow);
            Label_0176:
                this.<>f__this._waitHitDieCoroutine = null;
                this.$PC = -1;
            Label_0189:
                return false;
            Label_018B:
                return true;
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

        public enum DestroyMode
        {
            SetToBeRemoved,
            DeactivateOnly
        }
    }
}

