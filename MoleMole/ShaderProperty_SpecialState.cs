namespace MoleMole
{
    using System;
    using UnityEngine;

    public class ShaderProperty_SpecialState : ShaderProperty_Base
    {
        public float _SPIntensity;
        public float _SPNoiseScaler;
        public Color _SPOutlineColor;
        public float _SPTransition;
        public float _SPTransitionBloomFactor;
        public Color _SPTransitionColor;
        public float _SPTransitionEmissionScaler;

        public override void LerpTo(Material targetMat, ShaderProperty_Base to_, float normalized)
        {
            ShaderProperty_SpecialState state = (ShaderProperty_SpecialState) to_;
            targetMat.SetFloat("_SPNoiseScaler", Mathf.Lerp(this._SPNoiseScaler, state._SPNoiseScaler, normalized));
            targetMat.SetFloat("_SPIntensity", Mathf.Lerp(this._SPIntensity, state._SPIntensity, normalized));
            targetMat.SetFloat("_SPTransition", Mathf.Lerp(this._SPTransition, state._SPTransition, normalized));
            targetMat.SetColor("_SPTransitionColor", Color.Lerp(this._SPTransitionColor, state._SPTransitionColor, normalized));
            targetMat.SetColor("_SPOutlineColor", Color.Lerp(this._SPOutlineColor, state._SPOutlineColor, normalized));
            targetMat.SetFloat("_SPTransitionEmissionScaler", Mathf.Lerp(this._SPTransitionEmissionScaler, state._SPTransitionEmissionScaler, normalized));
            targetMat.SetFloat("_SPTransitionBloomFactor", Mathf.Lerp(this._SPTransitionBloomFactor, state._SPTransitionBloomFactor, normalized));
        }
    }
}

