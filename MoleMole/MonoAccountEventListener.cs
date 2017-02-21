namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoAccountEventListener : MonoBehaviour
    {
        public void Awake()
        {
            UnityEngine.Object.DontDestroyOnLoad(this);
        }

        public void BindTestFinishedCallBack(string param)
        {
            Singleton<AccountManager>.Instance.manager.BindTestFinishedCallBack(param);
        }

        public void DebugLog(string str)
        {
            Debug.Log(str);
        }

        public void ExitCallBack(string param)
        {
            Application.Quit();
        }

        public void InitFinishedCallBack(string param)
        {
            Singleton<AccountManager>.Instance.manager.InitFinishedCallBack(param);
        }

        public void LoginTestFinishedCallBack(string param)
        {
            Singleton<AccountManager>.Instance.manager.LoginTestFinishedCallBack(param);
        }

        public void PayFinishedCallBack(string param)
        {
            Singleton<AccountManager>.Instance.manager.PayFinishedCallBack(param);
        }

        public void ReloadCallBack(string param)
        {
            GeneralLogicManager.RestartGame();
        }

        public void SwitchAccountFinishedCallBack(string param)
        {
            Singleton<AccountManager>.Instance.manager.SwitchAccountFinishedCallBack(param);
        }
    }
}

