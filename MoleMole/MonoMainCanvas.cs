namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoMainCanvas : BaseMonoCanvas
    {
        private GalTouchContext _galTouchContext;
        private GameObject _mainCamera;
        private GameObject _spaceShip;
        private SpaceShipModelContext _spaceshipModelContext;
        private MonoVideoPlayer _videoPlayer;
        public Avatar3dModelContext avatar3dModelContext;
        public PlayerStatusWidgetContext playerBar;

        [DebuggerHidden]
        private IEnumerator ApplyGraphicsSetting()
        {
            return new <ApplyGraphicsSetting>c__Iterator55 { <>f__this = this };
        }

        public void Awake()
        {
            Singleton<MainUIManager>.Instance.SetMainCanvas(this);
        }

        public override void ClearAllWidgetContext()
        {
            if (this.playerBar != null)
            {
                this.playerBar.Destroy();
                this.playerBar = null;
            }
        }

        public override GameObject GetSpaceShipObj()
        {
            return this._spaceShip;
        }

        public void InitMainPageContexts()
        {
            if (this.avatar3dModelContext == null)
            {
                GameObject view = GameObject.Find("AvatarContainer");
                this.avatar3dModelContext = new Avatar3dModelContext(view);
                Singleton<MainUIManager>.Instance.ShowWidget(this.avatar3dModelContext, UIType.Root);
            }
            if (this._spaceshipModelContext == null)
            {
                GameObject obj3 = GameObject.Find("MainMenu_SpaceShip");
                if (obj3 == null)
                {
                    obj3 = UnityEngine.Object.Instantiate<GameObject>((GameObject) Miscs.LoadResource("Stage/MainMenu_SpaceShip/MainMenu_SpaceShip", BundleType.RESOURCE_FILE));
                    obj3.name = "MainMenu_SpaceShip";
                }
                this._spaceShip = obj3;
                this._spaceshipModelContext = new SpaceShipModelContext(this._spaceShip, this._mainCamera);
                Singleton<MainUIManager>.Instance.ShowWidget(this._spaceshipModelContext, UIType.Any);
            }
            if (this._galTouchContext == null)
            {
                this._galTouchContext = new GalTouchContext();
                Singleton<MainUIManager>.Instance.ShowWidget(this._galTouchContext, UIType.Any);
            }
        }

        public override void OnDestroy()
        {
            if ((this._spaceshipModelContext != null) && (this._spaceshipModelContext.view != null))
            {
                UnityEngine.Object.Destroy(this._spaceshipModelContext.view);
            }
            if ((this.avatar3dModelContext != null) && (this.avatar3dModelContext.view != null))
            {
                UnityEngine.Object.Destroy(this.avatar3dModelContext.view);
            }
            if (this._galTouchContext != null)
            {
                this._galTouchContext.Destroy();
            }
        }

        public override void PlayVideo(CgDataItem cgDataItem)
        {
            if (this._videoPlayer != null)
            {
                this._videoPlayer.LoadOrPlayVideo(cgDataItem, null, null, null, MonoVideoPlayer.VideoControlType.Play, true, true);
            }
        }

        private void ProcessRealtimeWeatherUpdate()
        {
            if (Singleton<RealTimeWeatherManager>.Instance.IsReadyToRetryQuery())
            {
                Singleton<RealTimeWeatherManager>.Instance.QueryWeatherInfo(null);
            }
        }

        private void ShowPage()
        {
            if (Singleton<MainUIManager>.Instance.HasContextInStash())
            {
                Singleton<MainUIManager>.Instance.CreateContextFromStash();
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowPage(new MainPageContext(), UIType.Page);
            }
        }

        public override void Start()
        {
            this._mainCamera = GameObject.Find("MainCamera");
            this.playerBar = new PlayerStatusWidgetContext();
            Singleton<MainUIManager>.Instance.ShowWidget(this.playerBar, UIType.Any);
            AudioSettingData.ApplySettingConfig();
            this._videoPlayer = GameObject.Find("VideoPlayer").GetComponent<MonoVideoPlayer>();
            LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
            if (instance != null)
            {
                if (((!instance.isTryLevel && !instance.isDebugLevel) && ((instance.stageEndRsp != null) && (instance.stageEndRsp.get_retcode() == null))) && instance.isLevelSuccess)
                {
                    LevelResultDialogContext dialogContext = new LevelResultDialogContext(null) {
                        onDestory = new Action(this.ShowPage)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                }
                else
                {
                    Singleton<LevelScoreManager>.Destroy();
                    this.ShowPage();
                }
            }
            else
            {
                this.ShowPage();
            }
            GraphicsSettingData.ApplySettingConfig();
            Camera main = Camera.main;
            if (main != null)
            {
                PostFX component = main.GetComponent<PostFX>();
                if (component != null)
                {
                    component.WriteDepthTexture = false;
                }
            }
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.DestroyLoadingScene, null));
            Resources.UnloadUnusedAssets();
            base.Start();
        }

        public override void Update()
        {
            if (Singleton<RealTimeWeatherManager>.Instance.Available)
            {
                this.ProcessRealtimeWeatherUpdate();
            }
            base.Update();
        }

        public MonoVideoPlayer VideoPlayer
        {
            get
            {
                return this._videoPlayer;
            }
        }

        [CompilerGenerated]
        private sealed class <ApplyGraphicsSetting>c__Iterator55 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoMainCanvas <>f__this;

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
                        this.<>f__this._mainCamera.SetActive(true);
                        GraphicsSettingData.ApplySettingConfig();
                        this.$current = null;
                        this.$PC = 1;
                        goto Label_00BA;

                    case 1:
                        this.$current = null;
                        this.$PC = 2;
                        goto Label_00BA;

                    case 2:
                        if (this.<>f__this._mainCamera.activeSelf && ((this.<>f__this._spaceshipModelContext == null) || !this.<>f__this._spaceshipModelContext.view.activeSelf))
                        {
                            this.<>f__this._mainCamera.SetActive(false);
                        }
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_00BA:
                return true;
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

