namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public class LevelMissionStatisticsPlugin : BaseActorPlugin
    {
        private LevelActor _levelActor;

        public LevelMissionStatisticsPlugin(LevelActor levelActor)
        {
            this._levelActor = levelActor;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtKilled) && this.ListenKill((EvtKilled) evt));
        }

        private bool ListenKill(EvtKilled evt)
        {
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 4)
            {
                BaseMonoAvatar avatar = Singleton<AvatarManager>.Instance.TryGetAvatarByRuntimeID(evt.killerID);
                if (avatar == null)
                {
                    return true;
                }
                Singleton<MissionModule>.Instance.TryToUpdateKillAnyEnemy();
                MonsterActor actor = (MonsterActor) Singleton<EventManager>.Instance.GetActor(evt.targetID);
                if (actor != null)
                {
                    if (actor.uniqueMonsterID != 0)
                    {
                        Singleton<MissionModule>.Instance.TryToUpdateKillUniqueMonsterMission(actor.uniqueMonsterID);
                    }
                    else
                    {
                        int monsterID = MonsterUIMetaDataReaderExtend.GetMonsterUIMetaDataByName(actor.metaConfig.subTypeName).monsterID;
                        Singleton<MissionModule>.Instance.TryToUpdateKillMonsterMission(monsterID);
                    }
                    Singleton<MissionModule>.Instance.TryToUpdateKillMonsterWithCategoryName(actor.metaConfig.categoryName);
                }
                ConfigAvatarAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(avatar.config, evt.killerAnimEventID);
                if (((event2 != null) && (event2.AttackProperty != null)) && (event2.AttackProperty.CategoryTag != null))
                {
                    AttackResult.AttackCategoryTag[] categoryTag = event2.AttackProperty.CategoryTag;
                    if (categoryTag.Length > 0)
                    {
                        Singleton<MissionModule>.Instance.TryToUpdateKillByAttackCategoryTag(categoryTag);
                    }
                }
            }
            return true;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(this._levelActor.runtimeID);
        }

        public override bool OnEvent(BaseEvent evt)
        {
            bool flag = base.OnEvent(evt);
            if (evt is EvtLevelState)
            {
                flag |= this.OnLevelState((EvtLevelState) evt);
            }
            return flag;
        }

        public bool OnLevelState(EvtLevelState evt)
        {
            if ((evt.state == EvtLevelState.State.EndWin) || (evt.state == EvtLevelState.State.EndLose))
            {
                Singleton<MissionModule>.Instance.FlushMissionDataToServer();
            }
            return true;
        }

        public override void OnRemoved()
        {
            Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(this._levelActor.runtimeID);
        }
    }
}

