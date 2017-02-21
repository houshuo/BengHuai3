namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    public class MonoImageFitter : MonoBehaviour
    {
        public bool fitX = true;
        public bool fitY;
        public Image image;

        public void FitImageSize()
        {
            if ((this.image != null) && (this.image.sprite != null))
            {
                float width = this.image.sprite.rect.width;
                float height = this.image.sprite.rect.height;
                Rect rect = base.GetComponent<RectTransform>().rect;
                if (this.fitX && (width > rect.width))
                {
                    height *= rect.width / width;
                    width = rect.width;
                }
                if (this.fitY && (height > rect.height))
                {
                    width *= rect.height / height;
                    height = rect.height;
                }
                this.image.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
                this.image.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }
        }

        public void Start()
        {
            this.FitImageSize();
        }
    }
}

