namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoAvatarEnhance : MonoBehaviour
    {
        private float _atkAdd;
        private int _avatarClassID;
        private float _crtAdd;
        private float _defAdd;
        private float _hpAdd;
        private float _spAdd;

        private void GetAddInfo()
        {
            CabinAvatarEnhanceDataItem avatarEnhanceCabinByClass = Singleton<IslandModule>.Instance.GetAvatarEnhanceCabinByClass(this._avatarClassID);
            this._hpAdd = avatarEnhanceCabinByClass.GetAvatarAttrEnhance(1) * 100f;
            this._spAdd = avatarEnhanceCabinByClass.GetAvatarAttrEnhance(2) * 100f;
            this._atkAdd = avatarEnhanceCabinByClass.GetAvatarAttrEnhance(3) * 100f;
            this._crtAdd = avatarEnhanceCabinByClass.GetAvatarAttrEnhance(5) * 100f;
            this._defAdd = avatarEnhanceCabinByClass.GetAvatarAttrEnhance(4) * 100f;
        }

        private string GetAvatarClassName()
        {
            switch (this._avatarClassID)
            {
                case 1:
                    return "KIANA";

                case 2:
                    return "MEI";

                case 3:
                    return "BRONYA";

                case 4:
                    return "HIMEKO";

                case 5:
                    return "FUKA";
            }
            return string.Empty;
        }

        private void SetAttrAddInfo()
        {
            Transform transform = base.transform.Find("AttrEnhanceList");
            transform.Find("HpEnhance").gameObject.SetActive(this._hpAdd > 0f);
            if (this._hpAdd > 0f)
            {
                transform.Find("HpEnhance/Num").GetComponent<Text>().text = this._hpAdd.ToString("0.00") + "%";
            }
            transform.Find("SpEnhance").gameObject.SetActive(this._spAdd > 0f);
            if (this._spAdd > 0f)
            {
                transform.Find("SpEnhance/Num").GetComponent<Text>().text = this._spAdd.ToString("0.00") + "%";
            }
            transform.Find("AtkEnhance").gameObject.SetActive(this._atkAdd > 0f);
            if (this._atkAdd > 0f)
            {
                transform.Find("AtkEnhance/Num").GetComponent<Text>().text = this._atkAdd.ToString("0.00") + "%";
            }
            transform.Find("DefEnhance").gameObject.SetActive(this._defAdd > 0f);
            if (this._defAdd > 0f)
            {
                transform.Find("DefEnhance/Num").GetComponent<Text>().text = this._defAdd.ToString("0.00") + "%";
            }
            transform.Find("CrtEnhance").gameObject.SetActive(this._crtAdd > 0f);
            if (this._crtAdd > 0f)
            {
                transform.Find("CrtEnhance/Num").GetComponent<Text>().text = this._crtAdd.ToString("0.00") + "%";
            }
            bool flag = ((((this._hpAdd + this._spAdd) + this._atkAdd) + this._defAdd) + this._crtAdd) > 0f;
            transform.Find("NoEnhance").gameObject.SetActive(!flag);
        }

        public void SetupView(int avatarClassID)
        {
            this._avatarClassID = avatarClassID;
            object[] replaceParams = new object[] { this.GetAvatarClassName() };
            base.transform.Find("AvatarInfo/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EnhanceSpeicalAvatarAttr", replaceParams);
            base.transform.Find("AvatarInfo/AvatarImage").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.IslandAvatarEnhanceClassImage[avatarClassID]);
            this.GetAddInfo();
            this.SetAttrAddInfo();
            this.ShowAttrAddInfo(base.transform.Find("AvatarInfo").GetComponent<Toggle>().isOn);
        }

        public void ShowAttrAddInfo(bool isOn)
        {
            base.transform.Find("AttrEnhanceList").gameObject.SetActive(isOn);
            base.transform.Find("AvatarInfo/Icon/Spread").gameObject.SetActive(isOn);
            base.transform.Find("AvatarInfo/Icon/Unspread").gameObject.SetActive(!isOn);
        }
    }
}

