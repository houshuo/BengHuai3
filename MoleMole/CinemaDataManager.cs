namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class CinemaDataManager
    {
        private Dictionary<string, ICinema> _loadedCinemaData = new Dictionary<string, ICinema>();

        private CinemaDataManager()
        {
        }

        public ICinema GetCinemaDataByAvatar(string avatar, AvatarCinemaType type)
        {
            string str = AvatarData.GetAvatarConfig(avatar).CinemaPaths[type];
            return this._loadedCinemaData[str];
        }

        public void InitAtAwake()
        {
            this._loadedCinemaData.Clear();
        }

        public void Preload(BaseMonoAvatar aMonoAvatar)
        {
            ConfigAvatar config = aMonoAvatar.config;
            if (config.CinemaPaths.Count != 0)
            {
                foreach (KeyValuePair<AvatarCinemaType, string> pair in config.CinemaPaths)
                {
                    string key = pair.Value;
                    if (!this._loadedCinemaData.ContainsKey(key))
                    {
                        ICinema component = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(key, BundleType.RESOURCE_FILE)).GetComponent<MonoBehaviour>() as ICinema;
                        component.GetCutscene().Optimize();
                        this._loadedCinemaData.Add(key, component);
                    }
                }
            }
        }
    }
}

