namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class EndlessFloorEndPageContext : BasePageContext
    {
        private SequenceAnimationManager _animationManager;
        private SequenceAnimationManager _dropItemAnimationManager;
        private List<DropItem> _dropItemList;
        private SequenceAnimationManager _dropPanelBGAnimationManager;
        private MonoGridScroller _dropScroller;
        private EndlessGroupMetaData _groupMetaData;
        private bool _hasAvatarDie;
        private LoadingWheelWidgetContext _loadingWheelDialogContext;
        private EndlessStageBeginRsp _stageBeginRsp;
        private const string DROP_ITEM_SCALE_07 = "DropItemScale07";
        public readonly EvtLevelState.LevelEndReason endReason;
        public readonly bool isSuccess;
        private const string MATERIAL_GRAY_SCALE_PATH = "Material/ImageGrayscale";
        private const float WAIT_TIME = 1f;

        public EndlessFloorEndPageContext(EvtLevelState.LevelEndReason reason)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "EndlessFloorEndPageContext",
                viewPrefabPath = "UI/Menus/Page/EndlessActivity/EndlessFloorEndPage"
            };
            base.config = pattern;
            this.endReason = reason;
            this.isSuccess = reason == EvtLevelState.LevelEndReason.EndWin;
            if (Singleton<LevelScoreManager>.Instance != null)
            {
                Singleton<LevelScoreManager>.Instance.isLevelSuccess = this.isSuccess;
            }
        }

        public EndlessFloorEndPageContext(bool isSuccess)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "EndlessFloorEndPageContext",
                viewPrefabPath = "UI/Menus/Page/EndlessActivity/EndlessFloorEndPage"
            };
            base.config = pattern;
            this.isSuccess = isSuccess;
            if (Singleton<LevelScoreManager>.Instance != null)
            {
                Singleton<LevelScoreManager>.Instance.isLevelSuccess = isSuccess;
            }
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Actions/ExitBtn/Button").GetComponent<Button>(), new UnityAction(this.Exit));
            base.BindViewCallback(base.view.transform.Find("Actions/ContinueBtn/Button").GetComponent<Button>(), new UnityAction(this.OnContinueBtnClick));
        }

        public override void Destroy()
        {
            if (this._loadingWheelDialogContext != null)
            {
                this._loadingWheelDialogContext.Finish();
            }
            base.Destroy();
        }

        private void DoBeginLevel()
        {
            Singleton<LevelScoreManager>.Create();
            int progress = !this._stageBeginRsp.get_progressSpecified() ? 1 : (((int) this._stageBeginRsp.get_progress()) + 1);
            this._groupMetaData = EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey(Singleton<EndlessModule>.Instance.currentGroupLevel);
            int hardLevel = Mathf.FloorToInt(this._groupMetaData.baseHardLevel + ((progress - 1) * this._groupMetaData.deltaHardLevel));
            List<string> appliedToolList = new List<string>();
            foreach (uint num3 in this._stageBeginRsp.get_effect_item_id_list())
            {
                EndlessToolDataItem item = new EndlessToolDataItem((int) num3, 1);
                if (item.ParamString != null)
                {
                    appliedToolList.Add(item.ParamString);
                }
            }
            Singleton<LevelScoreManager>.Instance.SetEndlessLevelBeginIntent(progress, hardLevel, appliedToolList, this._stageBeginRsp, MiscData.Config.BasicConfig.EndlessInitTimer, 1);
            this.ResetWaitPacketData();
            Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", false, true, true, null, true);
        }

        private void DoSetupView()
        {
            base.view.transform.Find("GroupPanel").gameObject.SetActive(true);
            this.InitAnimationAndDialogManager();
            Transform transform = base.view.transform.Find("GroupPanel/GroupInfotPanel");
            transform.Find("GroupName").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(EndlessGroupMetaDataReader.GetEndlessGroupMetaDataByKey(Singleton<EndlessModule>.Instance.currentGroupLevel).groupName, new object[0]);
            Color color = Miscs.ParseColor(MiscData.Config.EndlessGroupUnSelectColor[Singleton<EndlessModule>.Instance.currentGroupLevel]);
            Color color2 = Miscs.ParseColor(MiscData.Config.EndlessGroupBGColor[Singleton<EndlessModule>.Instance.currentGroupLevel]);
            transform.Find("GroupName").GetComponent<Text>().color = color;
            base.view.transform.Find("BG/GroupColor").GetComponent<Image>().color = color2;
            int currentFinishProgress = Singleton<EndlessModule>.Instance.CurrentFinishProgress;
            transform.Find("FloorNum").GetComponent<Text>().text = (currentFinishProgress != 0) ? currentFinishProgress.ToString() : "-";
            base.view.transform.Find("GroupPanel/Result/Win").gameObject.SetActive(this.isSuccess);
            base.view.transform.Find("GroupPanel/Result/Lost").gameObject.SetActive(!this.isSuccess);
            if (!this.isSuccess)
            {
                base.view.transform.Find("BG/TriangleFill").GetComponent<Image>().material = Miscs.LoadResource<Material>("Material/ImageGrayscale", BundleType.RESOURCE_FILE);
                base.view.transform.Find("BG").GetComponent<Image>().material = Miscs.LoadResource<Material>("Material/ImageGrayscale", BundleType.RESOURCE_FILE);
            }
            else
            {
                base.view.transform.Find("BG/TriangleFill").GetComponent<Image>().material = null;
                base.view.transform.Find("BG").GetComponent<Image>().material = null;
            }
            this.SetupReward();
            this._dropItemAnimationManager.StartPlay(0f, true);
        }

        private void Exit()
        {
            if ((BehaviorManager.instance != null) && (BehaviorManager.instance.gameObject != null))
            {
                UnityEngine.Object.DestroyImmediate(BehaviorManager.instance.gameObject);
            }
            Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, false, true, null, true);
        }

        private void InitAnimationAndDialogManager()
        {
            this._dropItemAnimationManager = new SequenceAnimationManager(new Action(this.OnDropItemAnimationEnd), null);
            this._dropScroller = base.view.transform.Find("GroupPanel/Drops/ScrollView").GetComponent<MonoGridScroller>();
        }

        private bool OnAvatarDieNotify()
        {
            this._hasAvatarDie = true;
            return false;
        }

        private void OnContinueBtnClick()
        {
            if (Singleton<LevelScoreManager>.Instance != null)
            {
                Singleton<LevelScoreManager>.Destroy();
            }
            this.RequestToEnterLevel();
        }

        private void OnDropItemAnimationEnd()
        {
            IEnumerator enumerator = base.view.transform.Find("GroupPanel/Drops/ScrollView/Content").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    current.SetLocalScaleX(1f);
                    current.SetLocalScaleY(1f);
                    current.GetComponent<CanvasGroup>().alpha = 1f;
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            base.view.transform.Find("Actions").gameObject.SetActive(true);
            if (this._hasAvatarDie)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_EndlessAvatarDie", new object[0]), 2f), UIType.Any);
            }
        }

        private void OnDropNewItemDialogsEnd()
        {
            this._dropItemAnimationManager.StartPlay(0f, true);
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.AvatarDie)
            {
                this.OnAvatarDieNotify();
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0x90:
                    return this.OnStageEndRsp(pkt.getData<EndlessStageEndRsp>());

                case 0x8e:
                    return this.OnStageBeginRsp(pkt.getData<EndlessStageBeginRsp>());
            }
            return false;
        }

        private void OnScrollerChange(Transform trans, int index)
        {
            Vector2 cellSize = this._dropScroller.grid.GetComponent<GridLayoutGroup>().cellSize;
            trans.SetLocalScaleX(cellSize.x / trans.GetComponent<MonoLevelDropIconButton>().width);
            trans.SetLocalScaleY(cellSize.y / trans.GetComponent<MonoLevelDropIconButton>().height);
            DropItem item = this._dropItemList[index];
            StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) item.get_item_id(), 1);
            dummyStorageDataItem.level = (int) item.get_level();
            dummyStorageDataItem.number = (int) item.get_num();
            trans.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, null, true, true, false, false);
            trans.GetComponent<MonoAnimationinSequence>().animationName = "DropItemScale07";
        }

        public bool OnStageBeginRsp(EndlessStageBeginRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                this._stageBeginRsp = rsp;
                this.DoBeginLevel();
            }
            else
            {
                this.ResetWaitPacketData();
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0])
                };
                dialogContext.desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            return false;
        }

        private bool OnStageEndRsp(EndlessStageEndRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                base.view.transform.Find("Actions/ContinueBtn").gameObject.SetActive((this.isSuccess && (rsp.get_progress() < Singleton<PlayerModule>.Instance.playerData.endlessMaxProgress)) && (Singleton<PlayerModule>.Instance.playerData.GetMemberList(4).Count > 0));
                this.DoSetupView();
            }
            else
            {
                string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput, 2f), UIType.Any);
                this.Exit();
            }
            return false;
        }

        private void RequestToEnterLevel()
        {
            this._loadingWheelDialogContext = new LoadingWheelWidgetContext();
            this._loadingWheelDialogContext.ignoreMaxWaitTime = true;
            Singleton<MainUIManager>.Instance.ShowWidget(this._loadingWheelDialogContext, UIType.Any);
            Singleton<NetworkManager>.Instance.RequestEndlessLevelBeginReq();
        }

        private void ResetWaitPacketData()
        {
            this._stageBeginRsp = null;
            if (this._loadingWheelDialogContext != null)
            {
                this._loadingWheelDialogContext.Finish();
            }
        }

        private void SetupReward()
        {
            if (this.isSuccess)
            {
                this._dropItemList = Singleton<LevelScoreManager>.Instance.GetTotalDropList();
                base.view.transform.Find("GroupPanel/Drops/ScrollView").GetComponent<MonoGridScroller>().Init((trans, index) => this.OnScrollerChange(trans, index), this._dropItemList.Count, null);
                this._dropItemAnimationManager.AddAllChildrenInTransform(base.view.transform.Find("GroupPanel/Drops/ScrollView/Content"));
                base.view.transform.Find("GroupPanel/Drops/Nothing").gameObject.SetActive(this._dropItemList.Count <= 0);
            }
        }

        protected override bool SetupView()
        {
            Singleton<LevelScoreManager>.Instance.HandleLevelEnd(this.endReason);
            base.view.transform.Find("Actions/ContinueBtn").gameObject.SetActive(this.isSuccess && (Singleton<EndlessModule>.Instance.CurrentFinishProgress < Singleton<PlayerModule>.Instance.playerData.endlessMaxProgress));
            base.view.transform.Find("GroupPanel").gameObject.SetActive(false);
            base.view.transform.Find("Actions").gameObject.SetActive(false);
            this.SyncRequestLevelEnd();
            return false;
        }

        private void SyncRequestLevelEnd()
        {
            LoadingWheelWidgetContext widget = new LoadingWheelWidgetContext(0x90, null) {
                ignoreMaxWaitTime = true
            };
            Singleton<MainUIManager>.Instance.ShowWidget(widget, UIType.Any);
            if (!Singleton<LevelScoreManager>.Instance.RequestLevelEnd())
            {
                widget.Finish();
            }
        }
    }
}

