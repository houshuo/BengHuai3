namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class InLevelPauseDialogContext : BaseDialogContext
    {
        private MonoSettingAudioTab _audioSetting;
        private MonoGridScroller _dropGridScroller;
        private List<DropItem> _dropItemList;
        private MonoSettingGraphicsTab _graphicSetting;
        private LevelDataItem _levelData;
        private LevelScoreManager _levelScoreManager;
        private TabManager _tabManager;
        private readonly string defaultTab;
        private const float DROP_ITEM_SCALE = 0.85f;
        public const string SettingTab = "SettingTab";
        public const string StatusTab = "StatusTab";

        public event OnClosedHandler OnClosed;

        public InLevelPauseDialogContext(string defaultTab = "StatusTab")
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "InLevelPauseDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/InLevelPauseDialog"
            };
            base.config = pattern;
            this.defaultTab = defaultTab;
            this._tabManager = new TabManager();
            this._tabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/TabBtns/TabBtn_1").GetComponent<Button>(), new UnityAction(this.OnStatusTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/TabBtns/TabBtn_2").GetComponent<Button>(), new UnityAction(this.OnSettingTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/StatusTab/Content/ActionBtns/SurrenderBtn").GetComponent<Button>(), new UnityAction(this.OnSurrenderBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/StatusTab/Content/ActionBtns/ContinueBtn").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/SettingTab/Content/Graphics/Content/NoSaveBtn").GetComponent<Button>(), new UnityAction(this.OnNoSaveBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/SettingTab/Content/Graphics/Content/SaveBtn").GetComponent<Button>(), new UnityAction(this.OnSaveBtnClick));
        }

        private void CheckSave()
        {
            if (this._audioSetting.CheckNeedSave() || this._graphicSetting.CheckNeedSave())
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.DoubleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgTitle", new object[0]),
                    desc = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgDesc", new object[0]),
                    okBtnText = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgSave", new object[0]),
                    cancelBtnText = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgNoSave", new object[0]),
                    buttonCallBack = delegate (bool confirmed) {
                        if (confirmed)
                        {
                            this.OnSaveBtnClick();
                        }
                    }
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
        }

        public override void Destroy()
        {
            Singleton<WwiseAudioManager>.Instance.Post("BGM_PauseMenu_Off", null, null, null);
            base.Destroy();
        }

        private string GetLocalizedText(string progressStr)
        {
            if (progressStr == "Succ")
            {
                return LocalizationGeneralLogic.GetText("Menu_InLevelPauseDialog_Challenge_Succ", new object[0]);
            }
            if (progressStr == "Fail")
            {
                return LocalizationGeneralLogic.GetText("Menu_InLevelPauseDialog_Challenge_Fail", new object[0]);
            }
            if (progressStr == "Doing")
            {
                return LocalizationGeneralLogic.GetText("Menu_InLevelPauseDialog_Challenge_Doing", new object[0]);
            }
            return progressStr;
        }

        private void OnBGBtnClick()
        {
            this.CheckSave();
            Singleton<LevelManager>.Instance.SetPause(false);
            if (this.OnClosed != null)
            {
                this.OnClosed();
                this.OnClosed = null;
            }
            this.Destroy();
        }

        private void OnDropItemLeftArrowClick()
        {
            base.view.transform.Find("Dialog/StatusTab/Content/CurrentGetItems/Items").Find("ScrollView").GetComponent<MonoGridScroller>().ScrollToPreItem();
        }

        private void OnDropItemRightArrowClick()
        {
            base.view.transform.Find("Dialog/StatusTab/Content/CurrentGetItems/Items").Find("ScrollView").GetComponent<MonoGridScroller>().ScrollToNextItem();
        }

        private void OnNoSaveBtnClick()
        {
            this._audioSetting.OnNoSaveBtnClick();
            this._graphicSetting.OnNoSaveBtnClick();
        }

        private void OnSaveBtnClick()
        {
            this._audioSetting.OnSaveBtnClick();
            this._graphicSetting.OnSaveBtnClick();
        }

        private void OnScrollerChange(Transform trans, int index)
        {
            Vector2 cellSize = this._dropGridScroller.grid.GetComponent<GridLayoutGroup>().cellSize;
            trans.SetLocalScaleX(0.85f);
            trans.SetLocalScaleY(0.85f);
            DropItem item = this._dropItemList[index];
            StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) item.get_item_id(), 1);
            dummyStorageDataItem.level = (int) item.get_level();
            dummyStorageDataItem.number = (int) item.get_num();
            trans.GetComponent<CanvasGroup>().alpha = 1f;
            trans.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, null, true, false, false, false);
        }

        public void OnSettingTabBtnClick()
        {
            this._tabManager.ShowTab("SettingTab");
            this._audioSetting.SetupView();
            this._graphicSetting.SetupView(true);
        }

        public void OnStatusTabBtnClick()
        {
            if (this._tabManager.GetShowingTabKey() != "StatusTab")
            {
                this.CheckSave();
            }
            this._tabManager.ShowTab("StatusTab");
        }

        private void OnSurrenderBtnClick()
        {
            Singleton<MainUIManager>.Instance.ShowDialog(new InLevelGiveUpConfirmDialogContext(this, new Action(this.OnSurrenderConfirm)), UIType.Any);
        }

        private void OnSurrenderConfirm()
        {
            Singleton<LevelManager>.Instance.SetPause(false);
            this.Destroy();
            Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.EndLose, EvtLevelState.LevelEndReason.EndLoseQuit, 0), MPEventDispatchMode.Normal);
        }

        private void OnTabSetActive(bool active, GameObject go, Button btn)
        {
            btn.GetComponent<Image>().color = !active ? MiscData.GetColor("Blue") : Color.white;
            btn.transform.Find("Text").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.interactable = !active;
            go.SetActive(active);
        }

        private void SetupSettingTab()
        {
            GameObject gameObject = base.view.transform.Find("Dialog/SettingTab").gameObject;
            Button component = base.view.transform.Find("Dialog/TabBtns/TabBtn_2").GetComponent<Button>();
            this._tabManager.SetTab("SettingTab", component, gameObject);
            this._audioSetting = gameObject.transform.Find("Content/Audio").GetComponent<MonoSettingAudioTab>();
            this._audioSetting.SetupView();
            this._graphicSetting = gameObject.transform.Find("Content/Graphics").GetComponent<MonoSettingGraphicsTab>();
            this._graphicSetting.SetupView(true);
        }

        private void SetupStatusTab()
        {
            GameObject gameObject = base.view.transform.Find("Dialog/StatusTab").gameObject;
            Button component = base.view.transform.Find("Dialog/TabBtns/TabBtn_1").GetComponent<Button>();
            this._tabManager.SetTab("StatusTab", component, gameObject);
            this._levelScoreManager = Singleton<LevelScoreManager>.Instance;
            if ((this._levelScoreManager.isTryLevel || this._levelScoreManager.isDebugLevel) || (this._levelScoreManager.LevelType == 4))
            {
                this.SetupViewForTryOrDebugLevel();
            }
            else
            {
                string str;
                this._levelData = Singleton<LevelModule>.Instance.GetLevelById(this._levelScoreManager.LevelId);
                if (this._levelData.LevelType == 1)
                {
                    string[] textArray1 = new string[] { this._levelScoreManager.chapterTitle, " ", this._levelScoreManager.actTitle, " ", this._levelScoreManager.stageName, " ", this._levelScoreManager.LevelTitle };
                    str = string.Concat(textArray1);
                }
                else
                {
                    str = Singleton<LevelModule>.Instance.GetWeekDayActivityByID(this._levelData.ActID).GetActitityTitle() + " " + this._levelData.Title;
                }
                base.view.transform.Find("Dialog/StatusTab/Content/Title/Text").GetComponent<Text>().text = str;
                base.view.transform.Find("Dialog/StatusTab/Content/CurrentGetItems/Scoin/Num").GetComponent<Text>().text = Mathf.FloorToInt(this._levelScoreManager.scoinInside).ToString();
                Transform transform = base.view.transform.Find("Dialog/StatusTab/Content/CurrentGetItems/Items");
                this._dropItemList = this._levelScoreManager.GetDropListToShow();
                transform.gameObject.SetActive(this._dropItemList.Count > 0);
                this._dropGridScroller = transform.Find("ScrollView").GetComponent<MonoGridScroller>();
                this._dropGridScroller.Init(new MonoGridScroller.OnChange(this.OnScrollerChange), this._dropItemList.Count, null);
                bool flag = this._dropItemList.Count > this._dropGridScroller.GetMaxItemCountWithouScroll();
                transform.Find("PrevBtn").gameObject.SetActive(flag);
                transform.Find("NextBtn").gameObject.SetActive(flag);
                Transform transform2 = base.view.transform.Find("Dialog/StatusTab/Content/ChallengePanel");
                List<LevelChallengeDataItem> list = new List<LevelChallengeDataItem>();
                LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
                LevelMetaData levelMetaDataByKey = LevelMetaDataReader.GetLevelMetaDataByKey(this._levelData.levelId);
                foreach (int num in instance.configChallengeIds)
                {
                    LevelChallengeDataItem item = new LevelChallengeDataItem(num, levelMetaDataByKey, 0);
                    list.Add(item);
                }
                Dictionary<int, BaseLevelChallenge> dictionary = new Dictionary<int, BaseLevelChallenge>();
                foreach (BaseLevelChallenge challenge in Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelChallengeHelperPlugin>().challengeList)
                {
                    dictionary[challenge.challengeId] = challenge;
                }
                for (int i = 0; i < list.Count; i++)
                {
                    LevelChallengeDataItem item2 = list[i];
                    Transform child = transform2.GetChild(i);
                    child.Find("Content").GetComponent<Text>().text = item2.DisplayTarget;
                    bool flag2 = !(dictionary.ContainsKey(item2.challengeId) && !dictionary[item2.challengeId].IsFinished());
                    bool flag3 = dictionary.ContainsKey(item2.challengeId);
                    child.Find("Achieve").gameObject.SetActive(flag2);
                    child.Find("Unachieve").gameObject.SetActive(!flag2);
                    child.Find("Achieve/CompleteMark").gameObject.SetActive(!flag3);
                    child.Find("Achieve/Progress").gameObject.SetActive(flag3);
                    child.Find("Unachieve/Progress").gameObject.SetActive(flag3);
                    if (flag3)
                    {
                        string localizedText = this.GetLocalizedText(dictionary[item2.challengeId].GetProcessMsg());
                        child.Find("Achieve/Progress").GetComponent<Text>().text = localizedText;
                        child.Find("Unachieve/Progress").GetComponent<Text>().text = localizedText;
                    }
                }
            }
        }

        protected override bool SetupView()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = !string.IsNullOrEmpty(showingTabKey) ? showingTabKey : this.defaultTab;
            this._tabManager.Clear();
            this.SetupStatusTab();
            this.SetupSettingTab();
            this._tabManager.ShowTab(searchKey);
            Singleton<WwiseAudioManager>.Instance.Post("BGM_PauseMenu_On", null, null, null);
            return false;
        }

        private void SetupViewForTryOrDebugLevel()
        {
            base.view.transform.Find("Dialog/StatusTab/Content/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_TrySkill", new object[0]);
            base.view.transform.Find("Dialog/StatusTab/Content/CurrentGetItems").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/StatusTab/Content/ChallengePanel").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/StatusTab/Content/Pause").gameObject.SetActive(true);
            if (Singleton<LevelScoreManager>.Instance.LevelType == 4)
            {
                EndlessGroupMetaData data = EndlessGroupMetaDataReader.TryGetEndlessGroupMetaDataByKey(Singleton<EndlessModule>.Instance.currentGroupLevel);
                base.view.transform.Find("Dialog/StatusTab/Content/Title/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(data.groupName, new object[0]);
            }
        }

        public delegate void OnClosedHandler();
    }
}

