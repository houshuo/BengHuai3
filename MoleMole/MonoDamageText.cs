namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoDamageText : MonoBehaviour
    {
        private Animation _animation;
        private BaseMonoEntity _attackee;
        private Vector3 _positionOffset = Vector3.zero;
        private float _speedX;
        private List<Text> _textList;
        private DamageType _type;
        private float _uiPositionXOffset;

        private Vector3 GetUIPositionWithOffset()
        {
            Vector3 pos = this._attackee.XZPosition + this._positionOffset;
            return Singleton<CameraManager>.Instance.GetMainCamera().WorldToUIPoint(pos);
        }

        private void Init()
        {
            this._animation = base.GetComponent<Animation>();
            Text component = base.transform.Find("Text/Critical").GetComponent<Text>();
            Text text2 = base.transform.Find("Text/Normal").GetComponent<Text>();
            Text text3 = base.transform.Find("Text/Restrain").GetComponent<Text>();
            Text text4 = base.transform.Find("Text/ENormal").GetComponent<Text>();
            Text text5 = base.transform.Find("Text/EFire").GetComponent<Text>();
            Text text6 = base.transform.Find("Text/EThunder").GetComponent<Text>();
            Text text7 = base.transform.Find("Text/EIce").GetComponent<Text>();
            Text text8 = base.transform.Find("Text/EAllien").GetComponent<Text>();
            this._textList = new List<Text> { text2, component, text4, text5, text6, text7, text8, text3 };
        }

        public void SetupView(DamageType type, float damage, Vector3 pos, BaseMonoEntity attackee)
        {
            this._type = type;
            this._attackee = attackee;
            this._positionOffset = pos - attackee.XZPosition;
            this._uiPositionXOffset = 0f;
            if (this._textList == null)
            {
                this.Init();
            }
            base.transform.position = Singleton<CameraManager>.Instance.GetMainCamera().WorldToUIPoint(pos);
            base.transform.SetLocalPositionZ(0f);
            bool flag = Singleton<AvatarManager>.Instance.IsLocalAvatar(attackee.GetRuntimeID());
            for (int i = 0; i < this._textList.Count; i++)
            {
                if (i == this._type)
                {
                    this._textList[i].gameObject.SetActive(true);
                    int num2 = UIUtil.FloorToIntCustom(damage);
                    this._textList[i].text = (!flag ? num2 : -num2).ToString();
                }
                else
                {
                    this._textList[i].gameObject.SetActive(false);
                }
            }
            if (flag)
            {
                this._animation.Play("DisplayAvatarHPDown");
            }
            else if (type == DamageType.Critical)
            {
                this._animation.Play("DamageTextCrit");
            }
            else
            {
                this._animation.Play("DamageTextMove");
            }
            this._speedX = UnityEngine.Random.Range((float) -1f, (float) 1f);
        }

        private void Update()
        {
            if (!this._animation.isPlaying || (this._attackee == null))
            {
                base.gameObject.SetActive(false);
            }
            else
            {
                base.transform.position = this.GetUIPositionWithOffset();
                this._uiPositionXOffset += Time.deltaTime * this._speedX;
                base.transform.SetPositionX(base.transform.position.x + this._uiPositionXOffset);
                base.transform.SetLocalPositionZ(0f);
            }
        }
    }
}

