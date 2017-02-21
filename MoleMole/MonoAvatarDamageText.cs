namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoAvatarDamageText : MonoBehaviour
    {
        private Animation _animation;
        private Text _downText;
        private Text _upText;

        private void Init()
        {
            this._animation = base.GetComponent<Animation>();
            this._upText = base.transform.Find("UpText").GetComponent<Text>();
            this._downText = base.transform.Find("DownText").GetComponent<Text>();
        }

        public void SetupView(Type type, float damage, Vector3 pos)
        {
            this.Init();
            base.transform.position = pos;
            base.transform.SetLocalPositionZ(0f);
            if (type == Type.Up)
            {
                this._upText.gameObject.SetActive(true);
                this._downText.gameObject.SetActive(false);
                this._upText.text = string.Format("+{0}", UIUtil.FloorToIntCustom(-damage));
                this._animation.Play("DisplayAvatarHPUp");
            }
            else
            {
                this._downText.gameObject.SetActive(true);
                this._upText.gameObject.SetActive(false);
                this._downText.text = string.Format("-{0}", UIUtil.FloorToIntCustom(damage));
                this._animation.Play("DisplayAvatarHPDown");
            }
        }

        private void Update()
        {
            if (!this._animation.isPlaying)
            {
                base.gameObject.SetActive(false);
                Vector3 vector = Singleton<CameraManager>.Instance.GetMainCamera().WorldToUIPoint(Singleton<AvatarManager>.Instance.GetLocalAvatar().RootNodePosition);
                base.transform.SetPositionX(vector.x);
            }
        }

        public enum Type
        {
            Up,
            Down
        }
    }
}

