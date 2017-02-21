namespace MoleMole.MainMenu
{
    using System;
    using UnityEngine;

    [Serializable]
    public class ConfigAtmosphereCommon
    {
        private int _sceneId;
        [Range(0f, 100f)]
        public float PlaybackSpeed;
        public ConfigCloudScene[] SceneList;
        public Texture SecondTex;
        public Texture Tex;
        [Tooltip("The max time (in second) in which transit from a frame to next ")]
        public float TransitionTime;

        public int GetSceneIdRandomly()
        {
            if (this.SceneList.Length == 0)
            {
                return 0;
            }
            int[] rates = new int[this.SceneList.Length];
            for (int i = 0; i < this.SceneList.Length; i++)
            {
                rates[i] = this.SceneList[i].ChooseRate;
            }
            return ConfigAtmosphereUtil.ChooseRandomly(rates);
        }

        public void InitAfterLoad()
        {
        }

        public void UpdateSceneNameNext()
        {
            this._sceneId++;
            if (this._sceneId >= this.SceneList.Length)
            {
                this._sceneId = 0;
            }
        }

        public void UpdateSceneRandomly()
        {
            if (this.SceneList.Length != 0)
            {
                int[] rates = new int[this.SceneList.Length];
                for (int i = 0; i < this.SceneList.Length; i++)
                {
                    rates[i] = this.SceneList[i].ChooseRate;
                }
                this._sceneId = ConfigAtmosphereUtil.ChooseRandomly(rates);
            }
        }

        public int SceneId
        {
            get
            {
                return this._sceneId;
            }
            set
            {
                this._sceneId = value;
            }
        }

        public string[] SceneNameList
        {
            get
            {
                string[] strArray = new string[this.SceneList.Length];
                for (int i = 0; i < strArray.Length; i++)
                {
                    strArray[i] = this.SceneList[i].Name;
                }
                return strArray;
            }
        }

        public string ScneneName
        {
            get
            {
                if (this.SceneList.Length == 0)
                {
                    return string.Empty;
                }
                return this.SceneList[this._sceneId].Name;
            }
        }
    }
}

