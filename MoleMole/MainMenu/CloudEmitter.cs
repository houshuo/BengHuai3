namespace MoleMole.MainMenu
{
    using MoleMole;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(ParticleSystem))]
    public class CloudEmitter : MonoBehaviour
    {
        private Dictionary<int, CollidedParticle> _collidedParticleMap = new Dictionary<int, CollidedParticle>();
        private int _frameCount;
        private bool _isPaused;
        private float _lastDeltaTime;
        private int _lastFrameCount = -1;
        private float _lastTime;
        private Material _material;
        private int _materialCount = 1;
        private int _maxCloudCount = 0x3e8;
        private bool _needRestoreLastState;
        private int _nextParticleId;
        private int _particleCount;
        private ParticleSystem.Particle[] _particles;
        private ParticleSystem _particleSystem;
        private ParticleSystemRenderer _renderer;
        private Rect _shipColliderRect;
        private float _shipColliderZ;
        public float AspectRatio = 1f;
        public Color BrightColor = Color.white;
        [Header("Cloud scenes")]
        public int CloudSceneId;
        public CloudScene[] CloudScenes;
        public Color DarkColor = Color.gray;
        [Header("Emitters")]
        public Vector3 EmitterSize = new Vector3(400f, 100f, 1000f);
        [Tooltip("X: constant Y: linear Z: quadratic")]
        public Vector3 FlashAttenuationFactors = new Vector3(1f, 1f, 0.001f);
        public Color FlashColor = Color.white;
        public int MaterialTileGridX = 1;
        public int MaterialTileGridY = 1;
        [Header("Material")]
        public Material ParticleMaterial;
        [Header("Control"), Range(0f, 100f)]
        public float PlaybackSpeed = 1f;
        public Color RimColor = Color.yellow;
        public Color SecondDarkColor = ((Color) (Color.gray * 0.5f));
        [Header("The box collider need to be axis aligned")]
        public BoxCollider ShipCollider;
        public float ShipColliderSkin = 5f;
        public float ShipFrontDistanceForSight = 2000f;
        public ParticleSystemSimulationSpace SimulationSpace;
        [Header("Clouds")]
        public float Speed = 30f;

        private void Awake()
        {
            this.Init();
        }

        public Vector3 CameraToLocal(Vector3 point)
        {
            Vector3 v = this._camera.cameraToWorldMatrix.MultiplyPoint(point);
            return base.transform.worldToLocalMatrix.MultiplyPoint(v);
        }

        private void Emit(float deltaTime)
        {
            this.EmitCloudScene(this.CloudSceneId, deltaTime);
        }

        private void EmitCloudScene(int cloudSceneId, float deltaTime)
        {
            if ((cloudSceneId >= 0) && (cloudSceneId < this.CloudScenes.Length))
            {
                foreach (CloudType type in this.CloudScenes[cloudSceneId].CloudTypes)
                {
                    if (type.Enable)
                    {
                        this.EmitCloudType(type, deltaTime);
                    }
                }
                foreach (CompoundCloudType type2 in this.CloudScenes[cloudSceneId].CompoundCloudTypes)
                {
                    if (type2.Enable)
                    {
                        this.EmitCompoundCloudType(type2, deltaTime);
                    }
                }
                foreach (LayerCloudType type3 in this.CloudScenes[cloudSceneId].LayerCloudTypes)
                {
                    if (type3.Enable)
                    {
                        this.EmitLayerCloudType(type3, deltaTime);
                    }
                }
            }
        }

        private void EmitCloudType(CloudType cloudType, float deltaTime)
        {
            if (cloudType.MaterialIds.Length != 0)
            {
                int emitCount = cloudType.GetEmitCount(deltaTime);
                if (emitCount != 0)
                {
                    int[] materialIds = cloudType.MaterialIds;
                    Vector3 vector = new Vector3();
                    Color color = new Color(0f, 0f, 1f, 1f);
                    Vector3 vector2 = (Vector3) (Vector3.forward * this._particleSystem.startSpeed);
                    Vector3 vector3 = (Vector3) (this.EmitterSize / 2f);
                    for (int i = 0; i < emitCount; i++)
                    {
                        int index = UnityEngine.Random.Range(0, materialIds.Length);
                        color.r = ((float) materialIds[index]) / ((float) this._materialCount);
                        color.r += 0.5f / ((float) this._materialCount);
                        float randomSize = cloudType.GetRandomSize();
                        color.g = randomSize / 2f;
                        vector = ((Vector3) (cloudType.GetRandomPosition() * 2f)) - Vector3.one;
                        vector.x *= vector3.x;
                        vector.x = -vector.x;
                        vector.y *= vector3.y;
                        vector.z = 0f;
                        ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams {
                            position = vector,
                            velocity = vector2,
                            startSize = this.GetParticleId(),
                            startLifetime = this._particleSystem.startLifetime,
                            startColor = color
                        };
                        this._particleSystem.Emit(emitParams, 1);
                    }
                }
            }
        }

        private void EmitCompoundCloudType(CompoundCloudType compoundCloudType, float deltaTime)
        {
            if (compoundCloudType.CloudTypes.Length != 0)
            {
                int emitCount = compoundCloudType.GetEmitCount(deltaTime);
                if (emitCount != 0)
                {
                    CloudType[] cloudTypes = compoundCloudType.CloudTypes;
                    Vector3 vector = (Vector3) (compoundCloudType.GetRandomSize() / 2f);
                    Vector3 vector2 = ((Vector3) (compoundCloudType.GetRandomPosition() * 2f)) - Vector3.one;
                    vector2.x = -vector2.x;
                    vector2.x *= this.EmitterSize.x / 2f;
                    vector2.y *= this.EmitterSize.y / 2f;
                    Color color = new Color(0f, 0f, 1f, 1f);
                    Vector3 vector3 = (Vector3) (Vector3.forward * this._particleSystem.startSpeed);
                    for (int i = 0; i < emitCount; i++)
                    {
                        foreach (CloudType type in cloudTypes)
                        {
                            for (int j = 0; j < type.EmitCount; j++)
                            {
                                int[] materialIds = type.MaterialIds;
                                Vector3 vector4 = new Vector3();
                                int index = UnityEngine.Random.Range(0, materialIds.Length);
                                color.r = ((float) materialIds[index]) / ((float) this._materialCount);
                                color.r += 0.5f / ((float) this._materialCount);
                                float randomSize = type.GetRandomSize();
                                color.g = randomSize / 2f;
                                vector4 = ((Vector3) (type.GetRandomPosition() * 2f)) - Vector3.one;
                                vector4.x *= vector.x;
                                vector4.x = -vector4.x;
                                vector4.y *= vector.y;
                                vector4 += vector2;
                                vector4.z += (vector.z * 2f) * UnityEngine.Random.value;
                                ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams {
                                    position = vector4,
                                    velocity = vector3,
                                    startSize = this.GetParticleId(),
                                    startLifetime = this._particleSystem.startLifetime,
                                    startColor = color
                                };
                                this._particleSystem.Emit(emitParams, 1);
                            }
                        }
                    }
                }
            }
        }

        private void EmitLayerCloudType(LayerCloudType layerCloudType, float deltaTime)
        {
            if ((layerCloudType.MaterialIds.Length != 0) && layerCloudType.IsEmit(deltaTime))
            {
                int[] materialIds = layerCloudType.MaterialIds;
                Vector3 vector = new Vector3();
                float num = 0f;
                if (!layerCloudType.IsOdd())
                {
                    num = -layerCloudType.InterleavedOffset * layerCloudType.GetRandomSize();
                }
                Color color = new Color(0f, 0f, 1f, 1f);
                Vector3 vector2 = (Vector3) (Vector3.forward * this._particleSystem.startSpeed);
                Vector3 vector3 = (Vector3) (this.EmitterSize / 2f);
                while (num < 1f)
                {
                    int index = UnityEngine.Random.Range(0, materialIds.Length);
                    color.r = ((float) materialIds[index]) / ((float) this._materialCount);
                    color.r += 0.5f / ((float) this._materialCount);
                    float randomSize = layerCloudType.GetRandomSize();
                    color.g = randomSize / 2f;
                    num += randomSize / 2f;
                    vector.x = num;
                    num += layerCloudType.GetRandomGap() * randomSize;
                    vector.y = layerCloudType.GetRandomPositionY();
                    vector = ((Vector3) (vector * 2f)) - Vector3.one;
                    vector.x *= vector3.x;
                    vector.x = -vector.x;
                    vector.y *= vector3.y;
                    vector.z = UnityEngine.Random.value * 10f;
                    ParticleSystem.EmitParams emitParams = new ParticleSystem.EmitParams {
                        position = vector,
                        velocity = vector2,
                        startSize = this.GetParticleId(),
                        startLifetime = this._particleSystem.startLifetime * UnityEngine.Random.Range((float) 0.99f, (float) 1.01f),
                        startColor = color
                    };
                    this._particleSystem.Emit(emitParams, 1);
                }
            }
        }

        private unsafe void EmitLightningType(LightningType lightningType, float deltaTime)
        {
            int num8;
            float num9;
            int emitCount = lightningType.GetEmitCount(deltaTime);
            Bounds emitArea = lightningType.EmitArea;
            Vector3 min = emitArea.min;
            Vector3 max = emitArea.max;
            for (int i = 0; i < 3; i++)
            {
                ref Vector3 vectorRef;
                num9 = vectorRef[num8];
                (vectorRef = (Vector3) &min)[num8 = i] = num9 * this.EmitterSize[i];
            }
            for (int j = 0; j < 3; j++)
            {
                ref Vector3 vectorRef2;
                num9 = vectorRef2[num8];
                (vectorRef2 = (Vector3) &max)[num8 = j] = num9 * this.EmitterSize[j];
            }
            emitArea.min = min;
            emitArea.max = max;
            while (emitCount-- != 0)
            {
                int index = -1;
                int num5 = 100;
                while ((index == -1) && (num5-- != 0))
                {
                    int num6 = UnityEngine.Random.Range(0, this._particleCount);
                    if (!this._collidedParticleMap.ContainsKey((int) this._particles[num6].startSize) && emitArea.Contains(this._particles[num6].position))
                    {
                        index = num6;
                        break;
                    }
                }
                if (index == -1)
                {
                    break;
                }
                float num7 = this.ParticleSize(ref this._particles[index]) * this.AspectRatio;
                Vector3 position = this._particles[index].position;
                position.y -= num7 * (lightningType.StartLocation - 0.5f);
                lightningType.Emit(position, this._particles[index].velocity);
            }
        }

        private void GetParticle()
        {
            if ((this._particles == null) || (this._particles.Length < this._particleSystem.maxParticles))
            {
                this._particles = new ParticleSystem.Particle[this._particleSystem.maxParticles];
            }
            this._particleCount = this._particleSystem.GetParticles(this._particles);
        }

        private int GetParticleId()
        {
            if (this._collidedParticleMap.ContainsKey(this._nextParticleId))
            {
                this._collidedParticleMap.Remove(this._nextParticleId);
            }
            int num = this._nextParticleId;
            this._nextParticleId++;
            if (this._nextParticleId > (this._maxCloudCount * 10))
            {
                this._nextParticleId = 0;
            }
            return num;
        }

        private void HandleCollision(float deltaTime)
        {
            for (int i = 0; i < this._particleCount; i++)
            {
                if (this._particles[i].position.z >= (this._shipColliderZ - this.ShipColliderSkin))
                {
                    CollidedParticle particle = null;
                    if (this._collidedParticleMap.ContainsKey((int) this._particles[i].startSize))
                    {
                        particle = this._collidedParticleMap[(int) this._particles[i].startSize];
                    }
                    else
                    {
                        Rect other = new Rect();
                        float x = this.ParticleSize(ref this._particles[i]);
                        other.size = new Vector2(x, x * this.AspectRatio);
                        other.center = this._particles[i].position;
                        if (this._shipColliderRect.Overlaps(other))
                        {
                            particle = new CollidedParticle {
                                RealPosition = this._particles[i].position,
                                RealVelocity = this._particles[i].velocity
                            };
                            this._collidedParticleMap.Add((int) this._particles[i].startSize, particle);
                            this._particles[i].velocity = Vector3.zero;
                            this._particles[i].lifetime = 1f;
                        }
                    }
                    if (particle != null)
                    {
                        particle.RealPosition += (Vector3) (particle.RealVelocity * deltaTime);
                        Vector3 point = this.LocalToCamera(particle.RealPosition);
                        if (point.z > -0.1f)
                        {
                            this._particles[i].lifetime = 0f;
                            this._collidedParticleMap.Remove((int) this._particles[i].startSize);
                        }
                        else
                        {
                            float num3 = this.LocalToCamera(this._particles[i].position).z / point.z;
                            point.x *= num3;
                            point.y *= num3;
                            Vector3 vector3 = this.CameraToLocal(point);
                            vector3.z = this._particles[i].position.z;
                            this._particles[i].position = vector3;
                            Color startColor = (Color) this._particles[i].startColor;
                            startColor.b = Mathf.Max((float) (1f / num3), (float) 0.003921569f);
                            startColor.a = 1f / num3;
                            this._particles[i].startColor = startColor;
                            this._particles[i].lifetime = 1f;
                        }
                    }
                }
            }
        }

        private void HandleCollisionForGoodSight(float deltaTime)
        {
            for (int i = 0; i < this._particleCount; i++)
            {
                if (this._particles[i].position.z > (this._shipColliderZ - this.ShipFrontDistanceForSight))
                {
                    Rect other = new Rect();
                    float x = this.ParticleSize(ref this._particles[i]);
                    other.size = new Vector2(x, x * this.AspectRatio);
                    other.size = (Vector2) (other.size * 0.8f);
                    other.center = this._particles[i].position;
                    this._shipColliderRect.size = (Vector2) (this._shipColliderRect.size * (this._particles[i].position.z / this._shipColliderZ));
                    if (this._shipColliderRect.Overlaps(other))
                    {
                        this._particles[i].lifetime = 0f;
                    }
                }
            }
        }

        private void Init()
        {
            if (!Application.isPlaying)
            {
                this._lastTime = Time.realtimeSinceStartup;
            }
            if (this.ShipCollider != null)
            {
                Vector3 center = this.ShipCollider.center;
                Vector3 size = this.ShipCollider.size;
                center = this.ShipCollider.transform.TransformPoint(center);
                center = base.transform.worldToLocalMatrix.MultiplyPoint(center);
                this._shipColliderZ = center.z - (size.z / 2f);
                this._shipColliderRect = new Rect();
                this._shipColliderRect.size = size;
                this._shipColliderRect.center = new Vector2(center.x, center.y);
            }
            this.InitParticleSystem();
            this.InitCloudScene();
        }

        private void InitCloudScene()
        {
            foreach (CloudScene scene in this.CloudScenes)
            {
                scene.Init(this);
            }
        }

        private void InitParticleSystem()
        {
            this._particleSystem = base.GetComponent<ParticleSystem>();
            this._renderer = base.GetComponent<ParticleSystemRenderer>();
            this._renderer.renderMode = ParticleSystemRenderMode.Mesh;
            this._renderer.mesh = MeshGenerator.BillboardQuad();
            this._material = new Material(this.ParticleMaterial);
            this._material.hideFlags = HideFlags.DontSave;
            this._renderer.material = this._material;
            this._materialCount = this.MaterialTileGridX * this.MaterialTileGridY;
            this._particles = new ParticleSystem.Particle[this._particleSystem.maxParticles];
            this.SetParticleSystem();
        }

        public bool IsPlaying()
        {
            return !this._isPaused;
        }

        public Vector3 LocalToCamera(Vector3 point)
        {
            Vector3 v = base.transform.TransformPoint(point);
            return this._camera.worldToCameraMatrix.MultiplyPoint(v);
        }

        public void OnDisable()
        {
            this._needRestoreLastState = true;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.matrix = base.transform.localToWorldMatrix;
            Vector3 center = new Vector3(0f, 0f, this.EmitterSize.z / 2f);
            Gizmos.DrawWireCube(center, this.EmitterSize);
        }

        public void OnEnable()
        {
            this.RestoreLastState();
        }

        private float ParticleSize(ref ParticleSystem.Particle particle)
        {
            return (((particle.startColor.g * 2f) / 255f) * this.EmitterSize.x);
        }

        public void Pause()
        {
            this._isPaused = true;
            this._particleSystem.Pause();
        }

        public void Play()
        {
            this._isPaused = false;
            this._particleSystem.Play();
        }

        public void Reset()
        {
            foreach (Transform transform in base.GetComponentsInChildren<Transform>(true))
            {
                if (transform != base.transform)
                {
                    UnityEngine.Object.Destroy(transform.gameObject);
                }
            }
            this.Init();
            this.SimulateInStart();
        }

        private void RestoreLastState()
        {
            if (this._needRestoreLastState)
            {
                this._needRestoreLastState = false;
                this._particleSystem.Clear();
                this._particleSystem.Emit(this._particleCount);
                this.SetParticle();
            }
        }

        private void SetMaterial()
        {
            this._material.SetFloat("_EmitterWidth", this.EmitterSize.x);
            this._material.SetFloat("_AspectRatio", this.AspectRatio);
            this._material.SetVector("_TileSize", new Vector4((float) this.MaterialTileGridX, (float) this.MaterialTileGridY, 0f, 0f));
            this._material.SetColor("_BrightColor", this.BrightColor);
            this._material.SetColor("_DarkColor", this.DarkColor);
            this._material.SetColor("_SecondDarkColor", this.SecondDarkColor);
            this._material.SetColor("_RimColor", this.RimColor);
            this._material.SetColor("_FlashColor", this.FlashColor);
            this._material.SetVector("_FlashAttenFactors", new Vector4(this.FlashAttenuationFactors.x, this.FlashAttenuationFactors.y, this.FlashAttenuationFactors.z));
        }

        private void SetParticle()
        {
            this._particleSystem.SetParticles(this._particles, this._particleCount);
        }

        private void SetParticleSystem()
        {
            this.SetParticleSystemParams();
            this.SetMaterial();
        }

        private void SetParticleSystemParams()
        {
            this._particleSystem.startSpeed = this.Speed;
            this._particleSystem.startLifetime = this.EmitterSize.z / this.Speed;
            this._particleSystem.simulationSpace = this.SimulationSpace;
            this._particleSystem.playbackSpeed = this.PlaybackSpeed;
            this._particleSystem.maxParticles = this._maxCloudCount;
        }

        public void SetupCloudConfig(ConfigAtmosphereCommon commonConfig, ConfigCloudStyle config)
        {
            bool flag = false;
            int num = -1;
            string scneneName = commonConfig.ScneneName;
            for (int i = 0; i < this.CloudScenes.Length; i++)
            {
                if (this.CloudScenes[i].Name == scneneName)
                {
                    num = i;
                    break;
                }
            }
            flag = num != this.CloudSceneId;
            this.CloudSceneId = num;
            this.PlaybackSpeed = commonConfig.PlaybackSpeed;
            this.BrightColor = config.BrightColor;
            this.DarkColor = config.DarkColor;
            this.SecondDarkColor = config.SecondDarkColor;
            this.RimColor = config.RimColor;
            this.FlashColor = config.FlashColor;
            this.FlashAttenuationFactors = config.FlashAttenuationFactors;
            if (flag)
            {
                this.Reset();
            }
            else
            {
                this.SetParticleSystem();
            }
        }

        private void SimulateInStart()
        {
            float deltaTime = 0.03333334f;
            int num2 = Mathf.FloorToInt(this._particleSystem.startLifetime / deltaTime);
            this._particleSystem.playbackSpeed = 1f;
            for (int i = 0; i < num2; i++)
            {
                this.GetParticle();
                if ((this.ShipCollider != null) && (this._camera != null))
                {
                    this.HandleCollisionForGoodSight(deltaTime);
                }
                this.SetParticle();
                this.Emit(deltaTime);
                this._particleSystem.Simulate(deltaTime, false, false);
            }
            this._particleSystem.playbackSpeed = this.PlaybackSpeed;
            this._particleSystem.Play();
        }

        private void Update()
        {
            this._frameCount++;
            this.Emit(this._deltaTime);
            this.GetParticle();
            if ((this.ShipCollider != null) && (this._camera != null))
            {
                this.HandleCollision(this._deltaTime);
            }
            this.UpdateLightning(this._deltaTime);
            this.SetParticle();
        }

        private void UpdateLightning(float deltaTime)
        {
            LightningType.PreUpdate(this._material);
            foreach (LightningType type in this.CloudScenes[this.CloudSceneId].LightningTypes)
            {
                if (type.Enable)
                {
                    this.EmitLightningType(type, deltaTime);
                    this.UpdateLightningType(type, deltaTime);
                }
            }
        }

        private void UpdateLightningType(LightningType lightningType, float deltaTime)
        {
            float realDeltaTime = deltaTime;
            if (this.PlaybackSpeed > 0.1f)
            {
                realDeltaTime /= this.PlaybackSpeed;
            }
            lightningType.Update(deltaTime, realDeltaTime, this._material);
        }

        private Camera _camera
        {
            get
            {
                return Camera.main;
            }
        }

        private float _deltaTime
        {
            get
            {
                if (this._particleSystem.isPaused)
                {
                    return 0f;
                }
                float deltaTime = 0f;
                if (Application.isPlaying)
                {
                    deltaTime = Time.deltaTime;
                }
                else if (this._lastFrameCount == this._frameCount)
                {
                    deltaTime = this._lastDeltaTime;
                }
                else
                {
                    deltaTime = Time.realtimeSinceStartup - this._lastTime;
                    this._lastTime = Time.realtimeSinceStartup;
                }
                this._lastFrameCount = this._frameCount;
                if (deltaTime > 0.03333334f)
                {
                    deltaTime = 0.03333334f;
                }
                this._lastDeltaTime = deltaTime;
                return (deltaTime * this.PlaybackSpeed);
            }
        }

        private class CollidedParticle
        {
            public Vector3 RealPosition;
            public Vector3 RealVelocity;
        }
    }
}

