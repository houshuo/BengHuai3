namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Avatar")]
    public class AvatarAttack : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAvatar _avatar;
        private int _skillIDIx;
        private AvatarAttackState _state;
        private float _timer;
        public SharedFloat AttackCD;
        public string[] AttackSkillIDs;
        public float FailSetCD;
        public float RetryTimeOut = 0.8f;
        public float SuccessSetCD;
        private const string TriggerAttackName = "ATK";

        private void AvatarBeHitCancelCallback(string skillID)
        {
            this._state = AvatarAttackState.BeHitCanceled;
        }

        public override void OnAwake()
        {
            this._avatar = base.GetComponent<BaseMonoAvatar>();
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(this._avatar.GetRuntimeID());
        }

        public override void OnEnd()
        {
            this._avatar.onBeHitCanceled = (Action<string>) Delegate.Remove(this._avatar.onBeHitCanceled, new Action<string>(this.AvatarBeHitCancelCallback));
        }

        public override void OnStart()
        {
            this._state = AvatarAttackState.Idle;
            this._timer = this.RetryTimeOut;
            this._skillIDIx = 0;
            this._avatar.onBeHitCanceled = (Action<string>) Delegate.Combine(this._avatar.onBeHitCanceled, new Action<string>(this.AvatarBeHitCancelCallback));
        }

        public override TaskStatus OnUpdate()
        {
            if (this._state == AvatarAttackState.Idle)
            {
                this._avatar.GetActiveAIController().TryUseSkill("ATK");
                this._state = AvatarAttackState.WaitingForAttackStart;
                return TaskStatus.Running;
            }
            if (this._state == AvatarAttackState.WaitingForAttackStart)
            {
                if (this._avatar.CurrentSkillID == this.AttackSkillIDs[0])
                {
                    this._state = AvatarAttackState.InAttack;
                    return TaskStatus.Running;
                }
                this._timer -= Time.deltaTime * this._avatar.TimeScale;
                if (this._timer < 0f)
                {
                    return TaskStatus.Failure;
                }
                this._avatar.GetActiveAIController().TryUseSkill("ATK");
                return TaskStatus.Running;
            }
            if (this._state == AvatarAttackState.InAttack)
            {
                if (this._skillIDIx == (this.AttackSkillIDs.Length - 1))
                {
                    if (this._avatar.CurrentSkillID == this.AttackSkillIDs[this._skillIDIx])
                    {
                        return TaskStatus.Running;
                    }
                    return TaskStatus.Success;
                }
                if (this._avatar.CurrentSkillID == this.AttackSkillIDs[this._skillIDIx])
                {
                    this._avatar.GetActiveAIController().TryUseSkill("ATK");
                    return TaskStatus.Running;
                }
                if (this._avatar.CurrentSkillID == this.AttackSkillIDs[this._skillIDIx + 1])
                {
                    this._avatar.GetActiveAIController().TryUseSkill("ATK");
                    this._skillIDIx++;
                    return TaskStatus.Running;
                }
                return TaskStatus.Failure;
            }
            if (this._state == AvatarAttackState.BeHitCanceled)
            {
                return TaskStatus.Failure;
            }
            return TaskStatus.Failure;
        }

        private enum AvatarAttackState
        {
            Idle,
            WaitingForAttackStart,
            InAttack,
            BeHitCanceled
        }
    }
}

