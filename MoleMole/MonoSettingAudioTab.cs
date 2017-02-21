namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoSettingAudioTab : MonoBehaviour
    {
        private ConfigAudioSetting _modifiedSettingConfig;
        public Transform[] BGMProcessPiecesGroups;
        public Transform[] BGMVolumeBtns;
        public Transform[] cvLanguageGroups;
        public string[] cvLanguageNames;
        public Transform[] soundEffectProcessPiecesGroups;
        public Transform[] soundEffectVolumeBtns;
        public Transform[] voiceProcessPiecesGroups;
        public Transform[] voiceVolumeBtns;

        public bool CheckNeedSave()
        {
            return !AudioSettingData.IsValueEqualToPersonalAudioConfig(this._modifiedSettingConfig);
        }

        public void OnBGMVolumeClick(int index)
        {
            Singleton<WwiseAudioManager>.Instance.SetParam("Vol_BGM", (float) index);
            this._modifiedSettingConfig.BGMVolume = index;
            this.SetVolumeBtns(this.BGMVolumeBtns, (int) this._modifiedSettingConfig.BGMVolume);
            this.SetProcessPieces(this.BGMProcessPiecesGroups, (int) this._modifiedSettingConfig.BGMVolume);
        }

        public void OnCVLanguageBtnClick(int index)
        {
            if (((index >= 0) && (index < this.cvLanguageNames.Length)) && (this._modifiedSettingConfig.CVLanguage != this.cvLanguageNames[index]))
            {
                this._modifiedSettingConfig.CVLanguage = this.cvLanguageNames[index];
                this.SetLanguageBtns();
            }
        }

        public void OnNoSaveBtnClick()
        {
            this.RecoverOriginState();
        }

        public void OnSaveBtnClick()
        {
            AudioSettingData.SavePersonalConfig(this._modifiedSettingConfig);
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_SettingSaveSuccess", new object[0]), 2f), UIType.Any);
        }

        public void OnSoundEffectVolumClick(int index)
        {
            Singleton<WwiseAudioManager>.Instance.SetParam("Vol_SE", (float) index);
            this._modifiedSettingConfig.SoundEffectVolume = index;
            this.SetVolumeBtns(this.soundEffectVolumeBtns, (int) this._modifiedSettingConfig.SoundEffectVolume);
            this.SetProcessPieces(this.soundEffectProcessPiecesGroups, (int) this._modifiedSettingConfig.SoundEffectVolume);
        }

        public void OnVoiceVolumClick(int index)
        {
            Singleton<WwiseAudioManager>.Instance.SetParam("Vol_Voice", (float) index);
            this._modifiedSettingConfig.VoiceVolume = index;
            this.SetVolumeBtns(this.voiceVolumeBtns, (int) this._modifiedSettingConfig.VoiceVolume);
            this.SetProcessPieces(this.voiceProcessPiecesGroups, (int) this._modifiedSettingConfig.VoiceVolume);
        }

        private void RecoverOriginState()
        {
            AudioSettingData.ApplySettingConfig();
            AudioSettingData.CopyPersonalAudioConfig(ref this._modifiedSettingConfig);
            this.SetVolumeBtns(this.BGMVolumeBtns, (int) this._modifiedSettingConfig.BGMVolume);
            this.SetVolumeBtns(this.soundEffectVolumeBtns, (int) this._modifiedSettingConfig.SoundEffectVolume);
            this.SetVolumeBtns(this.voiceVolumeBtns, (int) this._modifiedSettingConfig.VoiceVolume);
            this.SetProcessPieces(this.BGMProcessPiecesGroups, (int) this._modifiedSettingConfig.BGMVolume);
            this.SetProcessPieces(this.soundEffectProcessPiecesGroups, (int) this._modifiedSettingConfig.SoundEffectVolume);
            this.SetProcessPieces(this.voiceProcessPiecesGroups, (int) this._modifiedSettingConfig.VoiceVolume);
            this.SetLanguageBtns();
        }

        private void SetLanguageBtns()
        {
            int index = 0;
            int length = this.cvLanguageGroups.Length;
            while (index < length)
            {
                if (index >= this.cvLanguageNames.Length)
                {
                    break;
                }
                Transform transform = this.cvLanguageGroups[index].FindChild("Check");
                Transform transform2 = this.cvLanguageGroups[index].FindChild("Blue");
                Transform transform3 = this.cvLanguageGroups[index].FindChild("Grey");
                if (transform != null)
                {
                    bool flag = this.cvLanguageNames[index] == this._modifiedSettingConfig.CVLanguage;
                    transform.gameObject.SetActive(flag);
                    transform2.gameObject.SetActive(flag);
                    transform3.gameObject.SetActive(!flag);
                }
                index++;
            }
        }

        private void SetProcessPieces(Transform[] processPiecesGroups, int volume)
        {
            List<Transform> list = new List<Transform>();
            for (int i = 0; i < volume; i++)
            {
                IEnumerator enumerator = processPiecesGroups[i].GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        list.Add(current);
                    }
                }
                finally
                {
                    IDisposable disposable = enumerator as IDisposable;
                    if (disposable == null)
                    {
                    }
                    disposable.Dispose();
                }
            }
            for (int j = 0; j < list.Count; j++)
            {
                Transform transform3 = list[j];
                transform3.GetComponent<Image>().color = Color.Lerp(MiscData.GetColor("AudioSettingPieceYellow"), MiscData.GetColor("Blue"), ((float) j) / ((float) list.Count));
            }
            for (int k = volume; k < processPiecesGroups.Length; k++)
            {
                IEnumerator enumerator2 = processPiecesGroups[k].GetEnumerator();
                try
                {
                    while (enumerator2.MoveNext())
                    {
                        Transform transform5 = (Transform) enumerator2.Current;
                        transform5.GetComponent<Image>().color = MiscData.GetColor("TextGrey");
                    }
                }
                finally
                {
                    IDisposable disposable2 = enumerator2 as IDisposable;
                    if (disposable2 == null)
                    {
                    }
                    disposable2.Dispose();
                }
            }
        }

        public void SetupView()
        {
            this._modifiedSettingConfig = new ConfigAudioSetting();
            this.RecoverOriginState();
        }

        private void SetVolumeBtns(Transform[] volumeBtns, int volume)
        {
            for (int i = 0; i < volume; i++)
            {
                Transform transform = volumeBtns[i];
                transform.GetComponent<Image>().color = MiscData.GetColor("Blue");
                transform.FindChild("Check").gameObject.SetActive(false);
                transform.FindChild("Text").gameObject.SetActive(true);
            }
            volumeBtns[volume].GetComponent<Image>().color = MiscData.GetColor("Blue");
            volumeBtns[volume].FindChild("Check").gameObject.SetActive(true);
            volumeBtns[volume].FindChild("Text").gameObject.SetActive(false);
            for (int j = volume + 1; j < volumeBtns.Length; j++)
            {
                Transform transform2 = volumeBtns[j];
                transform2.GetComponent<Image>().color = MiscData.GetColor("TextGrey");
                transform2.FindChild("Check").gameObject.SetActive(false);
                transform2.FindChild("Text").gameObject.SetActive(true);
            }
        }
    }
}

