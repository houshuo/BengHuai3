namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoAvatarSkillIconButton : MonoBehaviour
    {
        private AvatarDataItem _avatarData;
        private bool _isRemoteAvatar;
        private AvatarSkillDataItem _skillData;
        private const string MATERIAL_GRAY_PATH = "Material/ImageGrayscale";

        public void OnClick()
        {
            if (this._skillData.UnLocked)
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SelectAvtarSkillIconChange, this._skillData.skillID));
            }
            else if (!this._isRemoteAvatar)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new AvatarSkillDialogContext(this._avatarData, this._skillData), UIType.Any);
            }
        }

        private void SetupIcon()
        {
            Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(this._skillData.IconPath);
            base.transform.Find("SkillIcon/Icon/Image").GetComponent<Image>().sprite = spriteByPrefab;
            string key = string.Empty;
            if (this._skillData.UnLocked)
            {
                key = "SkillBtnBlue";
                base.transform.Find("SkillIcon/Icon/Image").GetComponent<Image>().material = null;
            }
            else
            {
                key = "SkillBtnGrey";
                Material material = Miscs.LoadResource<Material>("Material/ImageGrayscale", BundleType.RESOURCE_FILE);
                base.transform.Find("SkillIcon/Icon/Image").GetComponent<Image>().material = material;
            }
            base.transform.Find("SkillIcon/Icon/BG").GetComponent<Image>().color = MiscData.GetColor(key);
        }

        private void SetupSkillPopUp()
        {
            if (!this._isRemoteAvatar && this._skillData.UnLocked)
            {
                bool flag = false;
                bool flag2 = false;
                Dictionary<int, SubSkillStatus> subSkillStatusDict = Singleton<MiHoYoGameData>.Instance.LocalData.SubSkillStatusDict;
                foreach (AvatarSubSkillDataItem item in this._skillData.avatarSubSkillList)
                {
                    if (subSkillStatusDict.ContainsKey(item.subSkillID))
                    {
                        flag = true;
                    }
                    if ((item.UnLocked && (item.level < item.MaxLv)) && ((this._avatarData.level >= item.LvUpNeedAvatarLevel) && (this._avatarData.star >= item.GetUpLevelStarNeed())))
                    {
                        flag2 = true;
                    }
                }
                base.transform.Find("SkillIcon/PopUp").gameObject.SetActive(flag);
                base.transform.Find("Upgradable").gameObject.SetActive(flag2);
            }
            else
            {
                base.transform.Find("SkillIcon/PopUp").gameObject.SetActive(false);
                base.transform.Find("Upgradable").gameObject.SetActive(false);
            }
        }

        public void SetupView(AvatarDataItem avatarDataItem, AvatarSkillDataItem skillData, bool isRemoteAvatar)
        {
            this._avatarData = avatarDataItem;
            this._skillData = skillData;
            this._isRemoteAvatar = isRemoteAvatar;
            Text component = base.transform.Find("Info/NameText").GetComponent<Text>();
            component.text = this._skillData.SkillName;
            component.color = !this._skillData.UnLocked ? MiscData.GetColor("TextGrey") : Color.white;
            Transform transform = base.transform.Find("SkillIcon/UnLockInfo");
            transform.gameObject.SetActive(!this._skillData.UnLocked);
            Transform transform2 = transform.Find("UnLockStar");
            Transform transform3 = transform.Find("UnLockLv");
            transform2.gameObject.SetActive(false);
            transform3.gameObject.SetActive(false);
            if (transform.gameObject.activeSelf)
            {
                base.transform.Find("SkillIcon/Icon").GetComponent<Image>().color = MiscData.GetColor("SkillBtnGrey");
                bool flag = this._avatarData.level < this._skillData.UnLockLv;
                if (this._avatarData.star < this._skillData.UnLockStar)
                {
                    transform2.gameObject.SetActive(true);
                    transform3.gameObject.SetActive(false);
                    transform2.Find("Star").GetComponent<MonoAvatarStar>().SetupView(this._skillData.UnLockStar);
                }
                else if (flag)
                {
                    transform2.gameObject.SetActive(false);
                    transform3.gameObject.SetActive(true);
                    transform3.Find("Lv").GetComponent<Text>().text = "Lv." + this._skillData.UnLockLv;
                }
            }
            int levelSum = this._skillData.GetLevelSum();
            int maxLevelSum = this._skillData.GetMaxLevelSum();
            Text text2 = base.transform.Find("Info/Row/AddPtText").GetComponent<Text>();
            text2.gameObject.SetActive(this._skillData.UnLocked);
            if (levelSum == maxLevelSum)
            {
                text2.text = "MAX";
            }
            else
            {
                text2.text = (levelSum <= 0) ? string.Empty : ("+" + levelSum);
            }
            this.SetupIcon();
            this.SetupSkillPopUp();
        }
    }
}

