namespace MoleMole
{
    using System;
    using UnityEngine;

    public class ShaderProperty_Shell : ShaderProperty_Base
    {
        public Color _ShellColor;
        public float _ShellEmission;
        public float _ShellNormalDisplacment;
        public Vector2 Tiling;

        public override void LerpTo(Material targetMat, ShaderProperty_Base to_, float normalized)
        {
            ShaderProperty_Shell shell = (ShaderProperty_Shell) to_;
            targetMat.SetFloat("_ShellNormalDisplacment", Mathf.Lerp(this._ShellNormalDisplacment, shell._ShellNormalDisplacment, normalized));
            targetMat.SetColor("_ShellColor", Color.Lerp(this._ShellColor, shell._ShellColor, normalized));
            targetMat.SetFloat("_ShellEmission", Mathf.Lerp(this._ShellEmission, shell._ShellEmission, normalized));
            targetMat.SetTextureScale("_ShellTex", Vector2.Lerp(this.Tiling, shell.Tiling, normalized));
        }
    }
}

