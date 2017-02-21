using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class MonoFadeImage : MonoBehaviour
{
    private float _fadeAlpha;
    private float _fadeSpeed;
    private bool _fading;
    private Image blackFade;

    private void Awake()
    {
        this.InitFade();
    }

    public void FadeIn(float speed = 1f)
    {
        this._fading = true;
        this._fadeSpeed = -speed * Time.deltaTime;
    }

    public void FadeOut(float speed = 1f)
    {
        this._fading = true;
        this._fadeSpeed = speed * Time.deltaTime;
    }

    private void InitFade()
    {
        this.blackFade = base.GetComponent<Image>();
    }

    private void Update()
    {
        this.UpdateFade();
    }

    private void UpdateFade()
    {
        if ((this.blackFade != null) && this._fading)
        {
            this._fadeAlpha += this._fadeSpeed;
            if (this._fadeAlpha < 0f)
            {
                this._fadeAlpha = 0f;
                this._fading = false;
            }
            else if (this._fadeAlpha > 1f)
            {
                this._fadeAlpha = 1f;
                this._fading = false;
            }
            this.blackFade.color = new Color(0f, 0f, 0f, this._fadeAlpha);
        }
    }
}

