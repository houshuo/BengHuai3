namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(ImageSmoothMask))]
    public class ImageForSmoothMask : Image
    {
        private ImageSmoothMask _mask;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            base.sprite = null;
        }

        protected override void OnEnable()
        {
            this._mask = base.GetComponent<ImageSmoothMask>();
            base.OnEnable();
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (this._mask.coverRatio >= 0.01f)
            {
                if (this._mask.coverRatio > 0.99f)
                {
                    base.OnPopulateMesh(vh);
                }
                else if (Application.isPlaying)
                {
                    List<UIVertex[]> list = this._mask.GenerateUIQuads();
                    for (int i = 0; i < list.Count; i++)
                    {
                        vh.AddUIVertexQuad(list[i]);
                    }
                }
            }
        }
    }
}

