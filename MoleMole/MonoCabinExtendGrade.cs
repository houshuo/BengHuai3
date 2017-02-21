namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(GridLayoutGroup))]
    public class MonoCabinExtendGrade : MonoBehaviour
    {
        private Color _darkColor;
        private Color _lightColor;
        public bool HideDarkStar = true;
        private const int MAX_STAR = 5;
        public int star;

        private void InitColor()
        {
            this._darkColor = UIUtil.SetupColor("#00009B37");
            this._lightColor = UIUtil.SetupColor("#FFFFFFFF");
        }

        public void SetupView(int star)
        {
            this.star = star;
            GridLayoutGroup component = base.GetComponent<GridLayoutGroup>();
            this.InitColor();
            component.startCorner = GridLayoutGroup.Corner.UpperLeft;
            component.childAlignment = TextAnchor.MiddleLeft;
            for (int i = 1; i <= 5; i++)
            {
                Transform transform = base.transform.Find(i.ToString());
                Image image = transform.GetComponent<Image>();
                if (i <= star)
                {
                    image.color = this._lightColor;
                    transform.gameObject.SetActive(true);
                }
                else
                {
                    image.color = this._darkColor;
                    transform.gameObject.SetActive(!this.HideDarkStar);
                }
            }
        }
    }
}

