namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoVentureDispatchAvatar : MonoBehaviour
    {
        private AvatarDataItem _avatarData;
        private int _index;
        private VentureDataItem _ventureData;
        private const string AVATAR_NULL_BG_PATH = "SpriteOutput/AvatarTachie/BgType4";

        public void OnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new DispatchAvatarDialogContext(this._ventureData, this._index), UIType.Any);
        }

        private void SetupAvatar()
        {
            base.transform.Find("Content/StarPanel/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(this._avatarData.star);
            base.transform.Find("Content/LVNum").GetComponent<Text>().text = this._avatarData.level.ToString();
            base.transform.Find("Content/Avatar").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._avatarData.AvatarTachie);
        }

        public void SetupView(int index, VentureDataItem ventureData)
        {
            this._index = index;
            this._ventureData = ventureData;
            if (this._ventureData.selectedAvatarList.Count >= index)
            {
                this._avatarData = Singleton<AvatarModule>.Instance.GetAvatarByID(this._ventureData.selectedAvatarList[index - 1]);
            }
            else
            {
                this._avatarData = null;
            }
            base.transform.Find("Content").gameObject.SetActive(this._avatarData != null);
            if (this._avatarData != null)
            {
                base.transform.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarAttributeBGSpriteList[this._avatarData.Attribute]);
                this.SetupAvatar();
            }
            else
            {
                base.transform.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/AvatarTachie/BgType4");
            }
        }
    }
}

