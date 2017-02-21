namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class Attack : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAbilityEntity _abilityEntity;
        private IAIController _aiController;
        private IAIEntity _aiEntity;
        private string _attackSkillID;
        private AttackState _attackState;
        private bool _hasSteeredToFacing;
        private bool _isTargetLocalAvatar;
        private LevelAIPlugin _levelAIPlugin;
        private int _skillIDIx;
        private Coroutine _steerToFacingIter;
        private float _timer;
        public SharedFloat AttackCD;
        public float attackFailCD;
        public float attackSuccessCD;
        public string attackType;
        public SharedInt AvatarBeAttackNum;
        public SharedFloat CDRatio;
        public SharedBool HitSuccess;
        private const float MAX_WAIT_TIME_AFTER_SET_TRIGGER = 5f;
        public float randomRange;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Attack needs to go through these attacks or it would be considered as fail")]
        public string[] SkillIDs;
        public bool SteerInAllSkillIDs;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("How many seconds it takes to steer to avatar. 0 means instant.")]
        public float SteerToTargetDuration;
        public bool SteerToTargetOnStart;

        private bool DoAttack()
        {
            return this._aiController.TryUseSkill(this.attackType);
        }

        private void MonsterBeHitCanceld(string skillID)
        {
            if ((this._attackState != AttackState.TO_START) && (this._attackState == AttackState.DOING))
            {
                this._attackState = AttackState.FAIL;
            }
        }

        private void MonsterSkillIDChanged(string from, string to)
        {
            if ((this._attackSkillID == null) && (to != null))
            {
                this._attackSkillID = to;
            }
            bool flag = false;
            if (this.SteerInAllSkillIDs)
            {
                for (int i = 0; i < this.SkillIDs.Length; i++)
                {
                    if (to == this.SkillIDs[i])
                    {
                        flag = true;
                        this._hasSteeredToFacing = true;
                    }
                }
            }
            else
            {
                string str = (this.SkillIDs.Length == 0) ? this.attackType : this.SkillIDs[0];
                if (to == str)
                {
                    flag = true;
                }
            }
            if ((this._hasSteeredToFacing && flag) && ((this._abilityEntity.GetAttackTarget() != null) && this._abilityEntity.GetAttackTarget().IsActive()))
            {
                Vector3 forward = this._abilityEntity.GetAttackTarget().XZPosition - this._abilityEntity.XZPosition;
                forward.Normalize();
                if (this.SteerToTargetDuration == 0f)
                {
                    this._abilityEntity.SteerFaceDirectionTo(forward);
                }
                else
                {
                    this._steerToFacingIter = this._abilityEntity.StartCoroutine(this.SteerToTargetFacingIter(this._abilityEntity.transform.forward, forward, this.SteerToTargetDuration));
                }
                this._hasSteeredToFacing = false;
            }
            if ((this.SkillIDs != null) && (this.SkillIDs.Length > 0))
            {
                if (this._attackState == AttackState.TO_START)
                {
                    if (to == this.SkillIDs[0])
                    {
                        this._attackState = AttackState.DOING;
                        this._skillIDIx = 0;
                        Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.TriggerHintArrowEffect(this._abilityEntity.GetRuntimeID(), MonoHintArrow.EffectType.Twinkle);
                    }
                }
                else if (this._attackState == AttackState.DOING)
                {
                    int index = Array.IndexOf<string>(this.SkillIDs, to);
                    if (index >= this._skillIDIx)
                    {
                        this._attackState = AttackState.DOING;
                        this._skillIDIx = index;
                    }
                    else if ((this._skillIDIx == (this.SkillIDs.Length - 1)) && (from == this.SkillIDs[this._skillIDIx]))
                    {
                        if (this._attackState != AttackState.FAIL)
                        {
                            this._attackState = AttackState.SUCCESS;
                        }
                    }
                    else
                    {
                        this._attackState = AttackState.FAIL;
                    }
                }
            }
        }

        public override void OnAwake()
        {
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            if (component is BaseMonoAvatar)
            {
                this._aiEntity = (BaseMonoAvatar) component;
            }
            else if (component is BaseMonoMonster)
            {
                this._aiEntity = (BaseMonoMonster) component;
            }
            this._aiController = this._aiEntity.GetActiveAIController();
            this._abilityEntity = component;
            this._levelAIPlugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelAIPlugin>();
        }

        public override void OnEnd()
        {
            if (this._isTargetLocalAvatar && this.AvatarBeAttackNum.IsShared)
            {
                this._levelAIPlugin.RemoveAttackingMonster(base.GetComponent<BaseMonoMonster>());
            }
            if (this.SteerToTargetOnStart || ((this.SkillIDs != null) && (this.SkillIDs.Length > 0)))
            {
                this._abilityEntity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(this._abilityEntity.onCurrentSkillIDChanged, new Action<string, string>(this.MonsterSkillIDChanged));
            }
            this._abilityEntity.onBeHitCanceled = (Action<string>) Delegate.Remove(this._abilityEntity.onBeHitCanceled, new Action<string>(this.MonsterBeHitCanceld));
            if (((this._steerToFacingIter != null) && (this._abilityEntity != null)) && this._abilityEntity.gameObject.activeSelf)
            {
                this._abilityEntity.StopCoroutine(this._steerToFacingIter);
                this._steerToFacingIter = null;
            }
        }

        public override void OnStart()
        {
            this._attackState = AttackState.NONE;
            BaseMonoAvatar attackTarget = this._aiEntity.AttackTarget as BaseMonoAvatar;
            if (attackTarget == null)
            {
                this._isTargetLocalAvatar = false;
            }
            else
            {
                this._isTargetLocalAvatar = Singleton<AvatarManager>.Instance.IsLocalAvatar(attackTarget.GetRuntimeID());
            }
            if (this._isTargetLocalAvatar && this.AvatarBeAttackNum.IsShared)
            {
                this._levelAIPlugin.AddAttackingMonster(base.GetComponent<BaseMonoMonster>());
            }
            if (this.SteerToTargetOnStart || ((this.SkillIDs != null) && (this.SkillIDs.Length > 0)))
            {
                this._abilityEntity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(this._abilityEntity.onCurrentSkillIDChanged, new Action<string, string>(this.MonsterSkillIDChanged));
                this._skillIDIx = -1;
                this._hasSteeredToFacing = this.SteerToTargetOnStart;
            }
            this._abilityEntity.onBeHitCanceled = (Action<string>) Delegate.Combine(this._abilityEntity.onBeHitCanceled, new Action<string>(this.MonsterBeHitCanceld));
            if (this.HitSuccess.IsShared)
            {
                this.HitSuccess.SetValue(false);
            }
        }

        public override TaskStatus OnUpdate()
        {
            if (this._attackState == AttackState.NONE)
            {
                if (this.DoAttack())
                {
                    this._attackState = AttackState.TO_START;
                    this._timer = 5f;
                }
                else
                {
                    this._attackState = AttackState.FAIL;
                }
            }
            if (this._attackState == AttackState.FAIL)
            {
                this.SetAttackCDWithHitSuccess(false);
                return TaskStatus.Failure;
            }
            if (this._attackState == AttackState.SUCCESS)
            {
                this.SetAttackCDWithHitSuccess(true);
                return TaskStatus.Success;
            }
            if (this._attackState == AttackState.TO_START)
            {
                this._timer -= Time.deltaTime * this._aiEntity.TimeScale;
                if (this._timer < 0f)
                {
                    this.SetAttackCDWithHitSuccess(false);
                    return TaskStatus.Failure;
                }
            }
            return TaskStatus.Running;
        }

        private void SetAttackCD(bool success)
        {
            float attackSuccessCD = 0f;
            if (success)
            {
                attackSuccessCD = this.attackSuccessCD;
            }
            else
            {
                attackSuccessCD = this.attackFailCD;
            }
            if (this.randomRange != 0f)
            {
                attackSuccessCD += this.randomRange * UnityEngine.Random.Range((float) -1f, (float) 1f);
            }
            if (this.CDRatio.Value > 0f)
            {
                attackSuccessCD *= this.CDRatio.Value;
            }
            this.AttackCD.SetValue(attackSuccessCD * this._aiEntity.GetProperty("AI_AttackCDRatio"));
        }

        private void SetAttackCDWithHitSuccess(bool success)
        {
            if (this.HitSuccess.IsShared)
            {
                this.SetAttackCD(this.HitSuccess.Value);
            }
            else
            {
                this.SetAttackCD(success);
            }
        }

        [DebuggerHidden]
        private IEnumerator SteerToTargetFacingIter(Vector3 fromForward, Vector3 targetForward, float duration)
        {
            return new <SteerToTargetFacingIter>c__Iterator0 { fromForward = fromForward, targetForward = targetForward, duration = duration, <$>fromForward = fromForward, <$>targetForward = targetForward, <$>duration = duration, <>f__this = this };
        }

        [CompilerGenerated]
        private sealed class <SteerToTargetFacingIter>c__Iterator0 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal float <$>duration;
            internal Vector3 <$>fromForward;
            internal Vector3 <$>targetForward;
            internal Attack <>f__this;
            internal float <t>__0;
            internal float duration;
            internal Vector3 fromForward;
            internal Vector3 targetForward;

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
                        this.<t>__0 = 0f;
                        this.fromForward.y = 0f;
                        this.fromForward.Normalize();
                        this.targetForward.y = 0f;
                        this.targetForward.Normalize();
                        break;

                    case 1:
                        this.<t>__0 += this.<>f__this._abilityEntity.TimeScale * Time.deltaTime;
                        break;

                    default:
                        goto Label_00EF;
                }
                if (this.<t>__0 < this.duration)
                {
                    this.<>f__this._abilityEntity.SteerFaceDirectionTo(Vector3.Slerp(this.fromForward, this.targetForward, this.<t>__0 / this.duration));
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.<>f__this._steerToFacingIter = null;
                this.$PC = -1;
            Label_00EF:
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

        private enum AttackState
        {
            NONE,
            TO_START,
            DOING,
            SUCCESS,
            FAIL
        }
    }
}

