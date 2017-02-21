namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class LevelAntiCheatPlugin : BaseActorPlugin
    {
        private LevelDamageStasticsPlugin _damagePlugin;
        private SafeInt32 _frameCount = 0;
        private SafeFloat _levelStartTime = 0f;
        private SafeFloat _unscaledLevelTime = 0f;

        public LevelAntiCheatPlugin(LevelDamageStasticsPlugin damagePlugin)
        {
            this._damagePlugin = damagePlugin;
            this._levelStartTime = (SafeFloat) Miscs.GetTimeStampFromDateTime(DateTime.Now);
        }

        private void AddData(StageCheatData.Type type, float value)
        {
            StageCheatData item = new StageCheatData();
            item.set_type(type);
            item.set_value(value);
            this.cheatDataList.Add(item);
        }

        public void CollectAntiCheatData()
        {
            this.cheatDataList = new List<StageCheatData>();
            this.AddData(0x3e9, (float) this._levelStartTime);
            this.AddData(0x3ea, (float) this._unscaledLevelTime);
            this.AddData(0x3eb, this._unscaledLevelTime / ((float) this._frameCount));
            List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
            for (int i = 0; i < allPlayerAvatars.Count; i++)
            {
                BaseMonoAvatar avatar = allPlayerAvatars[i];
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar.GetRuntimeID());
                AvatarStastics avatarStastics = this._damagePlugin.GetAvatarStastics(avatar.GetRuntimeID());
                int num2 = i * 100;
                this.AddData((StageCheatData.Type) (0x7d1 + num2), (float) avatar.AvatarTypeID);
                this.AddData((StageCheatData.Type) (0x7d2 + num2), (float) actor.level);
                this.AddData((StageCheatData.Type) (0x7d3 + num2), actor.avatarDataItem.CombatNum);
                this.AddData((StageCheatData.Type) (0x7d4 + num2), (float) actor.attack);
                this.AddData((StageCheatData.Type) (0x7e6 + num2), (float) avatarStastics.onStageTime);
                this.AddData((StageCheatData.Type) (0x7da + num2), (float) avatarStastics.hpMax);
                this.AddData((StageCheatData.Type) (0x7db + num2), (float) avatarStastics.hpBegin);
                this.AddData((StageCheatData.Type) (0x7dc + num2), (float) actor.HP);
                this.AddData((StageCheatData.Type) (0x7dd + num2), (float) avatarStastics.hpGain);
                this.AddData((StageCheatData.Type) (0x7d5 + num2), (float) avatarStastics.spMax);
                this.AddData((StageCheatData.Type) (0x7d6 + num2), (float) avatarStastics.spBegin);
                this.AddData((StageCheatData.Type) (0x7d7 + num2), (float) actor.SP);
                this.AddData((StageCheatData.Type) (0x7d8 + num2), (float) avatarStastics.SpRecover);
                this.AddData((StageCheatData.Type) (0x7d9 + num2), (float) avatarStastics.spUse);
                this.AddData((StageCheatData.Type) (0x7de + num2), (float) avatarStastics.avatarHitTimes);
                this.AddData((StageCheatData.Type) (0x7df + num2), (float) avatarStastics.avatarDamage);
                this.AddData((StageCheatData.Type) (0x7e0 + num2), (float) avatarStastics.hitNormalDamageMax);
                this.AddData((StageCheatData.Type) (0x7e1 + num2), (float) avatarStastics.hitCriticalDamageMax);
                this.AddData((StageCheatData.Type) (0x7e2 + num2), (float) avatarStastics.avatarBeingHitTimes);
                this.AddData((StageCheatData.Type) (0x7e3 + num2), (float) avatarStastics.behitNormalDamageMax);
                this.AddData((StageCheatData.Type) (0x7e4 + num2), (float) avatarStastics.behitCriticalDamageMax);
                this.AddData((StageCheatData.Type) (0x7e5 + num2), (float) avatarStastics.comboMax);
            }
        }

        public override void Core()
        {
            base.Core();
            this._unscaledLevelTime += Time.unscaledDeltaTime;
            this._frameCount += 1;
        }

        public List<StageCheatData> cheatDataList { get; private set; }
    }
}

