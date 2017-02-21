namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class SettingPageContext : BasePageContext
    {
        private MonoSettingGraphicsTab _graphicSetting;
        private TabManager _tabManager;
        public const string AudioTab = "AudioTab";
        public readonly string defaultTab;
        public const string GraphicsTab = "GraphicsTab";
        public const string ImageTab = "ImageTab";
        public const string OtherTab = "OtherTab";
        public const string PushTab = "PushTab";

        public SettingPageContext(string defaultTab = "AudioTab")
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "SettingPageContext",
                viewPrefabPath = "UI/Menus/Page/Setting/SettingPage"
            };
            base.config = pattern;
            base.showSpaceShip = true;
            this.defaultTab = defaultTab;
            this._tabManager = new TabManager();
            this._tabManager.onSetActive += new TabManager.OnSetActive(this.OnTabSetActive);
        }

        public override void BackPage()
        {
            this.DoLevelCurrentPageOrTab(new Action(this.BackPage));
        }

        public override void BackToMainMenuPage()
        {
            this.DoLevelCurrentPageOrTab(new Action(this.BackToMainMenuPage));
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("TabBtns/AudioTabBtn").GetComponent<Button>(), new UnityAction(this.OnAudioTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/GraphicsTabBtn").GetComponent<Button>(), new UnityAction(this.OnGraphicsTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/ImageTabBtn").GetComponent<Button>(), new UnityAction(this.OnImageTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/PushTabBtn").GetComponent<Button>(), new UnityAction(this.OnPushTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("TabBtns/OtherTabBtn").GetComponent<Button>(), new UnityAction(this.OnOtherTabBtnClick));
            base.BindViewCallback(base.view.transform.Find("ECOMode/Choice/Button").GetComponent<Button>(), new UnityAction(this.OnEcoModeBtnClick));
        }

        public override void Destroy()
        {
            base.view.transform.Find("ImageTab").FindChild("Content/RT/3dModel").GetComponent<MonoGammaSettingRenderImage>().ReleaseRenderTexture();
            base.Destroy();
        }

        private void DoLevelCurrentPageOrTab(Action callback)
        {
            <DoLevelCurrentPageOrTab>c__AnonStorey101 storey = new <DoLevelCurrentPageOrTab>c__AnonStorey101 {
                callback = callback
            };
            string showingTabKey = this._tabManager.GetShowingTabKey();
            if (!string.IsNullOrEmpty(showingTabKey))
            {
                <DoLevelCurrentPageOrTab>c__AnonStorey100 storey2 = new <DoLevelCurrentPageOrTab>c__AnonStorey100 {
                    <>f__ref$257 = storey
                };
                GameObject gameObject = base.view.transform.Find(showingTabKey).gameObject;
                storey2.noSaveAction = null;
                storey2.saveAction = null;
                bool flag = false;
                switch (showingTabKey)
                {
                    case "AudioTab":
                        flag = gameObject.GetComponent<MonoSettingAudioTab>().CheckNeedSave();
                        storey2.noSaveAction = new Action(gameObject.GetComponent<MonoSettingAudioTab>().OnNoSaveBtnClick);
                        storey2.saveAction = new Action(gameObject.GetComponent<MonoSettingAudioTab>().OnSaveBtnClick);
                        break;

                    case "GraphicsTab":
                        flag = gameObject.GetComponent<MonoSettingGraphicsTab>().CheckNeedSave();
                        storey2.noSaveAction = new Action(gameObject.GetComponent<MonoSettingGraphicsTab>().OnNoSaveBtnClick);
                        storey2.saveAction = new Action(gameObject.GetComponent<MonoSettingGraphicsTab>().OnSaveBtnClick);
                        break;

                    case "ImageTab":
                        flag = gameObject.GetComponent<MonoSettingImageTab>().CheckNeedSave();
                        storey2.noSaveAction = new Action(gameObject.GetComponent<MonoSettingImageTab>().OnNoSaveBtnClick);
                        storey2.saveAction = new Action(gameObject.GetComponent<MonoSettingImageTab>().OnSaveBtnClick);
                        break;

                    case "PushTab":
                        flag = gameObject.GetComponent<MonoSettingPushTab>().CheckNeedSave();
                        storey2.noSaveAction = new Action(gameObject.GetComponent<MonoSettingPushTab>().OnNoSaveBtnClick);
                        storey2.saveAction = new Action(gameObject.GetComponent<MonoSettingPushTab>().OnSaveBtnClick);
                        break;

                    case "OtherTab":
                        flag = gameObject.GetComponent<MonoSettingOtherTab>().CheckNeedSave();
                        storey2.noSaveAction = new Action(gameObject.GetComponent<MonoSettingOtherTab>().OnNoSaveBtnClick);
                        storey2.saveAction = new Action(gameObject.GetComponent<MonoSettingOtherTab>().OnSaveBtnClick);
                        break;
                }
                if (flag)
                {
                    GeneralDialogContext dialogContext = new GeneralDialogContext {
                        type = GeneralDialogContext.ButtonType.DoubleButton,
                        title = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgTitle", new object[0]),
                        desc = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgDesc", new object[0]),
                        okBtnText = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgSave", new object[0]),
                        cancelBtnText = LocalizationGeneralLogic.GetText("Menu_SettingSaveDlgNoSave", new object[0]),
                        buttonCallBack = new Action<bool>(storey2.<>m__19D)
                    };
                    Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
                    return;
                }
            }
            storey.callback();
        }

        public void OnAudioTabBtnClick()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(false, false)));
            this.DoLevelCurrentPageOrTab(delegate {
                this._tabManager.ShowTab("AudioTab");
                base.view.transform.Find("AudioTab").GetComponent<MonoSettingAudioTab>().SetupView();
                if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
                {
                    base.view.GetComponent<MonoFadeInAnimManager>().Play("AudioTabFadeIn", false, null);
                }
            });
        }

        public void OnEcoModeBtnClick()
        {
            this._graphicSetting.SwitchEcoMode();
        }

        public void OnGraphicsTabBtnClick()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(false, true)));
            this.DoLevelCurrentPageOrTab(delegate {
                this._tabManager.ShowTab("GraphicsTab");
                this._graphicSetting.SetupView(false);
                if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
                {
                    base.view.GetComponent<MonoFadeInAnimManager>().Play("GraphicsTabFadeIn", false, null);
                }
            });
        }

        public void OnImageTabBtnClick()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(true, false)));
            this.DoLevelCurrentPageOrTab(delegate {
                this._tabManager.ShowTab("ImageTab");
                base.view.transform.Find("ImageTab").GetComponent<MonoSettingImageTab>().SetupView();
                if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
                {
                    base.view.GetComponent<MonoFadeInAnimManager>().Play("ImageTabFadeIn", false, null);
                }
            });
        }

        public void OnOtherTabBtnClick()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(false, false)));
            this.DoLevelCurrentPageOrTab(delegate {
                this._tabManager.ShowTab("OtherTab");
                base.view.transform.Find("OtherTab").GetComponent<MonoSettingOtherTab>().SetupView();
            });
        }

        public void OnPushTabBtnClick()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(false, false)));
            this.DoLevelCurrentPageOrTab(delegate {
                this._tabManager.ShowTab("PushTab");
                base.view.transform.Find("PushTab").GetComponent<MonoSettingPushTab>().SetupView();
            });
        }

        private void OnTabSetActive(bool active, GameObject go, Button btn)
        {
            btn.GetComponent<Image>().color = !active ? MiscData.GetColor("Blue") : Color.white;
            btn.transform.Find("Text").GetComponent<Text>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.transform.Find("Image").GetComponent<Image>().color = !active ? Color.white : MiscData.GetColor("Black");
            btn.interactable = !active;
            go.SetActive(active);
        }

        private void SetupAudioTab()
        {
            GameObject gameObject = base.view.transform.Find("AudioTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/AudioTabBtn").GetComponent<Button>();
            this._tabManager.SetTab("AudioTab", component, gameObject);
            gameObject.GetComponent<MonoSettingAudioTab>().SetupView();
        }

        private void SetupGraphicsTab()
        {
            GameObject gameObject = base.view.transform.Find("GraphicsTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/GraphicsTabBtn").GetComponent<Button>();
            this._tabManager.SetTab("GraphicsTab", component, gameObject);
            this._graphicSetting = gameObject.GetComponent<MonoSettingGraphicsTab>();
            this._graphicSetting.SetupView(false);
        }

        private void SetupImageTab()
        {
            GameObject gameObject = base.view.transform.Find("ImageTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/ImageTabBtn").GetComponent<Button>();
            this._tabManager.SetTab("ImageTab", component, gameObject);
            gameObject.GetComponent<MonoSettingImageTab>().SetupView();
        }

        public void SetupOtherTab()
        {
            GameObject gameObject = base.view.transform.Find("OtherTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/OtherTabBtn").GetComponent<Button>();
            this._tabManager.SetTab("OtherTab", component, gameObject);
            gameObject.GetComponent<MonoSettingOtherTab>().SetupView();
        }

        private void SetupPushTab()
        {
            GameObject gameObject = base.view.transform.Find("PushTab").gameObject;
            Button component = base.view.transform.Find("TabBtns/PushTabBtn").GetComponent<Button>();
            this._tabManager.SetTab("PushTab", component, gameObject);
            gameObject.GetComponent<MonoSettingPushTab>().SetupView();
        }

        protected override bool SetupView()
        {
            string showingTabKey = this._tabManager.GetShowingTabKey();
            string searchKey = !string.IsNullOrEmpty(showingTabKey) ? showingTabKey : this.defaultTab;
            this._tabManager.Clear();
            this.SetupAudioTab();
            this.SetupGraphicsTab();
            this.SetupImageTab();
            this.SetupPushTab();
            this.SetupOtherTab();
            if (searchKey == "ImageTab")
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(true, false)));
            }
            else if (searchKey == "GraphicsTab")
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(false, true)));
            }
            else
            {
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(false, false)));
            }
            this._tabManager.ShowTab(searchKey);
            if (base.view.GetComponent<MonoFadeInAnimManager>() != null)
            {
                base.view.GetComponent<MonoFadeInAnimManager>().Play("AudioTabFadeIn", false, null);
            }
            return false;
        }

        [CompilerGenerated]
        private sealed class <DoLevelCurrentPageOrTab>c__AnonStorey100
        {
            internal SettingPageContext.<DoLevelCurrentPageOrTab>c__AnonStorey101 <>f__ref$257;
            internal Action noSaveAction;
            internal Action saveAction;

            internal void <>m__19D(bool confirmed)
            {
                if (confirmed)
                {
                    if (this.saveAction != null)
                    {
                        this.saveAction();
                    }
                    this.<>f__ref$257.callback();
                }
                else
                {
                    if (this.noSaveAction != null)
                    {
                        this.noSaveAction();
                    }
                    this.<>f__ref$257.callback();
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DoLevelCurrentPageOrTab>c__AnonStorey101
        {
            internal Action callback;
        }
    }
}

