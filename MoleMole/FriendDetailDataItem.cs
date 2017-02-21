namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class FriendDetailDataItem
    {
        private string _desc;
        private string _nickName;
        public AvatarDataItem leaderAvatar;
        public int level;
        public int uid;

        public FriendDetailDataItem(PlayerDetailData playerDetailData)
        {
            this.uid = (int) playerDetailData.get_uid();
            this._nickName = playerDetailData.get_nickname();
            this.level = (int) playerDetailData.get_level();
            this.SetLeaderAvatar(playerDetailData.get_leader_avatar());
            this._desc = playerDetailData.get_self_desc();
        }

        public FriendDetailDataItem(int uid, string nickName, int level, AvatarDataItem leaderAvatar = null, string friendDesc = null)
        {
            this.uid = uid;
            this._nickName = nickName;
            this.leaderAvatar = leaderAvatar;
            this._desc = friendDesc;
        }

        public void SetLeaderAvatar(AvatarDetailData avatarDetailData)
        {
            this.leaderAvatar = new AvatarDataItem((int) avatarDetailData.get_avatar_id(), (int) avatarDetailData.get_avatar_level(), (int) avatarDetailData.get_avatar_star());
            WeaponDetailData data = avatarDetailData.get_weapon();
            if ((data != null) && (WeaponMetaDataReader.GetWeaponMetaDataByKey((int) data.get_id()) != null))
            {
                WeaponDataItem item = new WeaponDataItem(0, WeaponMetaDataReader.GetWeaponMetaDataByKey((int) data.get_id())) {
                    level = (int) data.get_level()
                };
                this.leaderAvatar.equipsMap[1] = item;
            }
            EquipmentSlot[] slotArray = new EquipmentSlot[] { 2, 3, 4 };
            List<StigmataDetailData> list = new List<StigmataDetailData> {
                avatarDetailData.get_stigmata_1(),
                avatarDetailData.get_stigmata_2(),
                avatarDetailData.get_stigmata_3()
            };
            for (int i = 0; i < list.Count; i++)
            {
                StigmataDetailData data2 = list[i];
                if ((data2 != null) && (StigmataMetaDataReader.GetStigmataMetaDataByKey((int) data2.get_id()) != null))
                {
                    StigmataDataItem item2 = new StigmataDataItem(0, StigmataMetaDataReader.GetStigmataMetaDataByKey((int) data2.get_id())) {
                        level = (int) data2.get_level()
                    };
                    int num2 = !data2.get_pre_affix_idSpecified() ? 0 : ((int) data2.get_pre_affix_id());
                    int num3 = !data2.get_suf_affix_idSpecified() ? 0 : ((int) data2.get_suf_affix_id());
                    item2.SetAffixSkill(true, num2, num3);
                    this.leaderAvatar.equipsMap[slotArray[i]] = item2;
                }
            }
            foreach (AvatarSkillDetailData data3 in avatarDetailData.get_skill_list())
            {
                if (this.leaderAvatar.GetAvatarSkillBySkillID((int) data3.get_skill_id()) != null)
                {
                    AvatarSkillDataItem avatarSkillBySkillID = this.leaderAvatar.GetAvatarSkillBySkillID((int) data3.get_skill_id());
                    foreach (AvatarSubSkillDetailData data4 in data3.get_sub_skill_list())
                    {
                        if (avatarSkillBySkillID.GetAvatarSubSkillBySubSkillId((int) data4.get_sub_skill_id()) != null)
                        {
                            avatarSkillBySkillID.GetAvatarSubSkillBySubSkillId((int) data4.get_sub_skill_id()).level = (int) data4.get_level();
                        }
                    }
                }
            }
        }

        public string Desc
        {
            get
            {
                return (!string.IsNullOrEmpty(this._desc) ? this._desc : LocalizationGeneralLogic.GetText("Menu_DefaultSelfDesc", new object[0]));
            }
        }

        public string nickName
        {
            get
            {
                return (!string.IsNullOrEmpty(this._nickName) ? this._nickName : LocalizationGeneralLogic.GetText("Menu_DefaultNickname", new object[] { this.uid }));
            }
        }
    }
}

