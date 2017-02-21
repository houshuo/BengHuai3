namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CameraActorLastKillCloseUpPlugin : BaseActorPlugin
    {
        private bool _active;
        private MainCameraActor _cameraActor;
        private List<BaseMonoMonster> _waitDieMonsters;
        public bool AlwaysTrigger;

        public CameraActorLastKillCloseUpPlugin(MainCameraActor cameraActor)
        {
            this._cameraActor = cameraActor;
            this._waitDieMonsters = new List<BaseMonoMonster>();
        }

        private void AttachDieCallback(BaseMonoMonster monster)
        {
            if (!this._waitDieMonsters.Contains(monster))
            {
                monster.onDie = (Action<BaseMonoMonster>) Delegate.Combine(monster.onDie, new Action<BaseMonoMonster>(this.MonsterDieCallback));
                this._waitDieMonsters.Add(monster);
            }
        }

        public bool IsPending()
        {
            return (this._active && (this._waitDieMonsters.Count > 0));
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtLevelState)
            {
                return this.ListenLevelState((EvtLevelState) evt);
            }
            return ((evt is EvtMonsterCreated) && this.ListenMonsterCreated((EvtMonsterCreated) evt));
        }

        private bool ListenLevelState(EvtLevelState evt)
        {
            if ((evt.state == EvtLevelState.State.EndWin) || (evt.state == EvtLevelState.State.EndLose))
            {
                this._cameraActor.RemovePlugin(this);
            }
            return true;
        }

        private bool ListenMonsterCreated(EvtMonsterCreated evt)
        {
            this.AttachDieCallback(Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(evt.monsterID));
            return true;
        }

        private void MonsterDieCallback(BaseMonoMonster monster)
        {
            if ((Singleton<MonsterManager>.Instance.LivingMonsterCount() == 0) || this.AlwaysTrigger)
            {
                if (!Singleton<CameraManager>.Instance.GetMainCamera().levelAnimState.active)
                {
                    this.ShowLastKillCameraEffect(monster);
                }
                if (!this.AlwaysTrigger)
                {
                    this._cameraActor.RemovePlugin(this);
                }
            }
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelState>(this._cameraActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(this._cameraActor.runtimeID);
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            for (int i = 0; i < allMonsters.Count; i++)
            {
                this.AttachDieCallback(allMonsters[i]);
            }
            this._active = true;
        }

        public override void OnRemoved()
        {
            Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelState>(this._cameraActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(this._cameraActor.runtimeID);
            for (int i = 0; i < this._waitDieMonsters.Count; i++)
            {
                if (this._waitDieMonsters[i] != null)
                {
                    BaseMonoMonster local1 = this._waitDieMonsters[i];
                    local1.onDie = (Action<BaseMonoMonster>) Delegate.Remove(local1.onDie, new Action<BaseMonoMonster>(this.MonsterDieCallback));
                }
            }
            this._waitDieMonsters.Clear();
            this._active = false;
        }

        private void ShowLastKillCameraEffect(BaseMonoMonster monster)
        {
            BaseMonoAvatar avatar = Singleton<AvatarManager>.Instance.TryGetLocalAvatar();
            Vector3 vector = monster.XZPosition - avatar.XZPosition;
            float magnitude = vector.magnitude;
            string filePath = (Singleton<MonsterManager>.Instance.LivingMonsterCount() != 0) ? "SlowMotionKill/Normal" : "SlowMotionKill/LastKill";
            ConfigCameraSlowMotionKill config = ConfigUtil.LoadConfig<ConfigCameraSlowMotionKill>(filePath);
            MonoMainCamera mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
            Vector3 vector2 = mainCamera.XZPosition - avatar.XZPosition;
            float distCamera = vector2.magnitude;
            mainCamera.SetSlowMotionKill(config, magnitude, distCamera);
        }
    }
}

