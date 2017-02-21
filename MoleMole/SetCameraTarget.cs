namespace MoleMole
{
    using System;
    using UnityEngine;

    [ExecuteInEditMode, RequireComponent(typeof(Camera))]
    public class SetCameraTarget : MonoBehaviour
    {
        private RenderTexture _target;
        public DepthBitEnum DepthBit = DepthBitEnum.bit_16;
        public DownSampleEnum DownSample = DownSampleEnum.down_1x;
        public RenderTextureFormat TargetFormat;
        public string TargetNameInShader;

        private void Start()
        {
            Camera component = base.GetComponent<Camera>();
            this._target = new RenderTexture(Screen.width / this.DownSample, Screen.height / this.DownSample, 0x10, this.TargetFormat);
            component.SetTargetBuffers(this._target.colorBuffer, this._target.depthBuffer);
            component.targetTexture = this._target;
            Shader.SetGlobalTexture(this.TargetNameInShader, this._target);
        }

        public enum DepthBitEnum
        {
            bit_0 = 0,
            bit_16 = 0x10,
            bit_8 = 8
        }

        public enum DownSampleEnum
        {
            down_1x = 1,
            down_2x = 2,
            down_4x = 4
        }
    }
}

