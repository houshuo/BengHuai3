namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoApplicationBehaviour : MonoBehaviour
    {
        private ApplicationManager _applicationManager;
        private DateTime _timeEnterBG = TimeUtil.Now;

        public void Init(ApplicationManager applicationManager)
        {
            this._applicationManager = applicationManager;
        }

        private void OnApplicationPause(bool paused)
        {
            if (Singleton<LevelManager>.Instance != null)
            {
                Singleton<LevelManager>.Instance.SetPause(paused);
            }
            if (paused)
            {
                this._timeEnterBG = TimeUtil.Now;
            }
            else
            {
                bool flag = false;
                if ((MiscData.Config != null) && MiscData.Config.BasicConfig.IsRestartWhenGameResume)
                {
                    TimeSpan span = (TimeSpan) (TimeUtil.Now - this._timeEnterBG);
                    if (((span.TotalSeconds > MiscData.Config.BasicConfig.RestartGameTimeSpanSeconds) && (Singleton<NetworkManager>.Instance != null)) && Singleton<NetworkManager>.Instance.alreadyLogin)
                    {
                        flag = true;
                        GeneralLogicManager.RestartGame();
                    }
                }
                if (!flag && (Singleton<PlayerModule>.Instance != null))
                {
                    Singleton<ApplicationManager>.Instance.DetectCheat();
                }
                if (!flag)
                {
                    Singleton<AccountManager>.Instance.manager.ShowPausePage();
                    Singleton<AccountManager>.Instance.manager.ShowToolBar();
                }
            }
        }

        private void Update()
        {
            GraphicsUtils.RebindAllRenderTexturesToCamera();
            if (Singleton<QAManager>.Instance != null)
            {
                Singleton<QAManager>.Instance.UpdateSendMessageToSever();
            }
            this._applicationManager.ManualCoroutinesMoveNext();
            this._applicationManager.ClearFinishCoroutines();
        }
    }
}

