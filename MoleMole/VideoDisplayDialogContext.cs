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

    public class VideoDisplayDialogContext : BaseDialogContext
    {
        private CgDataItem _currentCgDataItem;
        private GeneralDialogContext _currentGeneralDialog;
        private MonoVideoPlayer _currentVideoPlayer;
        private float _originalSkipBtnPosY;
        private SkipButtonState _skipBtnState = SkipButtonState.Close;
        private bool _withSkipBtn;
        public Action SkipVideoCancelCallback;
        public Action SkipVideoClickedCallback;
        public Action SkipVideoConfirmCallback;

        public VideoDisplayDialogContext(CgDataItem cgDataItem, MonoVideoPlayer videoPlayer, bool withSkipBtn = true)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "VideoDisplayDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/VideoDisplayDialog",
                ignoreNotify = true
            };
            base.config = pattern;
            this._currentCgDataItem = cgDataItem;
            this._currentVideoPlayer = videoPlayer;
            if (this._currentVideoPlayer != null)
            {
                this._currentVideoPlayer.OnVideoEnd = (Action<CgDataItem>) Delegate.Combine(this._currentVideoPlayer.OnVideoEnd, new Action<CgDataItem>(this.OnVideoEndCallback));
            }
            this._withSkipBtn = withSkipBtn;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("SkipBtn").GetComponent<Button>(), new UnityAction(this.OnSkipBtnClicked));
            base.BindViewCallback(base.view.transform.GetComponent<Button>(), new UnityAction(this.OnBgBtnClicked));
        }

        public void OnBgBtnClicked()
        {
            if (this._withSkipBtn)
            {
                Animation component = base.view.transform.GetComponent<Animation>();
                if ((component != null) && !component.isPlaying)
                {
                    if (this._skipBtnState == SkipButtonState.Close)
                    {
                        component.Play("DisplayCgSkipBtn");
                        this._skipBtnState = SkipButtonState.TransitFromClose;
                        this.StartTransitSkipBtn();
                    }
                    else if (this._skipBtnState == SkipButtonState.Open)
                    {
                        component.Play("DisappearCgSkipBtn");
                        this._skipBtnState = SkipButtonState.TransitFromOpen;
                        this.StartTransitSkipBtn();
                    }
                }
            }
        }

        public void OnSkipBtnClicked()
        {
            if (this.SkipVideoClickedCallback != null)
            {
                this.SkipVideoClickedCallback();
            }
            GeneralDialogContext context = new GeneralDialogContext {
                type = GeneralDialogContext.ButtonType.DoubleButton,
                title = LocalizationGeneralLogic.GetText("SkipCG_Confirm_Title", new object[0]),
                desc = LocalizationGeneralLogic.GetText("SkipCG_Confirm_Content", new object[0]),
                notDestroyAfterTouchBG = false,
                notDestroyAfterCallback = false,
                buttonCallBack = delegate (bool confirmed) {
                    if (confirmed)
                    {
                        if (this.SkipVideoConfirmCallback != null)
                        {
                            base.view.transform.Find("SkipBtn").gameObject.SetActive(false);
                            this._skipBtnState = SkipButtonState.Close;
                            this.SkipVideoConfirmCallback();
                        }
                    }
                    else if (this.SkipVideoCancelCallback != null)
                    {
                        this.SkipVideoCancelCallback();
                    }
                }
            };
            this._currentGeneralDialog = context;
            Singleton<MainUIManager>.Instance.ShowDialog(this._currentGeneralDialog, UIType.Any);
        }

        private void OnVideoEndCallback(CgDataItem dataItem)
        {
            if ((this._currentCgDataItem != null) && (this._currentGeneralDialog != null))
            {
                this._currentGeneralDialog.Destroy();
                if (this._currentVideoPlayer != null)
                {
                    this._currentVideoPlayer.OnVideoEnd = (Action<CgDataItem>) Delegate.Remove(this._currentVideoPlayer.OnVideoEnd, new Action<CgDataItem>(this.OnVideoEndCallback));
                }
            }
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("SkipBtn").gameObject.SetActive(false);
            return false;
        }

        [DebuggerHidden]
        private IEnumerator StartSkipBtnTransit()
        {
            return new <StartSkipBtnTransit>c__Iterator5D { <>f__this = this };
        }

        private void StartTransitSkipBtn()
        {
            Singleton<ApplicationManager>.Instance.StartCoroutine(this.StartSkipBtnTransit());
        }

        [CompilerGenerated]
        private sealed class <StartSkipBtnTransit>c__Iterator5D : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal VideoDisplayDialogContext <>f__this;
            internal Animation <animation>__0;

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
                        this.<animation>__0 = this.<>f__this.view.transform.GetComponent<Animation>();
                        if (this.<animation>__0 != null)
                        {
                            break;
                        }
                        goto Label_00C0;

                    case 1:
                        break;

                    default:
                        goto Label_00C0;
                }
                while (this.<animation>__0.isPlaying)
                {
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                if (this.<>f__this._skipBtnState == VideoDisplayDialogContext.SkipButtonState.TransitFromOpen)
                {
                    this.<>f__this._skipBtnState = VideoDisplayDialogContext.SkipButtonState.Close;
                }
                else if (this.<>f__this._skipBtnState == VideoDisplayDialogContext.SkipButtonState.TransitFromClose)
                {
                    this.<>f__this._skipBtnState = VideoDisplayDialogContext.SkipButtonState.Open;
                }
                this.$PC = -1;
            Label_00C0:
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

        public enum SkipButtonState
        {
            Open,
            Close,
            TransitFromOpen,
            TransitFromClose
        }
    }
}

