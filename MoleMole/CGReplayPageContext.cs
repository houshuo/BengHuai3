namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class CGReplayPageContext : BasePageContext
    {
        private MonoGridScroller _cgScroller;
        private Dictionary<int, RectTransform> _dictBeforeFetch;
        private MonoScrollerFadeManager _fadeMgr;

        public CGReplayPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "CGReplayPageContext",
                viewPrefabPath = "UI/Menus/Page/CGReplay/CGReplay"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
        }

        public override void Destroy()
        {
            base.Destroy();
            BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
            if (mainCanvas != null)
            {
                MonoMainCanvas canvas2 = mainCanvas as MonoMainCanvas;
                if (canvas2 != null)
                {
                    MonoVideoPlayer videoPlayer = canvas2.VideoPlayer;
                    videoPlayer.OnVideoEnd = (Action<CgDataItem>) Delegate.Remove(videoPlayer.OnVideoEnd, new Action<CgDataItem>(this.OnVideoBegin));
                    MonoVideoPlayer player2 = canvas2.VideoPlayer;
                    player2.OnVideoEnd = (Action<CgDataItem>) Delegate.Remove(player2.OnVideoEnd, new Action<CgDataItem>(this.OnVideoEnd));
                }
            }
        }

        private void FetchWidget()
        {
            this._cgScroller = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoGridScroller>();
        }

        private bool IsMissionEqual(RectTransform missionNew, RectTransform missionOld)
        {
            if ((missionNew == null) || (missionOld == null))
            {
                return false;
            }
            MonoCgIconButton component = missionOld.GetComponent<MonoCgIconButton>();
            return (missionNew.GetComponent<MonoCgIconButton>()._item.cgID == component._item.cgID);
        }

        private void OnCgIconButtonClicked(CgDataItem data)
        {
            BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
            if (mainCanvas is MonoMainCanvas)
            {
                (mainCanvas as MonoMainCanvas).PlayVideo(data);
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            return false;
        }

        private void OnVideoBegin(CgDataItem cgDataItem)
        {
            BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
            if (mainCanvas != null)
            {
                MonoMainCanvas canvas2 = mainCanvas as MonoMainCanvas;
                if (canvas2 != null)
                {
                    this.SetStarEffectActive(false);
                }
            }
        }

        private void OnVideoEnd(CgDataItem cgDataItem)
        {
            BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
            if (mainCanvas != null)
            {
                MonoMainCanvas canvas2 = mainCanvas as MonoMainCanvas;
                if (canvas2 != null)
                {
                    this.SetStarEffectActive(true);
                }
            }
        }

        private void SetStarEffectActive(bool active)
        {
            base.view.transform.Find("MovingStars").gameObject.SetActive(active);
        }

        private void SetupAchieveInfoScroller()
        {
            <SetupAchieveInfoScroller>c__AnonStoreyE8 ye = new <SetupAchieveInfoScroller>c__AnonStoreyE8 {
                <>f__this = this,
                cgDataItemList = Singleton<CGModule>.Instance.GetCgDataItemList()
            };
            if (this._cgScroller != null)
            {
                this._cgScroller.Init(new MonoGridScroller.OnChange(ye.<>m__128), ye.cgDataItemList.Count, null);
                this._fadeMgr = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoScrollerFadeManager>();
                this._fadeMgr.Init(this._cgScroller.GetItemDict(), this._dictBeforeFetch, new Func<RectTransform, RectTransform, bool>(this.IsMissionEqual));
                this._fadeMgr.Play();
                this._dictBeforeFetch = null;
            }
        }

        protected override bool SetupView()
        {
            if ((base.view != null) && (base.view.transform != null))
            {
                this.FetchWidget();
                this.SetupAchieveInfoScroller();
                BaseMonoCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas();
                if (mainCanvas != null)
                {
                    MonoMainCanvas canvas2 = mainCanvas as MonoMainCanvas;
                    if (canvas2 != null)
                    {
                        MonoVideoPlayer videoPlayer = canvas2.VideoPlayer;
                        videoPlayer.OnVideoBegin = (Action<CgDataItem>) Delegate.Combine(videoPlayer.OnVideoBegin, new Action<CgDataItem>(this.OnVideoBegin));
                        MonoVideoPlayer player2 = canvas2.VideoPlayer;
                        player2.OnVideoEnd = (Action<CgDataItem>) Delegate.Combine(player2.OnVideoEnd, new Action<CgDataItem>(this.OnVideoEnd));
                    }
                }
            }
            return false;
        }

        [CompilerGenerated]
        private sealed class <SetupAchieveInfoScroller>c__AnonStoreyE8
        {
            internal CGReplayPageContext <>f__this;
            internal List<CgDataItem> cgDataItemList;

            internal void <>m__128(Transform trans, int index)
            {
                MonoCgIconButton component = trans.GetComponent<MonoCgIconButton>();
                if (component != null)
                {
                    component.SetupView(this.cgDataItemList[index]);
                    component.SetClickCallback(new MonoCgIconButton.ClickCallBack(this.<>f__this.OnCgIconButtonClicked));
                }
            }
        }
    }
}

