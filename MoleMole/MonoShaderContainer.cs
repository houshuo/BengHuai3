namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MonoShaderContainer : MonoBehaviour
    {
        [Header("Tick this to go to game entry directly on awake")]
        public bool GoToGameEntry;
        [Header("!!! THIS PREFAB NEEDS TO BE IN GAME ENTRY SCENE && CAN NOT BE PUT IN 'RESOURCES' DIRECTORY !!!")]
        public Shader[] shaders;

        private void Awake()
        {
            UnityEngine.Object.DontDestroyOnLoad(this);
            GraphicsUtils.WarmupAllShaders();
            if (this.GoToGameEntry)
            {
                SceneManager.LoadScene("GameEntry");
            }
        }
    }
}

