namespace MoleMole
{
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class TutorialModule : BaseModule
    {
        private Dictionary<string, List<int>> _contextTutorialDict;
        private List<int> _finishTutorialList;
        private bool _isInTutorial;
        private Dictionary<int, List<int>> _missionTutorialDict;
        private List<int> _skipList;
        private TutorialWaitingStatus _tutorialWaitingStatus;
        private Coroutine _waitingFinishOnEndCoroutine;
        private Coroutine _waitingFinishOnEndPreCoroutine;
        private Coroutine _waitingFinishOnStartCoroutine;
        private List<int> _waitingList;
        private WaitingListStatus _waitingListStatus;
        private Coroutine _waitingSkip;
        private LoadingWheelWidgetContext _wheelContext;

        public TutorialModule()
        {
            Singleton<NotifyManager>.Instance.RegisterModule(this);
            this._finishTutorialList = new List<int>();
            this._contextTutorialDict = new Dictionary<string, List<int>>();
            this._missionTutorialDict = new Dictionary<int, List<int>>();
            this._skipList = new List<int>();
        }

        private void CheckAndSetScrollViewBegin(int tutorialStepID)
        {
            string scrollUIPath = TutorialStepDataReader.GetTutorialStepDataByKey(tutorialStepID).scrollUIPath;
            if (scrollUIPath != string.Empty)
            {
                BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
                if (sceneCanvas != null)
                {
                    Transform transform = sceneCanvas.transform.FindChild(scrollUIPath);
                    if (((transform != null) && transform.gameObject.activeInHierarchy) && (transform.GetComponent<ScrollRect>() != null))
                    {
                        transform.GetComponent<MonoGridScroller>().ScrollToBegin();
                    }
                }
            }
        }

        private bool CheckCanDoStep(TutorialData tutorialData, int tutorialStepID)
        {
            if (!this.CheckCanDoStepSpecialCondition(tutorialData))
            {
                return false;
            }
            MissionDataItem missionDataItem = Singleton<MissionModule>.Instance.GetMissionDataItem(tutorialData.triggerMissionID);
            if (missionDataItem == null)
            {
                return false;
            }
            if (((!tutorialData.triggerOnDoing || (missionDataItem.status != 2)) && (!tutorialData.triggerOnFinish || (missionDataItem.status != 3))) && (!tutorialData.triggerOnClose || (missionDataItem.status != 5)))
            {
                return false;
            }
            if (!this.CheckCanDoStepByHighlight(tutorialStepID))
            {
                return false;
            }
            return true;
        }

        private bool CheckCanDoStepByHighlight(int tutorialStepID)
        {
            string targetUIPath = TutorialStepDataReader.GetTutorialStepDataByKey(tutorialStepID).targetUIPath;
            if (targetUIPath != string.Empty)
            {
                BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
                if (sceneCanvas == null)
                {
                    return false;
                }
                if (sceneCanvas.transform.FindChild(targetUIPath) == null)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckCanDoStepSpecialCondition(TutorialData tutorialData)
        {
            if (tutorialData.triggerSpecial != 0x63)
            {
                if ((tutorialData.triggerSpecial == 1) && !Singleton<AvatarModule>.Instance.anyAvatarCanUnlock)
                {
                    return false;
                }
                if (tutorialData.triggerSpecial == 2)
                {
                    BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
                    if ((currentPageContext != null) && (currentPageContext is StorageItemDetailPageContext))
                    {
                        StorageDataItemBase storageItem = ((StorageItemDetailPageContext) currentPageContext).storageItem;
                        if ((storageItem is StigmataDataItem) && !((StigmataDataItem) storageItem).IsAffixIdentify)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                if (tutorialData.triggerSpecial == 3)
                {
                    BasePageContext context2 = Singleton<MainUIManager>.Instance.CurrentPageContext;
                    if ((context2 != null) && (context2 is StorageItemDetailPageContext))
                    {
                        Transform transform = ((StorageItemDetailPageContext) context2).view.transform.Find("ActionBtns/NewAffixBtn");
                        if (((transform != null) && transform.gameObject.activeInHierarchy) && transform.GetComponent<Button>().interactable)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                if (tutorialData.triggerSpecial == 4)
                {
                    BasePageContext context3 = Singleton<MainUIManager>.Instance.CurrentPageContext;
                    if (context3 is MainPageContext)
                    {
                        Transform transform2 = ((MainPageContext) context3).view.transform.Find("MainBtns/IslandBtn");
                        if (((transform2 != null) && transform2.gameObject.activeInHierarchy) && transform2.GetComponent<Button>().interactable)
                        {
                            return true;
                        }
                    }
                    return false;
                }
                if (tutorialData.triggerSpecial == 5)
                {
                    BasePageContext context4 = Singleton<MainUIManager>.Instance.CurrentPageContext;
                    if ((context4 != null) && (context4 is MainPageContext))
                    {
                        foreach (MissionDataItem item in Singleton<MissionModule>.Instance.GetMissionDict().Values)
                        {
                            LinearMissionData linearMissionDataByKey = LinearMissionDataReader.GetLinearMissionDataByKey(item.id);
                            if (((linearMissionDataByKey != null) && (linearMissionDataByKey.IsAchievement == 1)) && (item.status == 3))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
                if (tutorialData.triggerSpecial == 6)
                {
                    List<BaseDialogContext> list = Singleton<MainUIManager>.Instance.CurrentPageContext.dialogContextList;
                    if (list != null)
                    {
                        foreach (BaseDialogContext context5 in list)
                        {
                            if ((context5 is LevelDetailDialogContextV2) && (((LevelDetailDialogContextV2) context5).levelData.LevelType == 5))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }
                if (tutorialData.triggerSpecial != 7)
                {
                    return true;
                }
                List<BaseDialogContext> dialogContextList = Singleton<MainUIManager>.Instance.CurrentPageContext.dialogContextList;
                if (dialogContextList != null)
                {
                    foreach (BaseDialogContext context6 in dialogContextList)
                    {
                        if (context6 is LevelDetailDialogContextV2)
                        {
                            LevelDataItem levelData = ((LevelDetailDialogContextV2) context6).levelData;
                            if (ActMetaDataReader.GetActMetaDataByKey(levelData.ActID) == null)
                            {
                                return false;
                            }
                            ActDataItem item4 = new ActDataItem(levelData.ActID);
                            if (item4.actType == ActDataItem.ActType.Extra)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool CheckCanSkip(TutorialData tutorialData, int tutorialStepID)
        {
            if (tutorialData.SkipGroup != 0)
            {
                if (tutorialStepID != tutorialData.startStepID)
                {
                    return (Singleton<MiHoYoGameData>.Instance.LocalData.IsVisited_Tutorial(tutorialData.startStepID) > 0);
                }
                if (Singleton<MiHoYoGameData>.Instance.LocalData.IsVisited_Tutorial(tutorialStepID) >= 0)
                {
                    Singleton<MiHoYoGameData>.Instance.LocalData.SetVisited_Tutorial(tutorialStepID);
                    return true;
                }
                Singleton<MiHoYoGameData>.Instance.LocalData.SetVisited_Tutorial(tutorialStepID);
            }
            return false;
        }

        private BaseContext CheckContextEnable(string contextName)
        {
            try
            {
                BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
                if (currentPageContext == null)
                {
                    return null;
                }
                if (currentPageContext.config.contextName == contextName)
                {
                    return currentPageContext;
                }
                foreach (BaseDialogContext context2 in currentPageContext.dialogContextList)
                {
                    if (context2.IsActive && (context2.config.contextName == contextName))
                    {
                        return context2;
                    }
                }
                if (contextName == "PlayerStatusWidgetContext")
                {
                    return this.CheckPlayerStatusWidgetEnable();
                }
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            return null;
        }

        private BaseContext CheckPlayerStatusWidgetEnable()
        {
            MonoMainCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas() as MonoMainCanvas;
            if ((mainCanvas != null) && mainCanvas.playerBar.IsActive)
            {
                return mainCanvas.playerBar;
            }
            return null;
        }

        private bool CheckTutorialListEqual(List<int> tutorialList1, List<int> tutorialList2)
        {
            if (tutorialList1.Count != tutorialList2.Count)
            {
                return false;
            }
            int count = tutorialList1.Count;
            for (int i = 0; i < count; i++)
            {
                if (tutorialList1[i] != tutorialList2[i])
                {
                    return false;
                }
            }
            return true;
        }

        private bool CheckTutorialListEqual(List<uint> tutorialList1, List<int> tutorialList2)
        {
            if ((tutorialList1 == null) || (tutorialList2 == null))
            {
                return false;
            }
            if (tutorialList1.Count != tutorialList2.Count)
            {
                return false;
            }
            int count = tutorialList1.Count;
            for (int i = 0; i < count; i++)
            {
                if (tutorialList1[i] != tutorialList2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public void Destroy()
        {
            this.StopAllCoroutines();
        }

        private void DoAfterFinishOnEnd(TutorialData tutorialData, TutorialStepData tutorialStepData)
        {
            if (tutorialStepData.nextStepID != 0)
            {
                this.DoNextStep(tutorialData, tutorialStepData);
            }
        }

        private void DoAfterFinishOnEndPre(TutorialData tutorialData, TutorialStepData tutorialStepData)
        {
            <DoAfterFinishOnEndPre>c__AnonStoreyD8 yd = new <DoAfterFinishOnEndPre>c__AnonStoreyD8 {
                tutorialData = tutorialData,
                tutorialStepData = tutorialStepData,
                <>f__this = this
            };
            bool flag = this.CheckCanSkip(yd.tutorialData, yd.tutorialStepData.id);
            string targetUIPath = yd.tutorialStepData.targetUIPath;
            Transform transform = null;
            if (targetUIPath != string.Empty)
            {
                transform = Singleton<MainUIManager>.Instance.SceneCanvas.transform.FindChild(targetUIPath);
            }
            bool flag2 = false;
            if (yd.tutorialStepData.stepType == 2)
            {
                flag2 = true;
            }
            Action action = null;
            yd.finishOnEndList = yd.tutorialStepData.FinishOnEnd;
            Func<bool> func = new Func<bool>(yd.<>m__F1);
            if (yd.tutorialStepData.nextStepID == 0)
            {
                action = new Action(yd.<>m__F2);
            }
            NewbieDialogContext dialogContext = new NewbieDialogContext {
                disableMask = flag2,
                highlightTrans = transform,
                highlightPath = targetUIPath,
                bubblePosType = (NewbieDialogContext.BubblePosType) yd.tutorialStepData.bubblePosType,
                handIconPosType = (NewbieDialogContext.HandIconPosType) yd.tutorialStepData.handIconPosType,
                disableHighlightEffect = !yd.tutorialStepData.playEffect,
                guideDesc = LocalizationGeneralLogic.GetText(yd.tutorialStepData.guideDesc, new object[0]),
                preCallback = action,
                pointerUpCallback = func,
                destroyButNoClickedCallback = new Action(yd.<>m__F3),
                delayShowTime = yd.tutorialStepData.delayTime,
                bShowReward = yd.tutorialStepData.stepType == 3,
                rewardID = yd.tutorialData.Reward,
                guideID = yd.tutorialData.id,
                bShowSkip = flag
            };
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        private void DoAfterFinishOnStart(TutorialData tutorialData, TutorialStepData tutorialStepData)
        {
            <DoAfterFinishOnStart>c__AnonStoreyD6 yd = new <DoAfterFinishOnStart>c__AnonStoreyD6 {
                <>f__this = this,
                finishOnEndList = tutorialStepData.FinishOnEnd
            };
            if (yd.finishOnEndList.Count > 0)
            {
                if (this.MarkTutorialIDFinishToServer(yd.finishOnEndList, false))
                {
                    this._wheelContext = new LoadingWheelWidgetContext(130, new Action(yd.<>m__EF));
                    Singleton<MainUIManager>.Instance.ShowWidget(this._wheelContext, UIType.Any);
                    if (Singleton<ApplicationManager>.Instance != null)
                    {
                        this._waitingFinishOnEndPreCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.FinishOnEndPreWait(yd.finishOnEndList, tutorialData, tutorialStepData));
                    }
                }
                else
                {
                    this.DoAfterFinishOnEndPre(tutorialData, tutorialStepData);
                }
            }
            else
            {
                this.DoAfterFinishOnEndPre(tutorialData, tutorialStepData);
            }
        }

        private void DoNextStep(TutorialData tutorialData, TutorialStepData currentTutorialStepData)
        {
            int nextStepID = currentTutorialStepData.nextStepID;
            if (!this.CheckCanDoStepByHighlight(nextStepID))
            {
                this._tutorialWaitingStatus = TutorialWaitingStatus.NoWaiting;
                this.SetTutorialFlag(false);
            }
            else
            {
                this.DoTutorial(tutorialData, nextStepID);
            }
        }

        private void DoTutorial(TutorialData tutorialData, int tutorialStepID)
        {
            <DoTutorial>c__AnonStoreyD5 yd = new <DoTutorial>c__AnonStoreyD5 {
                <>f__this = this
            };
            this._tutorialWaitingStatus = TutorialWaitingStatus.NoWaiting;
            this.SetTutorialFlag(true);
            TutorialStepData tutorialStepDataByKey = TutorialStepDataReader.GetTutorialStepDataByKey(tutorialStepID);
            this.CheckAndSetScrollViewBegin(tutorialStepID);
            yd.finishOnStartList = tutorialStepDataByKey.FinishOnStart;
            if (yd.finishOnStartList.Count > 0)
            {
                if (this.MarkTutorialIDFinishToServer(yd.finishOnStartList, true))
                {
                    if (this._wheelContext != null)
                    {
                        this._wheelContext.Finish();
                    }
                    this._wheelContext = new LoadingWheelWidgetContext(130, new Action(yd.<>m__EE));
                    Singleton<MainUIManager>.Instance.ShowWidget(this._wheelContext, UIType.Any);
                    if (Singleton<ApplicationManager>.Instance != null)
                    {
                        this._waitingFinishOnStartCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.FinishOnStartWait(yd.finishOnStartList, tutorialData, tutorialStepDataByKey));
                    }
                }
                else
                {
                    this.DoAfterFinishOnStart(tutorialData, tutorialStepDataByKey);
                }
            }
            else
            {
                this.DoAfterFinishOnStart(tutorialData, tutorialStepDataByKey);
            }
        }

        private void DoTutorialOver(TutorialData tutorialData, TutorialStepData currentTutorialStepData)
        {
            this._tutorialWaitingStatus = TutorialWaitingStatus.NoWaiting;
            this.SetTutorialFlag(false);
        }

        private bool DoTutorialWhenShowContext(BaseContext context)
        {
            List<int> list = this._contextTutorialDict[context.config.contextName];
            foreach (int num in list)
            {
                TutorialData tutorialDataByKey = TutorialDataReader.GetTutorialDataByKey(num);
                if (this.CheckCanDoStep(tutorialDataByKey, tutorialDataByKey.startStepID))
                {
                    this.DoTutorial(tutorialDataByKey, tutorialDataByKey.startStepID);
                    return true;
                }
            }
            return false;
        }

        private void FilterFinishedTutorial(int tutorialID)
        {
            if ((tutorialID <= LevelTutorialModule.BASE_LEVEL_TUTORIAL_ID) && this._finishTutorialList.Contains(tutorialID))
            {
                TutorialData tutorialDataByKey = TutorialDataReader.GetTutorialDataByKey(tutorialID);
                if (tutorialDataByKey != null)
                {
                    string triggerUIContextName = tutorialDataByKey.triggerUIContextName;
                    if (this._contextTutorialDict.ContainsKey(triggerUIContextName))
                    {
                        List<int> list = this._contextTutorialDict[triggerUIContextName];
                        if ((list != null) && list.Contains(tutorialID))
                        {
                            list.Remove(tutorialID);
                        }
                    }
                    int triggerMissionID = tutorialDataByKey.triggerMissionID;
                    if (this._missionTutorialDict.ContainsKey(triggerMissionID))
                    {
                        List<int> list2 = this._missionTutorialDict[triggerMissionID];
                        if ((list2 != null) && list2.Contains(tutorialID))
                        {
                            list2.Remove(tutorialID);
                        }
                    }
                }
            }
        }

        private void FilterUnfinishedTutorial()
        {
            foreach (TutorialData data in TutorialDataReader.GetItemList())
            {
                this.FilterUnfinishedTutorial(data.id);
            }
        }

        private void FilterUnfinishedTutorial(int tutorialID)
        {
            if (this._finishTutorialList.Contains(tutorialID))
            {
                this.FilterFinishedTutorial(tutorialID);
            }
            else
            {
                TutorialData tutorialDataByKey = TutorialDataReader.GetTutorialDataByKey(tutorialID);
                if (tutorialDataByKey != null)
                {
                    string triggerUIContextName = tutorialDataByKey.triggerUIContextName;
                    if (this._contextTutorialDict.ContainsKey(triggerUIContextName))
                    {
                        List<int> list = this._contextTutorialDict[triggerUIContextName];
                        if ((list != null) && !list.Contains(tutorialID))
                        {
                            list.Add(tutorialID);
                        }
                    }
                    else
                    {
                        List<int> list2 = new List<int> {
                            tutorialID
                        };
                        this._contextTutorialDict.Add(triggerUIContextName, list2);
                    }
                    int triggerMissionID = tutorialDataByKey.triggerMissionID;
                    if (this._missionTutorialDict.ContainsKey(triggerMissionID))
                    {
                        List<int> list3 = this._missionTutorialDict[triggerMissionID];
                        if ((list3 != null) && !list3.Contains(tutorialID))
                        {
                            list3.Add(tutorialID);
                        }
                    }
                    else
                    {
                        List<int> list4 = new List<int> {
                            tutorialID
                        };
                        this._missionTutorialDict.Add(triggerMissionID, list4);
                    }
                }
            }
        }

        [DebuggerHidden]
        private IEnumerator FinishOnEndPreWait(List<int> finishOnEndList, TutorialData tutorialData, TutorialStepData tutorialStepData)
        {
            return new <FinishOnEndPreWait>c__Iterator47 { finishOnEndList = finishOnEndList, tutorialData = tutorialData, tutorialStepData = tutorialStepData, <$>finishOnEndList = finishOnEndList, <$>tutorialData = tutorialData, <$>tutorialStepData = tutorialStepData, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator FinishOnEndWait(TutorialData tutorialData, TutorialStepData tutorialStepData)
        {
            return new <FinishOnEndWait>c__Iterator48 { tutorialStepData = tutorialStepData, tutorialData = tutorialData, <$>tutorialStepData = tutorialStepData, <$>tutorialData = tutorialData, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator FinishOnStartWait(List<int> finishOnStartList, TutorialData tutorialData, TutorialStepData tutorialStepData)
        {
            return new <FinishOnStartWait>c__Iterator46 { finishOnStartList = finishOnStartList, tutorialData = tutorialData, tutorialStepData = tutorialStepData, <$>finishOnStartList = finishOnStartList, <$>tutorialData = tutorialData, <$>tutorialStepData = tutorialStepData, <>f__this = this };
        }

        public bool IsTutorialIDFinish(int tutorialID)
        {
            return this._finishTutorialList.Contains(tutorialID);
        }

        private bool MarkTutorialIDFinishToServer(List<int> tutorialIDList, bool isForceFinish)
        {
            bool flag = false;
            foreach (int num in tutorialIDList)
            {
                if (!this._finishTutorialList.Contains(num))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                return false;
            }
            if ((this._waitingList != null) && (this._waitingList.Count > 0))
            {
                return false;
            }
            this._waitingList = tutorialIDList;
            this._waitingListStatus = WaitingListStatus.SendToServer;
            Singleton<NetworkManager>.Instance.RequestFinishGuideReport(tutorialIDList, isForceFinish);
            return true;
        }

        private void NewbieDialogCallback(TutorialData tutorialData, TutorialStepData tutorialStepData, bool waitServer)
        {
            <NewbieDialogCallback>c__AnonStoreyD9 yd = new <NewbieDialogCallback>c__AnonStoreyD9 {
                tutorialStepData = tutorialStepData,
                <>f__this = this
            };
            if (waitServer)
            {
                this._wheelContext = new LoadingWheelWidgetContext(130, new Action(yd.<>m__F4));
                Singleton<MainUIManager>.Instance.ShowWidget(this._wheelContext, UIType.Any);
                if (Singleton<ApplicationManager>.Instance != null)
                {
                    this._waitingFinishOnEndCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.FinishOnEndWait(tutorialData, yd.tutorialStepData));
                }
            }
            else
            {
                this.DoAfterFinishOnEnd(tutorialData, yd.tutorialStepData);
            }
        }

        private bool OnFinishGuideReportRsp(FinishGuideReportRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                List<uint> list = rsp.get_guide_id_list();
                if (this._tutorialWaitingStatus == TutorialWaitingStatus.NoWaiting)
                {
                    foreach (uint num in list)
                    {
                        this.UpdateFinishTutorialID((int) num);
                    }
                }
                else
                {
                    if (!this.CheckTutorialListEqual(list, this._waitingList))
                    {
                        return false;
                    }
                    if (this._tutorialWaitingStatus == TutorialWaitingStatus.WaitingFinishOnStart)
                    {
                        this._waitingListStatus = WaitingListStatus.Finished;
                        foreach (uint num2 in list)
                        {
                            this.UpdateFinishTutorialID((int) num2);
                        }
                    }
                    else if (this._tutorialWaitingStatus == TutorialWaitingStatus.WaitingFinishOnEndPre)
                    {
                        this._waitingListStatus = !rsp.get_is_finish() ? WaitingListStatus.WaitingFinish : WaitingListStatus.Finished;
                        if (this._waitingListStatus == WaitingListStatus.Finished)
                        {
                            foreach (uint num3 in list)
                            {
                                this.UpdateFinishTutorialID((int) num3);
                            }
                        }
                    }
                    else if (this._tutorialWaitingStatus == TutorialWaitingStatus.WaitingFinishOnEnd)
                    {
                        this._waitingListStatus = !rsp.get_is_finish() ? WaitingListStatus.WaitingFinish : WaitingListStatus.Finished;
                        if (this._waitingListStatus == WaitingListStatus.WaitingFinish)
                        {
                            GeneralLogicManager.RestartGame();
                            return false;
                        }
                        foreach (uint num4 in list)
                        {
                            this.UpdateFinishTutorialID((int) num4);
                        }
                    }
                    else if (this._tutorialWaitingStatus == TutorialWaitingStatus.WaitingSkip)
                    {
                        this._waitingListStatus = WaitingListStatus.Finished;
                        foreach (uint num5 in list)
                        {
                            this.UpdateFinishTutorialID((int) num5);
                        }
                    }
                }
            }
            return false;
        }

        private bool OnGetFinishGuideDataRsp(GetFinishGuideDataRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                foreach (uint num in rsp.get_guide_id_list())
                {
                    this.UpdateFinishTutorialID((int) num);
                }
            }
            this.FilterUnfinishedTutorial();
            this.TryToDoTutorialWhenLogin();
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x80:
                    return this.OnGetFinishGuideDataRsp(pkt.getData<GetFinishGuideDataRsp>());

                case 130:
                    return this.OnFinishGuideReportRsp(pkt.getData<FinishGuideReportRsp>());
            }
            return false;
        }

        private void RetryRequestFinishGuideReport(List<int> tutorialIDList, bool isForceFinish)
        {
            <RetryRequestFinishGuideReport>c__AnonStoreyD7 yd = new <RetryRequestFinishGuideReport>c__AnonStoreyD7 {
                tutorialIDList = tutorialIDList,
                isForceFinish = isForceFinish,
                <>f__this = this
            };
            Singleton<NetworkManager>.Instance.RequestFinishGuideReport(yd.tutorialIDList, yd.isForceFinish);
            this._wheelContext = new LoadingWheelWidgetContext(130, new Action(yd.<>m__F0));
            Singleton<MainUIManager>.Instance.ShowWidget(this._wheelContext, UIType.Any);
        }

        public void SetTutorialFlag(bool isInTutorial)
        {
            this._isInTutorial = isInTutorial;
        }

        [DebuggerHidden]
        private IEnumerator SkipWait(List<int> skipList, Action skipUICallback)
        {
            return new <SkipWait>c__Iterator45 { skipList = skipList, skipUICallback = skipUICallback, <$>skipList = skipList, <$>skipUICallback = skipUICallback, <>f__this = this };
        }

        private void StopAllCoroutines()
        {
            if (this._waitingFinishOnStartCoroutine != null)
            {
                if (Singleton<ApplicationManager>.Instance != null)
                {
                    Singleton<ApplicationManager>.Instance.StopCoroutine(this._waitingFinishOnStartCoroutine);
                }
                this._waitingFinishOnStartCoroutine = null;
            }
            if (this._waitingFinishOnEndPreCoroutine != null)
            {
                if (Singleton<ApplicationManager>.Instance != null)
                {
                    Singleton<ApplicationManager>.Instance.StopCoroutine(this._waitingFinishOnEndPreCoroutine);
                }
                this._waitingFinishOnEndPreCoroutine = null;
            }
            if (this._waitingFinishOnEndCoroutine != null)
            {
                if (Singleton<ApplicationManager>.Instance != null)
                {
                    Singleton<ApplicationManager>.Instance.StopCoroutine(this._waitingFinishOnEndCoroutine);
                }
                this._waitingFinishOnEndCoroutine = null;
            }
            if (this._waitingSkip != null)
            {
                if (Singleton<ApplicationManager>.Instance != null)
                {
                    Singleton<ApplicationManager>.Instance.StopCoroutine(this._waitingSkip);
                }
                this._waitingSkip = null;
            }
        }

        public void TryToDoTutoialWhenShowContext(BaseContext context)
        {
            if (!this._isInTutorial)
            {
                bool flag = false;
                if (this._contextTutorialDict.ContainsKey(context.config.contextName))
                {
                    flag = this.DoTutorialWhenShowContext(context);
                }
                if (!flag)
                {
                    this.TryToDoTutorialByCheckPlayerStatusWidget();
                }
            }
        }

        public void TryToDoTutoialWhenUpdateMissionStatus(Mission mission)
        {
            if (!this._isInTutorial && this._missionTutorialDict.ContainsKey((int) mission.get_mission_id()))
            {
                List<int> list = this._missionTutorialDict[(int) mission.get_mission_id()];
                foreach (int num in list)
                {
                    TutorialData tutorialDataByKey = TutorialDataReader.GetTutorialDataByKey(num);
                    if ((this.CheckContextEnable(tutorialDataByKey.triggerUIContextName) != null) && this.CheckCanDoStep(tutorialDataByKey, tutorialDataByKey.startStepID))
                    {
                        this.DoTutorial(tutorialDataByKey, tutorialDataByKey.startStepID);
                        break;
                    }
                }
            }
        }

        private void TryToDoTutorialByCheckPlayerStatusWidget()
        {
            string key = "PlayerStatusWidgetContext";
            if (this._contextTutorialDict.ContainsKey(key))
            {
                BaseContext context = this.CheckPlayerStatusWidgetEnable();
                if (context != null)
                {
                    this.DoTutorialWhenShowContext(context);
                }
            }
        }

        private void TryToDoTutorialWhenLogin()
        {
            if (!this._isInTutorial)
            {
                foreach (string str in this._contextTutorialDict.Keys)
                {
                    if (this.CheckContextEnable(str) != null)
                    {
                        List<int> list = this._contextTutorialDict[str];
                        foreach (int num in list)
                        {
                            TutorialData tutorialDataByKey = TutorialDataReader.GetTutorialDataByKey(num);
                            if ((tutorialDataByKey != null) && this.CheckCanDoStep(tutorialDataByKey, tutorialDataByKey.startStepID))
                            {
                                this.DoTutorial(tutorialDataByKey, tutorialDataByKey.startStepID);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void TryToSkipTutorial(int tutorialID, Action skipUICallback)
        {
            int skipGroup = TutorialDataReader.GetTutorialDataByKey(tutorialID).SkipGroup;
            if (skipGroup == 0)
            {
                UnityEngine.Debug.LogError("skip group is zero");
            }
            else
            {
                this._skipList.Clear();
                foreach (TutorialData data in TutorialDataReader.GetItemList())
                {
                    if (data.SkipGroup == skipGroup)
                    {
                        this._skipList.Add(data.id);
                    }
                }
                if (this.MarkTutorialIDFinishToServer(this._skipList, true))
                {
                    if (this._wheelContext != null)
                    {
                        this._wheelContext.Finish();
                    }
                    this._wheelContext = new LoadingWheelWidgetContext(130, () => this.RetryRequestFinishGuideReport(this._skipList, true));
                    Singleton<MainUIManager>.Instance.ShowWidget(this._wheelContext, UIType.Any);
                    if (Singleton<ApplicationManager>.Instance != null)
                    {
                        this._waitingSkip = Singleton<ApplicationManager>.Instance.StartCoroutine(this.SkipWait(this._skipList, skipUICallback));
                    }
                }
                else
                {
                    skipUICallback();
                }
            }
        }

        private void UpdateFinishTutorialID(int tutorialID)
        {
            if (!this._finishTutorialList.Contains(tutorialID))
            {
                this._finishTutorialList.Add(tutorialID);
                this.FilterFinishedTutorial(tutorialID);
            }
        }

        public bool IsInTutorial
        {
            get
            {
                return this._isInTutorial;
            }
        }

        [CompilerGenerated]
        private sealed class <DoAfterFinishOnEndPre>c__AnonStoreyD8
        {
            internal TutorialModule <>f__this;
            internal List<int> finishOnEndList;
            internal TutorialData tutorialData;
            internal TutorialStepData tutorialStepData;

            internal bool <>m__F1()
            {
                this.<>f__this.NewbieDialogCallback(this.tutorialData, this.tutorialStepData, this.finishOnEndList.Count > 0);
                return false;
            }

            internal void <>m__F2()
            {
                this.<>f__this.DoTutorialOver(this.tutorialData, this.tutorialStepData);
            }

            internal void <>m__F3()
            {
                this.<>f__this.DoTutorialOver(this.tutorialData, this.tutorialStepData);
            }
        }

        [CompilerGenerated]
        private sealed class <DoAfterFinishOnStart>c__AnonStoreyD6
        {
            internal TutorialModule <>f__this;
            internal List<int> finishOnEndList;

            internal void <>m__EF()
            {
                this.<>f__this.RetryRequestFinishGuideReport(this.finishOnEndList, false);
            }
        }

        [CompilerGenerated]
        private sealed class <DoTutorial>c__AnonStoreyD5
        {
            internal TutorialModule <>f__this;
            internal List<int> finishOnStartList;

            internal void <>m__EE()
            {
                this.<>f__this.RetryRequestFinishGuideReport(this.finishOnStartList, true);
            }
        }

        [CompilerGenerated]
        private sealed class <FinishOnEndPreWait>c__Iterator47 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal List<int> <$>finishOnEndList;
            internal TutorialData <$>tutorialData;
            internal TutorialStepData <$>tutorialStepData;
            internal TutorialModule <>f__this;
            internal List<int> finishOnEndList;
            internal TutorialData tutorialData;
            internal TutorialStepData tutorialStepData;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this._tutorialWaitingStatus = TutorialModule.TutorialWaitingStatus.WaitingFinishOnEndPre;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00D7;
                }
                if ((this.<>f__this._waitingList == this.finishOnEndList) && (this.<>f__this._waitingListStatus == TutorialModule.WaitingListStatus.SendToServer))
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                if (this.<>f__this._waitingListStatus == TutorialModule.WaitingListStatus.Finished)
                {
                    this.<>f__this._waitingList = null;
                    this.<>f__this._waitingListStatus = TutorialModule.WaitingListStatus.Invalid;
                }
                this.<>f__this._waitingFinishOnEndPreCoroutine = null;
                this.<>f__this._wheelContext = null;
                this.<>f__this._tutorialWaitingStatus = TutorialModule.TutorialWaitingStatus.NoWaiting;
                this.<>f__this.DoAfterFinishOnEndPre(this.tutorialData, this.tutorialStepData);
                this.$PC = -1;
            Label_00D7:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FinishOnEndWait>c__Iterator48 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal TutorialData <$>tutorialData;
            internal TutorialStepData <$>tutorialStepData;
            internal TutorialModule <>f__this;
            internal TutorialData tutorialData;
            internal TutorialStepData tutorialStepData;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this._tutorialWaitingStatus = TutorialModule.TutorialWaitingStatus.WaitingFinishOnEnd;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00CB;
                }
                if ((this.<>f__this._waitingList == this.tutorialStepData.FinishOnEnd) && (this.<>f__this._waitingListStatus != TutorialModule.WaitingListStatus.Finished))
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.<>f__this._waitingList = null;
                this.<>f__this._waitingListStatus = TutorialModule.WaitingListStatus.Invalid;
                this.<>f__this._waitingFinishOnEndCoroutine = null;
                this.<>f__this._wheelContext = null;
                this.<>f__this._tutorialWaitingStatus = TutorialModule.TutorialWaitingStatus.NoWaiting;
                this.<>f__this.DoAfterFinishOnEnd(this.tutorialData, this.tutorialStepData);
                this.$PC = -1;
            Label_00CB:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <FinishOnStartWait>c__Iterator46 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal List<int> <$>finishOnStartList;
            internal TutorialData <$>tutorialData;
            internal TutorialStepData <$>tutorialStepData;
            internal TutorialModule <>f__this;
            internal List<int> finishOnStartList;
            internal TutorialData tutorialData;
            internal TutorialStepData tutorialStepData;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this._tutorialWaitingStatus = TutorialModule.TutorialWaitingStatus.WaitingFinishOnStart;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00C6;
                }
                if ((this.<>f__this._waitingList == this.finishOnStartList) && (this.<>f__this._waitingListStatus == TutorialModule.WaitingListStatus.SendToServer))
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.<>f__this._waitingList = null;
                this.<>f__this._waitingListStatus = TutorialModule.WaitingListStatus.Invalid;
                this.<>f__this._waitingFinishOnStartCoroutine = null;
                this.<>f__this._wheelContext = null;
                this.<>f__this._tutorialWaitingStatus = TutorialModule.TutorialWaitingStatus.NoWaiting;
                this.<>f__this.DoAfterFinishOnStart(this.tutorialData, this.tutorialStepData);
                this.$PC = -1;
            Label_00C6:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        [CompilerGenerated]
        private sealed class <NewbieDialogCallback>c__AnonStoreyD9
        {
            internal TutorialModule <>f__this;
            internal TutorialStepData tutorialStepData;

            internal void <>m__F4()
            {
                this.<>f__this.RetryRequestFinishGuideReport(this.tutorialStepData.FinishOnEnd, false);
            }
        }

        [CompilerGenerated]
        private sealed class <RetryRequestFinishGuideReport>c__AnonStoreyD7
        {
            internal TutorialModule <>f__this;
            internal bool isForceFinish;
            internal List<int> tutorialIDList;

            internal void <>m__F0()
            {
                this.<>f__this.RetryRequestFinishGuideReport(this.tutorialIDList, this.isForceFinish);
            }
        }

        [CompilerGenerated]
        private sealed class <SkipWait>c__Iterator45 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal List<int> <$>skipList;
            internal Action <$>skipUICallback;
            internal TutorialModule <>f__this;
            internal List<int> skipList;
            internal Action skipUICallback;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<>f__this._tutorialWaitingStatus = TutorialModule.TutorialWaitingStatus.WaitingSkip;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_00BA;
                }
                if ((this.<>f__this._waitingList == this.skipList) && (this.<>f__this._waitingListStatus == TutorialModule.WaitingListStatus.SendToServer))
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.<>f__this._waitingList = null;
                this.<>f__this._waitingListStatus = TutorialModule.WaitingListStatus.Invalid;
                this.<>f__this._waitingSkip = null;
                this.<>f__this._wheelContext = null;
                this.<>f__this._tutorialWaitingStatus = TutorialModule.TutorialWaitingStatus.NoWaiting;
                this.skipUICallback();
                this.$PC = -1;
            Label_00BA:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        private enum TutorialWaitingStatus
        {
            NoWaiting,
            WaitingFinishOnStart,
            WaitingFinishOnEndPre,
            WaitingFinishOnEnd,
            WaitingSkip
        }

        private enum WaitingListStatus
        {
            Invalid,
            SendToServer,
            WaitingFinish,
            Finished
        }
    }
}

