namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class InLevelPlotPageContext : BasePageContext
    {
        private Coroutine _currentChatCoroutine;
        private int _currentChatIndex;
        private DialogDataItem _currentDialogDataItem;
        private int _dialogIndex = -1;
        private List<DialogMetaData> _dialogList;
        private PlotMetaData _plotData;
        private int _plotID;
        private MonoStoryScreen _storyScreen;

        public InLevelPlotPageContext(MonoStoryScreen storyScreen, int plotID, GameObject view = null)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "InLevelPlotPageContext",
                viewPrefabPath = "UI/Menus/Page/InLevel/InLevelPlotPage"
            };
            base.config = pattern;
            base.view = view;
            this._storyScreen = storyScreen;
            this._plotID = plotID;
            this._dialogList = new List<DialogMetaData>();
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("PlotDialog/Button").GetComponent<Button>(), new UnityAction(this.OnSkipBtnClick));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
        }

        public override void Destroy()
        {
            base.Destroy();
            this._dialogList.Clear();
            this._dialogIndex = -1;
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            if (!this._storyScreen.IsDialogProcessingOpen())
            {
                if (this._storyScreen.StoryDialogState == MonoStoryScreen.DialogState.Displaying)
                {
                    this.SkipDialog();
                }
                else if (this._storyScreen.StoryDialogState == MonoStoryScreen.DialogState.DialogEnd)
                {
                    this.ShowNextDialog();
                }
                else if (this._storyScreen.StoryDialogState == MonoStoryScreen.DialogState.ChatEnd)
                {
                    this.ShowNextChat();
                }
            }
        }

        private void OnChatFinished()
        {
            if (this._currentChatIndex < this._currentDialogDataItem.plotChatNodeList.Count)
            {
                this._storyScreen.StoryDialogState = MonoStoryScreen.DialogState.ChatEnd;
            }
            else
            {
                this._storyScreen.StoryDialogState = MonoStoryScreen.DialogState.DialogEnd;
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.OnPlotFinished) && this.OnPlotFinished());
        }

        private void OnOpenAnimChange(bool openState)
        {
            if (openState)
            {
                this.ShowNextDialog();
                this._storyScreen.onOpenAnimationChange = (Action<bool>) Delegate.Remove(this._storyScreen.onOpenAnimationChange, new Action<bool>(this.OnOpenAnimChange));
            }
        }

        private bool OnPlotFinished()
        {
            Singleton<MainUIManager>.Instance.BackPage();
            this.Destroy();
            return false;
        }

        public void OnSkipBtnClick()
        {
            this.ShowConfirmSkipDialog();
        }

        private bool OnSocketConnect()
        {
            return false;
        }

        private bool OnSocketDisconnect()
        {
            return false;
        }

        private void QuitPlot()
        {
            MainCameraStoryState storyState = Singleton<CameraManager>.Instance.GetMainCamera().storyState;
            if ((storyState != null) && storyState.active)
            {
                storyState.StartQuit();
                this.SetThisPlotIDFinished();
                Singleton<WwiseAudioManager>.Instance.Post("UI_StoryScreen_Close", null, null, null);
            }
        }

        private void ReadPlotData()
        {
            PlotMetaData plotMetaDataByKey = PlotMetaDataReader.GetPlotMetaDataByKey(this._plotID);
            this._plotData = plotMetaDataByKey;
            for (int i = plotMetaDataByKey.startDialogID; i <= plotMetaDataByKey.endDialogID; i++)
            {
                DialogMetaData dialogMetaDataByKey = DialogMetaDataReader.GetDialogMetaDataByKey(i);
                this._dialogList.Add(dialogMetaDataByKey);
            }
        }

        private void RefreshChatMsg(string chatContentKey, float chatDuration)
        {
            string text = LocalizationGeneralLogic.GetText(chatContentKey, new object[0]);
            this._storyScreen.SetDisplayText(text);
        }

        private void SetSkipButtonEnabled(bool enabled)
        {
            base.view.transform.Find("PlotDialog/Button").gameObject.SetActive(enabled);
        }

        private void SetThisPlotIDFinished()
        {
            if (Singleton<LevelPlotModule>.Instance.GetUnFinishedPlotIDList(this._plotData.levelID).Contains(this._plotData.plotID))
            {
                Singleton<LevelPlotModule>.Instance.MarkPlotIDFinish(this._plotID);
            }
        }

        protected override bool SetupView()
        {
            this.ReadPlotData();
            bool enabled = false;
            if (Singleton<LevelModule>.Instance.ContainLevelById(this._plotData.levelID))
            {
                enabled = true;
                bool flag2 = Singleton<LevelPlotModule>.Instance.IsPlotFinished(this._plotID);
                enabled = enabled && flag2;
            }
            else
            {
                enabled = true;
            }
            this.SetSkipButtonEnabled(enabled);
            this._storyScreen.onOpenAnimationChange = (Action<bool>) Delegate.Combine(this._storyScreen.onOpenAnimationChange, new Action<bool>(this.OnOpenAnimChange));
            this._storyScreen.Typewritter.myEvent.AddListener(new UnityAction(this.OnChatFinished));
            return false;
        }

        private void ShowConfirmSkipDialog()
        {
            GeneralDialogContext dialogContext = new GeneralDialogContext {
                type = GeneralDialogContext.ButtonType.DoubleButton,
                title = LocalizationGeneralLogic.GetText("Skip_Confirm_Title", new object[0]),
                desc = LocalizationGeneralLogic.GetText("Skip_Confirm_Content", new object[0]),
                notDestroyAfterTouchBG = true,
                notDestroyAfterCallback = false,
                buttonCallBack = delegate (bool confirmed) {
                    if (confirmed)
                    {
                        this.SkipPlot();
                    }
                }
            };
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        private void ShowNextChat()
        {
            if (this._currentChatIndex < this._currentDialogDataItem.plotChatNodeList.Count)
            {
                if (this._currentChatCoroutine != null)
                {
                    Singleton<LevelManager>.Instance.levelEntity.StopCoroutine(this._currentChatCoroutine);
                }
                this._currentChatCoroutine = Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(this.WaitAllChatsDone());
            }
        }

        private void ShowNextDialog()
        {
            this._dialogIndex++;
            if (this._currentChatCoroutine != null)
            {
                Singleton<LevelManager>.Instance.levelEntity.StopCoroutine(this._currentChatCoroutine);
            }
            if ((this._dialogIndex >= 0) && (this._dialogIndex < this._dialogList.Count))
            {
                DialogMetaData dialogMetaData = this._dialogList[this._dialogIndex];
                this._currentDialogDataItem = new DialogDataItem(dialogMetaData);
                int avatarID = dialogMetaData.avatarID;
                MonoStoryScreen.SelectScreenSide screenSide = (MonoStoryScreen.SelectScreenSide) dialogMetaData.screenSide;
                this._storyScreen.RefreshAvatar3dModel(avatarID, screenSide);
                this._storyScreen.RefreshCurrentSpeakerWidgets(this._currentDialogDataItem);
                this._currentChatIndex = 0;
                if (this._currentDialogDataItem.plotChatNodeList.Count > 1)
                {
                    this._currentChatCoroutine = Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(this.WaitAllChatsDone());
                }
                else
                {
                    this.RefreshChatMsg(this._currentDialogDataItem.plotChatNodeList[this._currentChatIndex].chatContent, this._currentDialogDataItem.plotChatNodeList[this._currentChatIndex].chatDuration);
                    this._storyScreen.StoryDialogState = MonoStoryScreen.DialogState.Displaying;
                    this._currentChatIndex++;
                }
                Singleton<WwiseAudioManager>.Instance.Post(this._currentDialogDataItem.audio, null, null, null);
            }
            else
            {
                this.QuitPlot();
            }
        }

        private void SkipDialog()
        {
            if (this._storyScreen.StoryDialogState == MonoStoryScreen.DialogState.Displaying)
            {
                this._storyScreen.SkipDialog();
                if (this._currentChatIndex < this._currentDialogDataItem.plotChatNodeList.Count)
                {
                    this._storyScreen.StoryDialogState = MonoStoryScreen.DialogState.ChatEnd;
                }
                else
                {
                    this._storyScreen.StoryDialogState = MonoStoryScreen.DialogState.DialogEnd;
                }
            }
        }

        private void SkipPlot()
        {
            this.QuitPlot();
        }

        public override void StartUp(Transform canvasTrans, Transform viewParent = null)
        {
            base.StartUp(canvasTrans, viewParent);
        }

        [DebuggerHidden]
        private IEnumerator WaitAllChatsDone()
        {
            return new <WaitAllChatsDone>c__Iterator65 { <>f__this = this };
        }

        [CompilerGenerated]
        private sealed class <WaitAllChatsDone>c__Iterator65 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal InLevelPlotPageContext <>f__this;
            internal float <duration>__0;

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
                        goto Label_015E;

                    case 1:
                        break;

                    default:
                        goto Label_019B;
                }
            Label_014D:
                this.<>f__this._storyScreen.StoryDialogState = MonoStoryScreen.DialogState.ChatEnd;
            Label_015E:
                if (this.<>f__this._currentChatIndex < this.<>f__this._currentDialogDataItem.plotChatNodeList.Count)
                {
                    if ((this.<>f__this._storyScreen.StoryDialogState == MonoStoryScreen.DialogState.ChatEnd) || (this.<>f__this._storyScreen.StoryDialogState == MonoStoryScreen.DialogState.Default))
                    {
                        this.<>f__this.RefreshChatMsg(this.<>f__this._currentDialogDataItem.plotChatNodeList[this.<>f__this._currentChatIndex].chatContent, this.<>f__this._currentDialogDataItem.plotChatNodeList[this.<>f__this._currentChatIndex].chatDuration);
                        this.<duration>__0 = this.<>f__this._currentDialogDataItem.plotChatNodeList[this.<>f__this._currentChatIndex].chatDuration;
                        this.<>f__this._currentChatIndex++;
                        if (this.<>f__this._currentChatIndex < this.<>f__this._currentDialogDataItem.plotChatNodeList.Count)
                        {
                            this.<>f__this._storyScreen.StoryDialogState = MonoStoryScreen.DialogState.Displaying;
                            this.$current = new WaitForSeconds(this.<duration>__0);
                            this.$PC = 1;
                            return true;
                        }
                        this.<>f__this._storyScreen.StoryDialogState = MonoStoryScreen.DialogState.Displaying;
                        goto Label_019B;
                    }
                    goto Label_014D;
                }
                this.<>f__this._storyScreen.StoryDialogState = MonoStoryScreen.DialogState.DialogEnd;
                this.$PC = -1;
            Label_019B:
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
    }
}

