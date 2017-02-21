namespace MoleMole
{
    using System;
    using UnityEngine;

    public abstract class RendererFader : IAlphaFader
    {
        protected int _propertyID;
        protected Renderer _renderer;

        protected RendererFader()
        {
        }

        public abstract void LerpAlpha(float t);
    }
}

