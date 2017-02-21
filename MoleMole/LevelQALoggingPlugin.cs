namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;

    public class LevelQALoggingPlugin : BaseActorPlugin
    {
        public LevelActor _levelActor;

        public LevelQALoggingPlugin(LevelActor levelActor)
        {
            this._levelActor = levelActor;
        }

        private void DebugLog(string format, params object[] args)
        {
        }

        private bool ListenAttackLanded(EvtAttackLanded evt)
        {
            ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID);
            ushort num2 = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.attackeeID);
            if (((num == 3) || (num == 4)) && ((num2 == 3) || (num2 == 4)))
            {
                object[] args = new object[] { Miscs.GetDebugActorName(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID)), Miscs.GetDebugActorName(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackeeID)), (evt.animEventID != null) ? evt.animEventID : "<!null>", evt.attackResult.GetDebugOutput() };
                this.DebugLog("{0} 外在攻击到 {1} 成功, 判定 ID {2}, 攻击结果 {3}", args);
            }
            return true;
        }

        private bool ListenAvatarCreated(EvtAvatarCreated evt)
        {
            <ListenAvatarCreated>c__AnonStoreyBA yba = new <ListenAvatarCreated>c__AnonStoreyBA {
                <>f__this = this,
                avatarActor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.avatarID)
            };
            yba.avatarActor.onAbilityStateAdd = (Action<AbilityState, bool>) Delegate.Combine(yba.avatarActor.onAbilityStateAdd, new Action<AbilityState, bool>(yba.<>m__7E));
            yba.avatarActor.onAbilityStateRemove = (Action<AbilityState>) Delegate.Combine(yba.avatarActor.onAbilityStateRemove, new Action<AbilityState>(yba.<>m__7F));
            yba.avatarActor.onHPChanged = (Action<float, float, float>) Delegate.Combine(yba.avatarActor.onHPChanged, new Action<float, float, float>(yba.<>m__80));
            yba.avatarActor.onSPChanged = (Action<float, float, float>) Delegate.Combine(yba.avatarActor.onSPChanged, new Action<float, float, float>(yba.<>m__81));
            yba.avatarActor.entity.onActiveChanged = (Action<bool>) Delegate.Combine(yba.avatarActor.entity.onActiveChanged, new Action<bool>(yba.<>m__82));
            yba.avatarActor.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(yba.avatarActor.entity.onCurrentSkillIDChanged, new Action<string, string>(yba.<>m__83));
            return true;
        }

        private bool ListenDamageLanded(EvtDamageLanded evt)
        {
            ushort num = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID);
            ushort num2 = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.attackeeID);
            if (((num == 3) || (num == 4)) && ((num2 == 3) || (num2 == 4)))
            {
                object[] args = new object[] { Miscs.GetDebugActorName(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID)), Miscs.GetDebugActorName(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.attackeeID)), evt.attackResult.GetDebugOutput() };
                this.DebugLog("{0} 内在攻击到 {1} 成功，攻击结果 {2}", args);
            }
            return true;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtAvatarCreated)
            {
                return this.ListenAvatarCreated((EvtAvatarCreated) evt);
            }
            if (evt is EvtMonsterCreated)
            {
                return this.ListenMonsterCreated((EvtMonsterCreated) evt);
            }
            if (evt is EvtKilled)
            {
                return this.ListenKilled((EvtKilled) evt);
            }
            if (evt is EvtAttackLanded)
            {
                return this.ListenAttackLanded((EvtAttackLanded) evt);
            }
            return ((evt is EvtDamageLanded) && this.ListenDamageLanded((EvtDamageLanded) evt));
        }

        private bool ListenKilled(EvtKilled evt)
        {
            if ((Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) != 3) && (Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) != 4))
            {
                return false;
            }
            object[] args = new object[] { Miscs.GetDebugActorName(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.targetID)), Miscs.GetDebugActorName(Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.killerID)) };
            this.DebugLog("{0} 被 {1} 杀死", args);
            return true;
        }

        private bool ListenMonsterCreated(EvtMonsterCreated evt)
        {
            <ListenMonsterCreated>c__AnonStoreyBB ybb = new <ListenMonsterCreated>c__AnonStoreyBB {
                <>f__this = this,
                monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.monsterID)
            };
            ybb.monsterActor.onAbilityStateAdd = (Action<AbilityState, bool>) Delegate.Combine(ybb.monsterActor.onAbilityStateAdd, new Action<AbilityState, bool>(ybb.<>m__84));
            ybb.monsterActor.onAbilityStateRemove = (Action<AbilityState>) Delegate.Combine(ybb.monsterActor.onAbilityStateRemove, new Action<AbilityState>(ybb.<>m__85));
            ybb.monsterActor.onHPChanged = (Action<float, float, float>) Delegate.Combine(ybb.monsterActor.onHPChanged, new Action<float, float, float>(ybb.<>m__86));
            ybb.monsterActor.entity.onCurrentSkillIDChanged = (Action<string, string>) Delegate.Combine(ybb.monsterActor.entity.onCurrentSkillIDChanged, new Action<string, string>(ybb.<>m__87));
            return true;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackLanded>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtDamageLanded>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarCreated>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(this._levelActor.runtimeID);
        }

        public override void OnRemoved()
        {
            Singleton<EventManager>.Instance.RemoveEventListener<EvtAttackLanded>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtDamageLanded>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarCreated>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(this._levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(this._levelActor.runtimeID);
        }

        [CompilerGenerated]
        private sealed class <ListenAvatarCreated>c__AnonStoreyBA
        {
            internal LevelQALoggingPlugin <>f__this;
            internal AvatarActor avatarActor;

            internal void <>m__7E(AbilityState state, bool muteDisplay)
            {
                object[] args = new object[] { Miscs.GetDebugActorName(this.avatarActor), state };
                this.<>f__this.DebugLog("{0} 开始Buff/Debuff: {1}", args);
            }

            internal void <>m__7F(AbilityState state)
            {
                object[] args = new object[] { Miscs.GetDebugActorName(this.avatarActor), state };
                this.<>f__this.DebugLog("{0} 停止Buff/Debuff: {1}", args);
            }

            internal void <>m__80(float orig, float newValue, float amount)
            {
                if (amount > 0f)
                {
                    object[] args = new object[] { Miscs.GetDebugActorName(this.avatarActor), amount, newValue };
                    this.<>f__this.DebugLog("{0} 回复HP {1}, 新 HP 值为 {2}", args);
                }
            }

            internal void <>m__81(float orig, float newValue, float amount)
            {
                if (amount > 0f)
                {
                    object[] args = new object[] { Miscs.GetDebugActorName(this.avatarActor), amount, newValue };
                    this.<>f__this.DebugLog("{0} 回复SP {1}，新 SP 值为 {2}", args);
                }
            }

            internal void <>m__82(bool active)
            {
                object[] args = new object[] { Miscs.GetDebugActorName(this.avatarActor), !active ? "下场" : "上场" };
                this.<>f__this.DebugLog("{0} {1}", args);
            }

            internal void <>m__83(string from, string to)
            {
                object[] args = new object[] { Miscs.GetDebugActorName(this.avatarActor), (to != null) ? to : "<null>" };
                this.<>f__this.DebugLog("{0} SkillID 变动为 {1}", args);
            }
        }

        [CompilerGenerated]
        private sealed class <ListenMonsterCreated>c__AnonStoreyBB
        {
            internal LevelQALoggingPlugin <>f__this;
            internal MonsterActor monsterActor;

            internal void <>m__84(AbilityState state, bool muteDisplay)
            {
                object[] args = new object[] { Miscs.GetDebugActorName(this.monsterActor), state };
                this.<>f__this.DebugLog("{0} 开始Buff/Debuff: {1}", args);
            }

            internal void <>m__85(AbilityState state)
            {
                object[] args = new object[] { Miscs.GetDebugActorName(this.monsterActor), state };
                this.<>f__this.DebugLog("{0} 停止Buff/Debuff: {1}", args);
            }

            internal void <>m__86(float orig, float newValue, float amount)
            {
                if (amount > 0f)
                {
                    object[] args = new object[] { Miscs.GetDebugActorName(this.monsterActor), amount, newValue };
                    this.<>f__this.DebugLog("{0} 回复HP {1}, 新 HP 值为 {2}", args);
                }
            }

            internal void <>m__87(string from, string to)
            {
                object[] args = new object[] { Miscs.GetDebugActorName(this.monsterActor), (to != null) ? to : "<null>" };
                this.<>f__this.DebugLog("{0} SkillID 变动为 {1}", args);
            }
        }
    }
}

