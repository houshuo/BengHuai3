namespace MoleMole
{
    using System;

    public class MainMenuBGM
    {
        private string _currentWeatherName = "Default";
        private bool _mainMenuEntered;

        public void InitAtAwakes()
        {
        }

        public void SetBGMSwitchByStage(bool isLast)
        {
            Singleton<WwiseAudioManager>.Instance.SetSwitch("Hyperion_Bridge_Weather", !isLast ? "Default" : "Lightning");
        }

        public void TryEnterMainMenu()
        {
            if (!this._mainMenuEntered)
            {
                this._mainMenuEntered = true;
                string[] soundBankNames = new string[] { "BK_MainMenu", "BK_Mei_C1", "BK_Bronya_Common" };
                Singleton<WwiseAudioManager>.Instance.PushSoundBankScale(soundBankNames);
                Singleton<WwiseAudioManager>.Instance.Post("BGM_MainMenu", null, null, null);
            }
        }

        public void TryExitMainMenu()
        {
            if (this._mainMenuEntered)
            {
                Singleton<WwiseAudioManager>.Instance.Post("BGM_MainMenu_Stop", null, null, null);
                Singleton<WwiseAudioManager>.Instance.PopSoundBankScale();
                this._mainMenuEntered = false;
            }
        }

        public void UpdateBGMByWeather(string weatherName)
        {
            if (this._currentWeatherName != weatherName)
            {
                if (!string.IsNullOrEmpty(weatherName))
                {
                    if (weatherName == "Lightning")
                    {
                        Singleton<WwiseAudioManager>.Instance.SetSwitch("Hyperion_Bridge_Weather", "Lightning");
                    }
                    else if (weatherName == "Cloudy")
                    {
                        Singleton<WwiseAudioManager>.Instance.SetSwitch("Hyperion_Bridge_Weather", "Cloudy");
                    }
                    else
                    {
                        Singleton<WwiseAudioManager>.Instance.SetSwitch("Hyperion_Bridge_Weather", "Default");
                    }
                }
                this._currentWeatherName = weatherName;
                if (this._mainMenuEntered)
                {
                    Singleton<WwiseAudioManager>.Instance.Post("BGM_MainMenu_Stop", null, null, null);
                    Singleton<WwiseAudioManager>.Instance.Post("BGM_MainMenu", null, null, null);
                }
            }
        }
    }
}

