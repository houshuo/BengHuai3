namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoBuffShader_SpecialTransition : MonoBuffShader_Base
    {
        public static string DefaultShaderKeyword = "SPECIAL_STATE";
        public float SPEnterDuration = 0.2f;
        public float SPExitDuration = 0.5f;
        public float SPIntensity;
        public float SPNoiseScalar = 1f;
        public Texture SPNoiseTex;
        public Color SPOutlineColor = Color.white;
        public Texture SPTex;
        public float SPTransitionBloomFactor = 1f;
        public Color SPTransitionColor = Color.white;
        public float SPTransitionEmissionScalar = 1f;
        [HideInInspector]
        public string TransitionName = "_SPTransition";

        public void PushValue(ref Material mat)
        {
            mat.SetTexture("_SPTex", this.SPTex);
            mat.SetTexture("_SPNoiseTex", this.SPNoiseTex);
            mat.SetFloat("_SPNoiseScaler", this.SPNoiseScalar);
            mat.SetFloat("_SPIntensity", this.SPIntensity);
            mat.SetColor("_SPTransitionColor", this.SPTransitionColor);
            mat.SetColor("_SPOutlineColor", this.SPOutlineColor);
            mat.SetFloat("_SPTransitionEmissionScaler", this.SPTransitionEmissionScalar);
            mat.SetFloat("_SPTransitionBloomFactor", this.SPTransitionBloomFactor);
        }
    }
}

