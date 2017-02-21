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
    using UnityEngine.SceneManagement;

    public class DownloadPageContext : BasePageContext
    {
        private bool _destroyUntilNotify;
        private List<GameObject> _sceneGameObjects;
        private const int STEP = 2;

        public DownloadPageContext(bool destroyUntilNotify = false)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "DownLoadPageContext",
                viewPrefabPath = "UI/Menus/Page/Loading/DownloadPage"
            };
            base.config = pattern;
            this._destroyUntilNotify = destroyUntilNotify;
            this._sceneGameObjects = new List<GameObject>();
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
            Singleton<NotifyManager>.Instance.RemoveContext(this);
        }

        [DebuggerHidden]
        public IEnumerator LoadLevelWithProgress()
        {
            return new <LoadLevelWithProgress>c__Iterator67 { <>f__this = this };
        }

        public override bool OnNotify(Notify ntf)
        {
            if ((ntf.type == NotifyTypes.DestroyLoadingScene) && this._destroyUntilNotify)
            {
                this.DestroyLoadingScene();
            }
            return false;
        }

        private void SetLoadingPercentage(int progress)
        {
            progress = Mathf.Clamp(progress, 0, 100);
            base.view.transform.Find("ProgressPanel/Num").GetComponent<Text>().text = progress + "%";
            base.view.transform.Find("ProgressPanel/ProgressBar").GetComponent<MonoMaskSlider>().UpdateValue((float) progress, 100f, 0f);
        }

        private void SetupTips()
        {
        }

        protected override bool SetupView()
        {
            this.SetupTips();
            Singleton<ApplicationManager>.Instance.StartCoroutine(this.LoadLevelWithProgress());
            return false;
        }

        [CompilerGenerated]
        private sealed class <LoadLevelWithProgress>c__Iterator67 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal DownloadPageContext <>f__this;
            internal int <displayProgress>__1;
            internal string <nextScene>__0;
            internal AsyncOperation <op>__3;
            internal float <targetProcess>__4;
            internal bool <targetProcessReach>__5;
            internal int <toProgress>__2;

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
                        this.<nextScene>__0 = Singleton<MainUIManager>.Instance.SceneAfterLoading;
                        Singleton<MainUIManager>.Instance.ResetOnMoveToNextScene(this.<nextScene>__0);
                        this.<displayProgress>__1 = 0;
                        this.<toProgress>__2 = 0;
                        this.<op>__3 = SceneManager.LoadSceneAsync(this.<nextScene>__0, LoadSceneMode.Additive);
                        this.<op>__3.allowSceneActivation = false;
                        this.<targetProcess>__4 = 0.6f;
                        this.<targetProcessReach>__5 = false;
                        goto Label_011F;

                    case 1:
                        break;

                    case 2:
                        goto Label_016F;

                    case 3:
                        this.$PC = -1;
                        goto Label_01DA;

                    default:
                        goto Label_01DA;
                }
            Label_010E:
                while (this.<displayProgress>__1 < this.<toProgress>__2)
                {
                    this.<displayProgress>__1 += 2;
                    this.<>f__this.SetLoadingPercentage(this.<displayProgress>__1);
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 1;
                    goto Label_01DC;
                }
            Label_011F:
                if (this.<op>__3.progress < 0.9f)
                {
                    this.<toProgress>__2 = ((int) this.<op>__3.progress) * 100;
                    if (!this.<targetProcessReach>__5 && (this.<op>__3.progress >= this.<targetProcess>__4))
                    {
                        this.<targetProcessReach>__5 = true;
                        Singleton<MainMenuBGM>.Instance.TryExitMainMenu();
                    }
                    goto Label_010E;
                }
            Label_016F:
                while (this.<displayProgress>__1 < this.<toProgress>__2)
                {
                    this.<displayProgress>__1 += 2;
                    this.<>f__this.SetLoadingPercentage(this.<displayProgress>__1);
                    this.$current = new WaitForEndOfFrame();
                    this.$PC = 2;
                    goto Label_01DC;
                }
                this.<op>__3.allowSceneActivation = true;
                if (this.<>f__this._destroyUntilNotify)
                {
                    Singleton<NotifyManager>.Instance.RegisterContext(this.<>f__this);
                }
                else
                {
                    this.<>f__this.DestroyLoadingScene();
                }
                this.$current = Resources.UnloadUnusedAssets();
                this.$PC = 3;
                goto Label_01DC;
            Label_01DA:
                return false;
            Label_01DC:
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

