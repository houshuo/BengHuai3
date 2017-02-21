namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class Footprint : MonoBehaviour
    {
        private Dictionary<BaseMonoAvatar, State> _avatarStateDict = new Dictionary<BaseMonoAvatar, State>();
        private Dictionary<string, Setting> _avatarTypeNameSettingDict = new Dictionary<string, Setting>();
        private Setting _defaultSetting;
        private Vector3 _lightDir = new Vector3(0f, -1f, 1f);
        private Transform _lightForwardTransform;
        private List<ParticleSystem> _particleSystemList = new List<ParticleSystem>();
        private Dictionary<Setting, ParticleSystem> _settingParticleSystemDict = new Dictionary<Setting, ParticleSystem>();
        public Setting[] avatarSettings;
        public float duration;
        public Material material;
        public int maxCount = 0x3e8;
        public Vector3 offset;
        public GameObject particleSystemPrefab;
        public float size = 1f;
        public MonoZone2D zone;

        private void AddFootprint(Vector3 position, Vector3 direction, State state)
        {
            Setting setting = state.setting;
            ParticleSystem particleSystem = state.particleSystem;
            float y = Mathf.Atan2(direction.x, direction.z) * 57.29578f;
            ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams();
            Vector3 vector = position + setting.offset;
            vector.y = 0f;
            emitParams.position = vector;
            emitParams.axisOfRotation = Vector3.up;
            emitParams.rotation = y;
            emitParams.startSize = setting.size;
            emitParams.startLifetime = this.duration;
            Vector3 vector2 = (Vector3) (Quaternion.Euler(0f, y, 0f) * -this._lightDir);
            vector2 = (Vector3) ((vector2 + Vector3.one) * 0.5f);
            emitParams.startColor = new Color(vector2.x, vector2.y, vector2.z, 1f);
            particleSystem.Emit(emitParams, 1);
        }

        private void Awake()
        {
            this.ParseAvatarSettings();
            this.CreateParticleSystems();
        }

        private void CreateParticleSystem(Setting setting)
        {
            GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(this.particleSystemPrefab);
            obj2.name = string.Format("__ParticelSystem_for_setting_{0}", setting.name);
            obj2.transform.SetParentAndReset(base.transform);
            ParticleSystem component = obj2.GetComponent<ParticleSystem>();
            this.SetParticleSystem(component, setting);
            this._particleSystemList.Add(component);
            this._settingParticleSystemDict.Add(setting, component);
        }

        private void CreateParticleSystems()
        {
            foreach (Setting setting in this.avatarSettings)
            {
                this.CreateParticleSystem(setting);
            }
            this.CreateParticleSystem(this._defaultSetting);
        }

        private void HandleStep(BaseStepController.Param param, ref BaseStepController.Pattern lastPatten, State state)
        {
            BaseStepController.Param param2 = param;
            if ((lastPatten == BaseStepController.Pattern.Down) && (param2.pattern == BaseStepController.Pattern.Static))
            {
                this.AddFootprint(param2.position, param2.toeForwardXZ, state);
            }
            lastPatten = param.pattern;
        }

        private void ParseAvatarSettings()
        {
            this._defaultSetting = new Setting();
            this._defaultSetting.name = "Default";
            this._defaultSetting.material = this.material;
            this._defaultSetting.size = this.size;
            this._defaultSetting.offset = this.offset;
            foreach (Setting setting in this.avatarSettings)
            {
                foreach (string str in setting.avatarTypeNames)
                {
                    this._avatarTypeNameSettingDict.Add(str, setting);
                }
            }
        }

        private void SetParticleSystem(ParticleSystem ps, Setting setting)
        {
            ps.simulationSpace = ParticleSystemSimulationSpace.World;
            ps.maxParticles = this.maxCount;
            ParticleSystemRenderer component = ps.GetComponent<ParticleSystemRenderer>();
            component.material = setting.material;
            component.renderMode = ParticleSystemRenderMode.HorizontalBillboard;
            ps.startSpeed = 0f;
            ps.emission.enabled = false;
        }

        private void Update()
        {
            if (this._lightForwardTransform == null)
            {
                this._lightForwardTransform = Singleton<StageManager>.Instance.GetStageEnv().lightForwardTransform;
                this._lightDir = this._lightForwardTransform.forward;
            }
            List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
            for (int i = 0; i < allAvatars.Count; i++)
            {
                BaseMonoAvatar key = allAvatars[i];
                if (key.IsActive() && ((this.zone == null) || this.zone.Contain(key.XZPosition)))
                {
                    State state;
                    if (this._avatarStateDict.ContainsKey(key))
                    {
                        state = this._avatarStateDict[key];
                    }
                    else
                    {
                        state = new State();
                        string avatarTypeName = key.AvatarTypeName;
                        state.setting = !this._avatarTypeNameSettingDict.ContainsKey(avatarTypeName) ? this._defaultSetting : this._avatarTypeNameSettingDict[avatarTypeName];
                        state.particleSystem = this._settingParticleSystemDict[state.setting];
                        state.controller = key.GetComponent<BaseStepController>();
                        state.lastLeftPatten = BaseStepController.Pattern.Void;
                        state.rightLeftPatten = BaseStepController.Pattern.Void;
                        if (state.controller == null)
                        {
                        }
                        this._avatarStateDict.Add(key, state);
                    }
                    BaseStepController controller = state.controller;
                    if (controller != null)
                    {
                        this.HandleStep(controller.currentLeftStepParam, ref state.lastLeftPatten, state);
                        this.HandleStep(controller.currentRightStepParam, ref state.rightLeftPatten, state);
                    }
                }
            }
        }

        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Setting
        {
            public string name;
            public string[] avatarTypeNames;
            public Material material;
            public float size;
            public Vector3 offset;
        }

        private class State
        {
            public BaseStepController controller;
            public BaseStepController.Pattern lastLeftPatten;
            public ParticleSystem particleSystem;
            public BaseStepController.Pattern rightLeftPatten;
            public Footprint.Setting setting;
        }
    }
}

