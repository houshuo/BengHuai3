namespace MoleMole
{
    using LuaInterface;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class GeneralLogicManager
    {
        private static bool _initialized;
        private static readonly string UI_DEFAULT_SHADER_NAME = "miHoYo/UI/Default";

        public static void DestroyAll()
        {
            Singleton<WwiseAudioManager>.Instance.Destroy();
            Singleton<WwiseAudioManager>.Destroy();
            Singleton<AssetBundleManager>.Instance.Destroy();
            Singleton<AssetBundleManager>.Destroy();
            Singleton<MiHoYoGameData>.Destroy();
            Singleton<AccountManager>.Destroy();
            Singleton<NotifyManager>.Destroy();
            Singleton<NetworkManager>.Instance.Destroy();
            Singleton<NetworkManager>.Destroy();
            Singleton<MainUIManager>.Destroy();
            Singleton<IslandModule>.Destroy();
            Singleton<StorageModule>.Destroy();
            Singleton<PlayerModule>.Destroy();
            Singleton<AvatarModule>.Destroy();
            Singleton<LevelModule>.Destroy();
            Singleton<GachaModule>.Destroy();
            Singleton<FriendModule>.Destroy();
            Singleton<MailModule>.Destroy();
            Singleton<ChatModule>.Destroy();
            Singleton<MissionModule>.Destroy();
            Singleton<TutorialModule>.Instance.Destroy();
            Singleton<TutorialModule>.Destroy();
            Singleton<LevelTutorialModule>.Destroy();
            Singleton<CGModule>.Destroy();
            Singleton<CommonIDModule>.Destroy();
            Singleton<LevelPlotModule>.Destroy();
            Singleton<GalTouchModule>.Destroy();
            Singleton<BulletinModule>.Destroy();
            if (Singleton<EndlessModule>.Instance != null)
            {
                Singleton<EndlessModule>.Destroy();
            }
            Singleton<TestModule>.Destroy();
            Singleton<ApplicationManager>.Instance.Destroy();
            Singleton<ApplicationManager>.Destroy();
            Singleton<ChannelPayModule>.Destroy();
            Singleton<ShopWelfareModule>.Destroy();
            Singleton<ItempediaModule>.Destroy();
            Singleton<StoreModule>.Destroy();
            Singleton<MainMenuBGM>.Destroy();
            Singleton<RealTimeWeatherManager>.Destroy();
            Singleton<EffectManager>.Instance.Destroy();
            Singleton<EffectManager>.Destroy();
            Singleton<QAManager>.Instance.Destroy();
            Singleton<QAManager>.Destroy();
        }

        private static void DoAfterRefreshGlobalData()
        {
            LocalizationGeneralLogic.InitOnDataAssetReady();
            Singleton<IslandModule>.Create();
            Singleton<StorageModule>.Create();
            Singleton<PlayerModule>.Create();
            Singleton<AvatarModule>.Create();
            Singleton<LevelModule>.Create();
            Singleton<GachaModule>.Create();
            Singleton<FriendModule>.Create();
            Singleton<MailModule>.Create();
            Singleton<ChatModule>.Create();
            Singleton<MissionModule>.Create();
            Singleton<TutorialModule>.Create();
            Singleton<LevelTutorialModule>.Create();
            Singleton<LevelPlotModule>.Create();
            Singleton<CGModule>.Create();
            Singleton<CommonIDModule>.Create();
            Singleton<GalTouchModule>.Create();
            Singleton<BulletinModule>.Create();
            Singleton<TestModule>.Create();
            Singleton<ChannelPayModule>.Create();
            Singleton<ShopWelfareModule>.Create();
            Singleton<ItempediaModule>.Create();
            Singleton<StoreModule>.Create();
            Singleton<MainMenuBGM>.Create();
        }

        public static void InitAll()
        {
            InitOnGameStart();
            InitOnDataAssetReady(false, null);
        }

        public static void InitOnDataAssetReady(bool async = false, Action refreshFinishNecessarCallback = null)
        {
            <InitOnDataAssetReady>c__AnonStoreyCC ycc = new <InitOnDataAssetReady>c__AnonStoreyCC {
                refreshFinishNecessarCallback = refreshFinishNecessarCallback
            };
            if (async)
            {
                GlobalDataManager.RefreshAsync(new Action(ycc.<>m__CE));
            }
            else
            {
                GlobalDataManager.Refresh();
                DoAfterRefreshGlobalData();
                if (ycc.refreshFinishNecessarCallback != null)
                {
                    ycc.refreshFinishNecessarCallback();
                }
            }
        }

        public static void InitOnGameStart()
        {
            if (_initialized)
            {
                DestroyAll();
            }
            _initialized = true;
            CommonUtils.commonFileReader = new CommonUtils.CommonFileReader(Miscs.LoadTextFileToString);
            LuaStatic.luaFileReader = new LuaStatic.LuaFileReader(Miscs.LoadTextFileToString);
            LocalizationGeneralLogic.InitOnGameStart();
            Singleton<ApplicationManager>.Create();
            Singleton<WwiseAudioManager>.Create();
            Singleton<AssetBundleManager>.Create();
            Singleton<MiHoYoGameData>.Create();
            Singleton<AccountManager>.Create();
            Singleton<NotifyManager>.Create();
            Singleton<NetworkManager>.Create();
            Singleton<MainUIManager>.Create();
            Singleton<RealTimeWeatherManager>.Create();
            Singleton<QAManager>.Create();
            Shader shader = Shader.Find(UI_DEFAULT_SHADER_NAME);
            if (shader == null)
            {
            }
            Canvas.GetDefaultCanvasMaterial().shader = shader;
            GraphicsUtils.SetShaderBloomMaxBlendParams();
            Singleton<AccountManager>.Instance.SetupApkCommentInfo();
            Singleton<AccountManager>.Instance.manager.Init();
        }

        public static void QuitGame()
        {
            Application.Quit();
        }

        public static void RestartGame()
        {
            if ((Singleton<MainUIManager>.Instance != null) && (Singleton<MainUIManager>.Instance.SceneCanvas != null))
            {
                MonoGameEntry sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
                if (sceneCanvas != null)
                {
                    sceneCanvas.OnRestartGame();
                }
            }
            Singleton<WwiseAudioManager>.Instance.StopAll();
            Singleton<WwiseAudioManager>.Instance.ClearUp();
            SceneManager.LoadScene("GameEntry");
        }

        [CompilerGenerated]
        private sealed class <InitOnDataAssetReady>c__AnonStoreyCC
        {
            internal Action refreshFinishNecessarCallback;

            internal void <>m__CE()
            {
                GeneralLogicManager.DoAfterRefreshGlobalData();
                if (this.refreshFinishNecessarCallback != null)
                {
                    this.refreshFinishNecessarCallback();
                }
            }
        }
    }
}

