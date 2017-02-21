namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(GridLayoutGroup))]
    public class MonoEquipStar : MonoBehaviour
    {
        public Dircetion dirction;
        private const int MAX_STAR = 7;
        public int star;

        public void Awake()
        {
            this.SetupView(this.star);
        }

        public void SetupView(int star)
        {
            this.star = star;
            GridLayoutGroup component = base.GetComponent<GridLayoutGroup>();
            switch (this.dirction)
            {
                case Dircetion.LeftToRight:
                    component.startCorner = GridLayoutGroup.Corner.UpperLeft;
                    component.childAlignment = TextAnchor.MiddleLeft;
                    break;

                case Dircetion.RightToLeft:
                    component.startCorner = GridLayoutGroup.Corner.UpperRight;
                    component.childAlignment = TextAnchor.MiddleRight;
                    break;

                case Dircetion.Center:
                    component.startCorner = GridLayoutGroup.Corner.UpperLeft;
                    component.childAlignment = TextAnchor.MiddleCenter;
                    break;
            }
            for (int i = 1; i <= 7; i++)
            {
                base.transform.Find(i.ToString()).gameObject.SetActive(i <= star);
            }
        }

        public enum Dircetion
        {
            LeftToRight,
            RightToLeft,
            Center
        }
    }
}

