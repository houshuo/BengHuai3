namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public abstract class BaseMonoCanvas : MonoBehaviour
    {
        private List<CanvasTimer> _canvasTimers = new List<CanvasTimer>();
        private GMTalkDialogContext _GMTalkDialogContext;
        private List<CanvasTimer> _newTimersDuringUpdate = new List<CanvasTimer>();
        private GeneralConfirmDialogContext _quitGameDialogContext;
        [CompilerGenerated]
        private static Predicate<CanvasTimer> <>f__am$cache4;
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache5;

        protected BaseMonoCanvas()
        {
        }

        public virtual void ClearAllWidgetContext()
        {
        }

        public CanvasTimer CreateInfiniteTimer(float m_triggerCD = 0)
        {
            CanvasTimer item = new CanvasTimer {
                infiniteTimeSpan = true,
                triggerCD = m_triggerCD
            };
            this._newTimersDuringUpdate.Add(item);
            return item;
        }

        public CanvasTimer CreateTimer(float m_timespan, float m_triggerCD = 0)
        {
            CanvasTimer item = new CanvasTimer {
                timespan = m_timespan,
                triggerCD = m_triggerCD
            };
            this._newTimersDuringUpdate.Add(item);
            return item;
        }

        public virtual GameObject GetSpaceShipObj()
        {
            return null;
        }

        public virtual void OnDestroy()
        {
        }

        public virtual void PlayVideo(CgDataItem cgDataItem)
        {
        }

        private void RemoveAllTimeUpTimer()
        {
            if (<>f__am$cache4 == null)
            {
                <>f__am$cache4 = x => x.IsTimeUp;
            }
            this._canvasTimers.RemoveAll(<>f__am$cache4);
        }

        public void ShowQuitGameDialog()
        {
            if (this._quitGameDialogContext == null)
            {
                GeneralConfirmDialogContext context = new GeneralConfirmDialogContext {
                    type = GeneralConfirmDialogContext.ButtonType.DoubleButton,
                    desc = LocalizationGeneralLogic.GetText("Menu_Desc_EscQuitGame", new object[0])
                };
                if (<>f__am$cache5 == null)
                {
                    <>f__am$cache5 = delegate (bool confirmed) {
                        if (confirmed)
                        {
                            Singleton<AccountManager>.Instance.manager.DoExit();
                        }
                        else
                        {
                            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnQuitGameDialogDestroy, null));
                        }
                    };
                }
                context.buttonCallBack = <>f__am$cache5;
                this._quitGameDialogContext = context;
            }
            if (this._quitGameDialogContext.view == null)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(this._quitGameDialogContext, UIType.Any);
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnQuitGameDialogShow, null));
            }
        }

        public virtual void Start()
        {
            if (GameObject.Find("EventSystem") != null)
            {
            }
            string sceneName = "Loading";
            if (this is MonoMainCanvas)
            {
                sceneName = "MainMenuWithSpaceship";
            }
            Action callBackWhenSceneLoaded = Singleton<MainUIManager>.Instance.GetCallBackWhenSceneLoaded(sceneName);
            if (callBackWhenSceneLoaded != null)
            {
                callBackWhenSceneLoaded();
            }
            Singleton<MainUIManager>.Instance.ResetSceneLoadedCallBack(sceneName);
        }

        public virtual void Update()
        {
            this.RemoveAllTimeUpTimer();
            this._canvasTimers.AddRange(this._newTimersDuringUpdate);
            this._newTimersDuringUpdate.Clear();
            for (int i = 0; i < this._canvasTimers.Count; i++)
            {
                this._canvasTimers[i].Core();
            }
            if (GlobalVars.DEBUG_FEATURE_ON)
            {
                this.UpdateForGMTalk();
            }
            this.UpdateEscapeListener();
        }

        private void UpdateEscapeListener()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Singleton<AccountManager>.Instance.manager.ShowExitUI();
            }
        }

        private void UpdateForGMTalk()
        {
            if (Input.touchCount == 4)
            {
                if (this._GMTalkDialogContext == null)
                {
                    this._GMTalkDialogContext = new GMTalkDialogContext();
                }
                if (this._GMTalkDialogContext.view == null)
                {
                    Singleton<MainUIManager>.Instance.ShowDialog(this._GMTalkDialogContext, UIType.Any);
                }
            }
        }
    }
}

