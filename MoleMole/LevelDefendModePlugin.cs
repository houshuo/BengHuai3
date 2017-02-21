namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class LevelDefendModePlugin : BaseActorPlugin
    {
        private bool _active;
        private List<int> _certainMonsterList;
        private List<DefendModeData> _defendModeDataList;
        private LevelActor _levelActor;
        private int _maxMonsterDisappearAmount;
        private int _monsterEnterAmount;
        private int _monsterKillAmount;
        private List<TriggerFieldActor> _triggerFieldActorList;

        public LevelDefendModePlugin(LevelActor levelActor)
        {
            this._defendModeDataList = new List<DefendModeData>();
            this._certainMonsterList = new List<int>();
            this._triggerFieldActorList = new List<TriggerFieldActor>();
            this._levelActor = levelActor;
            this._monsterEnterAmount = 0;
            this._monsterKillAmount = 0;
            this._maxMonsterDisappearAmount = 0;
            this.SetActive(false);
        }

        public LevelDefendModePlugin(LevelActor levelActor, int targetValue)
        {
            this._defendModeDataList = new List<DefendModeData>();
            this._certainMonsterList = new List<int>();
            this._triggerFieldActorList = new List<TriggerFieldActor>();
            this._levelActor = levelActor;
            this._monsterEnterAmount = 0;
            this._monsterKillAmount = 0;
            this._maxMonsterDisappearAmount = targetValue;
            this.SetActive(false);
        }

        public void AddModeData(int uniqueID, bool isKey = false)
        {
            DefendModeData item = new DefendModeData(uniqueID, isKey);
            this._defendModeDataList.Add(item);
            this._certainMonsterList.Add(uniqueID);
            if (isKey)
            {
                this.RefreshDisplay(item, uniqueID);
            }
        }

        public void AddModeData(DefendModeType modeType, int targetValue, bool isKey = false)
        {
            DefendModeData item = new DefendModeData(modeType, targetValue, isKey);
            this._defendModeDataList.Add(item);
            if (isKey)
            {
                this.RefreshDisplay(item, 0);
            }
        }

        public void AddModeData(DefendModeType modeType, int targetValue, int currentValue, bool isKey = false)
        {
            DefendModeData item = new DefendModeData(modeType, targetValue, currentValue, isKey);
            this._defendModeDataList.Add(item);
            if (isKey)
            {
                this.RefreshDisplay(item, 0);
            }
        }

        public void AddTriggerFieldActor(TriggerFieldActor triggerFieldActor)
        {
            if (triggerFieldActor != null)
            {
                this._triggerFieldActorList.Add(triggerFieldActor);
            }
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (this._active)
            {
                if (evt is EvtFieldEnter)
                {
                    return this.OnFieldEnter((EvtFieldEnter) evt);
                }
                if (evt is EvtKilled)
                {
                    return this.OnKilled((EvtKilled) evt);
                }
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(this._levelActor.runtimeID);
        }

        public override bool OnEvent(BaseEvent evt)
        {
            bool flag = base.OnEvent(evt);
            if (this._active && (evt is EvtLevelState))
            {
                flag |= this.OnLevelState((EvtLevelState) evt);
            }
            return flag;
        }

        public bool OnFieldEnter(EvtFieldEnter evt)
        {
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.otherID) == 4)
            {
                bool flag = false;
                foreach (TriggerFieldActor actor in this._triggerFieldActorList)
                {
                    if (evt.targetID == actor.runtimeID)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                {
                    BaseMonoMonster monsterByRuntimeID = Singleton<MonsterManager>.Instance.GetMonsterByRuntimeID(evt.otherID);
                    if (monsterByRuntimeID == null)
                    {
                        return false;
                    }
                    this._monsterEnterAmount++;
                    MonsterActor actor2 = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.otherID);
                    if (actor2 != null)
                    {
                        actor2.ForceRemoveImmediatelly();
                    }
                    if ((this._maxMonsterDisappearAmount != 0) && ((this._monsterEnterAmount + this._monsterKillAmount) == this._maxMonsterDisappearAmount))
                    {
                        Singleton<EventManager>.Instance.FireEvent(new EvtLevelDefendState(DefendModeType.Result, this._maxMonsterDisappearAmount), MPEventDispatchMode.Normal);
                    }
                    if (this._defendModeDataList.Count <= 0)
                    {
                        return false;
                    }
                    List<DefendModeData> list = new List<DefendModeData>();
                    foreach (DefendModeData data in this._defendModeDataList)
                    {
                        if ((data.type == DefendModeType.Single) && (data.targetValue > 0))
                        {
                            data.currentValue++;
                            this.RefreshDisplay(data, 0);
                            if (data.currentValue >= data.targetValue)
                            {
                                Singleton<EventManager>.Instance.FireEvent(new EvtLevelDefendState(data.type, data.targetValue), MPEventDispatchMode.Normal);
                                list.Add(data);
                            }
                        }
                        else if ((data.type == DefendModeType.Group) && (data.targetValue > 0))
                        {
                            data.currentValue++;
                            this.RefreshDisplay(data, 0);
                            if (data.currentValue >= data.targetValue)
                            {
                                Singleton<EventManager>.Instance.FireEvent(new EvtLevelDefendState(data.type, data.targetValue), MPEventDispatchMode.Normal);
                                data.currentValue = 0;
                            }
                        }
                        else if ((data.type == DefendModeType.Certain) && (data.uniqueID != 0))
                        {
                            if (this._certainMonsterList.Contains(data.uniqueID) && (monsterByRuntimeID.MonsterTagID == data.uniqueID))
                            {
                                this._certainMonsterList.Remove(data.uniqueID);
                                Singleton<EventManager>.Instance.FireEvent(new EvtLevelDefendState(data.uniqueID), MPEventDispatchMode.Normal);
                                this.RefreshDisplay(data, monsterByRuntimeID.MonsterTagID);
                                list.Add(data);
                            }
                            else
                            {
                                this.RefreshDisplay(data, monsterByRuntimeID.MonsterTagID);
                            }
                        }
                    }
                    foreach (DefendModeData data2 in list)
                    {
                        if (this._defendModeDataList.Contains(data2))
                        {
                            this._defendModeDataList.Remove(data2);
                        }
                    }
                    list.Clear();
                }
            }
            return false;
        }

        public bool OnKilled(EvtKilled evt)
        {
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.killerID) == 3)
            {
                if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) != 4)
                {
                    return false;
                }
                if (!Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.killerID))
                {
                    return false;
                }
                this._monsterKillAmount++;
                if ((this._maxMonsterDisappearAmount != 0) && ((this._monsterEnterAmount + this._monsterKillAmount) == this._maxMonsterDisappearAmount))
                {
                    Singleton<EventManager>.Instance.FireEvent(new EvtLevelDefendState(DefendModeType.Result, this._maxMonsterDisappearAmount), MPEventDispatchMode.Normal);
                }
            }
            return false;
        }

        public bool OnLevelState(EvtLevelState evt)
        {
            if (evt.state == EvtLevelState.State.Start)
            {
                this.SetActive(true);
            }
            return true;
        }

        public override void OnRemoved()
        {
            Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldEnter>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(this._levelActor.runtimeID);
        }

        private void RefreshDisplay(DefendModeData item, int monsterTagID = 0)
        {
            string body = string.Empty;
            if (item.isKey)
            {
                if (item.type == DefendModeType.Single)
                {
                    body = string.Format("{0}/{1}", item.currentValue, item.targetValue);
                }
                else if (item.type == DefendModeType.Group)
                {
                    body = string.Format("{0}/{1}", item.currentValue, item.targetValue);
                }
                else if (item.type == DefendModeType.Certain)
                {
                    if ((monsterTagID != 0) && (monsterTagID == item.uniqueID))
                    {
                        body = "1/1";
                    }
                    else
                    {
                        body = "0/1";
                    }
                }
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetDefendModeText, body));
            }
        }

        public void RemoveTriggerFieldActor(TriggerFieldActor triggerFieldActor)
        {
            if ((triggerFieldActor != null) && this._triggerFieldActorList.Contains(triggerFieldActor))
            {
                this._triggerFieldActorList.Remove(triggerFieldActor);
            }
        }

        public void Reset(int targetValue = 0)
        {
            this._monsterEnterAmount = 0;
            this._monsterKillAmount = 0;
            this._maxMonsterDisappearAmount = targetValue;
            this._defendModeDataList.Clear();
            this._certainMonsterList.Clear();
            this.SetActive(false);
        }

        public void SetActive(bool active)
        {
            this._active = active;
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShowDefendModeText, this._active));
        }

        public void Stop()
        {
            this._monsterEnterAmount = 0;
            this._monsterKillAmount = 0;
            this._maxMonsterDisappearAmount = 0;
            this._triggerFieldActorList.Clear();
            this._defendModeDataList.Clear();
            this._certainMonsterList.Clear();
            this.SetActive(false);
        }

        public int MonsterEnterAmount
        {
            get
            {
                return this._monsterEnterAmount;
            }
        }

        public int MonsterKillAmount
        {
            get
            {
                return this._monsterKillAmount;
            }
        }

        private class DefendModeData
        {
            public int currentValue;
            public bool isKey;
            public int targetValue;
            public DefendModeType type;
            public int uniqueID;

            public DefendModeData(int uniqueID, bool isKey)
            {
                this.type = DefendModeType.Certain;
                this.targetValue = 0;
                this.currentValue = 0;
                this.uniqueID = uniqueID;
                this.isKey = isKey;
            }

            public DefendModeData(DefendModeType type, int targetValue, bool isKey)
            {
                this.type = type;
                this.targetValue = targetValue;
                this.currentValue = 0;
                this.uniqueID = 0;
                this.isKey = isKey;
            }

            public DefendModeData(DefendModeType type, int value, int currentValue, bool isKey)
            {
                this.type = type;
                this.targetValue = value;
                this.currentValue = currentValue;
                this.uniqueID = 0;
                this.isKey = isKey;
            }
        }
    }
}

