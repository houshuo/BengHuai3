namespace MoleMole
{
    using MoleMole.Config;
    using MoleMole.MainMenu;
    using System;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;

    public class SpaceShipModelContext : BaseWidgetContext
    {
        private const string THUNDER_WEATHER_CONFIG_PATH = "Rendering/MainMenuAtmosphereConfig/Lightning";
        private const int THUNDER_WEATHER_SCENE_ID = 0;
        public readonly GameObject uiMainCamera;

        public SpaceShipModelContext(GameObject view, GameObject uiMainCamera)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "SpaceShipModelContext",
                viewPrefabPath = "Stage/MainMenu_SpaceShip/MainMenu_SpaceShip"
            };
            base.config = pattern;
            base.view = view;
            this.uiMainCamera = uiMainCamera;
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.SetSpaceShipActive)
            {
                return this.SetSpaceShipActive(((Tuple<bool, bool>) ntf.body).Item1, ((Tuple<bool, bool>) ntf.body).Item2);
            }
            if (ntf.type == NotifyTypes.ShowThunderWeather)
            {
                return this.OnShowThunderWeatherNotify();
            }
            if (ntf.type == NotifyTypes.ShowRandomWeather)
            {
                return this.OnShowRandomWeatherNotify();
            }
            if (ntf.type == NotifyTypes.ShowDefaultWeather)
            {
                return this.OnShowDefaultWeatherNotify();
            }
            return ((ntf.type == NotifyTypes.SetSpaceShipLight) && this.SetSpaceShipLight((bool) ntf.body));
        }

        private bool OnShowDefaultWeatherNotify()
        {
            ConfigAtmosphereSeries config = ConfigAtmosphereSeries.LoadFromFileAndDetach(Singleton<MiHoYoGameData>.Instance.LocalData.CurrentWeatherConfigPath);
            int currentWeatherSceneID = Singleton<MiHoYoGameData>.Instance.LocalData.CurrentWeatherSceneID;
            base.view.transform.GetComponent<MainMenuStage>().ChooseCloudScene(config, currentWeatherSceneID);
            return false;
        }

        private bool OnShowRandomWeatherNotify()
        {
            string pathRandomly = AtmosphereSeriesData.GetPathRandomly();
            ConfigAtmosphereSeries config = ConfigAtmosphereSeries.LoadFromFileAndDetach(pathRandomly);
            int sceneIdRandomly = config.GetSceneIdRandomly();
            base.view.transform.GetComponent<MainMenuStage>().ChooseCloudScene(config, sceneIdRandomly);
            UserLocalDataItem localData = Singleton<MiHoYoGameData>.Instance.LocalData;
            localData.StartDirtyCheck();
            if ((localData.CurrentWeatherConfigPath != pathRandomly) || (localData.CurrentWeatherSceneID != sceneIdRandomly))
            {
                localData.SetDirty();
            }
            localData.CurrentWeatherConfigPath = pathRandomly;
            localData.CurrentWeatherSceneID = sceneIdRandomly;
            if (localData.EndDirtyCheck())
            {
                Singleton<MiHoYoGameData>.Instance.Save();
            }
            return false;
        }

        private bool OnShowThunderWeatherNotify()
        {
            MainMenuStage component = base.view.transform.GetComponent<MainMenuStage>();
            ConfigAtmosphereSeries config = ConfigAtmosphereSeries.LoadFromFileAndDetach("Rendering/MainMenuAtmosphereConfig/Lightning");
            component.ChooseCloudScene(config, 0);
            UserLocalDataItem localData = Singleton<MiHoYoGameData>.Instance.LocalData;
            localData.StartDirtyCheck();
            if ((localData.CurrentWeatherConfigPath != "Rendering/MainMenuAtmosphereConfig/Lightning") || (localData.CurrentWeatherSceneID != 0))
            {
                localData.SetDirty();
            }
            localData.CurrentWeatherConfigPath = "Rendering/MainMenuAtmosphereConfig/Lightning";
            localData.CurrentWeatherSceneID = 0;
            if (localData.EndDirtyCheck())
            {
                Singleton<MiHoYoGameData>.Instance.Save();
            }
            return false;
        }

        private bool SetSpaceShipActive(bool active, bool setCameraComponentOnly = false)
        {
            if ((this.uiMainCamera != null) && (base.view != null))
            {
                if (!active)
                {
                    if (setCameraComponentOnly)
                    {
                        this.uiMainCamera.gameObject.SetActive(true);
                        this.uiMainCamera.GetComponent<Camera>().enabled = false;
                    }
                    else
                    {
                        this.uiMainCamera.gameObject.SetActive(false);
                    }
                }
                else
                {
                    this.uiMainCamera.GetComponent<Camera>().enabled = true;
                    this.uiMainCamera.gameObject.SetActive(true);
                }
                base.view.SetActive(active);
            }
            return false;
        }

        private bool SetSpaceShipLight(bool isGalTouch)
        {
            if ((base.view != null) && (base.view.transform.Find("DirLight").GetComponent<MainMenuLight>() != null))
            {
                base.view.transform.Find("DirLight").GetComponent<MainMenuLight>().mode = !isGalTouch ? MainMenuLight.Mode.Section : MainMenuLight.Mode.Fixed;
            }
            return false;
        }

        protected override bool SetupView()
        {
            return false;
        }
    }
}

