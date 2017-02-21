namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoAvatarIcon : MonoBehaviour
    {
        private AvatarDataItem _avatarData;
        private EndlessAvatarHp _avatarHp;
        private const string _border_path_jixie = "SpriteOutput/AvatarIcon/AvatarAttrJiXie";
        private const string _border_path_shengwu = "SpriteOutput/AvatarIcon/AvatarAttrShengWu";
        private const string _border_path_yineng = "SpriteOutput/AvatarIcon/AvatarAttrYiNeng";
        private bool _isSelected;
        public static string bg_path_jixie = "SpriteOutput/AvatarIcon/AttrJiXie";
        public static string bg_path_shengwu = "SpriteOutput/AvatarIcon/AttrShengWu";
        public static string bg_path_yineng = "SpriteOutput/AvatarIcon/AttrYiNeng";

        private void EndlessAvatarDieCallBack(bool avatarDie)
        {
            base.transform.Find("Panel").gameObject.SetActive(avatarDie);
            base.transform.Find("Icon").GetComponent<Image>().color = !avatarDie ? MiscData.GetColor("TotalWhite") : MiscData.GetColor("EndlessEnergyRunout");
            base.transform.Find("FlashHint").gameObject.SetActive(avatarDie);
            base.transform.Find("LvText").gameObject.SetActive(!avatarDie);
        }

        private Sprite GetBGSprite()
        {
            switch (((EntityNature) this._avatarData.Attribute))
            {
                case EntityNature.Mechanic:
                    return Miscs.GetSpriteByPrefab(bg_path_jixie);

                case EntityNature.Biology:
                    return Miscs.GetSpriteByPrefab(bg_path_shengwu);

                case EntityNature.Psycho:
                    return Miscs.GetSpriteByPrefab(bg_path_yineng);
            }
            return null;
        }

        private Sprite GetBorderSprite()
        {
            switch (((EntityNature) this._avatarData.Attribute))
            {
                case EntityNature.Mechanic:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AvatarAttrJiXie");

                case EntityNature.Biology:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AvatarAttrShengWu");

                case EntityNature.Psycho:
                    return Miscs.GetSpriteByPrefab("SpriteOutput/AvatarIcon/AvatarAttrYiNeng");
            }
            return null;
        }

        public void OnClick()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SelectAvtarIconChange, this._avatarData.avatarID));
        }

        private void SetAvatarStar()
        {
            for (int i = 1; i < 6; i++)
            {
                string name = string.Format("{0}/{1}", "AvatarStar", i);
                base.transform.Find(name).gameObject.SetActive(i == this._avatarData.star);
            }
        }

        private void SetUpAvatarDispatched(bool isDispatched)
        {
            base.transform.Find("Icon").GetComponent<Image>().color = !isDispatched ? MiscData.GetColor("TotalWhite") : MiscData.GetColor("EndlessEnergyRunout");
        }

        public void SetupSelectedView(bool isSelected)
        {
            this._isSelected = isSelected;
            base.transform.Find("Button").GetComponent<Button>().interactable = !this._isSelected;
        }

        public void SetupView(AvatarDataItem avatarDataItem, bool isSelected, EndlessAvatarHp avatarHP = null)
        {
            this._avatarData = avatarDataItem;
            this._avatarHp = avatarHP;
            base.transform.Find("Panel").gameObject.SetActive(false);
            base.transform.Find("BG").GetComponent<Image>().sprite = this.GetBGSprite();
            base.transform.Find("Frame").GetComponent<Image>().sprite = this.GetBorderSprite();
            base.transform.Find("Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._avatarData.IconPath);
            base.transform.Find("PopUp").gameObject.SetActive(this._avatarData.CanStarUp);
            base.transform.Find("LockImg").gameObject.SetActive(!this._avatarData.UnLocked);
            List<int> memberList = Singleton<PlayerModule>.Instance.playerData.GetMemberList(1);
            bool flag = (memberList.Count > 0) && memberList.Contains(this._avatarData.avatarID);
            bool flag2 = (memberList.Count > 0) && (this._avatarData.avatarID == memberList[0]);
            base.transform.Find("FlagImg").gameObject.SetActive(flag);
            base.transform.Find("FlagImg").GetComponent<Image>().color = !flag2 ? MiscData.GetColor("TotalWhite") : MiscData.GetColor("Yellow");
            base.transform.Find("AvatarStar").gameObject.SetActive(this._avatarData.UnLocked);
            if (this._avatarData.UnLocked)
            {
                this.SetAvatarStar();
            }
            base.transform.Find("LvText").GetComponent<Text>().text = !this._avatarData.UnLocked ? LocalizationGeneralLogic.GetText("Menu_AvatarLocked", new object[0]) : ("Lv." + this._avatarData.level);
            base.transform.Find("HPRemain").gameObject.SetActive(avatarHP != null);
            base.transform.Find("Icon").GetComponent<Image>().color = MiscData.GetColor("TotalWhite");
            base.transform.Find("FlashHint").gameObject.SetActive(false);
            base.transform.Find("LvText").gameObject.SetActive(true);
            this.SetUpAvatarDispatched(Singleton<IslandModule>.Instance.IsAvatarDispatched(this._avatarData.avatarID));
            if (avatarHP != null)
            {
                base.transform.Find("HPRemain").GetComponent<MonoRemainHP>().SetAvatarHPData(this._avatarHp, new Action<bool>(this.EndlessAvatarDieCallBack));
            }
            this.SetupSelectedView(isSelected);
        }
    }
}

