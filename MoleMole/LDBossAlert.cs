namespace MoleMole
{
    using System;
    using UnityEngine;

    public class LDBossAlert : BaseLDEvent
    {
        private EntityTimer _timer = new EntityTimer(2f);
        private const float WAIT_DURATION = 2f;

        public LDBossAlert()
        {
            BaseMonoAvatar avatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
            if (avatar == null)
            {
                base.Done();
            }
            else
            {
                Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("InLevel_BossAlert", avatar.XZPosition, avatar.FaceDirection, Vector3.one, Singleton<LevelManager>.Instance.levelEntity);
                this._timer.Reset(true);
            }
        }

        public override void Core()
        {
            this._timer.Core(1f);
            if (this._timer.isTimeUp)
            {
                this.PlayBossBornSound();
                base.Done();
            }
        }

        private void PlayBossBornSound()
        {
            MonoEntityAudio component = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetComponent<MonoEntityAudio>();
            if (component != null)
            {
                component.PostBossBorn();
            }
        }
    }
}

