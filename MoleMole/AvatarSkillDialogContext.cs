namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class AvatarSkillDialogContext : BaseDialogContext
    {
        public readonly AvatarDataItem avatarData;
        public readonly AvatarSkillDataItem skillData;

        public AvatarSkillDialogContext(AvatarDataItem avatarData, AvatarSkillDataItem skillData)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AvatarSkillDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/AvatarSkillDialogV2",
                ignoreNotify = true
            };
            base.config = pattern;
            this.avatarData = avatarData;
            this.skillData = skillData;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/SingleButton/Btn").GetComponent<Button>(), new UnityAction(this.OnBtnClick));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        public void OnBtnClick()
        {
            if (this.skillData.CanTry)
            {
                Singleton<LevelScoreManager>.Create();
                Singleton<LevelScoreManager>.Instance.SetTryLevelBeginIntent(this.avatarData.avatarID, "Lua/Levels/Common/LevelInfinityTest.lua", this.skillData.skillID, 0);
                Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", true, true, true, null, true);
            }
            else
            {
                this.Close();
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return false;
        }

        private void SetupLackInfo()
        {
            bool flag = this.avatarData.level < this.skillData.UnLockLv;
            bool flag2 = this.avatarData.star < this.skillData.UnLockStar;
            Transform transform = base.view.transform.Find("Dialog/Content/VerticalLayout/StarLack");
            Transform transform2 = base.view.transform.Find("Dialog/Content/VerticalLayout/LevelLack");
            if (flag2)
            {
                transform.gameObject.SetActive(true);
                transform2.gameObject.SetActive(false);
                transform.Find("UnLockStar").GetComponent<MonoAvatarStar>().SetupView(this.skillData.UnLockStar);
            }
            else if (flag)
            {
                transform.gameObject.SetActive(false);
                transform2.gameObject.SetActive(true);
                transform2.Find("LvNeed").GetComponent<Text>().text = this.skillData.UnLockLv.ToString();
            }
            else
            {
                transform.gameObject.SetActive(false);
                transform2.gameObject.SetActive(false);
            }
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/NameRow/NameText").GetComponent<Text>().text = this.skillData.SkillName;
            base.view.transform.Find("Dialog/Content/VerticalLayout/DescText").GetComponent<Text>().text = this.skillData.SkillInfo;
            Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(this.skillData.IconPath);
            base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/Icon/Image").GetComponent<Image>().sprite = spriteByPrefab;
            base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/RemainSkillPoint/Num").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.skillPoint.ToString();
            this.SetupLackInfo();
            if (string.IsNullOrEmpty(this.skillData.SkillStep))
            {
                base.view.transform.Find("Dialog/Content/VerticalLayout/Step").gameObject.SetActive(false);
            }
            else
            {
                base.view.transform.Find("Dialog/Content/VerticalLayout/Step").gameObject.SetActive(true);
                base.view.transform.Find("Dialog/Content/VerticalLayout/Step/Table").GetComponent<MonoAvatarSkillStep>().SetupView(this.avatarData, this.skillData.SkillStep);
            }
            base.view.transform.Find("Dialog/Content/SingleButton").gameObject.SetActive(true);
            string textID = !this.skillData.CanTry ? "Menu_OK" : "Menu_TrySkill";
            base.view.transform.Find("Dialog/Content/SingleButton/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID, new object[0]);
            return false;
        }
    }
}

