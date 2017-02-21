namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoAvatarDetailProfile : MonoBehaviour
    {
        private AvatarDataItem _avatarData;

        public void SetupView(AvatarDataItem avatarData)
        {
            this._avatarData = avatarData;
            base.transform.Find("Info/AttrImage/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._avatarData.AttributeIconPath);
            base.transform.Find("Info/NameText").GetComponent<Text>().text = this._avatarData.FullName;
            base.transform.Find("Info/LvText").GetComponent<Text>().text = "LV." + this._avatarData.level;
            base.transform.Find("Info/CombatNumText").GetComponent<Text>().text = Mathf.FloorToInt(this._avatarData.CombatNum).ToString();
            base.transform.Find("Info/Cost/CurrentCost").GetComponent<Text>().text = this._avatarData.GetCurrentCost().ToString();
            base.transform.Find("Info/Cost/MaxCost").GetComponent<Text>().text = this._avatarData.MaxCost.ToString();
        }

        public void UpdateInfo(int newCost, int maxCost, AvatarDataItem avatarData, StorageDataItemBase selectItem)
        {
            Text component = base.transform.Find("Info/Cost/CurrentCost").GetComponent<Text>();
            component.text = newCost.ToString();
            component.color = (newCost <= maxCost) ? Color.white : Color.red;
            base.transform.Find("Info/CombatNumText").GetComponent<Text>().text = Mathf.FloorToInt(avatarData.GetAvatarCombatUsingNewEquip(selectItem)).ToString();
        }
    }
}

