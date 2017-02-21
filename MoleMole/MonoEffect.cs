namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class MonoEffect : BaseMonoEffect
    {
        private Animation[] _allAnimations;
        private ParticleSystemRenderer[] _allParticleSystemRenderers;
        private ParticleSystem[] _allParticleSystems;
        private BaseMonoEffectPlugin[] _effectPlugins;
        private bool _isToBeRemoved;
        private float _lastTimeScale;
        private ParticleSystem _mainParticleSystem;
        [NonSerialized]
        public string belongSkillName;
        [NonSerialized]
        public bool disableGORecursively;
        public bool dontDestroyWhenOwnerEvade;
        public bool IgnoreTimescale;

        protected override void Awake()
        {
            base.Awake();
            this._allParticleSystems = base.GetComponentsInChildren<ParticleSystem>();
            this._allParticleSystemRenderers = new ParticleSystemRenderer[this._allParticleSystems.Length];
            for (int i = 0; i < this._allParticleSystems.Length; i++)
            {
                if (this._allParticleSystems[i].loop)
                {
                    this._allParticleSystems[i].subEmitters.enabled = true;
                }
                this._allParticleSystemRenderers[i] = this._allParticleSystems[i].GetComponent<ParticleSystemRenderer>();
            }
            this._mainParticleSystem = base.GetComponentInChildren<ParticleSystem>();
            this._allAnimations = base.GetComponentsInChildren<Animation>();
            this._effectPlugins = base.GetComponents<BaseMonoEffectPlugin>();
        }

        public override bool IsActive()
        {
            return !this._isToBeRemoved;
        }

        public override bool IsToBeRemove()
        {
            if (!this._isToBeRemoved)
            {
                for (int i = 0; i < this._effectPlugins.Length; i++)
                {
                    if (this._effectPlugins[i].IsToBeRemove())
                    {
                        return true;
                    }
                }
                if ((this._mainParticleSystem == null) || !this._mainParticleSystem.gameObject.activeInHierarchy)
                {
                    return this._isToBeRemoved;
                }
                for (int j = 0; j < this._allParticleSystems.Length; j++)
                {
                    if (this._allParticleSystems[j].IsAlive(false))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void OnDisable()
        {
            if (this.disableGORecursively)
            {
                int index = 0;
                int length = this._allParticleSystems.Length;
                while (index < length)
                {
                    this._allParticleSystems[index].gameObject.SetActive(false);
                    index++;
                }
            }
        }

        private void OnValidate()
        {
            if (base.GetComponentInParent<MonoEffect>() != null)
            {
            }
        }

        public void Pause()
        {
            this.SetParticleAndAnimationPaused(true);
        }

        public void Resume()
        {
            this.SetParticleAndAnimationPaused(false);
        }

        public void SetDestroy()
        {
            if (!this.dontDestroyWhenOwnerEvade)
            {
                for (int i = 0; i < this._effectPlugins.Length; i++)
                {
                    this._effectPlugins[i].SetDestroy();
                }
                for (int j = 0; j < this._allParticleSystems.Length; j++)
                {
                    this._allParticleSystems[j].Stop(false);
                    if (!this._allParticleSystemRenderers[j].isVisible)
                    {
                        this._allParticleSystems[j].Clear();
                    }
                }
            }
        }

        public void SetDestroyImmediately()
        {
            for (int i = 0; i < this._effectPlugins.Length; i++)
            {
                this._effectPlugins[i].SetDestroy();
            }
            for (int j = 0; j < this._allParticleSystems.Length; j++)
            {
                this._allParticleSystems[j].Stop(false);
                this._allParticleSystems[j].Clear();
            }
            this._isToBeRemoved = true;
        }

        public void SetOwner(BaseMonoEntity owner)
        {
            this.owner = owner;
        }

        private void SetParticleAndAnimationPaused(bool paused)
        {
            for (int i = 0; i < this._allParticleSystems.Length; i++)
            {
                ParticleSystem system = this._allParticleSystems[i];
                if (system != null)
                {
                    system.emission.enabled = !paused;
                }
            }
            if (this._allAnimations != null)
            {
                for (int j = 0; j < this._allAnimations.Length; j++)
                {
                    Animation animation = this._allAnimations[j];
                    if ((animation != null) && (animation.clip != null))
                    {
                        if (paused)
                        {
                            animation.Stop();
                        }
                        else
                        {
                            animation.Play();
                        }
                    }
                }
            }
            List<MeshRenderer> results = new List<MeshRenderer>();
            base.transform.GetComponentsInChildren<MeshRenderer>(results);
            foreach (MeshRenderer renderer in results)
            {
                renderer.enabled = !paused;
            }
        }

        private void SetParticleAndAnimationPlaySpeed(float speed)
        {
            if (this._allParticleSystems != null)
            {
                for (int i = 0; i < this._allParticleSystems.Length; i++)
                {
                    this._allParticleSystems[i].playbackSpeed = speed;
                }
            }
            if (this._allAnimations != null)
            {
                for (int j = 0; j < this._allAnimations.Length; j++)
                {
                    Animation animation = this._allAnimations[j];
                    if ((animation != null) && (animation.clip != null))
                    {
                        animation[animation.clip.name].speed = speed;
                    }
                }
            }
        }

        public override void Setup()
        {
            if (this._effectPlugins != null)
            {
                int num = 0;
                int num2 = this._effectPlugins.Length;
                while (num < num2)
                {
                    this._effectPlugins[num].Setup();
                    num++;
                }
            }
            this._isToBeRemoved = false;
            this._lastTimeScale = this.TimeScale;
            this.belongSkillName = null;
            this.SetParticleAndAnimationPlaySpeed(!this.IgnoreTimescale ? this._lastTimeScale : 1f);
            int index = 0;
            int length = this._allParticleSystems.Length;
            while (index < length)
            {
                this._allParticleSystems[index].gameObject.SetActive(true);
                index++;
            }
        }

        public void SetupOverride(BaseMonoEntity owner)
        {
            MonoEffectOverride component = owner.GetComponent<MonoEffectOverride>();
            if (component != null)
            {
                MonoEffectPluginTrailSmooth smooth = base.GetComponent<MonoEffectPluginTrailSmooth>();
                if (smooth != null)
                {
                    smooth.HandleEffectOverride(component);
                }
                else
                {
                    MonoEffectPluginTrailStatic @static = base.GetComponent<MonoEffectPluginTrailStatic>();
                    if (@static != null)
                    {
                        @static.HandleEffectOverride(component);
                    }
                }
                MonoEffectPluginOverrideHandler handler = base.GetComponent<MonoEffectPluginOverrideHandler>();
                if (handler != null)
                {
                    handler.HandleEffectOverride(component);
                }
            }
        }

        public void SetupPlugin()
        {
            if (base.GetComponent<MonoEffectPluginFollow>() != null)
            {
                base.GetComponent<MonoEffectPluginFollow>().SetFollowParentTarget(this.owner.transform);
            }
            if (base.GetComponent<MonoEffectPluginSkinMeshShape>() != null)
            {
                base.GetComponent<MonoEffectPluginSkinMeshShape>().SetupSkinmesh(this.owner);
            }
            if (base.GetComponent<MonoEffectPluginDisableOnPropertyBlock>() != null)
            {
                base.GetComponent<MonoEffectPluginDisableOnPropertyBlock>().SetupRenderer(this.owner);
            }
            this.SetupOverride(this.owner);
        }

        public void SetupPluginFromTo(BaseMonoEntity toEntity)
        {
            if (base.GetComponent<MonoEffectPluginFollow>() != null)
            {
                base.GetComponent<MonoEffectPluginFollow>().SetFollowParentTarget(toEntity.transform);
            }
            if (base.GetComponent<MonoEffectPluginMoveToTarget>() != null)
            {
                base.GetComponent<MonoEffectPluginMoveToTarget>().SetMoveToTarget(toEntity);
            }
            if (base.GetComponent<MonoEffectPluginSkinMeshShape>() != null)
            {
                base.GetComponent<MonoEffectPluginSkinMeshShape>().SetupSkinmesh(this.owner);
            }
            if (base.GetComponent<MonoEffectPluginDisableOnPropertyBlock>() != null)
            {
                base.GetComponent<MonoEffectPluginDisableOnPropertyBlock>().SetupRenderer(this.owner);
            }
            this.SetupOverride(this.owner);
        }

        public override void Update()
        {
            base.Update();
            if (this.IgnoreTimescale)
            {
                float timeScale = Time.timeScale;
                if (this._lastTimeScale != timeScale)
                {
                    this.SetParticleAndAnimationPlaySpeed((timeScale != 0f) ? (1f / timeScale) : 0f);
                }
                this._lastTimeScale = Time.timeScale;
            }
            else
            {
                float speed = this.TimeScale;
                if (this._lastTimeScale != speed)
                {
                    this.SetParticleAndAnimationPlaySpeed(speed);
                }
                this._lastTimeScale = this.TimeScale;
            }
        }

        public ParticleSystem mainParticleSystem
        {
            get
            {
                return this._mainParticleSystem;
            }
        }

        public BaseMonoEntity owner { get; private set; }

        public override float TimeScale
        {
            get
            {
                return ((this.owner == null) ? Singleton<LevelManager>.Instance.levelEntity.TimeScale : this.owner.TimeScale);
            }
        }
    }
}

