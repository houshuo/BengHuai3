namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoAvatarDetailSkillTab : MonoBehaviour
    {
        private AvatarDataItem _avatarData;
        private bool _isRemoteAvatar;
        private FriendDetailDataItem _userData;

        public void OnBackPage()
        {
            if (base.transform.Find("ListPanel/Info/Content/SubSkill").gameObject.activeSelf)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SelectAvtarSkillIconChange, 0));
            }
            else
            {
                Singleton<MainUIManager>.Instance.BackPage();
            }
        }

        public void OnSkillPointButtonClick()
        {
            if (!this._isRemoteAvatar)
            {
                if (Singleton<PlayerModule>.Instance.playerData.skillPointExchangeCache.Value != null)
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new SkillPointExchangeDialogContext(), UIType.Any);
                }
                else
                {
                    Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.ShouldShowSPExchange, null));
                    Singleton<NetworkManager>.Instance.RequestGetSkillPointExchangeInfo();
                }
            }
        }

        private void SetupSelectSkillDetailView(Transform trans, AvatarSkillDataItem selectedSkillData)
        {
            trans.Find("Content/NameText").GetComponent<Text>().text = selectedSkillData.SkillName;
            trans.Find("Content/DescText").GetComponent<Text>().text = selectedSkillData.SkillInfo;
            if (string.IsNullOrEmpty(selectedSkillData.SkillStep))
            {
                trans.Find("Content/Step").gameObject.SetActive(false);
            }
            else
            {
                trans.Find("Content/Step").gameObject.SetActive(true);
                trans.Find("Content/Step/Table").GetComponent<MonoAvatarSkillStep>().SetupView(this._avatarData, selectedSkillData.SkillStep);
            }
        }

        private void SetupSelectSkillSubSkillListView(Transform trans, AvatarSkillDataItem selectedSkillData)
        {
            trans.Find("ParentSkill/Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(selectedSkillData.IconPath);
            trans.Find("ParentSkill/Icon").GetComponent<Image>().color = MiscData.GetColor("Blue");
            HashSet<int> set = new HashSet<int> { 1, 2, 3 };
            foreach (AvatarSubSkillDataItem item in selectedSkillData.avatarSubSkillList)
            {
                string name = "SubSkill_" + item.ShowOrder;
                Transform transform = trans.Find(name);
                if (transform != null)
                {
                    transform.gameObject.SetActive(true);
                    transform.GetComponent<MonoAvatarSubSkillIconButton>().SetupView(this._avatarData, selectedSkillData, item, this._isRemoteAvatar);
                    set.Remove(item.ShowOrder);
                }
            }
            foreach (int num in set)
            {
                string str2 = "SubSkill_" + num;
                trans.Find(str2).gameObject.SetActive(false);
            }
        }

        private void SetupSelectSkillView(AvatarSkillDataItem selectedSkillData)
        {
            base.transform.Find("ListPanel/Info/Content/Skill").gameObject.SetActive(false);
            Transform trans = base.transform.Find("SelectedSkill");
            trans.gameObject.SetActive(true);
            this.SetupSelectSkillDetailView(trans, selectedSkillData);
            Transform transform2 = base.transform.Find("ListPanel/Info/Content/SubSkill");
            transform2.gameObject.SetActive(true);
            this.SetupSelectSkillSubSkillListView(transform2, selectedSkillData);
            base.transform.Find("ListPanel").GetComponent<Animation>().Play();
        }

        private void SetupSkillListView()
        {
            base.transform.Find("SelectedSkill").gameObject.SetActive(false);
            base.transform.Find("ListPanel/Info/Content/SubSkill").gameObject.SetActive(false);
            Transform transform = base.transform.Find("ListPanel/Info/Content/Skill");
            transform.gameObject.SetActive(true);
            transform.Find("Center").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarClassSkillIconPath[this._avatarData.ClassId]);
            foreach (AvatarSkillDataItem item in this._avatarData.skillDataList)
            {
                string name = "Skill_" + item.ShowOrder;
                Transform transform2 = transform.Find(name);
                if (transform2 != null)
                {
                    transform2.GetComponent<MonoAvatarSkillIconButton>().SetupView(this._avatarData, item, this._isRemoteAvatar);
                }
            }
            base.transform.Find("ListPanel").GetComponent<Animation>().Play();
        }

        public void SetupSkillPoint()
        {
            base.transform.Find("ListPanel/Info/Content/SkillPoint/Num").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.skillPoint.ToString();
            base.transform.Find("ListPanel/Info/Content/SkillPoint/MaxNum").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.skillPointLimit.ToString();
        }

        public void SetupView(AvatarDataItem avatarData, AvatarSkillDataItem selectedSkillData = null)
        {
            this._isRemoteAvatar = false;
            this._avatarData = avatarData;
            base.transform.Find("ListPanel/Info/Content/SkillPoint").gameObject.SetActive(true);
            this.SetupSkillPoint();
            if (selectedSkillData != null)
            {
                this.SetupSelectSkillView(selectedSkillData);
            }
            else
            {
                this.SetupSkillListView();
            }
        }

        public void SetupView(FriendDetailDataItem userData, AvatarSkillDataItem selectedSkillData = null)
        {
            this._isRemoteAvatar = true;
            this._userData = userData;
            this._avatarData = this._userData.leaderAvatar;
            base.transform.Find("ListPanel/Info/Content/SkillPoint").gameObject.SetActive(false);
            if (selectedSkillData != null)
            {
                this.SetupSelectSkillView(selectedSkillData);
            }
            else
            {
                this.SetupSkillListView();
            }
        }
    }
}

