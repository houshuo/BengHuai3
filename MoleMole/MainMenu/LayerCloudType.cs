namespace MoleMole.MainMenu
{
    using MoleMole;
    using System;
    using UnityEngine;

    [Serializable]
    public class LayerCloudType
    {
        private bool _isOdd = true;
        private float _remainEmitCount;
        public float EmitRate;
        public bool Enable = true;
        [Range(0f, 1f)]
        public float Gap;
        [Range(0f, 1f)]
        public float GapDeviation;
        [Range(0f, 1f)]
        public float InterleavedOffset;
        public int[] MaterialIds;
        [Range(0f, 1f)]
        public float MeanY = 0.5f;
        public string Name;
        [Range(0f, 1f)]
        public float Size;
        [Range(0f, 1f)]
        public float SizeDeviation;
        [Range(0f, 1f)]
        public float StdDevY = 0.5f;

        public float GetRandomGap()
        {
            return (this.Gap * (1f + (((UnityEngine.Random.value * 2f) - 1f) * this.GapDeviation)));
        }

        public float GetRandomPositionY()
        {
            float num;
            do
            {
                num = GaussianRandom.Val(this.MeanY, this.StdDevY);
            }
            while ((num < 0f) || (num > 1f));
            return num;
        }

        public float GetRandomSize()
        {
            return (this.Size * (1f + (((UnityEngine.Random.value * 2f) - 1f) * this.SizeDeviation)));
        }

        public bool IsEmit(float deltaTime)
        {
            this._remainEmitCount += (this.EmitRate * (1f + (((UnityEngine.Random.value * 2f) - 1f) * 0.2f))) * deltaTime;
            if (this._remainEmitCount > 1f)
            {
                this._remainEmitCount -= Mathf.FloorToInt(this._remainEmitCount);
                return true;
            }
            return false;
        }

        public bool IsOdd()
        {
            this._isOdd = !this._isOdd;
            return !this._isOdd;
        }
    }
}

