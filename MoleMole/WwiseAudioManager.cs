namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class WwiseAudioManager
    {
        private GameObject _customListenerObject;
        private GameObject _defaultPlayObject;
        private Vector3 _followingOffset = Vector3.zero;
        private Transform _followingTransform;
        private string _overridedLanguage;
        private List<string> manualPrepareBanks = new List<string>();
        private List<SoundBankScale> soundBankScaleStack = new List<SoundBankScale>();
        public bool useImplicitLoading;

        private WwiseAudioManager()
        {
        }

        public void ClearImplicitLoadedBanks()
        {
        }

        public void ClearManualPrepareBank()
        {
            int num = 0;
            int count = this.manualPrepareBanks.Count;
            while (num < count)
            {
                AkBankManager.UnloadBank(this.manualPrepareBanks[num]);
                num++;
            }
            this.manualPrepareBanks.Clear();
        }

        public void ClearUp()
        {
            if (this._defaultPlayObject != null)
            {
                UnityEngine.Object.Destroy(this._defaultPlayObject);
                this._defaultPlayObject = null;
            }
            int num = 0;
            int count = this.soundBankScaleStack.Count;
            while (num < count)
            {
                SoundBankScale scale = this.soundBankScaleStack[num];
                int index = 0;
                int length = scale.soundBankNames.Length;
                while (index < length)
                {
                    AkBankManager.UnloadBank(scale.soundBankNames[index]);
                    index++;
                }
                num++;
            }
            this.soundBankScaleStack.Clear();
            this.ClearManualPrepareBank();
        }

        public void Core()
        {
            if ((this._followingTransform != null) && (this._customListenerObject != null))
            {
                this._customListenerObject.transform.position = this._followingTransform.position + this._followingOffset;
                this._customListenerObject.transform.rotation = Camera.main.transform.rotation;
            }
        }

        public void Destroy()
        {
            if (this._defaultPlayObject != null)
            {
                UnityEngine.Object.Destroy(this._defaultPlayObject);
            }
        }

        public Transform GetFollowingTransform()
        {
            return this._followingTransform;
        }

        public string GetLanguage()
        {
            return ((this._overridedLanguage == null) ? AkInitializer.GetCurrentLanguage() : this._overridedLanguage);
        }

        public void InitAtAwake()
        {
        }

        public bool IsBankLoaded(string name)
        {
            int num = 0;
            int count = this.soundBankScaleStack.Count;
            while (num < count)
            {
                int index = 0;
                SoundBankScale scale = this.soundBankScaleStack[num];
                int length = scale.soundBankNames.Length;
                while (index < length)
                {
                    SoundBankScale scale2 = this.soundBankScaleStack[num];
                    if (name == scale2.soundBankNames[index])
                    {
                        return true;
                    }
                    index++;
                }
                num++;
            }
            int num5 = 0;
            int num6 = this.manualPrepareBanks.Count;
            while (num5 < num6)
            {
                if (name == this.manualPrepareBanks[num5])
                {
                    return true;
                }
                num5++;
            }
            return false;
        }

        private bool LoadBankImplicit(string name)
        {
            return false;
        }

        public void ManualPrepareBank(string name)
        {
            if (!this.IsBankLoaded(name))
            {
                AkBankManager.LoadBank(name);
                this.manualPrepareBanks.Add(name);
            }
        }

        public void ManualUnloadBank(string name)
        {
            int index = 0;
            int count = this.manualPrepareBanks.Count;
            while (index < count)
            {
                if (name == this.manualPrepareBanks[index])
                {
                    AkBankManager.UnloadBank(name);
                    this.manualPrepareBanks.RemoveAt(index);
                    return;
                }
                index++;
            }
        }

        public void PopSoundBankScale()
        {
            if (this.soundBankScaleStack.Count != 0)
            {
                SoundBankScale scale = this.soundBankScaleStack[0];
                int index = 0;
                int length = scale.soundBankNames.Length;
                while (index < length)
                {
                    AkBankManager.UnloadBank(scale.soundBankNames[index]);
                    index++;
                }
                this.soundBankScaleStack.RemoveAt(0);
            }
        }

        public void Post(string evtName, GameObject gameObj = null, AkCallbackManager.EventCallback endCallback = null, object cookie = null)
        {
            GameObject obj2 = (gameObj != null) ? gameObj : this.defaultPlayObject;
            uint num = (endCallback != null) ? AkSoundEngine.PostEvent(evtName, obj2, 1, endCallback, cookie) : AkSoundEngine.PostEvent(evtName, obj2);
            if ((num == 0) && this.useImplicitLoading)
            {
                if (this.PrepareEvent(evtName))
                {
                    num = (endCallback != null) ? AkSoundEngine.PostEvent(evtName, obj2, 1, endCallback, cookie) : AkSoundEngine.PostEvent(evtName, obj2);
                }
                if (num == 0)
                {
                }
            }
        }

        private bool PrepareEvent(string name)
        {
            int index = name.IndexOf("_");
            if (index == -1)
            {
                return false;
            }
            string str = name.Substring(0, index);
            if (!this.LoadBankImplicit(str))
            {
                return false;
            }
            return true;
        }

        public void PushSoundBankScale(string[] soundBankNames)
        {
            if (soundBankNames != null)
            {
                SoundBankScale item = new SoundBankScale {
                    soundBankNames = soundBankNames
                };
                int index = 0;
                int length = soundBankNames.Length;
                while (index < length)
                {
                    AkBankManager.LoadBank(soundBankNames[index]);
                    index++;
                }
                this.soundBankScaleStack.Insert(0, item);
            }
        }

        public void ReloadBanks()
        {
            uint num;
            int num2 = 0;
            int count = this.soundBankScaleStack.Count;
            while (num2 < count)
            {
                SoundBankScale scale = this.soundBankScaleStack[num2];
                int index = 0;
                int length = scale.soundBankNames.Length;
                while (index < length)
                {
                    AkSoundEngine.UnloadBank(scale.soundBankNames[index], IntPtr.Zero, null, null);
                    AkSoundEngine.LoadBank(scale.soundBankNames[index], -1, out num);
                    index++;
                }
                num2++;
            }
            int num6 = 0;
            int num7 = this.manualPrepareBanks.Count;
            while (num6 < num7)
            {
                AkSoundEngine.UnloadBank(this.manualPrepareBanks[num6], IntPtr.Zero, null, null);
                AkSoundEngine.LoadBank(this.manualPrepareBanks[num6], -1, out num);
                num6++;
            }
        }

        public void ResetListener()
        {
            if (this._customListenerObject != null)
            {
                this._followingTransform = null;
                this._followingOffset = Vector3.zero;
                UnityEngine.Object.Destroy(this._customListenerObject);
                this._customListenerObject = null;
                Camera.main.gameObject.AddComponent<AkAudioListener>();
            }
        }

        public void SetLanguage(string lang)
        {
            string language = this.GetLanguage();
            if (((language != null) && (language != lang)) && (AkSoundEngine.SetCurrentLanguage(lang) == AKRESULT.AK_Success))
            {
                this._overridedLanguage = lang;
                this.ReloadBanks();
            }
        }

        public void SetListenerFollowing(Transform trans, Vector3 offset)
        {
            if (this._customListenerObject == null)
            {
                this._customListenerObject = new GameObject("_WwiseListener");
                this._customListenerObject.AddComponent<AkAudioListener>();
                AkAudioListener component = Camera.main.GetComponent<AkAudioListener>();
                if (component != null)
                {
                    UnityEngine.Object.Destroy(component);
                }
            }
            this._followingTransform = trans;
            this._followingOffset = offset;
        }

        public void SetParam(string paramName, float value)
        {
            AkSoundEngine.SetRTPCValue(paramName, value);
        }

        public void SetState(string machineName, string stateName)
        {
            AkSoundEngine.SetState(machineName, stateName);
        }

        public void SetSwitch(string switchGroupName, string switchName)
        {
            AkSoundEngine.SetSwitch(switchGroupName, switchName, this.defaultPlayObject);
        }

        public void SetSwitch(string switchGroupName, string switchName, GameObject obj)
        {
            AkSoundEngine.SetSwitch(switchGroupName, switchName, obj);
        }

        public void StopAll()
        {
            AkSoundEngine.StopAll();
        }

        public void StopAll(GameObject gameObject)
        {
            AkSoundEngine.StopAll(gameObject);
        }

        private GameObject defaultPlayObject
        {
            get
            {
                if (this._defaultPlayObject == null)
                {
                    this._defaultPlayObject = new GameObject("_WwiseDefaultPlayer");
                    UnityEngine.Object.DontDestroyOnLoad(this._defaultPlayObject);
                    this._defaultPlayObject.AddComponent<BoxCollider>();
                }
                return this._defaultPlayObject;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SoundBankScale
        {
            public string[] soundBankNames;
        }
    }
}

