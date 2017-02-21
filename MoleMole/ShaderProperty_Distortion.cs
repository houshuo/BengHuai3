namespace MoleMole
{
    using System;
    using UnityEngine;

    public class ShaderProperty_Distortion : ShaderProperty_Base
    {
        public Vector4 _DTFresnel;
        public float _DTIntensity;
        public float _DTNormalDisplacment;
        public float _DTPlaySpeed;
        public float _DTUVScaleInX;
        public float _DTUVScaleInY;

        public override void LerpTo(Material targetMat, ShaderProperty_Base to_, float normalized)
        {
            ShaderProperty_Distortion distortion = (ShaderProperty_Distortion) to_;
            targetMat.SetFloat("_DTIntensity", Mathf.Lerp(this._DTIntensity, distortion._DTIntensity, normalized));
            targetMat.SetFloat("_DTPlaySpeed", Mathf.Lerp(this._DTPlaySpeed, distortion._DTPlaySpeed, normalized));
            targetMat.SetFloat("_DTNormalDisplacment", Mathf.Lerp(this._DTNormalDisplacment, distortion._DTNormalDisplacment, normalized));
            targetMat.SetFloat("_DTUVScaleInX", Mathf.Lerp(this._DTUVScaleInX, distortion._DTUVScaleInX, normalized));
            targetMat.SetFloat("_DTUVScaleInY", Mathf.Lerp(this._DTUVScaleInY, distortion._DTUVScaleInY, normalized));
            targetMat.SetVector("_DTFresnel", Vector4.Lerp(this._DTFresnel, distortion._DTFresnel, normalized));
        }
    }
}

