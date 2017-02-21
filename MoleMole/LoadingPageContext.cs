namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class LoadingPageContext : BasePageContext
    {
        private bool _destroyUntilNotify;
        private List<GameObject> _sceneGameObjects;

        public LoadingPageContext(bool destroyUntilNotify = false)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "LoadingPageContext",
                viewPrefabPath = "UI/Menus/Page/Loading/LoadingPage"
            };
            base.config = pattern;
            this._destroyUntilNotify = destroyUntilNotify;
            this._sceneGameObjects = new List<GameObject>();
            base.uiType = UIType.Page;
            BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
            this._sceneGameObjects.Add(sceneCanvas.gameObject);
            this._sceneGameObjects.Add(sceneCanvas.GetComponent<Canvas>().worldCamera.gameObject);
        }

        protected override void BindViewCallbacks()
        {
        }

        private void DestroyLoadingScene()
        {
            foreach (GameObject obj2 in this._sceneGameObjects)
            {
                UnityEngine.Object.Destroy(obj2);
            }
            UnityEngine.Object.Destroy(base.view);
            Singleton<NotifyManager>.Instance.RemoveContext(this);
            Resources.UnloadUnusedAssets();
        }

        private string GetRandomLoadingTip()
        {
            int max = 100;
            int num2 = 0;
            string text = string.Empty;
            do
            {
                num2 = UnityEngine.Random.Range(1, max);
                text = LocalizationGeneralLogic.GetText("LoadingTips_" + num2.ToString("D2"), new object[0]);
                max = num2;
            }
            while ((max > 1) && string.IsNullOrEmpty(text));
            return text;
        }

        public void LoadLevelWithProgress()
        {
            string sceneAfterLoading = Singleton<MainUIManager>.Instance.SceneAfterLoading;
            Singleton<MainUIManager>.Instance.ResetOnMoveToNextScene(sceneAfterLoading);
            SceneManager.LoadScene(sceneAfterLoading, LoadSceneMode.Additive);
            if (Singleton<MainUIManager>.Instance.SceneAfterLoading == "TestLevel01")
            {
                Singleton<MainMenuBGM>.Instance.TryExitMainMenu();
            }
            if (this._destroyUntilNotify)
            {
                Singleton<NotifyManager>.Instance.RegisterContext(this);
            }
            else
            {
                this.DestroyLoadingScene();
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            if ((ntf.type == NotifyTypes.DestroyLoadingScene) && this._destroyUntilNotify)
            {
                this.DestroyLoadingScene();
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.LoadingSceneDestroyed, null));
            }
            return false;
        }

        private void SetupTips()
        {
            if (Singleton<MainUIManager>.Instance.bShowLoadingTips)
            {
                base.view.transform.Find("Tips/Content").GetComponent<Text>().text = this.GetRandomLoadingTip();
            }
            else
            {
                base.view.transform.Find("Tips").gameObject.SetActive(false);
            }
        }

        protected override bool SetupView()
        {
            this.SetupTips();
            this.LoadLevelWithProgress();
            return false;
        }
    }
}

