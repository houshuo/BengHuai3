namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoSettingOtherTab : MonoBehaviour
    {
        private bool _enableRealTimeWeather;
        public Transform realTimeWeatherSettingBtn;

        public bool CheckNeedSave()
        {
            return (Singleton<MiHoYoGameData>.Instance.LocalData.EnableRealTimeWeather != this._enableRealTimeWeather);
        }

        private void OnDestroy()
        {
            this.realTimeWeatherSettingBtn = null;
        }

        public void OnNoSaveBtnClick()
        {
            this.RecoverOriginState();
        }

        public void OnRealTimeChoiseBtnClicked(bool enable)
        {
            this._enableRealTimeWeather = enable;
            this.UpdateView();
        }

        public void OnSaveBtnClick()
        {
            Singleton<MiHoYoGameData>.Instance.LocalData.EnableRealTimeWeather = this._enableRealTimeWeather;
            Singleton<MiHoYoGameData>.Instance.Save();
            Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_SettingSaveSuccess", new object[0]), 2f), UIType.Any);
        }

        private void RecoverOriginState()
        {
            this._enableRealTimeWeather = Singleton<MiHoYoGameData>.Instance.LocalData.EnableRealTimeWeather;
            this.UpdateView();
        }

        private void SetChoiceBtn(Transform btn, bool active)
        {
            if (btn != null)
            {
                Transform transform = btn.Find("Choice/On");
                Transform transform2 = btn.Find("Choice/Off");
                transform.gameObject.SetActive(active);
                transform2.gameObject.SetActive(!active);
            }
        }

        public void SetupView()
        {
            this.RecoverOriginState();
        }

        private void UpdateView()
        {
            this.SetChoiceBtn(this.realTimeWeatherSettingBtn, this._enableRealTimeWeather);
        }
    }
}

