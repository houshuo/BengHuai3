namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class FriendBriefDataItem
    {
        private string _nickName;
        public int avatarCombat;
        public AvatarSkillDataItem AvatarLeaderSkill;
        public int avatarLevel;
        public int avatarStar;
        public int level;
        public int showAvatarID;
        public int uid;

        public FriendBriefDataItem(PlayerFriendBriefData briefData)
        {
            this.uid = (int) briefData.get_uid();
            this._nickName = briefData.get_nickname();
            this.level = (int) briefData.get_level();
            this.showAvatarID = (int) briefData.get_avatar_id();
            this.avatarCombat = (int) briefData.get_avatar_combat();
            this.avatarStar = (int) briefData.get_avatar_star();
            this.avatarLevel = (int) briefData.get_avatar_level();
            if (this.avatarLevel == 0)
            {
                this.avatarLevel = 1;
            }
            this.AvatarLeaderSkill = new AvatarDataItem(this.showAvatarID, 1, 0).GetLeaderSkill();
            this.AvatarLeaderSkill.UnLocked = (this.avatarLevel >= this.AvatarLeaderSkill.UnLockLv) && (this.avatarStar >= this.AvatarLeaderSkill.UnLockStar);
            <FriendBriefDataItem>c__AnonStoreyC6 yc = new <FriendBriefDataItem>c__AnonStoreyC6();
            using (List<AvatarSubSkillDetailData>.Enumerator enumerator = briefData.get_main_sub_skill_list().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yc.subSkill = enumerator.Current;
                    AvatarSubSkillDataItem item = this.AvatarLeaderSkill.avatarSubSkillList.Find(new Predicate<AvatarSubSkillDataItem>(yc.<>m__C2));
                    if (item != null)
                    {
                        item.level = (int) yc.subSkill.get_level();
                    }
                }
            }
        }

        public static int CompareToAvatarCombatAsc(FriendBriefDataItem lobj, FriendBriefDataItem robj)
        {
            if (lobj.avatarCombat != robj.avatarCombat)
            {
                return (lobj.avatarCombat - robj.avatarCombat);
            }
            if (lobj.level != robj.level)
            {
                return (lobj.level - robj.level);
            }
            if (lobj.avatarStar != robj.avatarStar)
            {
                return (lobj.avatarStar - robj.avatarStar);
            }
            return (lobj.uid - robj.uid);
        }

        public static int CompareToAvatarCombatDesc(FriendBriefDataItem lobj, FriendBriefDataItem robj)
        {
            if (lobj.avatarCombat != robj.avatarCombat)
            {
                return (robj.avatarCombat - lobj.avatarCombat);
            }
            if (lobj.level != robj.level)
            {
                return (robj.level - lobj.level);
            }
            if (lobj.avatarStar != robj.avatarStar)
            {
                return (robj.avatarStar - lobj.avatarStar);
            }
            return (lobj.uid - robj.uid);
        }

        public static int CompareToAvatarStarAsc(FriendBriefDataItem lobj, FriendBriefDataItem robj)
        {
            if (lobj.avatarStar != robj.avatarStar)
            {
                return (lobj.avatarStar - robj.avatarStar);
            }
            if (lobj.avatarCombat != robj.avatarCombat)
            {
                return (lobj.avatarCombat - robj.avatarCombat);
            }
            if (lobj.level != robj.level)
            {
                return (lobj.level - robj.level);
            }
            return (lobj.uid - robj.uid);
        }

        public static int CompareToAvatarStarDesc(FriendBriefDataItem lobj, FriendBriefDataItem robj)
        {
            if (lobj.avatarStar != robj.avatarStar)
            {
                return (robj.avatarStar - lobj.avatarStar);
            }
            if (lobj.avatarCombat != robj.avatarCombat)
            {
                return (robj.avatarCombat - lobj.avatarCombat);
            }
            if (lobj.level != robj.level)
            {
                return (robj.level - lobj.level);
            }
            return (lobj.uid - robj.uid);
        }

        public static int CompareToFriendNew(FriendBriefDataItem lobj, FriendBriefDataItem robj)
        {
            if (!Singleton<FriendModule>.Instance.IsOldFriend(lobj.uid))
            {
                return -1;
            }
            if (!Singleton<FriendModule>.Instance.IsOldFriend(robj.uid))
            {
                return 1;
            }
            return CompareToLevelDesc(lobj, robj);
        }

        public static int CompareToLevelAsc(FriendBriefDataItem lobj, FriendBriefDataItem robj)
        {
            if (lobj.level != robj.level)
            {
                return (lobj.level - robj.level);
            }
            if (lobj.avatarStar != robj.avatarStar)
            {
                return (lobj.avatarStar - robj.avatarStar);
            }
            if (lobj.avatarCombat != robj.avatarCombat)
            {
                return (lobj.avatarCombat - robj.avatarCombat);
            }
            return (lobj.uid - robj.uid);
        }

        public static int CompareToLevelDesc(FriendBriefDataItem lobj, FriendBriefDataItem robj)
        {
            if (lobj.level != robj.level)
            {
                return (robj.level - lobj.level);
            }
            if (lobj.avatarStar != robj.avatarStar)
            {
                return (robj.avatarStar - lobj.avatarStar);
            }
            if (lobj.avatarCombat != robj.avatarCombat)
            {
                return (robj.avatarCombat - lobj.avatarCombat);
            }
            return (lobj.uid - robj.uid);
        }

        public static int CompareToRequestNew(FriendBriefDataItem lobj, FriendBriefDataItem robj)
        {
            if (!Singleton<FriendModule>.Instance.IsOldRequest(lobj.uid))
            {
                return -1;
            }
            if (!Singleton<FriendModule>.Instance.IsOldRequest(robj.uid))
            {
                return 1;
            }
            return CompareToLevelDesc(lobj, robj);
        }

        public string AvatarIconPath
        {
            get
            {
                return AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(this.showAvatarID, this.avatarStar).iconPath;
            }
        }

        public string nickName
        {
            get
            {
                return (!string.IsNullOrEmpty(this._nickName) ? this._nickName : LocalizationGeneralLogic.GetText("Menu_DefaultNickname", new object[] { this.uid }));
            }
        }

        [CompilerGenerated]
        private sealed class <FriendBriefDataItem>c__AnonStoreyC6
        {
            internal AvatarSubSkillDetailData subSkill;

            internal bool <>m__C2(AvatarSubSkillDataItem x)
            {
                return (x.subSkillID == this.subSkill.get_sub_skill_id());
            }
        }
    }
}

