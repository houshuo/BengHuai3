namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class LevelDamageStasticsPlugin : BaseActorPlugin
    {
        [ShowInInspector]
        private Dictionary<int, AvatarStastics> _avatarStasticsDict = new Dictionary<int, AvatarStastics>();
        private bool _isInit;
        private LevelActor _levelActor;
        [ShowInInspector]
        private Dictionary<MonsterKey, MonsterStastics> _monsterAverageStasticsDict = new Dictionary<MonsterKey, MonsterStastics>();
        [ShowInInspector]
        private Dictionary<uint, MonsterStastics> _monsterStasticsDict = new Dictionary<uint, MonsterStastics>();
        [ShowInInspector]
        private PlayerStastics _playerStastics = new PlayerStastics();
        private float _updateTimer;
        public float allDamage;
        public Dictionary<string, uint> attackTimeList = new Dictionary<string, uint>();
        public uint avatarActiveWeaponSkillTimes;
        public uint avatarAttackTimes;
        public uint avatarBeingBreakTimes;
        public uint avatarBeingHitTimes;
        public uint avatarBreakTimes;
        public float avatarDamage;
        public uint avatarEffectHitTimes;
        public uint avatarSkill01Times;
        public uint avatarSkill02Times;
        public float avatarWeaponDdamage;
        public List<string> basicInfoList = new List<string>();
        public uint evadeEffectTimes;
        public uint evadeSuccessTimes;
        public uint evadeTimes;
        public List<string> extraInfoList = new List<string>();
        public bool isStageCreated;
        public bool isUpdating;
        public uint missTimes;
        public uint monsterAttackTimes;
        public float monsterDamage;
        public uint monstersBeingHitTimes;
        public uint normalAttackTimes;
        private string resultStr = string.Empty;
        public int screenRotateTimes;
        public const int SIMPLE_RESULT_NUM = 7;
        public List<float> simpleInfoList = new List<float>();
        public uint specialAttackTimes;
        public float spGet;
        public float stageTime;

        public LevelDamageStasticsPlugin(LevelActor levelActor)
        {
            this._levelActor = levelActor;
        }

        public void AddScreenRotateTimes()
        {
            this.screenRotateTimes++;
            if (this.isStageCreated && (this._playerStastics != null))
            {
                this._playerStastics.screenRotateTimes = SafeInt32.op_Increment(this._playerStastics.screenRotateTimes);
            }
        }

        public void ControlDamageStastics(DamageStastcisControlType type)
        {
            if (type == DamageStastcisControlType.DamageStasticsStart)
            {
                this.ResetBasicPara();
                this.isUpdating = true;
            }
            else if (type == DamageStastcisControlType.DamageStasticsEnd)
            {
                this.ResetBasicPara();
                this.isUpdating = false;
            }
            else if (type == DamageStastcisControlType.DamageStasticsPause)
            {
                this.isUpdating = false;
            }
            else if (type == DamageStastcisControlType.DamageStasticsResume)
            {
                this.isUpdating = true;
            }
            else if (type == DamageStastcisControlType.DamageStasticsResult)
            {
                this.ShowResult();
            }
            else if (type == DamageStastcisControlType.DamageStasticsStoreResult)
            {
                this.StoreResult();
            }
            else if (type == DamageStastcisControlType.DamageStasticsStoreResultShow)
            {
                this.ShowStoreResult();
            }
        }

        public override void Core()
        {
            if (!this._isInit)
            {
                this.InitDate();
            }
            if (this.isStageCreated && (this._playerStastics != null))
            {
                this.stageTime += Time.deltaTime;
                this._playerStastics.stageTime += Time.deltaTime;
            }
            if (this.isUpdating)
            {
                this._updateTimer += Time.deltaTime;
                foreach (KeyValuePair<int, AvatarStastics> pair in this._avatarStasticsDict)
                {
                    AvatarStastics stastics = pair.Value;
                    if ((stastics.isAlive != 0) && (stastics.isOnStage != 0))
                    {
                        stastics.battleTime += Time.deltaTime;
                        stastics.onStageTime += Time.deltaTime;
                    }
                }
                Dictionary<uint, MonsterStastics>.Enumerator enumerator2 = this._monsterStasticsDict.GetEnumerator();
                try
                {
                    while (enumerator2.MoveNext())
                    {
                        KeyValuePair<uint, MonsterStastics> current = enumerator2.Current;
                        MonsterStastics stastics2 = current.Value;
                        if (stastics2.isAlive)
                        {
                            stastics2.aliveTime += Time.deltaTime;
                        }
                    }
                }
                finally
                {
                    Dictionary<int, AvatarStastics>.Enumerator enumerator;
                    enumerator.Dispose();
                }
            }
        }

        public AvatarStastics GetAvatarStastics(uint avatarRuntimeID)
        {
            BaseMonoAvatar helperAvatar = Singleton<AvatarManager>.Instance.GetHelperAvatar();
            if ((helperAvatar != null) && (helperAvatar.GetRuntimeID() == avatarRuntimeID))
            {
                return null;
            }
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatarRuntimeID);
            if (actor == null)
            {
                return null;
            }
            if (!this._avatarStasticsDict.ContainsKey(actor.avatarDataItem.avatarID))
            {
                AvatarStastics stastics = new AvatarStastics(actor.avatarDataItem.avatarID) {
                    avatarLevel = actor.level,
                    avatarStar = actor.avatarDataItem.star,
                    stageID = Singleton<LevelScoreManager>.Instance.LevelId
                };
                this._avatarStasticsDict[actor.avatarDataItem.avatarID] = stastics;
            }
            return this._avatarStasticsDict[actor.avatarDataItem.avatarID];
        }

        private MonsterStastics GetMonsterStastics(uint monsterRuntimeID)
        {
            MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(monsterRuntimeID);
            if (actor == null)
            {
                return null;
            }
            BaseMonoMonster monster = actor.monster;
            if (!this._monsterStasticsDict.ContainsKey(monsterRuntimeID))
            {
                this._monsterStasticsDict[monsterRuntimeID] = new MonsterStastics(monster.MonsterName, monster.TypeName, (int) actor.level);
            }
            return this._monsterStasticsDict[monsterRuntimeID];
        }

        private void InitDate()
        {
            if (Singleton<AvatarManager>.Instance.TryGetLocalAvatar() != null)
            {
                if (this._levelActor.levelMode == LevelActor.Mode.Single)
                {
                    AvatarStastics avatarStastics = this.GetAvatarStastics(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
                    avatarStastics.isOnStage = 1;
                    avatarStastics.swapInTimes = SafeInt32.op_Increment(avatarStastics.swapInTimes);
                }
                else if (this._levelActor.levelMode == LevelActor.Mode.Multi)
                {
                    foreach (BaseMonoAvatar avatar2 in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
                    {
                        AvatarStastics stastics2 = this.GetAvatarStastics(avatar2.GetRuntimeID());
                        stastics2.isOnStage = 1;
                        stastics2.swapInTimes = SafeInt32.op_Increment(stastics2.swapInTimes);
                    }
                }
                this._isInit = true;
            }
        }

        private bool ListenAttackStart(EvtAttackStart evt)
        {
            if (this.isUpdating)
            {
                switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID))
                {
                    case 3:
                    {
                        this.avatarAttackTimes++;
                        if (this.attackTimeList.ContainsKey(evt.skillID))
                        {
                            Dictionary<string, uint> dictionary;
                            string str;
                            uint num3 = dictionary[str];
                            (dictionary = this.attackTimeList)[str = evt.skillID] = num3 + 1;
                        }
                        else
                        {
                            this.attackTimeList[evt.skillID] = 1;
                        }
                        AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.targetID);
                        if (((actor != null) && actor.config.Skills.ContainsKey(evt.skillID)) && (actor.config.Skills[evt.skillID].SkillCategoryTag != null))
                        {
                            for (int i = 0; i < actor.config.Skills[evt.skillID].SkillCategoryTag.Length; i++)
                            {
                                if ((actor.config.Skills[evt.skillID].SkillCategoryTag[i] == AttackResult.AttackCategoryTag.Branch) || (actor.config.Skills[evt.skillID].SkillCategoryTag[i] == AttackResult.AttackCategoryTag.Charge))
                                {
                                    AvatarStastics avatarStastics = this.GetAvatarStastics(evt.targetID);
                                    if (avatarStastics != null)
                                    {
                                        avatarStastics.avatarSpecialAttackTimes = SafeInt32.op_Increment(avatarStastics.avatarSpecialAttackTimes);
                                    }
                                    this.specialAttackTimes++;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                    case 4:
                        this.monsterAttackTimes++;
                        break;
                }
            }
            return true;
        }

        private bool ListenAvatarCreated(EvtAvatarCreated evt)
        {
            if (Singleton<AvatarManager>.Instance.IsPlayerAvatar(evt.avatarID))
            {
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.avatarID);
                actor.onHPChanged = (Action<float, float, float>) Delegate.Combine(actor.onHPChanged, new Action<float, float, float>(this.OnHPChanged));
                actor.onSPChanged = (Action<float, float, float>) Delegate.Combine(actor.onSPChanged, new Action<float, float, float>(this.OnSPChanged));
                AvatarStastics avatarStastics = this.GetAvatarStastics(evt.avatarID);
                avatarStastics.hpBegin = actor.HP;
                avatarStastics.spBegin = actor.SP;
                avatarStastics.hpMax = actor.maxHP;
                avatarStastics.spMax = actor.maxSP;
            }
            return true;
        }

        private bool ListenAvatarSwapInEnd(EvtAvatarSwapInEnd evt)
        {
            AvatarStastics avatarStastics = this.GetAvatarStastics(evt.targetID);
            if (avatarStastics != null)
            {
                avatarStastics.swapInTimes = SafeInt32.op_Increment(avatarStastics.swapInTimes);
                avatarStastics.isOnStage = 1;
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
                actor.onSPChanged = (Action<float, float, float>) Delegate.Combine(actor.onSPChanged, new Action<float, float, float>(this.OnSPChanged));
            }
            return true;
        }

        private bool ListenAvatarSwapOutStart(EvtAvatarSwapOutStart evt)
        {
            AvatarStastics avatarStastics = this.GetAvatarStastics(evt.targetID);
            if (avatarStastics != null)
            {
                avatarStastics.swapOutTimes = SafeInt32.op_Increment(avatarStastics.swapOutTimes);
                avatarStastics.isOnStage = 0;
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
                actor.onSPChanged = (Action<float, float, float>) Delegate.Remove(actor.onSPChanged, new Action<float, float, float>(this.OnSPChanged));
            }
            return true;
        }

        private bool ListenBeingHit(EvtBeingHit evt)
        {
            if (evt.attackData.rejected)
            {
                return false;
            }
            if (this.isUpdating)
            {
                if (!evt.attackData.IsFinalResolved())
                {
                    return false;
                }
                if (!evt.attackData.isAnimEventAttack)
                {
                    return false;
                }
                this.allDamage += evt.attackData.GetTotalDamage();
                ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID);
                ushort num2 = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.sourceID);
                switch (num)
                {
                    case 3:
                    {
                        if (num2 == 4)
                        {
                            MonsterStastics monsterStastics = this.GetMonsterStastics(evt.sourceID);
                            if (monsterStastics != null)
                            {
                                monsterStastics.damage += evt.attackData.GetTotalDamage();
                                monsterStastics.hitAvatarTimes = SafeInt32.op_Increment(monsterStastics.hitAvatarTimes);
                                if (evt.attackData.hitEffect > AttackResult.AnimatorHitEffect.Light)
                                {
                                    monsterStastics.breakAvatarTimes = SafeInt32.op_Increment(monsterStastics.breakAvatarTimes);
                                }
                            }
                        }
                        AvatarStastics avatarStastics = this.GetAvatarStastics(evt.targetID);
                        if (avatarStastics == null)
                        {
                            return true;
                        }
                        avatarStastics.avatarBeDamaged += evt.attackData.GetTotalDamage();
                        avatarStastics.avatarBeingHitTimes = SafeInt32.op_Increment(avatarStastics.avatarBeingHitTimes);
                        if (evt.attackData.hitEffect > AttackResult.AnimatorHitEffect.Light)
                        {
                            avatarStastics.avatarBeingBreakTimes = SafeInt32.op_Increment(avatarStastics.avatarBeingBreakTimes);
                        }
                        this.monsterDamage += evt.attackData.GetTotalDamage();
                        this.avatarBeingHitTimes++;
                        if (evt.attackData.attackerAniDamageRatio > evt.attackData.attackeeAniDefenceRatio)
                        {
                            this.avatarBeingHitTimes++;
                        }
                        if (evt.attackData.hitLevel == AttackResult.ActorHitLevel.Normal)
                        {
                            avatarStastics.behitNormalDamageMax = Mathf.Max(evt.attackData.GetTotalDamage(), (float) avatarStastics.behitNormalDamageMax);
                        }
                        else if (evt.attackData.hitLevel == AttackResult.ActorHitLevel.Critical)
                        {
                            avatarStastics.behitCriticalDamageMax = Mathf.Max(evt.attackData.GetTotalDamage(), (float) avatarStastics.behitCriticalDamageMax);
                        }
                        break;
                    }
                    case 4:
                    {
                        MonsterActor attackee = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
                        if (num2 == 3)
                        {
                            AvatarStastics stastics3 = this.GetAvatarStastics(evt.sourceID);
                            if (stastics3 == null)
                            {
                                return true;
                            }
                            float a = DamageModelLogic.GetNatureDamageBonusRatio(evt.attackData.attackerNature, evt.attackData.attackeeNature, attackee);
                            if (a > 1f)
                            {
                                stastics3.restrictionDamage += evt.attackData.GetTotalDamage();
                            }
                            else if (a < 1f)
                            {
                                stastics3.beRestrictedDamage += evt.attackData.GetTotalDamage();
                            }
                            else if (Mathf.Approximately(a, 1f))
                            {
                                stastics3.normalDamage += evt.attackData.GetTotalDamage();
                            }
                            if (evt.attackData.attackCategoryTag.ContainsTag(AttackResult.AttackCategoryTag.Weapon))
                            {
                                stastics3.avatarActiveWeaponSkillDamage += evt.attackData.GetTotalDamage();
                                this.avatarWeaponDdamage += evt.attackData.GetTotalDamage();
                            }
                            if (evt.attackData.hitEffect > AttackResult.AnimatorHitEffect.Light)
                            {
                                stastics3.avatarBreakTimes = SafeInt32.op_Increment(stastics3.avatarBreakTimes);
                            }
                            stastics3.avatarDamage += evt.attackData.GetTotalDamage();
                            stastics3.avatarHitTimes = SafeInt32.op_Increment(stastics3.avatarHitTimes);
                            if (evt.attackData.isInComboCount)
                            {
                                stastics3.avatarEffectHitTimes = SafeInt32.op_Increment(stastics3.avatarEffectHitTimes);
                            }
                            if (evt.attackData.hitLevel == AttackResult.ActorHitLevel.Normal)
                            {
                                stastics3.hitNormalDamageMax = Mathf.Max(evt.attackData.GetTotalDamage(), (float) stastics3.hitNormalDamageMax);
                            }
                            else if (evt.attackData.hitLevel == AttackResult.ActorHitLevel.Critical)
                            {
                                stastics3.hitCriticalDamageMax = Mathf.Max(evt.attackData.GetTotalDamage(), (float) stastics3.hitCriticalDamageMax);
                            }
                        }
                        this.avatarDamage += evt.attackData.GetTotalDamage();
                        if (evt.attackData.isInComboCount)
                        {
                            this.avatarEffectHitTimes++;
                        }
                        this.monstersBeingHitTimes++;
                        if (evt.attackData.attackerAniDamageRatio > evt.attackData.attackeeAniDefenceRatio)
                        {
                            this.avatarBreakTimes++;
                        }
                        break;
                    }
                }
            }
            return true;
        }

        private bool ListenDefendStart(EvtDefendStart evt)
        {
            if (this.isUpdating)
            {
                AvatarStastics avatarStastics = this.GetAvatarStastics(evt.targetID);
                if (avatarStastics != null)
                {
                    avatarStastics.avatarEvadeTimes = SafeInt32.op_Increment(avatarStastics.avatarEvadeTimes);
                }
            }
            return true;
        }

        private bool ListenDefendSuccess(EvtDefendSuccess evt)
        {
            if (this.isUpdating)
            {
                AvatarStastics avatarStastics = this.GetAvatarStastics(evt.targetID);
                if (avatarStastics != null)
                {
                    avatarStastics.avatarEvadeSuccessTimes = SafeInt32.op_Increment(avatarStastics.avatarEvadeSuccessTimes);
                }
                this.evadeSuccessTimes++;
            }
            return true;
        }

        private bool ListenEvadeStart(EvtEvadeStart evt)
        {
            if (this.isUpdating)
            {
                AvatarStastics avatarStastics = this.GetAvatarStastics(evt.targetID);
                if (avatarStastics != null)
                {
                    avatarStastics.avatarEvadeTimes = SafeInt32.op_Increment(avatarStastics.avatarEvadeTimes);
                }
            }
            return true;
        }

        private bool ListenEvadeSuccess(EvtEvadeSuccess evt)
        {
            if (this.isUpdating)
            {
                AvatarStastics avatarStastics = this.GetAvatarStastics(evt.targetID);
                if (avatarStastics != null)
                {
                    avatarStastics.avatarEvadeSuccessTimes = SafeInt32.op_Increment(avatarStastics.avatarEvadeSuccessTimes);
                }
                this.evadeSuccessTimes++;
            }
            return true;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (!Singleton<LevelManager>.Instance.IsPaused())
            {
                if (evt is EvtStageReady)
                {
                    return this.ListenStageReady((EvtStageReady) evt);
                }
                if (evt is EvtBeingHit)
                {
                    return this.ListenBeingHit((EvtBeingHit) evt);
                }
                if (evt is EvtAttackStart)
                {
                    return this.ListenAttackStart((EvtAttackStart) evt);
                }
                if (evt is EvtLevelBuffState)
                {
                    return this.ListenLevelBuffState((EvtLevelBuffState) evt);
                }
                if (evt is EvtEvadeStart)
                {
                    return this.ListenEvadeStart((EvtEvadeStart) evt);
                }
                if (evt is EvtDefendStart)
                {
                    return this.ListenDefendStart((EvtDefendStart) evt);
                }
                if (evt is EvtEvadeSuccess)
                {
                    return this.ListenEvadeSuccess((EvtEvadeSuccess) evt);
                }
                if (evt is EvtDefendSuccess)
                {
                    return this.ListenDefendSuccess((EvtDefendSuccess) evt);
                }
                if (evt is EvtSkillStart)
                {
                    return this.ListenSkillStart((EvtSkillStart) evt);
                }
                if (evt is EvtAvatarCreated)
                {
                    return this.ListenAvatarCreated((EvtAvatarCreated) evt);
                }
                if (evt is EvtAvatarSwapInEnd)
                {
                    return this.ListenAvatarSwapInEnd((EvtAvatarSwapInEnd) evt);
                }
                if (evt is EvtAvatarSwapOutStart)
                {
                    return this.ListenAvatarSwapOutStart((EvtAvatarSwapOutStart) evt);
                }
                if (evt is EvtMonsterCreated)
                {
                    this.ListenMonsterCreated((EvtMonsterCreated) evt);
                }
                else if (evt is EvtKilled)
                {
                    this.ListenKilled((EvtKilled) evt);
                }
            }
            return false;
        }

        private bool ListenKilled(EvtKilled evt)
        {
            switch (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID))
            {
                case 3:
                {
                    AvatarStastics avatarStastics = this.GetAvatarStastics(evt.targetID);
                    if (avatarStastics == null)
                    {
                        return true;
                    }
                    avatarStastics.isAlive = 0;
                    break;
                }
                case 4:
                {
                    MonsterStastics monsterStastics = this.GetMonsterStastics(evt.targetID);
                    if (monsterStastics != null)
                    {
                        monsterStastics.isAlive = false;
                    }
                    break;
                }
            }
            return false;
        }

        private bool ListenLevelBuffState(EvtLevelBuffState evt)
        {
            if (this.isUpdating && (evt.levelBuff == LevelBuffType.WitchTime))
            {
                if (this.GetAvatarStastics(evt.sourceId) != null)
                {
                }
                this.evadeEffectTimes++;
            }
            return true;
        }

        private bool ListenMonsterCreated(EvtMonsterCreated evt)
        {
            this.GetMonsterStastics(evt.monsterID);
            return false;
        }

        private bool ListenSkillStart(EvtSkillStart evt)
        {
            if (this.isUpdating && (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 3))
            {
                AvatarStastics avatarStastics = this.GetAvatarStastics(evt.targetID);
                if (avatarStastics == null)
                {
                    return true;
                }
                if (evt.skillID == "SKL01")
                {
                    avatarStastics.avatarSkill01Times = SafeInt32.op_Increment(avatarStastics.avatarSkill01Times);
                    avatarStastics.avatarEvadeEffectTimes = SafeInt32.op_Increment(avatarStastics.avatarEvadeEffectTimes);
                    this.avatarSkill01Times++;
                }
                else if (evt.skillID == "SKL02")
                {
                    avatarStastics.avatarSkill02Times = SafeInt32.op_Increment(avatarStastics.avatarSkill02Times);
                    this.avatarSkill02Times++;
                }
                else if (evt.skillID == "SKL_WEAPON")
                {
                    avatarStastics.avatarActiveWeaponSkillTimes = SafeInt32.op_Increment(avatarStastics.avatarActiveWeaponSkillTimes);
                    this.avatarActiveWeaponSkillTimes++;
                }
            }
            return true;
        }

        private bool ListenStageReady(EvtStageReady evt)
        {
            if (!this.isStageCreated && (this._playerStastics != null))
            {
                this._playerStastics.ResetPlayerStasticsData();
            }
            this.isStageCreated = true;
            return true;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackStart>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelBuffState>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtEvadeStart>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtEvadeSuccess>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtDefendStart>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtSkillStart>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtDefendSuccess>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarCreated>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapInEnd>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapOutStart>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(this._levelActor.runtimeID);
            Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged = (Action<int, int>) Delegate.Combine(Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged, new Action<int, int>(this.OnLevelComboChanged));
        }

        private void OnHPChanged(float from, float to, float delta)
        {
            if (this.isUpdating)
            {
                AvatarStastics avatarStastics = this.GetAvatarStastics(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
                if (to > from)
                {
                    avatarStastics.hpGain += to - from;
                }
                if (avatarStastics.hpMax < to)
                {
                    avatarStastics.hpMax = to;
                }
            }
        }

        private void OnLevelComboChanged(int from, int to)
        {
            int num = to + 1;
            AvatarStastics avatarStastics = this.GetAvatarStastics(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
            if ((avatarStastics != null) && (avatarStastics.comboMax < num))
            {
                avatarStastics.comboMax = (SafeFloat) num;
            }
        }

        public override void OnRemoved()
        {
            Singleton<EventManager>.Instance.RemoveEventListener<EvtBeingHit>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtAttackStart>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtLevelBuffState>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtSkillStart>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtEvadeStart>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtEvadeSuccess>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtDefendStart>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtDefendSuccess>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarCreated>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarSwapInEnd>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarSwapOutStart>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(this._levelActor.runtimeID);
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
            actor.onSPChanged = (Action<float, float, float>) Delegate.Remove(actor.onSPChanged, new Action<float, float, float>(this.OnSPChanged));
            Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged = (Action<int, int>) Delegate.Remove(Singleton<LevelManager>.Instance.levelActor.onLevelComboChanged, new Action<int, int>(this.OnLevelComboChanged));
        }

        private void OnSPChanged(float from, float to, float delta)
        {
            if (this.isUpdating)
            {
                AvatarStastics avatarStastics = this.GetAvatarStastics(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
                if (to > from)
                {
                    avatarStastics.SpRecover += to - from;
                    if ((to - from) <= AvatarStastics.SELF_SP_RECOVE_UPBOUND)
                    {
                        avatarStastics.selfSPRecover += to - from;
                    }
                    this.spGet += to - from;
                }
                else
                {
                    avatarStastics.spUse += from - to;
                }
                if (avatarStastics.spMax < to)
                {
                    avatarStastics.spMax = to;
                }
            }
        }

        private void PrintIntoFile(string sPath, string content, bool clean = true)
        {
            StreamWriter writer;
            FileInfo info = new FileInfo(sPath);
            if (!info.Exists)
            {
                writer = info.CreateText();
            }
            else
            {
                writer = new StreamWriter(sPath, false);
            }
            writer.Write(content);
            writer.Close();
            writer.Dispose();
        }

        private void ResetBasicPara()
        {
            this.resultStr = string.Empty;
            this.allDamage = 0f;
            this.monsterDamage = 0f;
            this.avatarDamage = 0f;
            this.avatarBreakTimes = 0;
            this.avatarBeingBreakTimes = 0;
            this.avatarEffectHitTimes = 0;
            this.missTimes = 0;
            this.spGet = 0f;
            this.normalAttackTimes = 0;
            this.specialAttackTimes = 0;
            this.evadeTimes = 0;
            this.evadeEffectTimes = 0;
            this.screenRotateTimes = 0;
            this.stageTime = 0f;
            this.avatarAttackTimes = 0;
            this.attackTimeList.Clear();
            this.monsterAttackTimes = 0;
            this.avatarBeingHitTimes = 0;
            this.monstersBeingHitTimes = 0;
            this.avatarSkill01Times = 0;
            this.avatarSkill02Times = 0;
            this._updateTimer = 0f;
        }

        public void ShowResult()
        {
            if (!Singleton<LevelScoreManager>.Instance.useDebugFunction)
            {
                List<AvatarStastics> avatarInfoList = new List<AvatarStastics>();
                List<MonsterStastics> monsterInfoList = new List<MonsterStastics>();
                foreach (KeyValuePair<int, AvatarStastics> pair in this._avatarStasticsDict)
                {
                    AvatarStastics item = pair.Value;
                    item.dps = (item.battleTime <= 0f) ? 0f : (item.avatarDamage / item.battleTime);
                    item.restrictionDamageRatio = (item.avatarDamage <= 0f) ? 0f : (item.restrictionDamage / item.avatarDamage);
                    item.beRestrictedDamageRatio = (item.avatarDamage <= 0f) ? 0f : (item.beRestrictedDamage / item.avatarDamage);
                    item.normalDamageRatio = (item.avatarDamage <= 0f) ? 0f : (item.normalDamage / item.avatarDamage);
                    avatarInfoList.Add(item);
                }
                foreach (KeyValuePair<uint, MonsterStastics> pair2 in this._monsterStasticsDict)
                {
                    MonsterStastics stastics2 = pair2.Value;
                    stastics2.dps = stastics2.damage / stastics2.aliveTime;
                    if (!this._monsterAverageStasticsDict.ContainsKey(stastics2.key))
                    {
                        this._monsterAverageStasticsDict[stastics2.key] = new MonsterStastics(stastics2.key.monsterName, stastics2.key.configType, stastics2.key.level);
                    }
                    MonsterStastics stastics3 = this._monsterAverageStasticsDict[stastics2.key];
                    stastics3.monsterCount = SafeInt32.op_Increment(stastics3.monsterCount);
                    stastics3.damage += stastics2.damage;
                    stastics3.aliveTime += stastics2.aliveTime;
                    stastics3.hitAvatarTimes += stastics2.hitAvatarTimes;
                    stastics3.breakAvatarTimes += stastics2.breakAvatarTimes;
                    stastics3.dps += stastics2.dps;
                }
                foreach (KeyValuePair<MonsterKey, MonsterStastics> pair3 in this._monsterAverageStasticsDict)
                {
                    MonsterStastics stastics4 = pair3.Value;
                    stastics4.damage /= (float) stastics4.monsterCount;
                    stastics4.aliveTime /= (float) stastics4.monsterCount;
                    stastics4.hitAvatarTimes /= stastics4.monsterCount;
                    stastics4.dps /= (float) stastics4.monsterCount;
                    monsterInfoList.Add(stastics4);
                }
                PlayerStastics playerData = this._playerStastics;
                Singleton<NetworkManager>.Instance.RequestStageInnerDataReport(avatarInfoList, monsterInfoList, playerData);
            }
        }

        public void ShowStoreResult()
        {
            string str = string.Empty;
            for (int i = 0; i < this.simpleInfoList.Count; i++)
            {
                if ((i % 7) == 0)
                {
                    str = str + "\n";
                }
                str = str + this.simpleInfoList[i] + "\t";
            }
        }

        public void StoreResult()
        {
            this.simpleInfoList.Add(this._updateTimer);
            this.simpleInfoList.Add(this.avatarDamage);
            this.simpleInfoList.Add(this.avatarDamage / this._updateTimer);
            this.simpleInfoList.Add(this.spGet);
            this.simpleInfoList.Add((float) this.avatarEffectHitTimes);
        }

        public enum TargetType
        {
            AttackTarget,
            LocalAvatar
        }
    }
}

