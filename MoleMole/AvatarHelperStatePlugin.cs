namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class AvatarHelperStatePlugin : BaseActorPlugin
    {
        private BaseMonoAvatar _avatar;
        private AvatarActor _avatarActor;
        public State _state = State.Idle;

        public AvatarHelperStatePlugin(AvatarActor avatarActor)
        {
            this._avatarActor = avatarActor;
            this._avatar = this._avatarActor.avatar;
            this._avatar.onDie = (Action<BaseMonoAvatar>) Delegate.Combine(this._avatar.onDie, new Action<BaseMonoAvatar>(this.OnHelperDie));
        }

        public override void Core()
        {
            if (this._state == State.TriggerSwitchIn)
            {
                this.DoHelperSwitchIn();
                this._state = State.OnStage;
            }
        }

        private void DoHelperSwitchIn()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowHelperCutIn, null));
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            this._avatar.gameObject.SetActive(true);
            this._avatar.transform.position = localAvatar.XZPosition;
            this._avatar.transform.forward = localAvatar.FaceDirection;
            this._avatar.TriggerSwitchIn();
            this._avatar.SetShaderData(E_ShaderData.AvatarHelper, true);
            this._avatar.ForceUseAIController();
        }

        public bool IsOnStage()
        {
            return (this._state == State.OnStage);
        }

        private void OnHelperDie(BaseMonoAvatar avatar)
        {
            avatar.SetDied(KillEffect.KillImmediately);
            this._state = State.Dead;
        }

        public void TriggerSwitchIn()
        {
            if (this._state != State.Dead)
            {
                this._state = State.TriggerSwitchIn;
            }
        }

        public void TriggerSwitchOut(bool force)
        {
            if (this._state == State.OnStage)
            {
                this._avatar.TriggerSwitchOut(!force ? BaseMonoAvatar.AvatarSwapOutType.Delayed : BaseMonoAvatar.AvatarSwapOutType.Force);
                this._state = State.SwitchingOut;
            }
        }

        public enum State
        {
            Idle,
            TriggerSwitchIn,
            OnStage,
            SwitchingOut,
            Dead
        }
    }
}

