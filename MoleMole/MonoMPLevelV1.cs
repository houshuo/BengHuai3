namespace MoleMole
{
    using System;

    public class MonoMPLevelV1 : MonoTheLevelV1
    {
        protected override void CreateLevelManager()
        {
            Singleton<MPLevelManager>.Create();
            Singleton<LevelManager>.CreateByInstance(Singleton<MPLevelManager>.Instance);
            MonoLevelEntity entity = Singleton<LevelManager>.Instance.levelEntity = base.gameObject.AddComponent<MonoLevelEntity>();
            entity.Init(0x21800001);
            Singleton<LevelManager>.Instance.levelActor = Singleton<EventManager>.Instance.CreateActor<MPLevelActor>(entity);
            Singleton<LevelManager>.Instance.levelActor.PostInit();
        }
    }
}

