namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class LevelBuffStopWorld : BaseLevelBuff
    {
        private bool _enteringTimeSlow;
        private uint _stopWorldOwnerID;
        private EntityTimer _timer;
        private const float STOP_WORLD_TIMESCALE = 0.05f;

        public LevelBuffStopWorld(LevelActor levelActor)
        {
            base.levelActor = levelActor;
            base.levelBuffType = LevelBuffType.StopWorld;
            this._timer = new EntityTimer();
        }

        private void ApplyStopEffectOn(BaseMonoAbilityEntity target)
        {
            target.PushTimeScale(0.05f, 2);
        }

        private void ApplyStopWorld()
        {
            List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
            for (int i = 0; i < allAvatars.Count; i++)
            {
                if (allAvatars[i].GetRuntimeID() != this._stopWorldOwnerID)
                {
                    this.ApplyStopEffectOn(allAvatars[i]);
                }
            }
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            for (int j = 0; j < allMonsters.Count; j++)
            {
                if (allMonsters[j].GetRuntimeID() != this._stopWorldOwnerID)
                {
                    this.ApplyStopEffectOn(allMonsters[j]);
                }
            }
            List<BaseMonoPropObject> allPropObjects = Singleton<PropObjectManager>.Instance.GetAllPropObjects();
            for (int k = 0; k < allPropObjects.Count; k++)
            {
                if (allPropObjects[k].GetRuntimeID() != this._stopWorldOwnerID)
                {
                    this.ApplyStopEffectOn(allPropObjects[k]);
                }
            }
            Singleton<LevelManager>.Instance.levelEntity.auxTimeScaleStack.Push(2, 0.05f, false);
        }

        public override void Core()
        {
            if (!base.muteUpdateDuration)
            {
                this._timer.Core(1f);
                if (this._timer.isTimeUp)
                {
                    base.levelActor.StopLevelBuff(this);
                    this._timer.Reset(false);
                }
            }
        }

        public override void OnAdded()
        {
            if (this._enteringTimeSlow)
            {
                Singleton<LevelManager>.Instance.levelActor.TimeSlow(0.3f, 0.05f, delegate {
                    if (base.isActive)
                    {
                        this.ApplyStopWorld();
                    }
                });
            }
            else
            {
                this.ApplyStopWorld();
            }
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            if (evt is EvtMonsterCreated)
            {
                return this.OnPostMonsterCreated((EvtMonsterCreated) evt);
            }
            return ((evt is EvtPropObjectCreated) && this.OnPostPropObjectCreated((EvtPropObjectCreated) evt));
        }

        private bool OnPostMonsterCreated(EvtMonsterCreated evt)
        {
            if (evt.monsterID == this._stopWorldOwnerID)
            {
                return false;
            }
            BaseMonoMonster target = Singleton<MonsterManager>.Instance.TryGetMonsterByRuntimeID(evt.monsterID);
            if (target != null)
            {
                this.ApplyStopEffectOn(target);
            }
            return true;
        }

        private bool OnPostPropObjectCreated(EvtPropObjectCreated evt)
        {
            if (evt.objectID == this._stopWorldOwnerID)
            {
                return false;
            }
            BaseMonoPropObject target = Singleton<PropObjectManager>.Instance.TryGetPropObjectByRuntimeID(evt.objectID);
            if (target != null)
            {
                this.ApplyStopEffectOn(target);
            }
            return true;
        }

        public override void OnRemoved()
        {
            this._timer.Reset(false);
            this.RemoveStopWorld();
        }

        private void RemoveStopEffectOn(BaseMonoAbilityEntity target)
        {
            target.PopTimeScale(2);
        }

        private void RemoveStopWorld()
        {
            List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
            for (int i = 0; i < allAvatars.Count; i++)
            {
                if (allAvatars[i].GetRuntimeID() != this._stopWorldOwnerID)
                {
                    this.RemoveStopEffectOn(allAvatars[i]);
                }
            }
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            for (int j = 0; j < allMonsters.Count; j++)
            {
                if (allMonsters[j].GetRuntimeID() != this._stopWorldOwnerID)
                {
                    this.RemoveStopEffectOn(allMonsters[j]);
                }
            }
            List<BaseMonoPropObject> allPropObjects = Singleton<PropObjectManager>.Instance.GetAllPropObjects();
            for (int k = 0; k < allPropObjects.Count; k++)
            {
                if (allPropObjects[k].GetRuntimeID() != this._stopWorldOwnerID)
                {
                    this.RemoveStopEffectOn(allPropObjects[k]);
                }
            }
            Singleton<LevelManager>.Instance.levelEntity.auxTimeScaleStack.Pop(2);
        }

        public void Setup(bool enteringTimeSlow, float duration, uint ownerID)
        {
            this._timer.timespan = duration;
            this._timer.Reset(true);
            this._stopWorldOwnerID = ownerID;
            this._enteringTimeSlow = enteringTimeSlow;
        }
    }
}

