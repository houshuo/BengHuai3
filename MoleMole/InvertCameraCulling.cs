namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Camera))]
    public class InvertCameraCulling : MonoBehaviour
    {
        private bool _oldCulling;

        public void OnPostRender()
        {
            GL.invertCulling = this._oldCulling;
        }

        public void OnPreRender()
        {
            this._oldCulling = GL.invertCulling;
            GL.invertCulling = true;
        }
    }
}

