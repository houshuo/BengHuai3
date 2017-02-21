namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using System;
    using System.Diagnostics;
    using UnityEngine;

    public class LevelManager
    {
        private bool _mutePause;
        private int _pauseCount;
        public IGameMode gameMode;
        public LevelActor levelActor;
        public MonoLevelEntity levelEntity;

        protected LevelManager()
        {
            this.ResetGlobalTimeScale();
            InLevelData.InitInLevelData();
            AvatarData.InitAvatarData();
            MonsterData.InitMonsterData();
            this.CreateInLevelManagers();
        }

        [Conditional("NG_HSOD_DEBUG")]
        public void AssertIsNetworkedMP()
        {
        }

        [Conditional("NG_HSOD_DEBUG")]
        public void AssertIsSinglePlayer()
        {
        }

        public virtual void Core()
        {
            Singleton<StageManager>.Instance.Core();
            Singleton<AvatarManager>.Instance.Core();
            Singleton<CameraManager>.Instance.Core();
            Singleton<MonsterManager>.Instance.Core();
            Singleton<PropObjectManager>.Instance.Core();
            Singleton<DynamicObjectManager>.Instance.Core();
            Singleton<EffectManager>.Instance.Core();
            Singleton<EventManager>.Instance.Core();
            Singleton<LevelDesignManager>.Instance.Core();
            Singleton<AuxObjectManager>.Instance.Core();
            Singleton<DetourManager>.Instance.Core();
            if (Singleton<WwiseAudioManager>.Instance != null)
            {
                Singleton<WwiseAudioManager>.Instance.Core();
            }
        }

        public void CreateBehaviorManager()
        {
            Behavior.CreateBehaviorManager();
        }

        protected virtual void CreateInLevelManagers()
        {
            Singleton<RuntimeIDManager>.Create();
            Singleton<StageManager>.Create();
            Singleton<AvatarManager>.Create();
            Singleton<CameraManager>.Create();
            Singleton<MonsterManager>.Create();
            Singleton<PropObjectManager>.Create();
            Singleton<DynamicObjectManager>.Create();
            Singleton<EventManager>.Create();
            Singleton<LevelDesignManager>.Create();
            Singleton<AuxObjectManager>.Create();
            Singleton<DetourManager>.Create();
            Singleton<ShaderDataManager>.Create();
            Singleton<CinemaDataManager>.Create();
            this.gameMode = new OriginalSPGameMode();
        }

        public virtual void Destroy()
        {
            Singleton<DynamicObjectManager>.Instance.Destroy();
            Singleton<PropObjectManager>.Instance.Destroy();
            Singleton<MonsterManager>.Instance.Destroy();
            Singleton<AvatarManager>.Instance.Destroy();
            Singleton<EventManager>.Instance.Destroy();
            Singleton<LevelDesignManager>.Instance.Destroy();
            Singleton<AuxObjectManager>.Instance.Destroy();
            Singleton<EffectManager>.Instance.Clear();
            Singleton<DynamicObjectManager>.Destroy();
            Singleton<MonsterManager>.Destroy();
            Singleton<PropObjectManager>.Destroy();
            Singleton<CameraManager>.Destroy();
            Singleton<AvatarManager>.Destroy();
            Singleton<RuntimeIDManager>.Destroy();
            Singleton<StageManager>.Destroy();
            Singleton<EventManager>.Destroy();
            Singleton<LevelDesignManager>.Destroy();
            Singleton<AuxObjectManager>.Destroy();
            Singleton<DetourManager>.Destroy();
            Singleton<ShaderDataManager>.Destroy();
            Singleton<CinemaDataManager>.Destroy();
            this.DestroyBehaviorDesigner();
            this.ResetGlobalTimeScale();
        }

        public void DestroyBehaviorDesigner()
        {
            BehaviorManager manager = UnityEngine.Object.FindObjectOfType<BehaviorManager>();
            if (manager != null)
            {
                UnityEngine.Object.Destroy(manager.gameObject);
                BehaviorManager.instance = null;
            }
        }

        public virtual void InitAtAwake()
        {
            Singleton<RuntimeIDManager>.Instance.InitAtAwake();
            Singleton<StageManager>.Instance.InitAtAwake();
            Singleton<AvatarManager>.Instance.InitAtAwake();
            Singleton<CameraManager>.Instance.InitAtAwake();
            Singleton<MonsterManager>.Instance.InitAtAwake();
            Singleton<PropObjectManager>.Instance.InitAtAwake();
            Singleton<DynamicObjectManager>.Instance.InitAtAwake();
            Singleton<WwiseAudioManager>.Instance.InitAtAwake();
            Singleton<EventManager>.Instance.InitAtAwake();
            Singleton<LevelDesignManager>.Instance.InitAtAwake();
            Singleton<AuxObjectManager>.Instance.InitAtAwake();
            Singleton<DetourManager>.Instance.InitAtAwake();
            Singleton<ShaderDataManager>.Instance.InitAtAwake();
            Singleton<CinemaDataManager>.Instance.InitAtAwake();
            this.CreateBehaviorManager();
        }

        public virtual void InitAtStart()
        {
            Singleton<StageManager>.Instance.InitAtStart();
            Singleton<AvatarManager>.Instance.InitAtStart();
            Singleton<CameraManager>.Instance.InitAtStart();
            Singleton<MonsterManager>.Instance.InitAtStart();
            Singleton<PropObjectManager>.Instance.InitAtStart();
            Singleton<DynamicObjectManager>.Instance.InitAtStart();
            Singleton<EventManager>.Instance.InitAtStart();
            Singleton<LevelDesignManager>.Instance.InitAtStart();
            Singleton<AuxObjectManager>.Instance.InitAtStart();
            Singleton<DetourManager>.Instance.InitAtStart();
        }

        public bool IsPaused()
        {
            return (this._pauseCount > 0);
        }

        private void ResetGlobalTimeScale()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        public void SetMutePause(bool mutePause)
        {
            this._mutePause = mutePause;
        }

        public void SetPause(bool pause)
        {
            bool flag = this.IsPaused();
            this._pauseCount += !pause ? -1 : 1;
            if (this._pauseCount < 0)
            {
                this._pauseCount = 0;
            }
            if (!this._mutePause)
            {
                pause = this.IsPaused();
                if (pause != flag)
                {
                    Singleton<LevelDesignManager>.Instance.SetPause(pause);
                    Singleton<AvatarManager>.Instance.SetPause(pause);
                    Singleton<MonsterManager>.Instance.SetPause(pause);
                    Singleton<EventManager>.Instance.SetPauseDispatching(pause);
                    if (pause)
                    {
                        this.levelEntity.timeScaleStack.Push(6, 0f, false);
                    }
                    else
                    {
                        this.levelEntity.timeScaleStack.Pop(6);
                    }
                }
            }
        }

        public void SetTutorialTimeScale(float timeScale)
        {
            timeScale = Mathf.Clamp(timeScale, 0f, 1f);
            if (timeScale != 1f)
            {
                if (this.levelEntity.timeScaleStack.IsOccupied(7))
                {
                    this.levelEntity.timeScaleStack.Set(7, timeScale, false);
                }
                else
                {
                    this.levelEntity.timeScaleStack.Push(7, timeScale, false);
                }
            }
            else if (this.levelEntity.timeScaleStack.IsOccupied(7))
            {
                this.levelEntity.timeScaleStack.Pop(7);
            }
            else
            {
                this.levelEntity.timeScaleStack.Set(7, 1f, false);
            }
        }
    }
}

