namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Avatar")]
    public class AvatarSkill : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAvatar _avatar;
        private AvatarActor _avatarActor;
        private AvatarSkillState _state;
        private float _timer;
        public float FailSetCD;
        public float NormalizedEndTime = 1f;
        public float RetryTimeOut = 0.8f;
        public SharedFloat SkillCD;
        public string SKillID;
        public bool StopMove;
        public float SuccessSetCD;
        public string TriggerSkillName;

        private void AvatarBeHitCancelCallback(string skillID)
        {
            this._state = AvatarSkillState.BeHitCanceled;
        }

        public override void OnAwake()
        {
            this._avatar = base.GetComponent<BaseMonoAvatar>();
            this._avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(this._avatar.GetRuntimeID());
        }

        public override void OnEnd()
        {
            this._avatar.onBeHitCanceled = (Action<string>) Delegate.Remove(this._avatar.onBeHitCanceled, new Action<string>(this.AvatarBeHitCancelCallback));
        }

        public override void OnStart()
        {
            this._state = AvatarSkillState.Idle;
            this._timer = this.RetryTimeOut;
            this._avatar.onBeHitCanceled = (Action<string>) Delegate.Combine(this._avatar.onBeHitCanceled, new Action<string>(this.AvatarBeHitCancelCallback));
        }

        public override TaskStatus OnUpdate()
        {
            if (this._state == AvatarSkillState.Idle)
            {
                if (this._avatarActor.CanUseSkill(this.TriggerSkillName))
                {
                    this.TryTriggerSkill();
                    this._state = AvatarSkillState.WaitingForSkillStart;
                    return TaskStatus.Running;
                }
                this.SkillCD.SetValue(this.FailSetCD);
                return TaskStatus.Failure;
            }
            if (this._state == AvatarSkillState.WaitingForSkillStart)
            {
                if (this._avatar.CurrentSkillID == this.SKillID)
                {
                    this._state = AvatarSkillState.InSkill;
                    return TaskStatus.Running;
                }
                this._timer -= Time.deltaTime * this._avatar.TimeScale;
                if (this._timer < 0f)
                {
                    this.SkillCD.SetValue(this.FailSetCD);
                    return TaskStatus.Failure;
                }
                this.TryTriggerSkill();
                return TaskStatus.Running;
            }
            if (this._state == AvatarSkillState.InSkill)
            {
                if ((this._avatar.CurrentSkillID == this.SKillID) && (this._avatar.GetCurrentNormalizedTime() < this.NormalizedEndTime))
                {
                    return TaskStatus.Running;
                }
                this.SkillCD.SetValue(this.SuccessSetCD);
                return TaskStatus.Success;
            }
            if (this._state == AvatarSkillState.BeHitCanceled)
            {
                this.SkillCD.SetValue(this.FailSetCD);
                return TaskStatus.Failure;
            }
            return TaskStatus.Failure;
        }

        private void TryTriggerSkill()
        {
            this._avatar.GetActiveAIController().TryUseSkill(this.TriggerSkillName);
            if (this.StopMove)
            {
                this._avatar.GetActiveAIController().TryStop();
            }
        }

        private enum AvatarSkillState
        {
            Idle,
            WaitingForSkillStart,
            InSkill,
            BeHitCanceled
        }
    }
}

