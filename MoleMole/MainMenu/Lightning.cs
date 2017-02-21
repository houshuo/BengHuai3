namespace MoleMole.MainMenu
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Lightning
    {
        private LightningObject[] _objects = new LightningObject[LightningType.MaxFlashCount];
        private Transform _parentTransform;
        public bool Active;
        public Vector3 EndPosition = Vector3.zero;
        public int FlashCount;
        public bool LightningVisible;
        public Vector3 StartPosition = Vector3.zero;
        public Vector3 Velocity = Vector3.zero;

        public void Emit(Vector3 position, Vector3 velocity, bool lightningVisible, LightningType lightningType)
        {
            this.Active = true;
            this.LightningVisible = lightningVisible;
            this.StartPosition = position;
            this.EndPosition = this.StartPosition - ((Vector3) (Vector3.up * lightningType.Size));
            this.Velocity = velocity;
            this.FlashCount = UnityEngine.Random.Range(1, LightningType.MaxFlashCount);
            float num = 0f;
            HashSet<int> set = new HashSet<int>();
            int length = lightningType.Materials.Length;
            for (int i = 0; i < this.FlashCount; i++)
            {
                LightningObject obj2 = this._objects[i];
                if (lightningVisible)
                {
                    int num4;
                    if (set.Count >= length)
                    {
                        break;
                    }
                    do
                    {
                        num4 = UnityEngine.Random.Range(0, length);
                    }
                    while (set.Contains(num4));
                    set.Add(num4);
                    obj2.Mat = lightningType.Materials[num4];
                    obj2.UpdatePosition(position);
                }
                obj2.Intensity = UnityEngine.Random.Range(lightningType.MinIntensity, lightningType.MaxIntensity);
                obj2.Size = lightningType.Size;
                float num5 = lightningType.StartLifttime * (1f + (((UnityEngine.Random.value * 2f) - 1f) * lightningType.StartLifttimeDeviation));
                obj2.StartLifttime = num5 + num;
                obj2.DelayTime = num;
                num += num5;
                obj2.Lifttime = 0f;
                obj2.Active = true;
            }
        }

        public void Init(GameObject prefab, Transform parent, bool needLightning)
        {
            this._parentTransform = parent;
            for (int i = 0; i < this._objects.Length; i++)
            {
                LightningObject obj2 = new LightningObject();
                this._objects[i] = obj2;
                if (needLightning)
                {
                    obj2.Object = UnityEngine.Object.Instantiate<GameObject>(prefab);
                    obj2.Object.hideFlags = HideFlags.DontSave;
                    Transform transform = obj2.Object.transform;
                    transform.parent = this._parentTransform;
                    transform.localPosition = Vector3.zero;
                    obj2.OrigScale = transform.localScale;
                }
                obj2.Active = false;
            }
        }

        private void SetFlashPoint(Vector3 position, float intensity, Material cloudMaterial, ref int flashPointId)
        {
            if (flashPointId < LightningType.MaxFlashPoint)
            {
                Vector3 vector = this._parentTransform.TransformPoint(position);
                cloudMaterial.SetVector("_FlashPoint0" + ((int) flashPointId), new Vector4(vector.x, vector.y, vector.z, intensity));
                flashPointId++;
            }
        }

        public void Update(float deltaTime, float realDeltaTime, LightningType lightningType, ref int flashPointId, Material cloudMaterial)
        {
            bool flag = false;
            float num = 0f;
            for (int i = 0; i < this.FlashCount; i++)
            {
                LightningObject obj2 = this._objects[i];
                if (obj2.Active)
                {
                    flag = true;
                    obj2.Lifttime += realDeltaTime;
                    if (this.LightningVisible && (obj2.Lifttime > obj2.DelayTime))
                    {
                        obj2.Show(true);
                    }
                    if (obj2.Lifttime > obj2.StartLifttime)
                    {
                        obj2.Active = false;
                    }
                    float num3 = lightningType.IntensityCurve.Evaluate((obj2.Lifttime - obj2.DelayTime) / (obj2.StartLifttime - obj2.DelayTime)) * obj2.Intensity;
                    num += num3;
                    if (this.LightningVisible)
                    {
                        Transform transform = obj2.Object.transform;
                        transform.localPosition += (Vector3) (this.Velocity * deltaTime);
                        lightningType.DumbMPB.SetFloat("_TexOffsetX", obj2.Mat.texOffsetX);
                        lightningType.DumbMPB.SetFloat("_EmissionScaler", num3);
                        obj2.Object.GetComponent<Renderer>().SetPropertyBlock(lightningType.DumbMPB);
                    }
                }
            }
            this.Active = flag;
            if (this.Active)
            {
                float intensity = num * (!this.LightningVisible ? lightningType.CloudLitIntensinty2 : lightningType.CloudLitIntensinty);
                this.SetFlashPoint(this.StartPosition, intensity, cloudMaterial, ref flashPointId);
                if (this.LightningVisible)
                {
                    this.SetFlashPoint(this.EndPosition, intensity, cloudMaterial, ref flashPointId);
                }
            }
        }
    }
}

