namespace MoleMole
{
    using RenderHeads.Media.AVProVideo;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class MonoVideoPlayer : MonoBehaviour
    {
        private CgDataItem _currentCgDataItem;
        private VideoControlType _currentControlType;
        private VideoDisplayDialogContext _currentDisplayDialog;
        private bool _endDestroyDisplay;
        private MediaPlayer _mediaPlayer;
        private DisplayUGUI _videoDisplayUgui;
        private bool _videoLoaded;
        private bool _videoStarted;
        public Action<CgDataItem> OnVideoBegin;
        public Action<CgDataItem> OnVideoEnd;

        public void Awake()
        {
            this._mediaPlayer = base.transform.Find("MediaPlayer").GetComponent<MediaPlayer>();
            this._mediaPlayer.Events.AddListener(new UnityAction<MediaPlayer, MediaPlayerEvent.EventType, ErrorCode>(this.OnVideoEvent));
            this._mediaPlayer.gameObject.SetActive(false);
        }

        public void DestroyCurrentDisplayDialog()
        {
            if (this._currentDisplayDialog != null)
            {
                this._currentDisplayDialog.Destroy();
                this._currentDisplayDialog = null;
            }
        }

        [DebuggerHidden]
        private IEnumerator fadeSkipIter()
        {
            return new <fadeSkipIter>c__Iterator80 { <>f__this = this };
        }

        public void LoadOrPlayVideo(CgDataItem cgDataItem, Action OverrideSkipCallback = null, Action<CgDataItem> OnVideoBeginCallback = null, Action<CgDataItem> OnVideoEndCallback = null, VideoControlType controlType = 1, bool withSkipBtn = true, bool endDestroyDisplay = true)
        {
            if (this._currentControlType == VideoControlType.Unload)
            {
                this._currentDisplayDialog = new VideoDisplayDialogContext(cgDataItem, this, withSkipBtn);
                this._currentDisplayDialog.SkipVideoConfirmCallback = (OverrideSkipCallback == null) ? new Action(this.StartSkipWithFade) : OverrideSkipCallback;
                Singleton<MainUIManager>.Instance.ShowDialog(this._currentDisplayDialog, UIType.Any);
                this._videoDisplayUgui = this._currentDisplayDialog.view.GetComponentInChildren<DisplayUGUI>();
                if (this._videoDisplayUgui != null)
                {
                    this._videoDisplayUgui.CurrentMediaPlayer = this._mediaPlayer;
                    this._videoDisplayUgui.gameObject.SetActive(false);
                }
                this._currentControlType = controlType;
                this._mediaPlayer.gameObject.SetActive(true);
                this._mediaPlayer.Stop();
                this._mediaPlayer.CloseVideo();
                string path = string.Format("Video/{0}.mp4", cgDataItem.cgPath);
                if (!this._mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, path, false))
                {
                    this._mediaPlayer.Events.RemoveAllListeners();
                    this._videoLoaded = false;
                    this._mediaPlayer.gameObject.SetActive(false);
                }
                this._videoLoaded = true;
                this._endDestroyDisplay = endDestroyDisplay;
                this._currentCgDataItem = cgDataItem;
                this.OnVideoBegin = (Action<CgDataItem>) Delegate.Combine(this.OnVideoBegin, OnVideoBeginCallback);
                this.OnVideoEnd = (Action<CgDataItem>) Delegate.Combine(this.OnVideoEnd, OnVideoEndCallback);
            }
            else if (((this._currentControlType == VideoControlType.Load) && (controlType == VideoControlType.Play)) && this._videoLoaded)
            {
                this.OnVideoStarted();
            }
        }

        private void OnVideoEnded()
        {
            this.SetGameAudioEnabled(true);
            this._currentControlType = VideoControlType.Unload;
            this._videoLoaded = false;
            this._videoStarted = false;
            this._mediaPlayer.gameObject.SetActive(false);
            if ((this._currentCgDataItem != null) && (this.OnVideoEnd != null))
            {
                this.OnVideoEnd(this._currentCgDataItem);
            }
            if (this._endDestroyDisplay && (this._currentDisplayDialog != null))
            {
                this._currentDisplayDialog.Destroy();
            }
            Screen.sleepTimeout = -2;
        }

        private void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType et, ErrorCode ec)
        {
            switch (et)
            {
                case MediaPlayerEvent.EventType.ReadyToPlay:
                    if ((this._currentControlType == VideoControlType.Play) && !this._videoStarted)
                    {
                        this.OnVideoStarted();
                    }
                    break;

                case MediaPlayerEvent.EventType.FirstFrameReady:
                    base.StartCoroutine(this.WaitActiveUgui());
                    break;

                case MediaPlayerEvent.EventType.FinishedPlaying:
                case MediaPlayerEvent.EventType.Error:
                    this.OnVideoEnded();
                    break;
            }
        }

        private void OnVideoStarted()
        {
            this._currentControlType = VideoControlType.Play;
            this.SetGameAudioEnabled(false);
            this._mediaPlayer.Play();
            this._videoStarted = true;
            if ((this._currentCgDataItem != null) && (this.OnVideoBegin != null))
            {
                this.OnVideoBegin(this._currentCgDataItem);
            }
            Screen.sleepTimeout = -1;
        }

        private void SetGameAudioEnabled(bool enabled)
        {
            if (enabled)
            {
                Singleton<WwiseAudioManager>.Instance.Post("UI_CG_Exit", null, null, null);
            }
            else
            {
                Singleton<WwiseAudioManager>.Instance.Post("UI_CG_Enter_Long", null, null, null);
            }
        }

        public void SkipCgDisplay()
        {
            if (this._mediaPlayer != null)
            {
                this._mediaPlayer.CloseVideo();
                this.OnVideoEnded();
            }
        }

        private void Start()
        {
            this._currentCgDataItem = null;
            this.OnVideoBegin = null;
            this.OnVideoEnd = null;
            this._videoLoaded = false;
            this._videoStarted = false;
            this._endDestroyDisplay = true;
        }

        public void StartSkipWithFade()
        {
            base.StartCoroutine(this.fadeSkipIter());
        }

        private void Update()
        {
        }

        [DebuggerHidden]
        private IEnumerator WaitActiveUgui()
        {
            return new <WaitActiveUgui>c__Iterator7F { <>f__this = this };
        }

        [CompilerGenerated]
        private sealed class <fadeSkipIter>c__Iterator80 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoVideoPlayer <>f__this;
            internal float <fadeRatio>__0;
            internal Color <preColor>__1;

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
                        if (((this.<>f__this._mediaPlayer != null) && (this.<>f__this._videoDisplayUgui != null)) && this.<>f__this._videoDisplayUgui.gameObject.activeSelf)
                        {
                            this.<fadeRatio>__0 = 1f;
                            break;
                        }
                        goto Label_013A;

                    case 1:
                        break;

                    default:
                        goto Label_013A;
                }
                while (this.<fadeRatio>__0 > 0f)
                {
                    if (!this.<>f__this._videoDisplayUgui.gameObject.activeSelf)
                    {
                        goto Label_013A;
                    }
                    this.<preColor>__1 = this.<>f__this._videoDisplayUgui.color;
                    this.<>f__this._videoDisplayUgui.color = new Color(this.<preColor>__1.r, this.<preColor>__1.g, this.<preColor>__1.b, this.<fadeRatio>__0);
                    this.<fadeRatio>__0 -= Time.deltaTime * 2f;
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.<>f__this.SkipCgDisplay();
                this.$PC = -1;
            Label_013A:
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
        private sealed class <WaitActiveUgui>c__Iterator7F : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoVideoPlayer <>f__this;

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
                        if (this.<>f__this._videoDisplayUgui != null)
                        {
                            this.<>f__this._videoDisplayUgui.color = new Color(0f, 0f, 0f, 0f);
                            this.$current = new WaitForSeconds(0.1f);
                            this.$PC = 1;
                            return true;
                        }
                        break;

                    case 1:
                        this.<>f__this._videoDisplayUgui.gameObject.SetActive(true);
                        this.<>f__this._videoDisplayUgui.color = Color.white;
                        this.$PC = -1;
                        break;
                }
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

        public enum VideoControlType
        {
            Unload,
            Play,
            Load
        }
    }
}

