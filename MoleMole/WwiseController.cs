namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class WwiseController : MonoBehaviour
    {
        private string _eventName;
        private string _stateMachineName;
        private string _switchGroupName;
        private string _switchName;
        public Text eventNameText;
        public Text paramNameText;
        public Text stateGroupNameText;
        public Text stateNameText;
        public Text switchGroupNameText;
        public Text switchNameText;

        private void Awake()
        {
            Singleton<WwiseAudioManager>.Create();
            Singleton<WwiseAudioManager>.Instance.InitAtAwake();
            string[] soundBankNames = new string[] { "MainMenuBank", "Avatar_Generic_Bank" };
            Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(soundBankNames);
        }

        public void OnPlayButton()
        {
            Singleton<WwiseAudioManager>.Instance.Post(this.eventNameText.text, null, null, null);
        }

        public void OnSetStateButton()
        {
            Singleton<WwiseAudioManager>.Instance.SetState(this.stateGroupNameText.text, this.stateNameText.text);
        }

        public void OnSlideValueChanged(float val)
        {
            Singleton<WwiseAudioManager>.Instance.SetParam(this.paramNameText.text, val);
        }

        public void OnSwitchButton()
        {
            Singleton<WwiseAudioManager>.Instance.SetSwitch(this.switchGroupNameText.text, this.switchNameText.text);
        }

        public void OnUnloadBank()
        {
        }
    }
}

