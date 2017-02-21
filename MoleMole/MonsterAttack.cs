namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Monster")]
    public class MonsterAttack : BehaviorDesigner.Runtime.Tasks.Action
    {
        protected IAIController _controller;
        protected string _curSkillID;
        private bool _isTargetLocalAvatar;
        private LevelAIPlugin _levelAIPlugin;
        protected BaseMonoMonster _monster;
        protected int _skillIx;
        protected State _state;
        private bool _triggerSetted;
        private float _waitAttackTimer;
        public SharedFloat AttackCD;
        public float attackFailCD;
        public float attackSuccessCD;
        public string attackType;
        public SharedInt AvatarBeAttackNum;
        public SharedFloat CDRatio;
        public SharedBool HitSuccess;
        private const float MAX_WAIT_TIME_AFTER_SET_TRIGGER = 5f;
        public float randomRange;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Attack needs to go through these IDs or it would be considered as fail")]
        public string[] skillIDS;

        protected virtual bool DoAttack()
        {
            return this._controller.TryUseSkill(this.attackType);
        }

        protected virtual void DoCalcSteer()
        {
        }

        protected void MonsterBeHitCanceled(string skillID)
        {
            if ((this._state == State.Doing) && (this._state == State.WaitAttackStart))
            {
                this.OnTransit(this._state, State.Fail);
                this._state = State.Fail;
            }
        }

        protected void MonsterSkillIDChanged(string from, string to)
        {
            if (this._state == State.WaitAttackStart)
            {
                if (to == this.skillIDS[0])
                {
                    this.OnTransit(this._state, State.Doing);
                    this._state = State.Doing;
                    this._curSkillID = to;
                    Singleton<MainUIManager>.Instance.GetInLevelUICanvas().hintArrowManager.TriggerHintArrowEffect(this._monster.GetRuntimeID(), MonoHintArrow.EffectType.Twinkle);
                }
            }
            else if (this._state == State.Doing)
            {
                int num = Array.IndexOf<string>(this.skillIDS, to, this._skillIx);
                if (num >= this._skillIx)
                {
                    this._skillIx = num;
                    this._curSkillID = to;
                }
                else if ((this._skillIx == (this.skillIDS.Length - 1)) && (from == this.skillIDS[this._skillIx]))
                {
                    if (this._state != State.Fail)
                    {
                        this.OnTransit(this._state, State.Success);
                        this._state = State.Success;
                    }
                }
                else
                {
                    this.OnTransit(this._state, State.Fail);
                    this._state = State.Fail;
                }
            }
        }

        public override void OnAwake()
        {
            this._monster = base.GetComponent<BaseMonoMonster>();
            this._controller = this._monster.GetActiveAIController();
            this._levelAIPlugin = Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelAIPlugin>();
        }

        public override void OnEnd()
        {
            if (this._isTargetLocalAvatar && this.AvatarBeAttackNum.IsShared)
            {
                this._levelAIPlugin.RemoveAttackingMonster(base.GetComponent<BaseMonoMonster>());
            }
            this._monster.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Remove(this._monster.onCurrentSkillIDChanged, new Action<string, string>(this.MonsterSkillIDChanged));
            this._monster.onBeHitCanceled = (Action<string>) Delegate.Remove(this._monster.onBeHitCanceled, new Action<string>(this.MonsterBeHitCanceled));
            this._triggerSetted = false;
        }

        public override void OnStart()
        {
            this._monster.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(this._monster.onCurrentSkillIDChanged, new Action<string, string>(this.MonsterSkillIDChanged));
            this._monster.onBeHitCanceled = (Action<string>) Delegate.Combine(this._monster.onBeHitCanceled, new Action<string>(this.MonsterBeHitCanceled));
            this._waitAttackTimer = 0f;
            this._skillIx = 0;
            this._state = State.WaitAttackStart;
            BaseMonoAvatar attackTarget = this._monster.AttackTarget as BaseMonoAvatar;
            if ((attackTarget != null) && Singleton<AvatarManager>.Instance.IsLocalAvatar(attackTarget.GetRuntimeID()))
            {
                if (this.AvatarBeAttackNum.IsShared)
                {
                    this._levelAIPlugin.AddAttackingMonster(base.GetComponent<BaseMonoMonster>());
                }
                this._isTargetLocalAvatar = true;
            }
            this.DoCalcSteer();
            this.DoAttack();
            if (this.HitSuccess.IsShared)
            {
                this.HitSuccess.SetValue(false);
            }
        }

        protected virtual void OnTransit(State from, State to)
        {
        }

        public override TaskStatus OnUpdate()
        {
            if (this._state == State.WaitAttackStart)
            {
                this._waitAttackTimer += Time.deltaTime * this._monster.TimeScale;
                if (!this._triggerSetted)
                {
                    this._triggerSetted = this.DoAttack();
                }
                if (this._waitAttackTimer > 5f)
                {
                    this.OnTransit(this._state, State.Fail);
                    this._state = State.Fail;
                }
            }
            else
            {
                if (this._state == State.Success)
                {
                    this.SetAttackCDWithHitSuccess(true);
                    return TaskStatus.Success;
                }
                if (this._state == State.Fail)
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
            this.AttackCD.SetValue(attackSuccessCD * this._monster.GetProperty("AI_AttackCDRatio"));
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

        protected enum State
        {
            WaitAttackStart,
            Doing,
            Success,
            Fail
        }
    }
}

