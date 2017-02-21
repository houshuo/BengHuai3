namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class LevelEndPageContext : BasePageContext
    {
        private int _cgId;
        private bool _forceEnableWhenSetup;
        private CanvasTimer _timer;
        private int ANIMATOR_LEVEL_WIN_BOOL_ID;
        private int ANIMATOR_TRIGGER_PLAY_ID;
        public readonly EvtLevelState.LevelEndReason endReason;
        public readonly bool isSuccess;
        private const float WAIT_TIME = 10f;

        public LevelEndPageContext(bool isSuccess)
        {
            this.ANIMATOR_LEVEL_WIN_BOOL_ID = Animator.StringToHash("LevelWin");
            this.ANIMATOR_TRIGGER_PLAY_ID = Animator.StringToHash("TriggerPlay");
            ContextPattern pattern = new ContextPattern {
                contextName = "LevelEndPageContext",
                viewPrefabPath = "UI/Menus/Page/InLevel/LevelEndPage"
            };
            base.config = pattern;
            this.isSuccess = isSuccess;
            this._cgId = 0;
            if (Singleton<LevelScoreManager>.Instance != null)
            {
                Singleton<LevelScoreManager>.Instance.isLevelSuccess = isSuccess;
            }
        }

        public LevelEndPageContext(EvtLevelState.LevelEndReason reason, bool forceEnableWhenSetup = false, int cgId = 0)
        {
            this.ANIMATOR_LEVEL_WIN_BOOL_ID = Animator.StringToHash("LevelWin");
            this.ANIMATOR_TRIGGER_PLAY_ID = Animator.StringToHash("TriggerPlay");
            ContextPattern pattern = new ContextPattern {
                contextName = "LevelEndPageContext",
                viewPrefabPath = "UI/Menus/Page/InLevel/LevelEndPage"
            };
            base.config = pattern;
            this.endReason = reason;
            this.isSuccess = reason == EvtLevelState.LevelEndReason.EndWin;
            if (Singleton<LevelScoreManager>.Instance != null)
            {
                Singleton<LevelScoreManager>.Instance.isLevelSuccess = this.isSuccess;
            }
            this._forceEnableWhenSetup = forceEnableWhenSetup;
            this._cgId = cgId;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Button").GetComponent<Button>(), new UnityAction(this.OnFinishButtonClicked));
        }

        public override void Destroy()
        {
            if (this._timer != null)
            {
                this._timer.Destroy();
            }
            base.Destroy();
        }

        private void Finish()
        {
            if (this._timer != null)
            {
                this._timer.Destroy();
            }
            if ((BehaviorManager.instance != null) && (BehaviorManager.instance.gameObject != null))
            {
                UnityEngine.Object.DestroyImmediate(BehaviorManager.instance.gameObject);
            }
            Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceshipWithUI", false, false, true, null, true);
        }

        private int GetStageTime()
        {
            if ((Singleton<LevelManager>.Instance.levelActor != null) && (Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorTimerPlugin>() != null))
            {
                return Mathf.FloorToInt(Singleton<LevelManager>.Instance.levelActor.GetPlugin<LevelActorTimerPlugin>().Timer + 1f);
            }
            return 0;
        }

        private void OnCgFinishedCallback(CgDataItem cgDataItem)
        {
            Singleton<CGModule>.Instance.MarkCGIDFinish(cgDataItem.cgID);
            Singleton<EventManager>.Instance.FireEvent(new EvtVideoState((uint) cgDataItem.cgID, EvtVideoState.State.Finish), MPEventDispatchMode.Normal);
            this.Finish();
        }

        private void OnFinishButtonClicked()
        {
            if (this._cgId != 0)
            {
                <OnFinishButtonClicked>c__AnonStoreyF7 yf = new <OnFinishButtonClicked>c__AnonStoreyF7 {
                    <>f__this = this,
                    cgDataItem = Singleton<CGModule>.Instance.GetCgDataItem(this._cgId),
                    allowPlayCgWithSkipBtn = false
                };
                if (yf.cgDataItem != null)
                {
                    yf.allowPlayCgWithSkipBtn = Singleton<LevelDesignManager>.Instance.AllowSkipVideo(this._cgId);
                    Singleton<CGModule>.Instance.MarkCGIDFinish(yf.cgDataItem.cgID);
                }
                yf.canvas = Singleton<MainUIManager>.Instance.GetInLevelUICanvas();
                if ((yf.canvas != null) && (yf.cgDataItem != null))
                {
                    Action fadeStartCallback = new Action(yf.<>m__175);
                    yf.canvas.FadeOutStageTransitPanel(1f, false, fadeStartCallback, new Action(yf.<>m__176));
                }
                else
                {
                    this.Finish();
                }
            }
            else
            {
                this.Finish();
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x2e) && this.OnStageEndRsp(pkt.getData<StageEndRsp>()));
        }

        private bool OnStageEndRsp(StageEndRsp rsp)
        {
            Singleton<LevelScoreManager>.Instance.stageEndRsp = rsp;
            Singleton<LevelModule>.Instance.ClearLevelEndReqInfo();
            if (rsp.get_retcode() == null)
            {
                if (this.isSuccess)
                {
                    this.SetupWinPanel(rsp);
                }
                else
                {
                    this.SetupHintListPanel();
                }
                this.SetupDelayTimer();
            }
            else
            {
                string networkErrCodeOutput = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(networkErrCodeOutput, 2f), UIType.Any);
                this.SetupDelayTimer();
            }
            return false;
        }

        private void PlayLevelEndAnim(bool isWin)
        {
            Animator component = base.view.GetComponent<Animator>();
            component.SetTrigger(this.ANIMATOR_TRIGGER_PLAY_ID);
            component.SetBool(this.ANIMATOR_LEVEL_WIN_BOOL_ID, isWin);
        }

        private void SetupDelayTimer()
        {
            base.view.transform.Find("Button").gameObject.SetActive(true);
            if (this._timer != null)
            {
                this._timer.Destroy();
            }
            float num = 10f;
            this._timer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(!this.isSuccess ? (num * 2f) : num, 0f);
            this._timer.timeUpCallback = new Action(this.OnFinishButtonClicked);
        }

        private void SetupHintListPanel()
        {
            this.SetupLoseDescription();
            this.PlayLevelEndAnim(false);
        }

        private void SetupLoseDescription()
        {
            List<string> loseDescList = Singleton<LevelModule>.Instance.GetLevelById(Singleton<LevelScoreManager>.Instance.LevelId).LoseDescList;
            List<string> list = new List<string>();
            list.AddRange(loseDescList);
            if (loseDescList.Count > 3)
            {
                list.Shuffle<string>();
                list = list.GetRange(0, 3);
            }
            Transform transform = base.view.transform.Find("LosePanel/HintListPanel");
            for (int i = 0; i < list.Count; i++)
            {
                transform.FindChild(i.ToString() + "/Content").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(list[i], new object[0]);
            }
        }

        private int SetupReward(Transform trans, StageChallengeData challengeData)
        {
            int number = 0;
            Image component = trans.Find("Icon").GetComponent<Image>();
            Text text = trans.Find("Num").GetComponent<Text>();
            if (challengeData.get_reward().get_hcoin() > 0)
            {
                component.sprite = UIUtil.GetResourceSprite(ResourceType.Hcoin, null);
                number = (int) challengeData.get_reward().get_hcoin();
            }
            if (challengeData.get_reward().get_scoin() > 0)
            {
                component.sprite = UIUtil.GetResourceSprite(ResourceType.Scoin, null);
                number = (int) challengeData.get_reward().get_scoin();
            }
            if (challengeData.get_reward().get_stamina() > 0)
            {
                component.sprite = UIUtil.GetResourceSprite(ResourceType.Stamina, null);
                number = (int) challengeData.get_reward().get_stamina();
            }
            if (challengeData.get_reward().get_skill_point() > 0)
            {
                component.sprite = UIUtil.GetResourceSprite(ResourceType.SkillPoint, null);
                number = (int) challengeData.get_reward().get_skill_point();
            }
            if (challengeData.get_reward().get_exp() > 0)
            {
                component.sprite = UIUtil.GetResourceSprite(ResourceType.PlayerExp, null);
                number = (int) challengeData.get_reward().get_exp();
            }
            if (challengeData.get_reward().get_item_list().Count > 0)
            {
                RewardItemData data = challengeData.get_reward().get_item_list()[0];
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) data.get_id(), 1);
                dummyStorageDataItem.level = (int) data.get_level();
                dummyStorageDataItem.number = (int) data.get_num();
                component.sprite = Miscs.GetSpriteByPrefab(dummyStorageDataItem.GetIconPath());
                number = dummyStorageDataItem.number;
            }
            text.text = number.ToString();
            return number;
        }

        private void SetupTitle()
        {
            LevelDataItem item = Singleton<LevelModule>.Instance.TryGetLevelById(Singleton<LevelScoreManager>.Instance.LevelId);
            if (item != null)
            {
                string stageName = string.Empty;
                string title = string.Empty;
                switch (item.LevelType)
                {
                    case 1:
                        stageName = item.StageName;
                        break;

                    case 2:
                    case 3:
                        stageName = Singleton<LevelModule>.Instance.GetWeekDayActivityByID(item.ActID).GetActitityTitle();
                        break;
                }
                title = item.Title;
                base.view.transform.Find("WinPanel/Title/LevelInfo/ActName").GetComponent<Text>().text = stageName;
                base.view.transform.Find("LosePanel/Title/LevelInfo/ActName").GetComponent<Text>().text = stageName;
                base.view.transform.Find("WinPanel/Title/LevelInfo/LevelName").GetComponent<Text>().text = title;
                base.view.transform.Find("LosePanel/Title/LevelInfo/LevelName").GetComponent<Text>().text = title;
            }
        }

        protected override bool SetupView()
        {
            Singleton<LevelScoreManager>.Instance.HandleLevelEnd(this.endReason);
            base.view.transform.Find("WinPanel").gameObject.SetActive(this.isSuccess);
            base.view.transform.Find("LosePanel").gameObject.SetActive(!this.isSuccess);
            base.view.transform.Find("Button").gameObject.SetActive(false);
            this.SetupTitle();
            if (Singleton<LevelScoreManager>.Instance.isTryLevel || Singleton<LevelScoreManager>.Instance.isDebugLevel)
            {
                Singleton<LevelScoreManager>.Destroy();
                base.view.transform.Find("WinPanel/ChallengePanel").gameObject.SetActive(false);
                Singleton<MainUIManager>.Instance.ShowDialog(new HintWithConfirmDialogContext(LocalizationGeneralLogic.GetText("Menu_Desc_LevelTryEnd", new object[0]), null, new Action(this.Finish), LocalizationGeneralLogic.GetText("Menu_Tips", new object[0])), UIType.Any);
            }
            else
            {
                if (this.isSuccess)
                {
                    base.view.transform.Find("WinPanel/ChallengePanel").gameObject.SetActive(false);
                }
                if (!this._forceEnableWhenSetup && ((this.endReason == EvtLevelState.LevelEndReason.EndWin) || (this.endReason == EvtLevelState.LevelEndReason.EndLoseNotMeetCondition)))
                {
                    base.view.transform.Find("WinPanel").gameObject.SetActive(false);
                    base.view.transform.Find("LosePanel").gameObject.SetActive(false);
                    this.SetActive(false);
                }
                LoadingWheelWidgetContext widget = new LoadingWheelWidgetContext(0x2e, null) {
                    ignoreMaxWaitTime = true
                };
                Singleton<MainUIManager>.Instance.ShowWidget(widget, UIType.Any);
                if (!Singleton<LevelScoreManager>.Instance.RequestLevelEnd())
                {
                    widget.Finish();
                }
            }
            return false;
        }

        private void SetupWinPanel(StageEndRsp rsp)
        {
            Singleton<MainUIManager>.Instance.LockUI(true, 2f);
            base.view.transform.Find("WinPanel/ChallengePanel").gameObject.SetActive(true);
            base.view.transform.Find("WinPanel/DataPanel/0/Num").GetComponent<Text>().text = "0";
            base.view.transform.Find("WinPanel/DataPanel/0").GetComponent<MonoNumberJump>().SetTargetValue((int) Singleton<LevelScoreManager>.Instance.maxComboNum, false, false);
            base.view.transform.Find("WinPanel/DataPanel/1/Num").GetComponent<Text>().text = "00:00";
            base.view.transform.Find("WinPanel/DataPanel/1").GetComponent<MonoNumberJump>().SetTargetValue(this.GetStageTime(), true, false);
            List<LevelChallengeDataItem> list = new List<LevelChallengeDataItem>();
            Dictionary<int, LevelChallengeDataItem> dictionary = new Dictionary<int, LevelChallengeDataItem>();
            LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
            LevelMetaData levelMeta = LevelMetaDataReader.TryGetLevelMetaDataByKey((int) rsp.get_stage_id());
            foreach (int num in instance.configChallengeIds)
            {
                LevelChallengeDataItem item = new LevelChallengeDataItem(num, levelMeta, 0) {
                    Finished = true
                };
                list.Add(item);
                dictionary[num] = item;
            }
            foreach (int num2 in instance.trackChallengeIds)
            {
                dictionary[num2].Finished = false;
            }
            Dictionary<int, StageChallengeData> dictionary2 = new Dictionary<int, StageChallengeData>();
            foreach (StageChallengeData data2 in rsp.get_challenge_list())
            {
                int num3 = (int) data2.get_challenge_index();
                if (num3 < list.Count)
                {
                    int challengeId = list[num3].challengeId;
                    dictionary[challengeId].Finished = true;
                    dictionary2[challengeId] = data2;
                }
            }
            Dictionary<int, StageSpecialChallengeData> dictionary3 = new Dictionary<int, StageSpecialChallengeData>();
            foreach (StageSpecialChallengeData data3 in rsp.get_special_challenge_list())
            {
                int num5 = (int) data3.get_challenge_index();
                if (num5 < list.Count)
                {
                    int num6 = list[num5].challengeId;
                    dictionary[num6].Finished = true;
                    dictionary3[num6] = data3;
                }
            }
            Transform transform = base.view.transform.Find("WinPanel/ChallengePanel");
            for (int i = 0; i < list.Count; i++)
            {
                LevelChallengeDataItem item2 = list[i];
                Transform child = transform.GetChild(i);
                child.Find("Content").GetComponent<Text>().text = item2.DisplayTarget;
                child.Find("Achieve").gameObject.SetActive(item2.Finished);
                child.Find("Unachieve").gameObject.SetActive(!item2.Finished);
                if (dictionary2.ContainsKey(item2.challengeId))
                {
                    StageChallengeData challengeData = dictionary2[item2.challengeId];
                    child.Find("Achieve/CompleteMark").gameObject.SetActive(false);
                    child.Find("Achieve/Reward").gameObject.SetActive(true);
                    child.Find("Achieve/Box").gameObject.SetActive(false);
                    int targetValue = this.SetupReward(child.Find("Achieve/Reward"), challengeData);
                    child.GetComponent<MonoNumberJump>().SetTargetValue(targetValue, false, false);
                }
                else if (dictionary3.ContainsKey(item2.challengeId))
                {
                    child.Find("Achieve/CompleteMark").gameObject.SetActive(false);
                    child.Find("Achieve/Reward").gameObject.SetActive(false);
                    child.Find("Achieve/Box").gameObject.SetActive(true);
                }
                else
                {
                    child.Find("Achieve/CompleteMark").gameObject.SetActive(true);
                    child.Find("Achieve/Reward").gameObject.SetActive(false);
                }
            }
            this.PlayLevelEndAnim(true);
        }

        [CompilerGenerated]
        private sealed class <OnFinishButtonClicked>c__AnonStoreyF7
        {
            internal LevelEndPageContext <>f__this;
            internal bool allowPlayCgWithSkipBtn;
            internal MonoInLevelUICanvas canvas;
            internal CgDataItem cgDataItem;

            internal void <>m__175()
            {
                if (this.<>f__this._timer != null)
                {
                    this.<>f__this._timer.Destroy();
                }
                Singleton<WwiseAudioManager>.Instance.Post("UI_CG_Enter_Long", null, null, null);
            }

            internal void <>m__176()
            {
                Action<CgDataItem> onVideoEndCallback = new Action<CgDataItem>(this.<>f__this.OnCgFinishedCallback);
                bool allowPlayCgWithSkipBtn = this.allowPlayCgWithSkipBtn;
                this.canvas.VideoPlayer.LoadOrPlayVideo(this.cgDataItem, null, null, onVideoEndCallback, MonoVideoPlayer.VideoControlType.Play, allowPlayCgWithSkipBtn, false);
            }
        }
    }
}

