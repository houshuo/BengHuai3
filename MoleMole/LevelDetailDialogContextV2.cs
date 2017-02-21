namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class LevelDetailDialogContextV2 : BaseDialogContext
    {
        private LevelDiffculty difficulty;
        public readonly LevelDataItem levelData;

        public LevelDetailDialogContextV2(LevelDataItem levelData, LevelDiffculty difficulty)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "LevelDetailDialogContext1",
                viewPrefabPath = "UI/Menus/Dialog/LevelDetailDialogV2",
                ignoreNotify = false
            };
            base.config = pattern;
            this.levelData = levelData;
            this.difficulty = difficulty;
            base.uiType = UIType.SpecialDialog;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Missions/Prepare/Btn").GetComponent<Button>(), new UnityAction(this.OnOkButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("Missions/Challenge/ResetBtn").GetComponent<Button>(), new UnityAction(this.OnResetButtonCallBack));
        }

        private void EnterPreparePage()
        {
            LevelPreparePageContext context = new LevelPreparePageContext(this.levelData);
            Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
            Singleton<AssetBundleManager>.Instance.CheckSVNVersion();
        }

        private void OnDropItemBtnClick(StorageDataItemBase itemData)
        {
            UIUtil.ShowItemDetail(itemData, true, true);
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.StageDropListUpdated)
            {
                this.RefreshDropList();
                this.RefreshChallengeNumber(null);
            }
            return false;
        }

        public void OnOkButtonCallBack()
        {
            if ((this.levelData.LevelType == 1) && (this.levelData.UnlockChanllengeNum > Singleton<LevelModule>.Instance.GetChapterById(this.levelData.ChapterID).GetTotalFinishedChanllengeNum(this.levelData.Diffculty)))
            {
                object[] replaceParams = new object[] { this.levelData.UnlockChanllengeNum };
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ChallengeLackLock", replaceParams), 2f), UIType.Any);
            }
            else if (this.levelData.isDropActivityOpen && (this.levelData.dropActivityEndTime <= TimeUtil.Now))
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.DoubleButton,
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_LevelDropActivityEndHint", new object[0]),
                    buttonCallBack = delegate (bool confirmed) {
                        if (confirmed)
                        {
                            this.EnterPreparePage();
                        }
                    }
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                this.SetupView();
            }
            else
            {
                this.EnterPreparePage();
            }
        }

        public void OnResetButtonCallBack()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new ResetLevelDialogContext(this.levelData, this), UIType.Any);
        }

        public void RefreshChallengeNumber(Transform missionTrans = null)
        {
            if (missionTrans == null)
            {
                missionTrans = base.view.transform.Find("Missions");
            }
            bool flag = this.levelData.isDropActivityOpen && (this.levelData.dropActivityMaxEnterTimes > 0);
            int num = this.levelData.MaxEnterTimes - this.levelData.enterTimes;
            int maxEnterTimes = this.levelData.MaxEnterTimes;
            if (flag)
            {
                num += this.levelData.dropActivityMaxEnterTimes - this.levelData.dropActivityEnterTimes;
                maxEnterTimes += this.levelData.dropActivityMaxEnterTimes;
            }
            if (maxEnterTimes < 0xffff)
            {
                missionTrans.Find("Challenge").gameObject.SetActive(true);
                missionTrans.Find("Challenge/Frame").gameObject.SetActive(!flag);
                missionTrans.Find("Challenge/FrameFever").gameObject.SetActive(flag);
                string str = (num <= 0) ? "red" : "white";
                string str2 = string.Format(" <color={0}>{1}</color>/{2}", str, num, maxEnterTimes);
                missionTrans.Find("Challenge/EnterNumber").GetComponent<Text>().text = str2;
                missionTrans.Find("Challenge/ResetBtn").gameObject.SetActive(num <= 0);
            }
            else
            {
                missionTrans.Find("Challenge").gameObject.SetActive(false);
            }
        }

        private void RefreshDropList()
        {
            List<int> displayFirstDropList;
            string text = string.Empty;
            if (((this.levelData.LevelType == 1) && (this.levelData.displayFirstDropList.Count > 0)) && (this.levelData.status != 3))
            {
                displayFirstDropList = this.levelData.displayFirstDropList;
                text = LocalizationGeneralLogic.GetText("Menu_FirstDropList", new object[0]);
            }
            else
            {
                displayFirstDropList = this.levelData.displayDropList;
                text = LocalizationGeneralLogic.GetText("Menu_DisplayDropList", new object[0]);
            }
            List<int> displayBonusDropList = this.levelData.displayBonusDropList;
            if (!this.levelData.isDropActivityOpen)
            {
                displayBonusDropList = new List<int>();
            }
            base.view.transform.Find("Missions/Drop/Title/Text").GetComponent<Text>().text = text;
            Transform transform = base.view.transform.Find("Missions/Drop/Drops/ScollerContent");
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                bool flag = i < (displayFirstDropList.Count + displayBonusDropList.Count);
                bool flag2 = i >= displayFirstDropList.Count;
                child.gameObject.SetActive(flag);
                if (flag)
                {
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(!flag2 ? displayFirstDropList[i] : displayBonusDropList[i - displayFirstDropList.Count], 1);
                    child.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, new DropItemButtonClickCallBack(this.OnDropItemBtnClick), true, false, false, false);
                    child.Find("BG/Desc").gameObject.SetActive(false);
                    child.Find("AddDropIcon").gameObject.SetActive(flag2);
                }
            }
            base.view.transform.Find("Missions/Drop/Title/IconDropUp").gameObject.SetActive(this.levelData.isDropActivityOpen && this.levelData.isDoubleDrop);
        }

        private void SetupCost()
        {
            base.view.transform.Find("Missions/Prepare/Cost/Stamina").GetComponent<Text>().text = this.levelData.StaminaCost.ToString();
        }

        private void SetupMissionPanel()
        {
            Transform missionTrans = base.view.transform.Find("Missions");
            missionTrans.Find("Title/Name/Text").GetComponent<Text>().text = this.levelData.Title;
            missionTrans.Find("Title/HorizontialLayOut/Recommand/LvNum").GetComponent<Text>().text = this.levelData.RecommandLv.ToString();
            if (this.levelData.LevelType != 1)
            {
                missionTrans.Find("Title/HorizontialLayOut/Difficulty").gameObject.SetActive(false);
            }
            else
            {
                Transform transform2 = missionTrans.Find("Title/HorizontialLayOut/Difficulty/Difficulty");
                Color difficultyColor = Miscs.GetDifficultyColor(this.difficulty);
                string str = Miscs.GetDifficultyDesc(this.difficulty).Substring(0, 2);
                string difficultyMark = UIUtil.GetDifficultyMark(this.difficulty);
                transform2.Find("Color").GetComponent<Image>().color = difficultyColor;
                transform2.Find("Desc").GetComponent<Text>().text = str;
                transform2.Find("Icon/Image").GetComponent<Image>().color = difficultyColor;
                transform2.Find("Icon/Image/Text").GetComponent<Text>().text = difficultyMark;
                base.view.transform.Find("BG/GradualRight").GetComponent<Image>().color = difficultyColor;
                missionTrans.Find("BG/GradualLeft").GetComponent<Image>().color = difficultyColor;
            }
            Transform transform3 = missionTrans.Find("MissionList/MissionPanel");
            for (int i = 0; i < transform3.childCount; i++)
            {
                Transform child = transform3.GetChild(i);
                if (i >= this.levelData.challengeList.Count)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    LevelChallengeDataItem item = this.levelData.challengeList[i];
                    string displayTarget = item.DisplayTarget;
                    child.Find("Achieve/Text").GetComponent<Text>().text = displayTarget;
                    child.Find("Unachieve/Text").GetComponent<Text>().text = displayTarget;
                    child.Find("Achieve").gameObject.SetActive(item.Finished);
                    child.Find("Unachieve").gameObject.SetActive(!item.Finished);
                    child.Find("Loop").gameObject.SetActive(item.IsSpecialChallenge());
                }
            }
            this.RefreshDropList();
            this.RefreshChallengeNumber(missionTrans);
        }

        private void SetupProfile()
        {
            Transform transform = base.view.transform.Find("Profile");
            switch (this.levelData.LevelType)
            {
                case 1:
                {
                    ActDataItem item = new ActDataItem(this.levelData.ActID);
                    transform.Find("Title/Desc").GetComponent<Text>().text = item.actTitle + " " + item.actName;
                    break;
                }
                case 2:
                case 3:
                    transform.Find("Title/Desc").GetComponent<Text>().text = Singleton<LevelModule>.Instance.GetWeekDayActivityByID(this.levelData.ActID).GetActitityTitle();
                    break;
            }
            transform.Find("Title/Desc").GetComponent<TypewriterEffect>().RestartRead();
            transform.Find("Pic/LevelName").gameObject.SetActive(false);
            transform.Find("Pic/LevelName").GetComponent<Text>().text = this.levelData.StageName;
            transform.Find("Pic/Icon").GetComponent<Image>().sprite = this.levelData.GetDetailPicSprite();
            transform.Find("Info/Text").GetComponent<Text>().text = this.levelData.Desc;
            transform.Find("Info/Text").GetComponent<TypewriterEffect>().RestartRead();
        }

        protected override bool SetupView()
        {
            this.SetupProfile();
            this.SetupMissionPanel();
            this.SetupCost();
            return false;
        }
    }
}

