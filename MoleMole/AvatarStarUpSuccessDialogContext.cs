namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class AvatarStarUpSuccessDialogContext : BaseDialogContext
    {
        private AvatarDataItem avatarData;

        public AvatarStarUpSuccessDialogContext(AvatarDataItem avatarData)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ChangeNicknameDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/AvatarStarUpSuccessDialog",
                ignoreNotify = true
            };
            base.config = pattern;
            this.avatarData = avatarData;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/SingleButton/Btn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        protected override bool SetupView()
        {
            AvatarStarMetaData avatarStarMetaDataByKey = AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(this.avatarData.avatarID, this.avatarData.star - 1);
            AvatarStarMetaData data2 = AvatarStarMetaDataReader.GetAvatarStarMetaDataByKey(this.avatarData.avatarID, this.avatarData.star);
            base.view.transform.Find("Dialog/Content/HP/RatioBeforeNumText").GetComponent<Text>().text = avatarStarMetaDataByKey.hpAdd.ToString();
            base.view.transform.Find("Dialog/Content/HP/RatioAfterNumText").GetComponent<Text>().text = data2.hpAdd.ToString();
            base.view.transform.Find("Dialog/Content/HP/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", (data2.hpBase - avatarStarMetaDataByKey.hpBase) + ((data2.hpAdd - avatarStarMetaDataByKey.hpAdd) * this.avatarData.level));
            base.view.transform.Find("Dialog/Content/SP/RatioBeforeNumText").GetComponent<Text>().text = avatarStarMetaDataByKey.spAdd.ToString();
            base.view.transform.Find("Dialog/Content/SP/RatioAfterNumText").GetComponent<Text>().text = data2.spAdd.ToString();
            base.view.transform.Find("Dialog/Content/SP/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", (data2.spBase - avatarStarMetaDataByKey.spBase) + ((data2.spAdd - avatarStarMetaDataByKey.spAdd) * this.avatarData.level));
            base.view.transform.Find("Dialog/Content/ATK/RatioBeforeNumText").GetComponent<Text>().text = avatarStarMetaDataByKey.atkAdd.ToString();
            base.view.transform.Find("Dialog/Content/ATK/RatioAfterNumText").GetComponent<Text>().text = data2.atkAdd.ToString();
            base.view.transform.Find("Dialog/Content/ATK/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", (data2.atkBase - avatarStarMetaDataByKey.atkBase) + ((data2.atkAdd - avatarStarMetaDataByKey.atkAdd) * this.avatarData.level));
            base.view.transform.Find("Dialog/Content/DEF/RatioBeforeNumText").GetComponent<Text>().text = avatarStarMetaDataByKey.dfsAdd.ToString();
            base.view.transform.Find("Dialog/Content/DEF/RatioAfterNumText").GetComponent<Text>().text = data2.dfsAdd.ToString();
            base.view.transform.Find("Dialog/Content/DEF/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", (data2.dfsBase - avatarStarMetaDataByKey.dfsBase) + ((data2.dfsAdd - avatarStarMetaDataByKey.dfsAdd) * this.avatarData.level));
            base.view.transform.Find("Dialog/Content/CRT/RatioBeforeNumText").GetComponent<Text>().text = avatarStarMetaDataByKey.crtAdd.ToString();
            base.view.transform.Find("Dialog/Content/CRT/RatioAfterNumText").GetComponent<Text>().text = data2.crtAdd.ToString();
            base.view.transform.Find("Dialog/Content/CRT/AddNumText").GetComponent<Text>().text = string.Format("+{0:N2}", (data2.crtBase - avatarStarMetaDataByKey.crtBase) + ((data2.crtAdd - avatarStarMetaDataByKey.crtAdd) * this.avatarData.level));
            return false;
        }
    }
}

