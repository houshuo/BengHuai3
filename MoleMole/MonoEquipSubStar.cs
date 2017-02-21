namespace MoleMole
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(GridLayoutGroup))]
    public class MonoEquipSubStar : MonoBehaviour
    {
        private string _activeImageName = "ActiveImage";
        private string _unactiveImageName = "UnactiveImage";
        public int activeStars;
        public Dircetion dirction;
        public int maxStars = 7;

        public void Awake()
        {
            this.SetupView(this.activeStars, this.maxStars);
        }

        public void SetupView(int activeStars, int maxStars)
        {
            this.activeStars = activeStars;
            this.maxStars = maxStars;
            GridLayoutGroup component = base.GetComponent<GridLayoutGroup>();
            bool flag = false;
            switch (this.dirction)
            {
                case Dircetion.LeftToRight:
                    component.startCorner = GridLayoutGroup.Corner.UpperLeft;
                    component.childAlignment = TextAnchor.MiddleLeft;
                    break;

                case Dircetion.RightToLeft:
                    component.startCorner = GridLayoutGroup.Corner.UpperRight;
                    component.childAlignment = TextAnchor.MiddleRight;
                    flag = true;
                    break;

                case Dircetion.Center:
                    component.startCorner = GridLayoutGroup.Corner.UpperLeft;
                    component.childAlignment = TextAnchor.MiddleCenter;
                    break;
            }
            IEnumerator enumerator = base.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    int num = int.Parse(current.name);
                    bool flag2 = (!flag && (num <= maxStars)) || (flag && (num > (base.transform.childCount - maxStars)));
                    current.gameObject.SetActive(flag2);
                    if (flag2)
                    {
                        bool flag3 = (!flag && (num <= activeStars)) || (flag && ((base.transform.childCount - num) < activeStars));
                        current.Find(this._activeImageName).gameObject.SetActive(flag3);
                        current.Find(this._unactiveImageName).gameObject.SetActive(!flag3);
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
        }

        public enum Dircetion
        {
            LeftToRight,
            RightToLeft,
            Center
        }
    }
}

