namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MainCameraActor : BasePluggedActor
    {
        private CameraActorLastKillCloseUpPlugin _closeUpPlugin;
        private MonoMainCamera _mainCamera;

        public override void Init(BaseMonoEntity entity)
        {
            this._mainCamera = (MonoMainCamera) entity;
            base.runtimeID = this._mainCamera.GetRuntimeID();
            this._closeUpPlugin = new CameraActorLastKillCloseUpPlugin(this);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarCreated>(base.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(base.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(base.runtimeID);
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            bool flag = base.ListenEvent(evt);
            if (evt is EvtStageReady)
            {
                flag |= this.ListenStageReady((EvtStageReady) evt);
            }
            else if (evt is EvtKilled)
            {
                flag |= this.ListenKill((EvtKilled) evt);
            }
            return false;
        }

        public bool ListenKill(EvtKilled evt)
        {
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            if ((mainCamera.followState.active && mainCamera.followState.followAvatarAndBossState.active) && (mainCamera.followState.followAvatarAndBossState.bossTarget.GetRuntimeID() == evt.targetID))
            {
                Singleton<CameraManager>.Instance.DisableBossCamera();
            }
            return true;
        }

        private bool ListenStageReady(EvtStageReady evt)
        {
            BaseMonoAvatar avatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
            if (this._mainCamera.followState.active)
            {
                Singleton<CameraManager>.Instance.GetMainCamera().SuddenSwitchFollowAvatar(avatar.GetRuntimeID(), false);
            }
            else
            {
                Singleton<WwiseAudioManager>.Instance.SetListenerFollowing(avatar.transform, new Vector3(0f, 2f, 0f));
                this._mainCamera.SetupFollowAvatar(avatar.GetRuntimeID());
                this._mainCamera.followState.SetEnterPolarMode(MainCameraFollowState.EnterPolarMode.AlongAvatarFacing, 0f);
                this._mainCamera.TransitToFollow();
            }
            return true;
        }

        public void SetupLastKillCloseUp()
        {
            if (!base.HasPlugin<CameraActorLastKillCloseUpPlugin>())
            {
                base.AddPlugin(this._closeUpPlugin);
            }
        }
    }
}

