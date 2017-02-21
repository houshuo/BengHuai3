namespace MoleMole
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoTheLevelV1 : MonoBehaviour
    {
        [Conditional("UNITY_EDITOR")]
        public void AttachLabelToTransform(Transform target, Vector3 offset, Func<string> textCallback)
        {
        }

        public void Awake()
        {
            this.CreateLevelManager();
            Singleton<LevelManager>.Instance.InitAtAwake();
        }

        protected virtual void CreateLevelManager()
        {
            Singleton<LevelManager>.Create();
            MonoLevelEntity entity = Singleton<LevelManager>.Instance.levelEntity = base.gameObject.AddComponent<MonoLevelEntity>();
            entity.Init(0x21800001);
            Singleton<LevelManager>.Instance.levelActor = Singleton<EventManager>.Instance.CreateActor<LevelActor>(entity);
            Singleton<LevelManager>.Instance.levelActor.PostInit();
        }

        private void OnApplicationQuit()
        {
            if (Singleton<LevelManager>.Instance != null)
            {
                Singleton<LevelManager>.Instance.levelActor.SuddenLevelEnd();
            }
        }

        public void OnDestroy()
        {
            if (Singleton<WwiseAudioManager>.Instance != null)
            {
                Singleton<WwiseAudioManager>.Instance.PopSoundBankScale();
                Singleton<MainMenuBGM>.Instance.TryEnterMainMenu();
            }
            Singleton<LevelManager>.Instance.Destroy();
            Singleton<LevelManager>.Destroy();
        }

        [Conditional("UNITY_EDITOR")]
        public void PopupLabelToTransform(Transform target, Vector3 offset, string text, float duration = 2f)
        {
        }

        public void Start()
        {
            Singleton<LevelManager>.Instance.InitAtStart();
            GraphicsSettingData.ApplySettingConfig();
            AudioSettingData.ApplySettingConfig();
        }

        public void Update()
        {
            Singleton<LevelManager>.Instance.Core();
        }
    }
}

