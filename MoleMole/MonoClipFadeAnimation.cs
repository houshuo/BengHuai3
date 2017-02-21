namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Animation))]
    public class MonoClipFadeAnimation : MonoAuxObject
    {
        private List<Material> _materialList;
        [Header("Use animation to key this instead of transform.rotation for working constant tangent")]
        public Vector4 keyedClipPlane;

        public void Awake()
        {
            this._materialList = new List<Material>();
            foreach (SkinnedMeshRenderer renderer in base.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                this._materialList.Add(renderer.material);
            }
        }

        private void LateUpdate()
        {
            for (int i = 0; i < this._materialList.Count; i++)
            {
                this._materialList[i].SetVector("_ClipPlane", this.keyedClipPlane);
            }
        }
    }
}

