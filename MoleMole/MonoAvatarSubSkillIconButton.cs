namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoAvatarSubSkillIconButton : MonoBehaviour
    {
        private AvatarDataItem _avatarData;
        private bool _isRemoteAvatar;
        private AvatarSkillDataItem _skillData;
        private AvatarSubSkillDataItem _subSkillData;
        private const string MATERIAL_GRAY_PATH = "Material/ImageGrayscale";

        private void ClearSubSkillStatusInLocalData()
        {
            Singleton<MiHoYoGameData>.Instance.LocalData.SubSkillStatusDict.Remove(this._subSkillData.subSkillID);
            Singleton<MiHoYoGameData>.Instance.Save();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SubSkillStatusCacheUpdate, null));
        }

        public void OnClick()
        {
            if (!this._isRemoteAvatar)
            {
                if (this._subSkillData.level == this._subSkillData.MaxLv)
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Err_SubSkillMaxLv", new object[0]), 2f), UIType.Any);
                }
                else
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(new AvatarSubSkillDialogContext(this._avatarData, this._skillData, this._subSkillData), UIType.Any);
                    this.ClearSubSkillStatusInLocalData();
                    this.SetupSubSkillPopUp();
                }
            }
        }

        private void SetupIcon()
        {
            Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(this._subSkillData.IconPath);
            base.transform.Find("SkillIcon/Icon/Image").GetComponent<Image>().sprite = spriteByPrefab;
            string key = string.Empty;
            string str2 = string.Empty;
            if (this._subSkillData.UnLocked)
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
            str2 = "Blue";
            base.transform.Find("Frame").GetComponent<Image>().color = MiscData.GetColor(str2);
            base.transform.Find("SkillIcon/Icon").GetComponent<Image>().color = MiscData.GetColor(str2);
            base.transform.Find("SkillIcon/Icon/BG").GetComponent<Image>().color = MiscData.GetColor(key);
        }

        private void SetupSubSkillPopUp()
        {
            bool flag = Singleton<MiHoYoGameData>.Instance.LocalData.SubSkillStatusDict.ContainsKey(this._subSkillData.subSkillID);
            base.transform.Find("SkillIcon/PopUp").gameObject.SetActive(!this._isRemoteAvatar && flag);
        }

        public void SetupView(AvatarDataItem avatarDataItem, AvatarSkillDataItem skillData, AvatarSubSkillDataItem subSkillData, bool isRemoteAvatar)
        {
            this._avatarData = avatarDataItem;
            this._skillData = skillData;
            this._subSkillData = subSkillData;
            this._isRemoteAvatar = isRemoteAvatar;
            base.transform.Find("Row/NameText").GetComponent<Text>().text = this._subSkillData.Name;
            base.transform.Find("RowText/DescText").GetComponent<Text>().text = this._subSkillData.Info;
            this.SetupIcon();
            Transform transform = base.transform.Find("SkillIcon/UnLockInfo");
            bool flag = this._avatarData.level < this._subSkillData.UnlockLv;
            bool flag2 = this._avatarData.star < this._subSkillData.UnlockStar;
            transform.gameObject.SetActive(!this._subSkillData.UnLocked);
            if (!this._subSkillData.UnLocked)
            {
                Transform transform2 = transform.Find("UnLockStar");
                Transform transform3 = transform.Find("UnLockLv");
                base.transform.Find("SkillIcon/Icon").GetComponent<Image>().color = MiscData.GetColor("SkillBtnGrey");
                if (flag2)
                {
                    transform2.gameObject.SetActive(true);
                    transform3.gameObject.SetActive(false);
                    transform2.Find("Star").GetComponent<MonoAvatarStar>().SetupView(this._subSkillData.UnlockStar);
                }
                else if (flag)
                {
                    transform2.gameObject.SetActive(false);
                    transform3.gameObject.SetActive(true);
                    transform3.Find("Lv").GetComponent<Text>().text = "Lv." + this._subSkillData.UnlockLv;
                }
                else
                {
                    transform2.gameObject.SetActive(false);
                    transform3.gameObject.SetActive(false);
                    base.transform.Find("SkillIcon/Icon").GetComponent<Image>().color = MiscData.GetColor("Blue");
                }
            }
            Transform transform4 = base.transform.Find("RowText/NextLv");
            Transform transform5 = base.transform.Find("RowText/NextStar");
            transform4.gameObject.SetActive(false);
            transform5.gameObject.SetActive(false);
            if (this._subSkillData.UnLocked && (this._subSkillData.level < this._subSkillData.MaxLv))
            {
                bool flag3 = this._avatarData.level < this._subSkillData.LvUpNeedAvatarLevel;
                if (this._avatarData.star < this._subSkillData.GetUpLevelStarNeed())
                {
                    transform4.gameObject.SetActive(false);
                    transform5.gameObject.SetActive(true);
                    transform5.Find("Star").GetComponent<MonoAvatarStar>().SetupView(this._subSkillData.GetUpLevelStarNeed());
                }
                else if (flag3)
                {
                    transform4.gameObject.SetActive(true);
                    transform5.gameObject.SetActive(false);
                    transform4.Find("Lv").GetComponent<Text>().text = "Lv." + this._subSkillData.LvUpNeedAvatarLevel;
                }
            }
            Text component = base.transform.Find("Row/AddPtText").GetComponent<Text>();
            component.gameObject.SetActive(this._subSkillData.UnLocked);
            if (this._subSkillData.level == this._subSkillData.MaxLv)
            {
                component.text = "MAX";
            }
            else
            {
                component.text = (this._subSkillData.level <= 0) ? string.Empty : string.Format("+{0}", this._subSkillData.level);
            }
            this.SetupSubSkillPopUp();
            if (this._subSkillData.level == this._subSkillData.MaxLv)
            {
                base.transform.Find("SkillIcon/PopUp").gameObject.SetActive(false);
            }
            base.transform.Find("SkillIcon/Max").gameObject.SetActive(this._subSkillData.level == this._subSkillData.MaxLv);
            base.transform.Find("SkillIcon/Upgradable").gameObject.SetActive(((!this._isRemoteAvatar && this._subSkillData.UnLocked) && ((this._subSkillData.level < this._subSkillData.MaxLv) && (this._avatarData.level >= this._subSkillData.LvUpNeedAvatarLevel))) && (this._avatarData.star >= this._subSkillData.GetUpLevelStarNeed()));
        }
    }
}

