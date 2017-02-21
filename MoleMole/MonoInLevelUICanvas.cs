namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoInLevelUICanvas : BaseMonoCanvas
    {
        private AchieveUnlockInLevelContext _achieveUnlockInLevelContext;
        private MonsterActor _activeTargetMonster;
        private Action _fadeEndCallback;
        private FadeState _fadeState;
        private MonoInLevelLock _lockEffect;
        private float _movieBarAlphaFrom;
        private float _movieBarAlphaTo;
        private CanvasGroup _movieBarCanvasGroup;
        private float _movieBarFadeTimer;
        private GameObject _movieBarPanel;
        private EntityTimer _showHPTimer;
        private Color _stageTransitColorFrom;
        private Color _stageTransitColorTo;
        private float _stageTransitFadeTimer;
        private float _stageTransitFadeTimeSpan;
        private Image _stageTransitImage;
        private GameObject _stageTransitPanel;
        private MonoVideoPlayer _videoPlayer;
        private const string FADE_IN_STATE_NAME = "StageTransitFadeIn";
        private const string FADE_OUT_STATE_NAME = "StageTransitFadeout";
        private const float HP_BAR_SHOWING_TIME = float.MaxValue;
        public GameObject inLevelMainPage;
        public InLevelMainPageContext mainPageContext;
        private const float MOVIE_BAR_FADE_DURATION = 0.15f;
        private const string MOVIE_BAR_PANEL_NAME = "StageMovieBarPanel";
        private static Color TRANSIT_BLACK = new Color(0f, 0f, 0f, 1f);
        private const float TRANSIT_FADE_DURATION = 0.18f;
        private static Color TRANSIT_WHITE = new Color(0f, 0f, 0f, 0f);

        private void Awake()
        {
            this.hintArrowManager = new HintArrowManager();
            this._showHPTimer = new EntityTimer(float.MaxValue);
            this._showHPTimer.SetActive(false);
            base.transform.Find("TopPanels/StageTransitPanel").gameObject.SetActive(true);
            Singleton<MainUIManager>.Instance.SetMainCanvas(this);
            this._movieBarPanel = base.transform.Find("TopPanels/StageMovieBarPanel").gameObject;
            this._movieBarCanvasGroup = this._movieBarPanel.GetComponent<CanvasGroup>();
            this._stageTransitPanel = base.transform.Find("TopPanels/StageTransitPanel").gameObject;
            this._stageTransitImage = this._stageTransitPanel.GetComponent<Image>();
            this._fadeState = FadeState.Normal;
            this._stageTransitFadeTimeSpan = 0.18f;
            this._fadeEndCallback = null;
        }

        public void FadeInStageTransitPanel(float fadeDuration = 0.18f, bool instant = false, Action fadeStartCallback = null, Action fadeEndCallback = null)
        {
            if (this._stageTransitImage.color.a == 0f)
            {
                if (fadeStartCallback != null)
                {
                    fadeStartCallback();
                }
                if (fadeEndCallback != null)
                {
                    fadeEndCallback();
                }
            }
            this._stageTransitPanel.SetActive(true);
            if (this._stageTransitPanel.activeSelf)
            {
                if (fadeStartCallback != null)
                {
                    fadeStartCallback();
                }
                if (fadeEndCallback != null)
                {
                    this._fadeEndCallback = fadeEndCallback;
                }
                if (instant)
                {
                    this._stageTransitFadeTimer = 0f;
                    this._stageTransitImage.color = TRANSIT_WHITE;
                    this._stageTransitPanel.SetActive(false);
                    if (this._fadeEndCallback != null)
                    {
                        this._fadeEndCallback();
                    }
                }
                else
                {
                    this._stageTransitColorFrom = TRANSIT_BLACK;
                    this._stageTransitColorTo = TRANSIT_WHITE;
                    this._stageTransitFadeTimeSpan = fadeDuration;
                    this._stageTransitFadeTimer = this._stageTransitFadeTimeSpan;
                    this._stageTransitImage.color = TRANSIT_BLACK;
                    this._fadeState = FadeState.FadeIn;
                }
            }
        }

        public void FadeOutStageTransitPanel(float fadeDuration = 0.18f, bool instant = false, Action fadeStartCallback = null, Action fadeEndCallback = null)
        {
            if (this._stageTransitImage.color.a == 1f)
            {
                this._fadeState = FadeState.FadeOut;
                if (fadeStartCallback != null)
                {
                    fadeStartCallback();
                }
                if (fadeEndCallback != null)
                {
                    fadeEndCallback();
                }
            }
            else
            {
                this._stageTransitPanel.SetActive(true);
                this._stageTransitPanel.transform.SetAsLastSibling();
                if (fadeStartCallback != null)
                {
                    fadeStartCallback();
                }
                if (fadeEndCallback != null)
                {
                    this._fadeEndCallback = fadeEndCallback;
                }
                if (instant)
                {
                    this._stageTransitFadeTimer = 0f;
                    this._stageTransitImage.color = TRANSIT_BLACK;
                    if (this._fadeEndCallback != null)
                    {
                        this._fadeEndCallback();
                    }
                }
                else
                {
                    this._stageTransitColorFrom = TRANSIT_WHITE;
                    this._stageTransitColorTo = TRANSIT_BLACK;
                    this._stageTransitFadeTimeSpan = fadeDuration;
                    this._stageTransitFadeTimer = this._stageTransitFadeTimeSpan;
                    this._stageTransitImage.color = TRANSIT_WHITE;
                    this._fadeState = FadeState.FadeOut;
                }
            }
        }

        public void HideMovieBar(bool instant)
        {
            if (this._movieBarPanel.activeSelf)
            {
                if (instant)
                {
                    this._movieBarFadeTimer = 0f;
                    this._movieBarPanel.SetActive(false);
                }
                else
                {
                    this._movieBarAlphaFrom = 1f;
                    this._movieBarAlphaTo = 0f;
                    if (this._movieBarFadeTimer <= 0f)
                    {
                        this._movieBarFadeTimer = 0.15f;
                        this._movieBarCanvasGroup.alpha = 1f;
                    }
                }
            }
        }

        public bool IsStageTransitPanelFading()
        {
            return ((this._stageTransitFadeTimer > 0f) && ((this._fadeState == FadeState.FadeIn) || (this._fadeState == FadeState.FadeOut)));
        }

        public void LoadVideo(CgDataItem cgDataItem)
        {
            if (this._videoPlayer != null)
            {
                bool withSkipBtn = false;
                if (cgDataItem != null)
                {
                    withSkipBtn = Singleton<LevelDesignManager>.Instance.AllowSkipVideo(cgDataItem.cgID);
                }
                Action<CgDataItem> onVideoBeginCallback = new Action<CgDataItem>(this.OnInLevelVideoBeginCallback);
                this._videoPlayer.LoadOrPlayVideo(cgDataItem, null, onVideoBeginCallback, new Action<CgDataItem>(this.OnInLevelVideoEndCallback), MonoVideoPlayer.VideoControlType.Load, withSkipBtn, true);
            }
        }

        public override void OnDestroy()
        {
            Singleton<NotifyManager>.Instance.RemoveContext(this._achieveUnlockInLevelContext);
            this._achieveUnlockInLevelContext.Destroy();
            this._achieveUnlockInLevelContext = null;
        }

        public void OnInLevelVideoBeginCallback(CgDataItem cgDataItem)
        {
            Singleton<LevelManager>.Instance.SetPause(true);
        }

        public void OnInLevelVideoEndCallback(CgDataItem cgDataItem)
        {
            <OnInLevelVideoEndCallback>c__AnonStorey108 storey = new <OnInLevelVideoEndCallback>c__AnonStorey108 {
                cgDataItem = cgDataItem
            };
            Singleton<LevelManager>.Instance.SetPause(false);
            Action fadeEndCallback = new Action(storey.<>m__1B9);
            this.FadeInStageTransitPanel(0.18f, false, null, fadeEndCallback);
        }

        private void OnUpdateAttackTarget(BaseMonoEntity entity)
        {
            this.UpdateLockEntity(entity);
            MonsterActor targetBefore = this._activeTargetMonster;
            if (entity != null)
            {
                BaseMonoEntity owner = null;
                if (entity is BaseMonoMonster)
                {
                    owner = entity;
                }
                else if ((entity is MonoBodyPartEntity) && (((MonoBodyPartEntity) entity).owner is BaseMonoMonster))
                {
                    owner = ((MonoBodyPartEntity) entity).owner;
                }
                if (owner != null)
                {
                    MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(owner.GetRuntimeID());
                    this._activeTargetMonster = actor;
                    this._showHPTimer.Reset(false);
                }
            }
            MonsterActor targetAfter = this._activeTargetMonster;
            if (targetBefore != targetAfter)
            {
                this.mainPageContext.OnTargetMonsterChange(targetBefore, targetAfter);
            }
        }

        public void OnUpdateLocalAvatar(uint runtimeID, uint oldRuntimeId = 0)
        {
            if (oldRuntimeId != 0)
            {
                BaseMonoAvatar avatar1 = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(oldRuntimeId);
                avatar1.onAttackTargetChanged = (Action<BaseMonoEntity>) Delegate.Remove(avatar1.onAttackTargetChanged, new Action<BaseMonoEntity>(this.OnUpdateAttackTarget));
            }
            BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(runtimeID);
            avatarByRuntimeID.onAttackTargetChanged = (Action<BaseMonoEntity>) Delegate.Combine(avatarByRuntimeID.onAttackTargetChanged, new Action<BaseMonoEntity>(this.OnUpdateAttackTarget));
            AvatarActor avatarAfter = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
            AvatarActor avatarBefore = (oldRuntimeId != 0) ? ((AvatarActor) Singleton<EventManager>.Instance.GetActor(oldRuntimeId)) : null;
            this.mainPageContext.OnLocalAvatarChanged(avatarBefore, avatarAfter);
        }

        public void OnUpdateLocalAvatarAbilityDisplay(uint runtimeID, uint oldRuntimeID = 0)
        {
            bool flag = false;
            if (oldRuntimeID != 0)
            {
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(oldRuntimeID);
                if (actor.abilityPlugin.HasDisplayFloat("IsOverheat"))
                {
                    flag = true;
                    actor.abilityPlugin.SubDetachDisplayFloat("IsOverheat", new Action<float, float>(this.mainPageContext.OnIsOverheatChanged));
                    actor.abilityPlugin.SubDetachDisplayFloat("OverheatRatio", new Action<float, float>(this.mainPageContext.OnOverheatRatioChanged));
                }
            }
            AvatarActor actor2 = Singleton<EventManager>.Instance.GetActor<AvatarActor>(runtimeID);
            if (actor2.abilityPlugin.HasDisplayFloat("IsOverheat"))
            {
                if (!flag)
                {
                    this.mainPageContext.SetOverHeatViewActive(true);
                }
                float curValue = 0f;
                float num2 = 0f;
                float ceiling = 0f;
                float floor = 0f;
                actor2.abilityPlugin.SubAttachDisplayFloat("IsOverheat", new Action<float, float>(this.mainPageContext.OnIsOverheatChanged), ref curValue, ref floor, ref ceiling);
                actor2.abilityPlugin.SubAttachDisplayFloat("OverheatRatio", new Action<float, float>(this.mainPageContext.OnOverheatRatioChanged), ref num2, ref floor, ref ceiling);
                this.mainPageContext.UpdateOverHeatView(curValue > 0f, num2);
            }
            else if (flag)
            {
                this.mainPageContext.SetOverHeatViewActive(false);
            }
        }

        public override void PlayVideo(CgDataItem cgDataItem)
        {
            if (this._videoPlayer != null)
            {
                bool withSkipBtn = false;
                if (cgDataItem != null)
                {
                    withSkipBtn = Singleton<LevelDesignManager>.Instance.AllowSkipVideo(cgDataItem.cgID);
                }
                Action<CgDataItem> onVideoBeginCallback = new Action<CgDataItem>(this.OnInLevelVideoBeginCallback);
                this._videoPlayer.LoadOrPlayVideo(cgDataItem, null, onVideoBeginCallback, new Action<CgDataItem>(this.OnInLevelVideoEndCallback), MonoVideoPlayer.VideoControlType.Play, withSkipBtn, true);
            }
        }

        public void SetInLevelUIActive(bool active)
        {
            BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
            if (currentPageContext != null)
            {
                if (currentPageContext is InLevelMainPageContext)
                {
                    (currentPageContext as InLevelMainPageContext).SetInLevelMainPageActive(active, false, false);
                }
                else
                {
                    Singleton<MainUIManager>.Instance.CurrentPageContext.SetActive(active);
                }
            }
        }

        public void SetWhiteTransitPanelActive(bool enable)
        {
            base.transform.Find("TopPanels/WhiteTransitPanel").gameObject.SetActive(enable);
        }

        public void ShowMovieBar(bool instant)
        {
            if (!this._movieBarPanel.activeSelf || (this._movieBarCanvasGroup.alpha != 1f))
            {
                this._movieBarPanel.SetActive(true);
                this._movieBarPanel.transform.SetAsLastSibling();
                if (instant)
                {
                    this._movieBarFadeTimer = 0f;
                    this._movieBarCanvasGroup.alpha = 1f;
                }
                else
                {
                    this._movieBarAlphaFrom = 0f;
                    this._movieBarAlphaTo = 1f;
                    if (this._movieBarFadeTimer <= 0f)
                    {
                        this._movieBarFadeTimer = 0.15f;
                        this._movieBarCanvasGroup.alpha = 0f;
                    }
                }
            }
        }

        public override void Start()
        {
            base.GetComponent<Canvas>().worldCamera = UnityEngine.Object.FindObjectOfType<MonoInLevelUICamera>().GetComponent<Camera>();
            this.inLevelMainPage.SetActive(true);
            this.mainPageContext = new InLevelMainPageContext(this.inLevelMainPage);
            Singleton<MainUIManager>.Instance.ShowPage(this.mainPageContext, UIType.Page);
            this.mainPageContext.view.name = "InLevelMainPage";
            Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.Start, EvtLevelState.LevelEndReason.EndUncertainReason, 0), MPEventDispatchMode.Normal);
            this.FadeOutStageTransitPanel(0.18f, true, null, null);
            this._lockEffect = this.mainPageContext.view.transform.Find("InLevel_Lock_02").GetComponent<MonoInLevelLock>();
            this._lockEffect.gameObject.SetActive(false);
            this.hintArrowManager.InitAtStart();
            this._achieveUnlockInLevelContext = new AchieveUnlockInLevelContext();
            Singleton<MainUIManager>.Instance.ShowWidget(this._achieveUnlockInLevelContext, UIType.Any);
            this._videoPlayer = GameObject.Find("VideoPlayer").GetComponent<MonoVideoPlayer>();
            base.Start();
        }

        public void StartPlayVideo(CgDataItem cgDataItem)
        {
            <StartPlayVideo>c__AnonStorey107 storey = new <StartPlayVideo>c__AnonStorey107 {
                cgDataItem = cgDataItem,
                <>f__this = this
            };
            Action fadeEndCallback = new Action(storey.<>m__1B8);
            this.FadeOutStageTransitPanel(0.18f, false, null, fadeEndCallback);
        }

        public override void Update()
        {
            base.Update();
            this.hintArrowManager.Core();
            this._lockEffect.Core();
            this._showHPTimer.Core(1f);
            if ((this._showHPTimer.isTimeUp || (this._activeTargetMonster == null)) || ((this._activeTargetMonster.monster == null) || !this._activeTargetMonster.monster.IsActive()))
            {
                if (this.mainPageContext.view != null)
                {
                    this.mainPageContext.HideMonsterStatus();
                }
                this._showHPTimer.Reset(false);
            }
            if (((this._fadeState == FadeState.FadeIn) || (this._fadeState == FadeState.FadeOut)) && (this._stageTransitFadeTimer >= 0f))
            {
                this._stageTransitImage.color = Color.Lerp(this._stageTransitColorFrom, this._stageTransitColorTo, (this._stageTransitFadeTimeSpan - this._stageTransitFadeTimer) / this._stageTransitFadeTimeSpan);
                this._stageTransitFadeTimer -= Time.deltaTime;
                if (this._stageTransitFadeTimer <= 0f)
                {
                    this._stageTransitImage.color = this._stageTransitColorTo;
                    this._fadeState = FadeState.Normal;
                    this._stageTransitFadeTimer = 0f;
                    if (this._stageTransitColorTo == TRANSIT_WHITE)
                    {
                        this._stageTransitPanel.gameObject.SetActive(false);
                    }
                    if (this._fadeEndCallback != null)
                    {
                        this._fadeEndCallback();
                    }
                }
            }
            if (this._movieBarFadeTimer > 0f)
            {
                this._movieBarCanvasGroup.alpha = Mathf.Lerp(this._movieBarAlphaFrom, this._movieBarAlphaTo, (0.15f - this._movieBarFadeTimer) / 0.15f);
                this._movieBarFadeTimer -= Time.deltaTime;
                if (this._movieBarFadeTimer <= 0f)
                {
                    this._movieBarCanvasGroup.alpha = this._movieBarAlphaTo;
                    if (this._movieBarAlphaTo == 0f)
                    {
                        this._movieBarCanvasGroup.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void UpdateLockEntity(BaseMonoEntity entity)
        {
            this._lockEffect.SetLockFollowTarget(entity);
        }

        public HintArrowManager hintArrowManager { get; private set; }

        public MonoVideoPlayer VideoPlayer
        {
            get
            {
                return this._videoPlayer;
            }
        }

        [CompilerGenerated]
        private sealed class <OnInLevelVideoEndCallback>c__AnonStorey108
        {
            internal CgDataItem cgDataItem;

            internal void <>m__1B9()
            {
                Singleton<CGModule>.Instance.MarkCGIDFinish(this.cgDataItem.cgID);
                Singleton<EventManager>.Instance.FireEvent(new EvtVideoState((uint) this.cgDataItem.cgID, EvtVideoState.State.Finish), MPEventDispatchMode.Normal);
            }
        }

        [CompilerGenerated]
        private sealed class <StartPlayVideo>c__AnonStorey107
        {
            internal MonoInLevelUICanvas <>f__this;
            internal CgDataItem cgDataItem;

            internal void <>m__1B8()
            {
                this.<>f__this.PlayVideo(this.cgDataItem);
            }
        }

        public enum FadeState
        {
            Normal,
            FadeIn,
            FadeOut
        }
    }
}

