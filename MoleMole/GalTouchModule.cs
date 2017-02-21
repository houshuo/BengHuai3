namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class GalTouchModule : BaseModule
    {
        private GalTouchInfoItem _currentGalTouchInfo;
        private List<GalTouchInfoItem> _galTouchInfoItems = new List<GalTouchInfoItem>();
        private int _maxDialyAddGoodFeel;
        private bool _waitingForResponse;

        public event Action<int, int> CurrentAvatarChanged;

        public event Action<int, int> GalAddBuff;

        public event GalTouchInfoChangedHandler GalTouchInfoChanged;

        public GalTouchModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
        }

        public void AddBuff(int avatarId, int buffId)
        {
            int num = 0;
            int count = this._galTouchInfoItems.Count;
            while (num < count)
            {
                if (this._galTouchInfoItems[num].avatarId == avatarId)
                {
                    this._galTouchInfoItems[num].buffId = buffId;
                    this._galTouchInfoItems[num].buffRestTime = 5;
                    if (this.GalAddBuff != null)
                    {
                        this.GalAddBuff(avatarId, buffId);
                    }
                }
                num++;
            }
        }

        public void ChangeAvatar(int id)
        {
            if (this._galTouchInfoItems == null)
            {
                Debug.LogError("Gal Touch Module Not Setup");
            }
            else if ((this._currentGalTouchInfo == null) || (this._currentGalTouchInfo.avatarId != id))
            {
                GalTouchInfoItem item = null;
                int num = 0;
                int count = this._galTouchInfoItems.Count;
                while (num < count)
                {
                    if (this._galTouchInfoItems[num].avatarId == id)
                    {
                        item = this._galTouchInfoItems[num];
                        break;
                    }
                    num++;
                }
                if (item == null)
                {
                    Debug.LogError("Invalid Avatar Id");
                }
                else
                {
                    if (this.CurrentAvatarChanged != null)
                    {
                        this.CurrentAvatarChanged((this._currentGalTouchInfo != null) ? this._currentGalTouchInfo.avatarId : -1, item.avatarId);
                    }
                    this._currentGalTouchInfo = item;
                    Singleton<MiHoYoGameData>.Instance.LocalData.LastGalAvatarId = this._currentGalTouchInfo.avatarId;
                    Singleton<MiHoYoGameData>.Instance.Save();
                }
            }
        }

        private void DoSetHeartLevelAndGoodFeel(GalTouchInfoItem item, int heartLevel, int touchGoodFeel, int battleGoodFeel)
        {
            if (item != null)
            {
                int num = touchGoodFeel + battleGoodFeel;
                heartLevel = Mathf.Clamp(heartLevel, 0, 5);
                int num2 = GalTouchData.QueryLevelUpFeelNeed(heartLevel);
                if (num2 != 0)
                {
                    num = Mathf.Clamp(num, 0, num2 - 1);
                }
                item.heartLevel = heartLevel;
                item.touchGoodFeel = Mathf.Min(touchGoodFeel, num);
                item.battleGoodFeel = Mathf.Max(num - item.touchGoodFeel, 0);
            }
        }

        public int GetAvatarGalTouchBuffId(int avatarId)
        {
            int num = 0;
            int count = this._galTouchInfoItems.Count;
            while (num < count)
            {
                if (this._galTouchInfoItems[num].avatarId == avatarId)
                {
                    return this._galTouchInfoItems[num].buffId;
                }
                num++;
            }
            return 0;
        }

        public int GetAvatarGalTouchBuffRestTime(int avatarId)
        {
            int num = 0;
            int count = this._galTouchInfoItems.Count;
            while (num < count)
            {
                if (this._galTouchInfoItems[num].avatarId == avatarId)
                {
                    return this._galTouchInfoItems[num].buffRestTime;
                }
                num++;
            }
            return 0;
        }

        public int GetCharacterBattleGoodFeel()
        {
            if (this._currentGalTouchInfo == null)
            {
                return 0;
            }
            return this._currentGalTouchInfo.battleGoodFeel;
        }

        public int GetCharacterBattleGoodFeel(int id)
        {
            int num = 0;
            int count = this._galTouchInfoItems.Count;
            while (num < count)
            {
                GalTouchInfoItem item = this._galTouchInfoItems[num];
                if (item.avatarId == id)
                {
                    return item.battleGoodFeel;
                }
                num++;
            }
            return 0;
        }

        public int GetCharacterHeartLevel()
        {
            if (this._currentGalTouchInfo == null)
            {
                return 0;
            }
            return this._currentGalTouchInfo.heartLevel;
        }

        public int GetCharacterHeartLevel(int id)
        {
            int num = 0;
            int count = this._galTouchInfoItems.Count;
            while (num < count)
            {
                GalTouchInfoItem item = this._galTouchInfoItems[num];
                if (item.avatarId == id)
                {
                    return item.heartLevel;
                }
                num++;
            }
            return 0;
        }

        public int GetCharacterTodayAddedFeel()
        {
            if (this._currentGalTouchInfo == null)
            {
                return 0;
            }
            return this._currentGalTouchInfo.todayAddedFeel;
        }

        public int GetCharacterTodayAddedFeel(int id)
        {
            int num = 0;
            int count = this._galTouchInfoItems.Count;
            while (num < count)
            {
                GalTouchInfoItem item = this._galTouchInfoItems[num];
                if (item.avatarId == id)
                {
                    return item.todayAddedFeel;
                }
                num++;
            }
            return 0;
        }

        public int GetCharacterTouchGoodFeel()
        {
            if (this._currentGalTouchInfo == null)
            {
                return 0;
            }
            return this._currentGalTouchInfo.touchGoodFeel;
        }

        public int GetCharacterTouchGoodFeel(int id)
        {
            int num = 0;
            int count = this._galTouchInfoItems.Count;
            while (num < count)
            {
                GalTouchInfoItem item = this._galTouchInfoItems[num];
                if (item.avatarId == id)
                {
                    return item.touchGoodFeel;
                }
                num++;
            }
            return 0;
        }

        public int GetCurrentTouchAvatarID()
        {
            return ((this._currentGalTouchInfo == null) ? 0 : this._currentGalTouchInfo.avatarId);
        }

        private int GetLevelTotalTouchLimit(int level)
        {
            int num = 0;
            level = Mathf.Clamp(level, 1, 5);
            for (int i = 1; i < level; i++)
            {
                num += GalTouchData.QueryLevelUpFeelNeed(i);
            }
            if (level != 5)
            {
                num += GalTouchData.QueryLevelUpFeelNeedTouch(level);
            }
            return num;
        }

        public int GetReadyToTouchAvatarID()
        {
            return Singleton<MiHoYoGameData>.Instance.LocalData.LastGalAvatarId;
        }

        public int GetTodayRemainGoodFeel()
        {
            int num = 0;
            int num2 = 0;
            int count = this._galTouchInfoItems.Count;
            while (num2 < count)
            {
                num += this._galTouchInfoItems[num2].todayAddedFeel;
                num2++;
            }
            int num4 = this._maxDialyAddGoodFeel - num;
            if (num4 < 0)
            {
                num4 = 0;
            }
            return num4;
        }

        private void HeartLevelAndGoodFeelToTotal(int feel, int level, out int total)
        {
            int num = 0;
            level = Mathf.Clamp(level, 1, 5);
            for (int i = 2; i <= level; i++)
            {
                num += GalTouchData.QueryLevelUpFeelNeed(i - 1);
            }
            if (level != 5)
            {
                num += feel;
            }
            total = num;
        }

        private void HeartLevelAndGoodFeelToTotalBattle(int feel, int level, out int total)
        {
            int num = 0;
            level = Mathf.Clamp(level, 1, 5);
            for (int i = 2; i <= level; i++)
            {
                num += GalTouchData.QueryLevelUpFeelNeedBattle(i - 1);
            }
            if (level != 5)
            {
                num += feel;
            }
            total = num;
        }

        private void HeartLevelAndGoodFeelToTotalTouch(int feel, int level, out int total)
        {
            int num = 0;
            level = Mathf.Clamp(level, 1, 5);
            for (int i = 2; i <= level; i++)
            {
                num += GalTouchData.QueryLevelUpFeelNeedTouch(i - 1);
            }
            if (level != 5)
            {
                num += feel;
            }
            total = num;
        }

        public void IncreaseBattleGoodFeel(int avatarId, int amount)
        {
            GalTouchInfoItem item = null;
            int num = 0;
            int count = this._galTouchInfoItems.Count;
            while (num < count)
            {
                if (this._galTouchInfoItems[num].avatarId == avatarId)
                {
                    item = this._galTouchInfoItems[num];
                    break;
                }
                num++;
            }
            if (item != null)
            {
                amount = Mathf.Clamp(amount, -item.battleGoodFeel, this.GetTodayRemainGoodFeel());
                int b = GalTouchData.QueryLevelUpFeelNeedBattle(item.heartLevel);
                int num4 = Mathf.Min(item.battleGoodFeel + amount, b);
                amount = num4 - item.battleGoodFeel;
                Singleton<NetworkManager>.Instance.RequestGalAddGoodFeel(item.avatarId, amount, 2);
                item.todayAddedFeel += amount;
                item.battleGoodFeel = num4;
            }
        }

        public void IncreaseTouchGoodFeel(int amount)
        {
            if ((this._currentGalTouchInfo != null) && !this._waitingForResponse)
            {
                GoodFeelLimitType none = GoodFeelLimitType.None;
                bool flag = amount <= 0;
                amount = Mathf.Clamp(amount, -this._currentGalTouchInfo.touchGoodFeel, this.GetTodayRemainGoodFeel());
                if (this._currentGalTouchInfo.heartLevel > 4)
                {
                    if (this.GalTouchInfoChanged != null)
                    {
                        this.GalTouchInfoChanged(this._currentGalTouchInfo.touchGoodFeel + this._currentGalTouchInfo.battleGoodFeel, this._currentGalTouchInfo.heartLevel, this._currentGalTouchInfo.touchGoodFeel + this._currentGalTouchInfo.battleGoodFeel, this._currentGalTouchInfo.heartLevel, GoodFeelLimitType.ReachMax);
                    }
                }
                else
                {
                    int num = GalTouchData.QueryLevelUpFeelNeedTouch(this._currentGalTouchInfo.heartLevel);
                    int num2 = GalTouchData.QueryLevelUpFeelNeedBattle(this._currentGalTouchInfo.heartLevel);
                    if ((this._currentGalTouchInfo.battleGoodFeel >= num2) && !this.IsLimitedByMission(this._currentGalTouchInfo))
                    {
                        int total = 0;
                        int feel = 0;
                        int level = 1;
                        this.HeartLevelAndGoodFeelToTotal(this._currentGalTouchInfo.touchGoodFeel + this._currentGalTouchInfo.battleGoodFeel, this._currentGalTouchInfo.heartLevel, out total);
                        int num6 = total + amount;
                        this.TotalToHeartLevelAndGoodFeel(num6, out feel, out level);
                        if (this._currentGalTouchInfo.heartLevel != level)
                        {
                            num6 = Mathf.Min(num6, this.GetLevelTotalTouchLimit(this._currentGalTouchInfo.heartLevel + 1));
                            this.TotalToHeartLevelAndGoodFeel(num6, out feel, out level);
                        }
                        this.HeartLevelAndGoodFeelToTotal(feel, level, out num6);
                        this._currentGalTouchInfo.todayAddedFeel += num6 - total;
                        if ((amount == 0) && (this.GetTodayRemainGoodFeel() == 0))
                        {
                            none = GoodFeelLimitType.DialyGoodFeel;
                        }
                        if (this.GalTouchInfoChanged != null)
                        {
                            this.GalTouchInfoChanged(this._currentGalTouchInfo.touchGoodFeel + this._currentGalTouchInfo.battleGoodFeel, this._currentGalTouchInfo.heartLevel, feel, level, none);
                        }
                        if (level != this._currentGalTouchInfo.heartLevel)
                        {
                            this._currentGalTouchInfo.battleGoodFeel = 0;
                        }
                        this._currentGalTouchInfo.touchGoodFeel = feel - this._currentGalTouchInfo.battleGoodFeel;
                        this._currentGalTouchInfo.heartLevel = level;
                        Singleton<NetworkManager>.Instance.RequestGalAddGoodFeel(this._currentGalTouchInfo.avatarId, num6 - total, 1);
                        this._waitingForResponse = true;
                    }
                    else
                    {
                        int num7 = Mathf.Clamp(this._currentGalTouchInfo.touchGoodFeel + amount, 0, num - 1);
                        amount = num7 - this._currentGalTouchInfo.touchGoodFeel;
                        if ((amount == 0) && !flag)
                        {
                            if (this.GetTodayRemainGoodFeel() == 0)
                            {
                                none = GoodFeelLimitType.DialyGoodFeel;
                            }
                            else if (this._currentGalTouchInfo.battleGoodFeel < num2)
                            {
                                none = GoodFeelLimitType.Battle;
                            }
                            else if (this.IsLimitedByMission(this._currentGalTouchInfo))
                            {
                                none = GoodFeelLimitType.Mission;
                            }
                        }
                        else
                        {
                            none = GoodFeelLimitType.None;
                        }
                        if (this.GalTouchInfoChanged != null)
                        {
                            this.GalTouchInfoChanged(this._currentGalTouchInfo.touchGoodFeel + this._currentGalTouchInfo.battleGoodFeel, this._currentGalTouchInfo.heartLevel, num7 + this._currentGalTouchInfo.battleGoodFeel, this._currentGalTouchInfo.heartLevel, none);
                        }
                        Singleton<NetworkManager>.Instance.RequestGalAddGoodFeel(this._currentGalTouchInfo.avatarId, amount, 1);
                        this._currentGalTouchInfo.todayAddedFeel += amount;
                        this._waitingForResponse = true;
                        this._currentGalTouchInfo.touchGoodFeel = num7;
                    }
                }
            }
        }

        public bool IsCurrentAvatarLimitedByMission()
        {
            return this.IsLimitedByMission(this._currentGalTouchInfo);
        }

        private bool IsLimitedByMission(GalTouchInfoItem item)
        {
            TouchMissionItem touchMissionItem = GalTouchData.GetTouchMissionItem(item.avatarId, item.heartLevel);
            if (touchMissionItem == null)
            {
                return false;
            }
            MissionDataItem missionDataItem = Singleton<MissionModule>.Instance.GetMissionDataItem(touchMissionItem.missionId);
            return ((missionDataItem == null) || (missionDataItem.status == 2));
        }

        private bool OnAddGoodfeelRsp(AddGoodfeelRsp rsp)
        {
            this._waitingForResponse = false;
            return false;
        }

        private bool OnGetAvatarDataRsp(GetAvatarDataRsp rsp)
        {
            List<Avatar> list = rsp.get_avatar_list();
            int num = 0;
            int count = list.Count;
            while (num < count)
            {
                Avatar avatar = list[num];
                if (avatar.get_touch_goodfeelSpecified() && avatar.get_stage_goodfeelSpecified())
                {
                    GalTouchInfoItem item = null;
                    int num3 = 0;
                    int num4 = this._galTouchInfoItems.Count;
                    while (num3 < num4)
                    {
                        if (this._galTouchInfoItems[num3].avatarId == avatar.get_avatar_id())
                        {
                            item = this._galTouchInfoItems[num3];
                            break;
                        }
                        num3++;
                    }
                    if (item == null)
                    {
                        item = new GalTouchInfoItem {
                            avatarId = (int) avatar.get_avatar_id()
                        };
                        this._galTouchInfoItems.Add(item);
                    }
                    int feel = 0;
                    int level = 1;
                    this.TotalToHeartLevelAndGoodFeel((int) (avatar.get_touch_goodfeel() + avatar.get_stage_goodfeel()), out feel, out level);
                    int num7 = 0;
                    this.TotalToHeartLevelAndGoodFeelBattle((int) avatar.get_stage_goodfeel(), level, out num7);
                    int num8 = feel - num7;
                    item.heartLevel = level;
                    item.touchGoodFeel = num8;
                    item.battleGoodFeel = num7;
                    if (avatar.get_today_has_add_goodfeelSpecified())
                    {
                        item.todayAddedFeel = (int) avatar.get_today_has_add_goodfeel();
                    }
                }
                num++;
            }
            return false;
        }

        private bool OnGetConfigDataRsp(GetConfigRsp rsp)
        {
            if (rsp.get_avatar_max_add_goodfeelSpecified())
            {
                this._maxDialyAddGoodFeel = (int) rsp.get_avatar_max_add_goodfeel();
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 packet)
        {
            switch (packet.getCmdId())
            {
                case 0x19:
                    return this.OnGetAvatarDataRsp(packet.getData<GetAvatarDataRsp>());

                case 0x6f:
                    return this.OnGetConfigDataRsp(packet.getData<GetConfigRsp>());

                case 0x9b:
                    return this.OnAddGoodfeelRsp(packet.getData<AddGoodfeelRsp>());
            }
            return false;
        }

        public void SetHeartLevelAndGoodFeel(int heartLevel, int touchGoodFeel, int battleGoodFeel)
        {
            this.DoSetHeartLevelAndGoodFeel(this._currentGalTouchInfo, heartLevel, touchGoodFeel, battleGoodFeel);
        }

        public void SetHeartLevelAndGoodFeel(int id, int heartLevel, int touchGoodFeel, int battleGoodFeel)
        {
            int num = 0;
            int count = this._galTouchInfoItems.Count;
            while (num < count)
            {
                GalTouchInfoItem item = this._galTouchInfoItems[num];
                if (item.avatarId == id)
                {
                    this.DoSetHeartLevelAndGoodFeel(item, heartLevel, touchGoodFeel, battleGoodFeel);
                    return;
                }
                num++;
            }
        }

        private void TotalToHeartLevelAndGoodFeel(int total, out int feel, out int level)
        {
            total = (total >= 0) ? total : 0;
            int num = 1;
            while (true)
            {
                if (num == 5)
                {
                    total = GalTouchData.QueryLevelUpFeelNeed(4);
                    break;
                }
                int num2 = GalTouchData.QueryLevelUpFeelNeed(num);
                if (total < num2)
                {
                    break;
                }
                total -= num2;
                num++;
            }
            feel = total;
            level = num;
        }

        private void TotalToHeartLevelAndGoodFeelBattle(int total, int level, out int feel)
        {
            total = (total >= 0) ? total : 0;
            for (int i = 1; i < level; i++)
            {
                total -= GalTouchData.QueryLevelUpFeelNeedBattle(i);
            }
            feel = (total >= 0) ? total : 0;
        }

        public int UseBuff(int avatarId)
        {
            int buffId = 0;
            int num2 = 0;
            int count = this._galTouchInfoItems.Count;
            while (num2 < count)
            {
                if (this._galTouchInfoItems[num2].avatarId == avatarId)
                {
                    if ((this._galTouchInfoItems[num2].buffId > 0) && (this._galTouchInfoItems[num2].buffRestTime > 0))
                    {
                        buffId = this._galTouchInfoItems[num2].buffId;
                        GalTouchInfoItem local1 = this._galTouchInfoItems[num2];
                        local1.buffRestTime--;
                        if (this._galTouchInfoItems[num2].buffRestTime <= 0)
                        {
                            this._galTouchInfoItems[num2].buffId = 0;
                        }
                    }
                    return buffId;
                }
                num2++;
            }
            return buffId;
        }

        public delegate void GalTouchInfoChangedHandler(int oldGoodFeel, int oldHeartLevel, int newGoodFeel, int newHeartLevel, GoodFeelLimitType limitType);
    }
}

