namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;
    using UnityEngine.Events;

    public class GalTouchContext : BaseWidgetContext
    {
        private GameObject _fullGoodFeelEffect;
        private MonoGalTouchView _galTouchView;
        private bool _setup;
        private const string FULL_LEVEL_EFFECT_PREFAB_PATH = "UI/Menus/Widget/Storage/Sakura";

        public GalTouchContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "GalTouchContext",
                viewPrefabPath = "UI/Menus/Dialog/Impression/Impression",
                ignoreNotify = false,
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
            base.uiType = UIType.SuspendBar;
        }

        private void ActiveFullLevelEffect(bool active)
        {
            if (this._fullGoodFeelEffect == null)
            {
                GameObject original = Resources.Load<GameObject>("UI/Menus/Widget/Storage/Sakura");
                if (original != null)
                {
                    this._fullGoodFeelEffect = UnityEngine.Object.Instantiate<GameObject>(original);
                }
            }
            if (this._fullGoodFeelEffect != null)
            {
                this._fullGoodFeelEffect.SetActive(active);
            }
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Bottom/Button").GetComponent<Button>(), new UnityAction(this.OnAvatarReplaceButtonClick));
        }

        public void Close()
        {
            if (this._setup)
            {
                Singleton<GalTouchModule>.Instance.GalTouchInfoChanged -= new GalTouchModule.GalTouchInfoChangedHandler(this.OnGalTouchInfoChanged);
                Singleton<NotifyManager>.Instance.RemoveContext(this);
                this._setup = false;
            }
        }

        public override void Destroy()
        {
            this.Close();
            base.Destroy();
        }

        private void OnAvatarReplaceButtonClick()
        {
            if (this._galTouchView.shown)
            {
                base.view.SetActive(false);
                AvatarOverviewPageContext context = new AvatarOverviewPageContext {
                    type = AvatarOverviewPageContext.PageType.GalTouchReplace,
                    selectedAvatarID = Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.lastSelectedAvatarID
                };
                Singleton<MainUIManager>.Instance.ShowPage(context, UIType.Page);
            }
        }

        private void OnEnterMainPage()
        {
            this.ActiveFullLevelEffect(Singleton<GalTouchModule>.Instance.GetCharacterHeartLevel() >= 5);
        }

        private void OnExitMainPage()
        {
            this.ActiveFullLevelEffect(false);
        }

        public void OnGalTouchInfoChanged(int oldGoodFeel, int oldHeartLevel, int newGoodFeel, int newHeartLevel, GoodFeelLimitType limitType)
        {
            if (this._galTouchView != null)
            {
                this.UpdateHint(limitType);
                int num = GalTouchData.QueryLevelUpFeelNeed(oldHeartLevel);
                float sliderFrom = 0f;
                if (num != 0)
                {
                    sliderFrom = ((float) oldGoodFeel) / ((float) num);
                }
                sliderFrom += oldHeartLevel;
                int num3 = GalTouchData.QueryLevelUpFeelNeed(newHeartLevel);
                float sliderTo = 0f;
                if (num3 != 0)
                {
                    sliderTo = ((float) newGoodFeel) / ((float) num3);
                }
                sliderTo += newHeartLevel;
                string additionalText = string.Empty;
                int avatarGalTouchBuffId = Singleton<GalTouchModule>.Instance.GetAvatarGalTouchBuffId(Singleton<GalTouchModule>.Instance.GetCurrentTouchAvatarID());
                if (avatarGalTouchBuffId != 0)
                {
                    TouchBuffItem touchBuffItem = GalTouchData.GetTouchBuffItem(avatarGalTouchBuffId);
                    if (touchBuffItem != null)
                    {
                        object[] replaceParams = new object[] { GalTouchBuffData.GetCalculatedParam(touchBuffItem.param1, touchBuffItem.param1Add, newHeartLevel).ToString(), GalTouchBuffData.GetCalculatedParam(touchBuffItem.param2, touchBuffItem.param2Add, newHeartLevel).ToString(), GalTouchBuffData.GetCalculatedParam(touchBuffItem.param3, touchBuffItem.param3Add, newHeartLevel).ToString() };
                        additionalText = LocalizationGeneralLogic.GetText(touchBuffItem.detail, replaceParams);
                    }
                }
                this._galTouchView.Show(sliderFrom, sliderTo, newGoodFeel, additionalText);
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.EnterMainPage)
            {
                this.OnEnterMainPage();
            }
            if (ntf.type == NotifyTypes.ExitMainPage)
            {
                this.OnExitMainPage();
            }
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return false;
        }

        private void OnViewUpgrade()
        {
            this.ActiveFullLevelEffect(Singleton<GalTouchModule>.Instance.GetCharacterHeartLevel() >= 5);
        }

        protected override bool SetupView()
        {
            Singleton<GalTouchModule>.Instance.GalTouchInfoChanged += new GalTouchModule.GalTouchInfoChangedHandler(this.OnGalTouchInfoChanged);
            this._galTouchView = base.view.GetComponent<MonoGalTouchView>();
            if (this._galTouchView != null)
            {
                this._galTouchView.Upgrade += new Action(this.OnViewUpgrade);
            }
            Singleton<NotifyManager>.Instance.RegisterContext(this);
            this._setup = true;
            base.view.SetActive(false);
            return false;
        }

        private void UpdateHint(GoodFeelLimitType limitType)
        {
            if (limitType == GoodFeelLimitType.DialyGoodFeel)
            {
                this._galTouchView.SetHintContent(LocalizationGeneralLogic.GetText("Impression_Dialy_Limit", new object[0]));
                this._galTouchView.SetHintVisible(true);
            }
            else if (limitType == GoodFeelLimitType.Battle)
            {
                this._galTouchView.SetHintContent(LocalizationGeneralLogic.GetText("Impression_Battle_Limit", new object[0]));
                this._galTouchView.SetHintVisible(true);
            }
            else if (limitType == GoodFeelLimitType.Mission)
            {
                this._galTouchView.SetHintContent(LocalizationGeneralLogic.GetText("Impression_Mission_Limit", new object[0]));
                this._galTouchView.SetHintVisible(true);
            }
            else
            {
                this._galTouchView.SetHintVisible(false);
            }
        }
    }
}

