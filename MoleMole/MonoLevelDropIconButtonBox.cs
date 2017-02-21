namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoLevelDropIconButtonBox : MonoBehaviour
    {
        private bool _isOpen;
        private Type _type;

        public string GetOpenAnimationName()
        {
            return (!this.IsSenior() ? "DropItemBoxOpenOrdinary" : "DropItemBoxOpenSenior");
        }

        public bool IsSenior()
        {
            return (this._type != Type.DefaultDrop);
        }

        public void SetItemAfterAnimation()
        {
            base.transform.SetLocalScaleX(0.7f);
            base.transform.SetLocalScaleY(0.7f);
            Transform transform = base.transform.Find("Item");
            transform.SetLocalScaleX(1f);
            transform.SetLocalScaleY(1f);
        }

        public void SetOpenStatusView(bool isOpen)
        {
            this._isOpen = isOpen;
        }

        public void SetupTypeView(Type type, bool isOpen)
        {
            this._type = type;
            this._isOpen = isOpen;
            this.SetupView();
        }

        private void SetupView()
        {
            bool flag = this.IsSenior();
            base.transform.Find("Item").gameObject.SetActive(this._isOpen);
            base.transform.Find("Senior").gameObject.SetActive(!this._isOpen && flag);
            base.transform.Find("Ordinary").gameObject.SetActive(!this._isOpen && !flag);
            switch (this._type)
            {
                case Type.DefaultDrop:
                    base.transform.Find("Ordinary/Box2/Label").gameObject.SetActive(false);
                    break;

                case Type.NormalFinishChallengeReward:
                    base.transform.Find("Senior/Box2/Label").gameObject.SetActive(true);
                    base.transform.Find("Senior/Box2/Label/NormalSpeed").gameObject.SetActive(true);
                    base.transform.Find("Senior/Box2/Label/FastSpeed").gameObject.SetActive(false);
                    base.transform.Find("Senior/Box2/Label/TopSpeed").gameObject.SetActive(false);
                    break;

                case Type.FastFinishChallengeReward:
                    base.transform.Find("Senior/Box2/Label").gameObject.SetActive(true);
                    base.transform.Find("Senior/Box2/Label/NormalSpeed").gameObject.SetActive(false);
                    base.transform.Find("Senior/Box2/Label/FastSpeed").gameObject.SetActive(true);
                    base.transform.Find("Senior/Box2/Label/TopSpeed").gameObject.SetActive(false);
                    break;

                case Type.SonicFinishChallengeReward:
                    base.transform.Find("Senior/Box2/Label").gameObject.SetActive(true);
                    base.transform.Find("Senior/Box2/Label/NormalSpeed").gameObject.SetActive(false);
                    base.transform.Find("Senior/Box2/Label/FastSpeed").gameObject.SetActive(false);
                    base.transform.Find("Senior/Box2/Label/TopSpeed").gameObject.SetActive(true);
                    break;
            }
        }

        public enum Type
        {
            DefaultDrop,
            NormalFinishChallengeReward,
            FastFinishChallengeReward,
            SonicFinishChallengeReward
        }
    }
}

