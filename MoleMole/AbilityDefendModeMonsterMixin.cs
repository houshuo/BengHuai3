namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilityDefendModeMonsterMixin : BaseAbilityMixin
    {
        private int _currentHatredAIValue;
        private int _defaultHatredAIValue;
        private List<float> _hatredAIAreaSections;
        private List<int> _hatredAIValues;
        private EntityTimer _hatredDecreaseTimer;
        private EntityTimer _minAISwitchTimer;
        private BaseMonoMonster _monster;
        private MonsterActor _monsterActor;
        private Dictionary<uint, float> _monsterHatredDic;
        private const string AI_TYPE_VARIABLE = "_CommonAIType";
        private DefendModeMonsterMixin config;

        public AbilityDefendModeMonsterMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._hatredAIAreaSections = new List<float>();
            this._hatredAIValues = new List<int>();
            this.config = (DefendModeMonsterMixin) config;
            this._monsterActor = base.actor as MonsterActor;
            this._monster = base.entity as BaseMonoMonster;
            this._monsterHatredDic = new Dictionary<uint, float>();
            this._monsterHatredDic.Clear();
            this._hatredDecreaseTimer = new EntityTimer(instancedAbility.Evaluate(this.config.HatredDecreaseInterval));
            this._hatredDecreaseTimer.Reset(false);
            this._minAISwitchTimer = new EntityTimer(instancedAbility.Evaluate(this.config.MinAISwitchDuration));
            this._minAISwitchTimer.Reset(false);
            for (int i = 0; i < this.config.hatredAIAreaSections.Length; i++)
            {
                this._hatredAIAreaSections.Add(this.config.hatredAIAreaSections[i]);
            }
            this._hatredAIAreaSections.Add(1f);
            for (int j = 0; j < this.config.hatredAIValues.Length; j++)
            {
                this._hatredAIValues.Add(this.config.hatredAIValues[j]);
            }
            this._defaultHatredAIValue = this.config.DefaultAIValue;
            this._currentHatredAIValue = this._defaultHatredAIValue;
            BTreeMonsterAIController activeAIController = (BTreeMonsterAIController) this._monster.GetActiveAIController();
            if (activeAIController != null)
            {
                activeAIController.OnAIActive = (Action<bool>) Delegate.Combine(activeAIController.OnAIActive, new Action<bool>(this.OnMonsterAIActive));
            }
        }

        private void AddHatred(uint targetID, float hatred)
        {
            if (!this._hatredDecreaseTimer.isActive)
            {
                this._hatredDecreaseTimer.Reset(true);
            }
            if (this._monsterHatredDic.ContainsKey(targetID))
            {
                this._monsterHatredDic[targetID] = Mathf.Clamp((float) (this._monsterHatredDic[targetID] + hatred), (float) 0f, (float) 1f);
            }
            else
            {
                this._monsterHatredDic.Add(targetID, hatred);
            }
        }

        private void ClearHatred()
        {
            this._monsterHatredDic.Clear();
            this._hatredDecreaseTimer.Reset(false);
        }

        public override void Core()
        {
            if (this.HasHatred())
            {
                this._hatredDecreaseTimer.Core(1f);
                this._minAISwitchTimer.Core(1f);
                if (this._hatredDecreaseTimer.isTimeUp && this._hatredDecreaseTimer.isActive)
                {
                    List<uint> list = new List<uint>();
                    List<uint> list2 = new List<uint>();
                    foreach (KeyValuePair<uint, float> pair in this._monsterHatredDic)
                    {
                        if (pair.Value > 0f)
                        {
                            if ((this._monsterHatredDic[pair.Key] - base.instancedAbility.Evaluate(this.config.HatredDecreateRateByInterval)) < 0f)
                            {
                                list.Add(pair.Key);
                            }
                            else
                            {
                                list2.Add(pair.Key);
                            }
                        }
                    }
                    foreach (uint num in list2)
                    {
                        Dictionary<uint, float> dictionary;
                        uint num7;
                        float num8 = dictionary[num7];
                        (dictionary = this._monsterHatredDic)[num7 = num] = num8 - base.instancedAbility.Evaluate(this.config.HatredDecreateRateByInterval);
                    }
                    foreach (uint num2 in list)
                    {
                        this._monsterHatredDic.Remove(num2);
                    }
                    list.Clear();
                    list2.Clear();
                    if (!this.HasHatred())
                    {
                        this._hatredDecreaseTimer.Reset(false);
                    }
                    else
                    {
                        this._hatredDecreaseTimer.Reset(true);
                    }
                }
                uint num3 = 0;
                foreach (uint num4 in this._monsterHatredDic.Keys)
                {
                    float num5 = 0f;
                    if (this._monsterHatredDic[num4] > num5)
                    {
                        num5 = this._monsterHatredDic[num4];
                        num3 = num4;
                    }
                }
                if (num3 != 0)
                {
                    int hatredAIValue = this.GetHatredAIValue(this._monsterHatredDic[num3]);
                    if (this._currentHatredAIValue != hatredAIValue)
                    {
                        this.SwitchAI(hatredAIValue);
                    }
                }
                else if (this._currentHatredAIValue != this._defaultHatredAIValue)
                {
                    this.SwitchAI(this._defaultHatredAIValue);
                }
            }
        }

        private int GetHatredAIValue(float hatredValue)
        {
            int num = this._currentHatredAIValue;
            for (int i = 0; i < this._hatredAIAreaSections.Count; i++)
            {
                if (hatredValue <= this._hatredAIAreaSections[i])
                {
                    return this._hatredAIValues[i];
                }
            }
            return num;
        }

        private bool HasHatred()
        {
            return (this._monsterHatredDic.Count > 0);
        }

        private bool IsMonsterAIRunning()
        {
            if (this._monster.IsAIControllerActive())
            {
                BTreeMonsterAIController activeAIController = (BTreeMonsterAIController) this._monster.GetActiveAIController();
                return ((activeAIController != null) && activeAIController.IsBehaviorRunning());
            }
            return false;
        }

        public override void OnAdded()
        {
        }

        private bool OnBeingHit(EvtBeingHit evt)
        {
            if (this._monsterActor != null)
            {
                if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.sourceID) != 3)
                {
                    return false;
                }
                float totalDamage = evt.attackData.GetTotalDamage();
                if (totalDamage < 0f)
                {
                    return false;
                }
                float maxHP = (float) this._monsterActor.maxHP;
                float num3 = totalDamage / maxHP;
                if (num3 > base.instancedAbility.Evaluate(this.config.HatredAddThreholdRatioByDamage))
                {
                    this.AddHatred(evt.sourceID, base.instancedAbility.Evaluate(this.config.HatredAddRateByDamage));
                }
            }
            return false;
        }

        private bool OnBuffAdd(EvtBuffAdd evt)
        {
            return false;
        }

        private void OnMonsterAIActive(bool active)
        {
            if (active)
            {
                BTreeMonsterAIController activeAIController = (BTreeMonsterAIController) this._monster.GetActiveAIController();
                activeAIController.SetBehaviorVariable<int>("_CommonAIType", this._defaultHatredAIValue);
                if (activeAIController != null)
                {
                    activeAIController.OnAIActive = (Action<bool>) Delegate.Remove(activeAIController.OnAIActive, new Action<bool>(this.OnMonsterAIActive));
                }
            }
        }

        private bool OnMonsterCreated(EvtMonsterCreated evt)
        {
            return false;
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            if (evt is EvtBeingHit)
            {
                return this.OnBeingHit((EvtBeingHit) evt);
            }
            if (evt is EvtBuffAdd)
            {
                return this.OnBuffAdd((EvtBuffAdd) evt);
            }
            return ((evt is EvtMonsterCreated) && this.OnMonsterCreated((EvtMonsterCreated) evt));
        }

        public override void OnRemoved()
        {
        }

        private void SwitchAI(int aiTypeValue)
        {
            if (!this._minAISwitchTimer.isActive)
            {
                this._minAISwitchTimer.Reset(true);
            }
            if (this._minAISwitchTimer.isTimeUp && this._monster.IsAIControllerActive())
            {
                BTreeMonsterAIController activeAIController = (BTreeMonsterAIController) this._monster.GetActiveAIController();
                if (activeAIController != null)
                {
                    if (!activeAIController.IsBehaviorRunning())
                    {
                        activeAIController.EnableBehavior();
                    }
                    activeAIController.SetBehaviorVariable<int>("_CommonAIType", aiTypeValue);
                    this._currentHatredAIValue = aiTypeValue;
                }
            }
        }
    }
}

