namespace MoleMole
{
    using MoleMole.MPProtocol;
    using System;

    public class MPLevelManager : LevelManager
    {
        public LevelIdentity levelIdentity;
        public MPMode mpMode;

        public MPLevelManager()
        {
            Singleton<MPManager>.Create();
        }

        public override void Core()
        {
            Singleton<MPManager>.Instance.Core();
            base.Core();
            Singleton<MPManager>.Instance.PostCore();
        }

        protected override void CreateInLevelManagers()
        {
            Singleton<RuntimeIDManager>.Create();
            Singleton<StageManager>.Create();
            Singleton<AvatarManager>.Create();
            Singleton<CameraManager>.Create();
            Singleton<MonsterManager>.Create();
            Singleton<PropObjectManager>.Create();
            Singleton<DynamicObjectManager>.Create();
            Singleton<MPEventManager>.Create();
            Singleton<EventManager>.CreateByInstance(Singleton<MPEventManager>.Instance);
            Singleton<LevelDesignManager>.Create();
            Singleton<AuxObjectManager>.Create();
            Singleton<DetourManager>.Create();
            Singleton<ShaderDataManager>.Create();
            Singleton<CinemaDataManager>.Create();
            base.gameMode = new NetworkedMP_Default_GameMode();
        }

        public override void Destroy()
        {
            Singleton<MPManager>.Instance.Destroy();
            Singleton<MPManager>.Destroy();
            base.Destroy();
        }

        public override void InitAtAwake()
        {
            Singleton<MPManager>.Instance.InitAtAwake();
            base.InitAtAwake();
        }

        public override void InitAtStart()
        {
            Singleton<MPManager>.Instance.InitAtStart();
            base.InitAtStart();
        }
    }
}

