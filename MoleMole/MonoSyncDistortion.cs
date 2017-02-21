namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoSyncDistortion : MonoBehaviour
    {
        private MaterialPropertyBlock _sourceMaterial;
        private List<KeyValuePair<MeshRenderer, MaterialPropertyBlock>> _targetMaterials;
        private const string DISTORTION_INTENSITY_NAME = "_DistortionIntensity";
        private const string DISTORTION_MATERIAL_NAME = "DistortionNormal";
        private const string DISTORTION_SPTRANSITION_NAME = "_SPTransition";
        private bool needSync;

        private void Start()
        {
            this._targetMaterials = new List<KeyValuePair<MeshRenderer, MaterialPropertyBlock>>();
            IEnumerator enumerator = base.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (current.GetComponent<MeshRenderer>() != null)
                    {
                        this.needSync = true;
                        MaterialPropertyBlock dest = new MaterialPropertyBlock();
                        current.GetComponent<MeshRenderer>().GetPropertyBlock(dest);
                        this._targetMaterials.Add(new KeyValuePair<MeshRenderer, MaterialPropertyBlock>(current.GetComponent<MeshRenderer>(), dest));
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            this._sourceMaterial = new MaterialPropertyBlock();
            base.transform.GetComponent<MeshRenderer>().GetPropertyBlock(this._sourceMaterial);
        }

        private void Update()
        {
            if (this.needSync)
            {
                base.transform.GetComponent<MeshRenderer>().GetPropertyBlock(this._sourceMaterial);
                foreach (KeyValuePair<MeshRenderer, MaterialPropertyBlock> pair in this._targetMaterials)
                {
                    pair.Value.SetFloat("_DistortionIntensity", this._sourceMaterial.GetFloat("_DistortionIntensity"));
                    pair.Value.SetFloat("_SPTransition", this._sourceMaterial.GetFloat("_SPTransition"));
                    if (pair.Key != null)
                    {
                        pair.Key.SetPropertyBlock(pair.Value);
                    }
                }
            }
        }
    }
}

