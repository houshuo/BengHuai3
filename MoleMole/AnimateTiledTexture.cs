namespace MoleMole
{
    using System;
    using UnityEngine;

    public class AnimateTiledTexture : MonoBehaviour
    {
        [Header("Frames per second")]
        public float _framesPerSecond = 30f;
        [Header("Phase")]
        public int _phase;
        [Header("UV animation tile X")]
        public int _uvAnimationTileX = 4;
        [Header("UV animation tile Y")]
        public int _uvAnimationTileY = 0x10;

        private void Update()
        {
            int num = (int) (Time.time * this._framesPerSecond);
            num = (num + this._phase) % (this._uvAnimationTileX * this._uvAnimationTileY);
            Vector2 scale = new Vector2(1f / ((float) this._uvAnimationTileX), 1f / ((float) this._uvAnimationTileY));
            int num2 = num % this._uvAnimationTileX;
            int num3 = num / this._uvAnimationTileX;
            Vector2 offset = new Vector2(num2 * scale.x, (1f - scale.y) - (num3 * scale.y));
            base.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", offset);
            base.GetComponent<Renderer>().material.SetTextureScale("_MainTex", scale);
        }
    }
}

