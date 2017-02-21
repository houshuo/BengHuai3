namespace MoleMole
{
    using MoleMole.MainMenu;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoCloundDebug : MonoBehaviour
    {
        private MainMenuStage __stage;
        private GameObject _cameraObj;
        private GameObject _cloudObj;
        private int _currentKey = -1;
        [CompilerGenerated]
        private static Action<WeatherInfo> <>f__am$cache14;
        public Toggle cameraAutoToggle;
        public Toggle cameraToggle;
        public Toggle cloudAutoToggle;
        public Text CloudNameText;
        public Toggle cloudToggle;
        public Toggle FarPrefabVisibleToggle;
        private float period = 0.5f;
        public Text ProfileNameText;
        public Toggle spaceShipAutoToggle;
        public Toggle spaceShipToggle;
        public Toggle StaticModeToggle;
        private float timer;
        public Toggle TimeSettingEnableToggle;
        public Slider TimeSlider;
        public Text TimeText;
        public Text WeatherText;

        public void OnBenchmarkToggle(bool isOn)
        {
            MonoBenchmarkSwitches switches = UnityEngine.Object.FindObjectOfType<MonoBenchmarkSwitches>();
            if (switches == null)
            {
                if (isOn)
                {
                    System.Type[] components = new System.Type[] { typeof(MonoBenchmarkSwitches) };
                    new GameObject("__benchmark", components);
                }
            }
            else
            {
                switches.gameObject.SetActive(isOn);
            }
        }

        public void OnCameraToggle()
        {
            this._cameraObj.SetActive(this.cameraToggle.isOn);
        }

        public void OnChangeCloud()
        {
            this._stage.ChooseCloudSceneNext();
        }

        public void OnChangeProfile()
        {
            this._stage.ChooseAtmosphereSeriesNext();
            this._currentKey = -1;
        }

        public void OnCloudToggle()
        {
            this._cloudObj.SetActive(this.cloudToggle.isOn);
        }

        public void OnCloundBtnDebugBtnClick()
        {
            base.transform.gameObject.SetActive(!base.transform.gameObject.activeSelf);
            if (base.transform.gameObject.activeSelf)
            {
                this.SetProfileName();
                this.CloudNameText.text = this._stage.CloudSceneName;
                this.TimeText.text = this.TimeSlider.value.ToString();
            }
        }

        public void OnEnableToggle()
        {
            this._stage.UpdateAtmosphereWithTransition = !this.TimeSettingEnableToggle.isOn;
            this._stage.ForceUpdateAtmosphere = true;
        }

        public void OnFarPrefabVisibleToggle()
        {
            foreach (Transform transform in this._stage.GetComponentsInChildren<Transform>(true))
            {
                if (transform.name == "Warship")
                {
                    transform.gameObject.SetActive(this.FarPrefabVisibleToggle.isOn);
                }
            }
        }

        public void OnManualUpdateWeatherButtonClicked()
        {
            if (<>f__am$cache14 == null)
            {
                <>f__am$cache14 = info => UIUtil.SpaceshipCheckWeather();
            }
            Singleton<RealTimeWeatherManager>.Instance.QueryWeatherInfo(<>f__am$cache14);
        }

        public void OnNextButtonClick()
        {
            if (this._currentKey != -1)
            {
                this._currentKey++;
            }
            else
            {
                this._currentKey = this._stage.AtmosphereConfigSeries.KeyBeforeTime(this.TimeSlider.value);
            }
            if (this._currentKey == this._stage.AtmosphereConfigSeries.KeyCount())
            {
                this._currentKey = 0;
            }
            this.TimeSlider.value = this._stage.AtmosphereConfigSeries.TimeAtKey(this._currentKey);
            this._stage.UpdateAtmosphereWithTransition = true;
        }

        public void OnPrevButtonClick()
        {
            if (this._currentKey != -1)
            {
                this._currentKey--;
            }
            else
            {
                this._currentKey = this._stage.AtmosphereConfigSeries.KeyBeforeTime(this.TimeSlider.value);
            }
            if (this._currentKey == -1)
            {
                this._currentKey = this._stage.AtmosphereConfigSeries.KeyCount() - 1;
            }
            this.TimeSlider.value = this._stage.AtmosphereConfigSeries.TimeAtKey(this._currentKey);
            this._stage.UpdateAtmosphereWithTransition = true;
        }

        public void OnSpaceShipToggle()
        {
            this._stage.gameObject.SetActive(this.spaceShipToggle.isOn);
        }

        public void OnStaticModeToggle()
        {
            GlobalVars.STATIC_CLOUD_MODE = this.StaticModeToggle.isOn;
        }

        public void OnTimeSliderDrag()
        {
            this._stage.UpdateAtmosphereWithTransition = false;
            this._stage.IsInTransition = false;
            this.TimeText.text = this.TimeSlider.value.ToString();
        }

        private void SetProfileName()
        {
            string atmosphereConfigSeriesPath = this._stage.AtmosphereConfigSeriesPath;
            this.ProfileNameText.text = atmosphereConfigSeriesPath.Substring(atmosphereConfigSeriesPath.LastIndexOf('/') + 1);
        }

        public void Update()
        {
            if (this.TimeSettingEnableToggle.isOn)
            {
                this._stage.DayTime = this.TimeSlider.value;
            }
            else
            {
                this._stage.DayTime = -1f;
            }
            this.CloudNameText.text = this._stage.CloudSceneName;
            this.SetProfileName();
            if (this._cameraObj == null)
            {
                this._cameraObj = UnityEngine.Object.FindObjectOfType<MainMenuCamera>().gameObject;
            }
            if (this._cloudObj == null)
            {
                CloudEmitter emitter = UnityEngine.Object.FindObjectOfType<CloudEmitter>();
                this._cloudObj = (emitter == null) ? null : emitter.gameObject;
            }
            this.timer += Time.deltaTime;
            if (this.timer >= this.period)
            {
                this.timer = 0f;
                if (this.cameraAutoToggle.isOn && (this._cameraObj != null))
                {
                    this._cameraObj.SetActive(!this._cameraObj.activeSelf);
                }
                if (this.cloudAutoToggle.isOn && (this._cloudObj != null))
                {
                    this._cloudObj.SetActive(!this._cloudObj.activeSelf);
                }
                if (this.spaceShipAutoToggle.isOn && (this._stage != null))
                {
                    this._stage.gameObject.SetActive(!this._stage.gameObject.activeSelf);
                }
            }
            WeatherInfo weatherInfo = Singleton<RealTimeWeatherManager>.Instance.GetWeatherInfo();
            if ((this.WeatherText != null) && (weatherInfo != null))
            {
                this.WeatherText.text = weatherInfo.ToString();
            }
        }

        private MainMenuStage _stage
        {
            get
            {
                if (this.__stage == null)
                {
                    this.__stage = UnityEngine.Object.FindObjectOfType<MainMenuStage>();
                }
                return this.__stage;
            }
        }
    }
}

