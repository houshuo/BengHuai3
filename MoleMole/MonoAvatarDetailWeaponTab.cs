namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class MonoAvatarDetailWeaponTab : MonoBehaviour
    {
        private AvatarDataItem _avatarData;
        private bool _isRemoteAvatar;
        private FriendDetailDataItem _userData;

        public void OnChangeBtnClick()
        {
            if (!this._isRemoteAvatar)
            {
                AvatarChangeEquipPageContext context = new AvatarChangeEquipPageContext(this._avatarData, this._avatarData.GetWeapon(), 1);
                Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
            }
        }

        public void OnContentClick(BaseEventData data = null)
        {
            if (this._isRemoteAvatar)
            {
                WeaponDataItem weapon = this._userData.leaderAvatar.GetWeapon();
                if (weapon != null)
                {
                    Singleton<MainUIManager>.Instance.ShowPage(new StorageItemDetailPageContext(weapon, true, true), UIType.Page);
                }
            }
            else
            {
                WeaponDataItem storageItem = this._avatarData.GetWeapon();
                if (storageItem == null)
                {
                    this.OnChangeBtnClick();
                }
                else
                {
                    Singleton<MainUIManager>.Instance.ShowPage(new StorageItemDetailPageContext(storageItem, false, true), UIType.Page);
                }
            }
        }

        public void SetupView(AvatarDataItem avatarData)
        {
            this._isRemoteAvatar = false;
            this._avatarData = avatarData;
            base.transform.Find("Info/ChangeBtn").gameObject.SetActive(true);
            this.SetupWeapon(this._avatarData.GetWeapon());
        }

        public void SetupView(FriendDetailDataItem userData)
        {
            this._isRemoteAvatar = true;
            this._userData = userData;
            this._avatarData = this._userData.leaderAvatar;
            base.transform.Find("Info/ChangeBtn").gameObject.SetActive(false);
            this.SetupWeapon(this._userData.leaderAvatar.GetWeapon());
        }

        private void SetupWeapon(WeaponDataItem weaponData)
        {
            base.transform.Find("Info/Title/Equipment").gameObject.SetActive(weaponData != null);
            base.transform.Find("Info/Content/Equipment").gameObject.SetActive(weaponData != null);
            if (weaponData != null)
            {
                base.transform.Find("Info/Title/Equipment/Name").GetComponent<Text>().text = weaponData.GetDisplayTitle();
                string prefabPath = MiscData.Config.PrefabPath.WeaponBaseTypeIcon[weaponData.GetBaseType()];
                base.transform.Find("Info/Title/Equipment/TypeIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(prefabPath);
                base.transform.Find("Info/Content/Equipment/3dModel").GetComponent<MonoWeaponRenderImage>().SetupView(weaponData, true, 0);
                base.transform.Find("Info/Content/Equipment/Cost/Num").GetComponent<Text>().text = weaponData.GetCost().ToString();
                base.transform.Find("Info/Content/Equipment/Lv").GetComponent<Text>().text = "LV." + weaponData.level;
                base.transform.Find("Info/Content/Equipment/Star/EquipStar").GetComponent<MonoEquipSubStar>().SetupView(weaponData.rarity, weaponData.GetMaxRarity());
                base.transform.Find("Info/Content/Equipment/Star/EquipSubStar").GetComponent<MonoEquipSubStar>().SetupView(weaponData.GetSubRarity(), weaponData.GetMaxSubRarity() - 1);
            }
        }
    }
}

