namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using FullInspector;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonsterAIPlugin : BaseActorPlugin
    {
        protected BehaviorDesigner.Runtime.BehaviorTree _aiTree;
        [ShowInInspector]
        private float _escapeRadius = 1f;
        private LevelActor _levelActor;
        protected MonsterActor _owner;
        public SortedList<uint, int> _threatTable;
        [ShowInInspector]
        private AbilityTriggerField _warningFieldActor;
        [ShowInInspector]
        private WarningFieldState _warningFieldState;
        [ShowInInspector]
        private float _warningRadius = 1f;
        public const int BEING_HIT_TREAT_DELTA = 1;
        public const int CHANGE_TARGET_THREAT_DIFF = 10;
        public const int INIT_NEAREST_AVATAR_THREAT = 20;
        public const int MAX_THREAT_LEVEL = 100;
        public const AbilityState SHOULD_INTERRUPT_DEBUFF = (AbilityState.Frozen | AbilityState.Paralyze | AbilityState.Stun);
        public const int SKILL_START_CUR_TARGET_THREAT_DELTA = -15;
        public const int SKILL_START_THREAT_DELTA = -5;

        public MonsterAIPlugin(MonsterActor owner)
        {
            this._owner = owner;
            this._aiTree = this._owner.entity.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            this._levelActor = Singleton<LevelManager>.Instance.levelActor;
            this._threatTable = new SortedList<uint, int>();
        }

        public int GetThreat(uint avatarID)
        {
            if (this._threatTable.ContainsKey(avatarID))
            {
                return this._threatTable[avatarID];
            }
            return 0;
        }

        public void InitNearestAvatarThreat(uint avatarID)
        {
            if (this._threatTable.ContainsKey(avatarID))
            {
                this._threatTable[avatarID] = 20;
            }
            else
            {
                this._threatTable.Add(avatarID, 20);
            }
            this.ThreatTableChanged();
        }

        public void InitWarningField(float warningRadius, float escapeRadius)
        {
            if ((this._warningFieldState == WarningFieldState.None) && ((warningRadius > 0f) && (escapeRadius >= warningRadius)))
            {
                this._warningRadius = warningRadius;
                this._escapeRadius = escapeRadius;
                this._warningFieldActor = Singleton<DynamicObjectManager>.Instance.CreateAbilityTriggerField(this._owner.monster.transform.position, this._owner.monster.transform.forward, this._owner, this._warningRadius, MixinTargetting.Enemy, Singleton<DynamicObjectManager>.Instance.GetNextNonSyncedDynamicObjectRuntimeID(), true);
                this._warningFieldState = WarningFieldState.Outside;
                this._owner.monster.OrderMove = false;
                this._owner.monster.ClearHitTrigger();
                this._owner.monster.ClearAttackTriggers();
                this._owner.monster.SetUseAIController(false);
                Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(this._owner.runtimeID);
                Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldExit>(this._owner.runtimeID);
            }
        }

        private void InterruptMainAI(bool Interruption)
        {
            this._aiTree.SendEvent<object, object>("Interruption", Interruption, Interruption);
        }

        private void InterruptMainAISub(bool Interruption)
        {
            List<SharedVariable> allVariables = this._aiTree.GetAllVariables();
            for (int i = 0; i < allVariables.Count; i++)
            {
                if ((allVariables[i].Name == "_CommonAIType") && (((int) allVariables[i].GetValue()) != 0))
                {
                    return;
                }
            }
            this._aiTree.SendEvent<object>("Interruption", Interruption);
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtStageReady)
            {
                return this.ListenStageReady((EvtStageReady) evt);
            }
            if (evt is EvtKilled)
            {
                return this.ListenKilled((EvtKilled) evt);
            }
            if (evt is EvtFieldEnter)
            {
                return this.OnFieldEnter((EvtFieldEnter) evt);
            }
            return ((evt is EvtFieldExit) && this.OnFieldExit((EvtFieldExit) evt));
        }

        private bool ListenKilled(EvtKilled evt)
        {
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) != 3)
            {
                return false;
            }
            if (this._threatTable.ContainsKey(evt.targetID))
            {
                this._threatTable.Remove(evt.targetID);
            }
            return true;
        }

        private bool ListenStageReady(EvtStageReady evt)
        {
            if (evt.isBorn)
            {
                this.Preparation();
                Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(this._owner.runtimeID);
            }
            return true;
        }

        private void OnAbilityStateAdd(AbilityState state, bool muteDisplayEffect)
        {
            if ((state & (AbilityState.Frozen | AbilityState.Paralyze | AbilityState.Stun)) != AbilityState.None)
            {
                this.InterruptMainAI(true);
            }
        }

        private void OnAbilityStateRemove(AbilityState state)
        {
            if ((state & (AbilityState.Frozen | AbilityState.Paralyze | AbilityState.Stun)) != AbilityState.None)
            {
                this.InterruptMainAI(false);
            }
        }

        public override void OnAdded()
        {
            if (this._levelActor.levelState == LevelActor.LevelState.LevelRunning)
            {
                this.Preparation();
            }
            else
            {
                Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(this._owner.runtimeID);
            }
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(this._owner.runtimeID);
            if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
            {
                AvatarManager instance = Singleton<AvatarManager>.Instance;
                instance.onLocalAvatarChanged = (Action<BaseMonoAvatar, BaseMonoAvatar>) Delegate.Combine(instance.onLocalAvatarChanged, new Action<BaseMonoAvatar, BaseMonoAvatar>(this.OnLocalAvatarChanged));
            }
            this._owner.onAbilityStateAdd = (Action<AbilityState, bool>) Delegate.Combine(this._owner.onAbilityStateAdd, new Action<AbilityState, bool>(this.OnAbilityStateAdd));
            this._owner.onAbilityStateRemove = (Action<AbilityState>) Delegate.Combine(this._owner.onAbilityStateRemove, new Action<AbilityState>(this.OnAbilityStateRemove));
            this._owner.monster.onHitStateChanged = (Action<BaseMonoMonster, bool, bool>) Delegate.Combine(this._owner.monster.onHitStateChanged, new Action<BaseMonoMonster, bool, bool>(this.OnHitStateChanged));
        }

        private bool OnAttackStart(EvtAttackStart evt)
        {
            for (int i = 0; i < this._threatTable.Count; i++)
            {
                uint num2 = this._threatTable.Keys[i];
                this._threatTable[num2] = Mathf.Clamp(this._threatTable[num2] + -5, 0, 100);
            }
            if ((this._owner.monster.AttackTarget != null) && this._threatTable.ContainsKey(this._owner.monster.AttackTarget.GetRuntimeID()))
            {
                uint runtimeID = this._owner.monster.AttackTarget.GetRuntimeID();
                this._threatTable[runtimeID] = Mathf.Clamp(this._threatTable[runtimeID] + -15, 0, 100);
            }
            this.ThreatTableChanged();
            return false;
        }

        private bool OnBeingHit(EvtBeingHit evt)
        {
            if (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.sourceID) != 3)
            {
                return false;
            }
            if (this._threatTable.ContainsKey(evt.sourceID))
            {
                this._threatTable[evt.sourceID] = Mathf.Clamp(this._threatTable[evt.sourceID] + 1, 0, 100);
            }
            else
            {
                this._threatTable[evt.sourceID] = 1;
            }
            this.ThreatTableChanged();
            return true;
        }

        public override bool OnEvent(BaseEvent evt)
        {
            if (evt is EvtBeingHit)
            {
                return this.OnBeingHit((EvtBeingHit) evt);
            }
            return ((evt is EvtAttackStart) && this.OnAttackStart((EvtAttackStart) evt));
        }

        private bool OnFieldEnter(EvtFieldEnter evt)
        {
            if ((this._warningFieldActor == null) || (this._warningFieldActor.runtimeID != evt.targetID))
            {
                return false;
            }
            if (this._warningFieldState != WarningFieldState.Outside)
            {
                return false;
            }
            this._owner.monster.SetUseAIController(true);
            this._warningFieldActor.triggerField.transform.localScale = (Vector3) (Vector3.one * this._escapeRadius);
            this._warningFieldState = WarningFieldState.Inside;
            return true;
        }

        private bool OnFieldExit(EvtFieldExit evt)
        {
            if ((this._warningFieldActor == null) || (this._warningFieldActor.runtimeID != evt.targetID))
            {
                return false;
            }
            if (this._warningFieldState != WarningFieldState.Inside)
            {
                return false;
            }
            this._owner.monster.OrderMove = false;
            this._owner.monster.ClearHitTrigger();
            this._owner.monster.ClearAttackTriggers();
            this._owner.monster.SetUseAIController(false);
            this._warningFieldActor.triggerField.transform.localScale = (Vector3) (Vector3.one * this._warningRadius);
            this._warningFieldState = WarningFieldState.Outside;
            return true;
        }

        private void OnHitStateChanged(BaseMonoMonster monster, bool fromHitState, bool toHitState)
        {
            if (!fromHitState && toHitState)
            {
                this.InterruptMainAISub(true);
            }
            else if (fromHitState && !toHitState)
            {
                this.InterruptMainAISub(false);
            }
        }

        private void OnLocalAvatarChanged(BaseMonoAvatar from, BaseMonoAvatar to)
        {
            if (this._owner.monster.AttackTarget == from)
            {
                this._owner.monster.SetAttackTarget(to);
            }
        }

        public override void OnRemoved()
        {
            if (this._warningFieldState > WarningFieldState.None)
            {
                Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldEnter>(this._owner.runtimeID);
                Singleton<EventManager>.Instance.RemoveEventListener<EvtFieldExit>(this._owner.runtimeID);
                this._warningFieldActor.Kill();
                this._warningFieldState = WarningFieldState.None;
            }
            if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
            {
                AvatarManager instance = Singleton<AvatarManager>.Instance;
                instance.onLocalAvatarChanged = (Action<BaseMonoAvatar, BaseMonoAvatar>) Delegate.Remove(instance.onLocalAvatarChanged, new Action<BaseMonoAvatar, BaseMonoAvatar>(this.OnLocalAvatarChanged));
            }
            this._owner.onAbilityStateAdd = (Action<AbilityState, bool>) Delegate.Remove(this._owner.onAbilityStateAdd, new Action<AbilityState, bool>(this.OnAbilityStateAdd));
            this._owner.onAbilityStateRemove = (Action<AbilityState>) Delegate.Remove(this._owner.onAbilityStateRemove, new Action<AbilityState>(this.OnAbilityStateRemove));
            this._owner.monster.onHitStateChanged = (Action<BaseMonoMonster, bool, bool>) Delegate.Remove(this._owner.monster.onHitStateChanged, new Action<BaseMonoMonster, bool, bool>(this.OnHitStateChanged));
        }

        private void Preparation()
        {
        }

        public void RestartMainAI()
        {
            this.InterruptMainAI(true);
            this.InterruptMainAI(false);
        }

        public uint RetargetByThreat(uint curTargetID)
        {
            int num = 0;
            uint num2 = 0;
            for (int i = 0; i < this._threatTable.Count; i++)
            {
                int num4 = this._threatTable.Values[i];
                if (num4 > num)
                {
                    uint runtimeID = this._threatTable.Keys[i];
                    if (Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID) != null)
                    {
                        BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID);
                        if ((avatarByRuntimeID != null) && avatarByRuntimeID.IsActive())
                        {
                            num = num4;
                            num2 = this._threatTable.Keys[i];
                        }
                    }
                }
            }
            if (num2 != curTargetID)
            {
                float threat = this.GetThreat(curTargetID);
                if (num > (threat + 10f))
                {
                    return num2;
                }
            }
            return curTargetID;
        }

        private void ThreatTableChanged()
        {
            if ((this._owner.monster.AttackTarget != null) && this._owner.monster.AttackTarget.IsActive())
            {
                uint runtimeID = this._owner.monster.AttackTarget.GetRuntimeID();
                uint num2 = this.RetargetByThreat(runtimeID);
                if (runtimeID != num2)
                {
                    this._aiTree.SendEvent<object>("AIThreatRetarget_1", Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(num2));
                }
            }
        }

        private enum WarningFieldState
        {
            None,
            Outside,
            Inside
        }
    }
}

