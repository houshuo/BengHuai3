namespace MoleMole.MainMenu
{
    using System;
    using UnityEngine;

    [Serializable]
    public class LightningType
    {
        private Lightning[] _buffer = new Lightning[10];
        private float _RemainEmitCount;
        [Range(0f, 100f)]
        public float CloudLitIntensinty = 1f;
        [Range(0f, 100f), Tooltip("Without lightning")]
        public float CloudLitIntensinty2 = 1f;
        public MaterialPropertyBlock DumbMPB;
        public Bounds EmitArea;
        [Range(0f, 1f)]
        public float EmitRate;
        [Range(0f, 100f)]
        public float EmitRateDeviation;
        public bool Enable = true;
        public static int FlashPointId;
        public AnimationCurve IntensityCurve;
        public LightningMaterial[] Materials;
        public static readonly int MaxFlashCount = 4;
        public static readonly int MaxFlashPoint = 6;
        [Range(0f, 10f)]
        public float MaxIntensity = 1f;
        [Range(0f, 10f)]
        public float MinIntensity = 1f;
        public string Name;
        public GameObject Prefab;
        public float Size;
        [Range(0f, 1f)]
        public float StartLifttime;
        [Range(0f, 1f)]
        public float StartLifttimeDeviation;
        [Tooltip("Relative y location of a cloud from which a lightning start"), Range(0f, 1f)]
        public float StartLocation;
        [Range(0f, 1f)]
        public float VisibleLightningRatio;

        public void Emit(Vector3 position, Vector3 velocity)
        {
            Lightning lightning = null;
            foreach (Lightning lightning2 in this._buffer)
            {
                if (lightning2 == null)
                {
                    if (Application.isPlaying)
                    {
                        Debug.LogError("Missing lightning object");
                        return;
                    }
                    UnityEngine.Object.FindObjectOfType<CloudEmitter>().Reset();
                    return;
                }
                if (!lightning2.Active)
                {
                    lightning = lightning2;
                    break;
                }
            }
            if (lightning != null)
            {
                bool lightningVisible = UnityEngine.Random.value < this.VisibleLightningRatio;
                lightning.Emit(position, velocity, lightningVisible, this);
            }
        }

        public int GetEmitCount(float deltaTime)
        {
            this._RemainEmitCount += (this.EmitRate * (1f + (((UnityEngine.Random.value * 2f) - 1f) * this.EmitRateDeviation))) * deltaTime;
            int num = Mathf.FloorToInt(this._RemainEmitCount);
            if (num < 1)
            {
                return 0;
            }
            this._RemainEmitCount -= num;
            return num;
        }

        public void Init(CloudEmitter cloudEmitter)
        {
            this.DumbMPB = new MaterialPropertyBlock();
            this.DumbMPB.SetFloat("_TexOffsetX", 0f);
            this.DumbMPB.SetFloat("_TexScaleX", 0.25f);
            this.DumbMPB.SetFloat("_EmissionScaler", 1f);
            for (int i = 0; i < this._buffer.Length; i++)
            {
                this._buffer[i] = new Lightning();
                this._buffer[i].Init(this.Prefab, cloudEmitter.transform, this.VisibleLightningRatio > 0.01f);
            }
        }

        public static void PreUpdate(Material cloudMaterial)
        {
            FlashPointId = 0;
            for (int i = 0; i < MaxFlashPoint; i++)
            {
                cloudMaterial.SetVector("_FlashPoint0" + i, Vector4.zero);
            }
        }

        public void Update(float deltaTime, float realDeltaTime, Material cloudMaterial)
        {
            foreach (Lightning lightning in this._buffer)
            {
                if (lightning == null)
                {
                    if (!Application.isPlaying)
                    {
                        UnityEngine.Object.FindObjectOfType<CloudEmitter>().Reset();
                        return;
                    }
                    Debug.LogError("Missing lightning object");
                }
                if (lightning.Active)
                {
                    lightning.Update(deltaTime, realDeltaTime, this, ref FlashPointId, cloudMaterial);
                }
            }
        }

        [Serializable]
        public class LightningMaterial
        {
            public Vector3 pivot;
            public float texOffsetX;
        }
    }
}

