namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class ResourceDetailDialogContext : BaseDialogContext
    {
        public readonly RewardUIData resourceData;

        public ResourceDetailDialogContext(RewardUIData resourceData)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "ResourceDetailDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/ItemDetailDialog",
                ignoreNotify = true
            };
            base.config = pattern;
            this.resourceData = resourceData;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
        }

        private string GetAllDesc()
        {
            return UIUtil.ProcessStrWithNewLine(LocalizationGeneralLogic.GetText(this.resourceData.descTextID, new object[0]));
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        private void SetupRarityView()
        {
            base.view.transform.Find("Dialog/Content/Icon").GetComponent<Image>().color = (this.resourceData.rewardType != ResourceType.Hcoin) ? MiscData.GetColor("ResourceBlue") : MiscData.GetColor("ResourcePurple");
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Dialog/Content/RarityUpBtn").gameObject.SetActive(false);
            this.SetupRarityView();
            base.view.transform.Find("Dialog/Content/Icon/Image").GetComponent<Image>().sprite = this.resourceData.GetImageSprite();
            base.view.transform.Find("Dialog/Content/Star/EquipStar").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/NameText").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(this.resourceData.nameTextID, new object[0]);
            base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = this.GetAllDesc();
            base.view.transform.Find("Dialog/Content/Num").gameObject.SetActive(true);
            base.view.transform.Find("Dialog/Content/Num/Text").GetComponent<Text>().text = this.resourceData.value.ToString();
            base.view.transform.Find("Dialog/Content/RarityUpBtn").gameObject.SetActive(false);
            return false;
        }
    }
}

