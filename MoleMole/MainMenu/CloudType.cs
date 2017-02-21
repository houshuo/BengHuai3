namespace MoleMole.MainMenu
{
    using MoleMole;
    using System;
    using UnityEngine;

    [Serializable]
    public class CloudType
    {
        private float _RemainEmitCount;
        public int EmitCount;
        public float EmitRate;
        public bool Enable = true;
        public int[] MaterialIds;
        [Range(0f, 1f)]
        public float MeanX = 0.5f;
        [Range(0f, 1f)]
        public float MeanY = 0.5f;
        public string Name;
        [Range(0f, 1f)]
        public float Size;
        [Range(0f, 1f)]
        public float SizeDeviation;
        [Range(0f, 1f)]
        public float StdDevX = 0.5f;
        [Range(0f, 1f)]
        public float StdDevY = 0.5f;

        public int GetEmitCount(float deltaTime)
        {
            this._RemainEmitCount += (this.EmitRate * (1f + (((UnityEngine.Random.value * 2f) - 1f) * 0.2f))) * deltaTime;
            int num = Mathf.FloorToInt(this._RemainEmitCount);
            this._RemainEmitCount -= num;
            return num;
        }

        public Vector2 GetRandomPosition()
        {
            float num;
            float num2;
            do
            {
                num = GaussianRandom.Val(this.MeanX, this.StdDevX);
            }
            while ((num < 0f) || (num > 1f));
            do
            {
                num2 = GaussianRandom.Val(this.MeanY, this.StdDevY);
            }
            while ((num2 < 0f) || (num2 > 1f));
            return new Vector2(num, num2);
        }

        public float GetRandomSize()
        {
            return (this.Size * (1f + (((UnityEngine.Random.value * 2f) - 1f) * this.SizeDeviation)));
        }
    }
}

