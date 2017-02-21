namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoAvatarStar : MonoBehaviour
    {
        private Image _image;
        private const int MAX_STAR = 5;
        public int star;

        public void Awake()
        {
            this._image = base.transform.Find("Image").GetComponent<Image>();
        }

        public void SetupView(int star)
        {
            this.star = star;
            if (this._image == null)
            {
                this._image = base.transform.Find("Image").GetComponent<Image>();
            }
            this._image.sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarStarIcons[this.star]);
        }
    }
}

