using System;
using UnityEngine;

public class MonoAvatarFaceControlPart : MonoBehaviour
{
    private int _currentLeftEyeIndex;
    private int _currentMouthIndex;
    private int _currentRightEyeIndex;
    public Renderer leftEyeRenderer;
    public Texture2D[] leftEyeTextures;
    public Renderer mouthRenderer;
    public Texture2D[] mouthTextures;
    public Renderer rightEyeRenderer;
    public Texture2D[] rightEyeTextures;
    public float targetLeftEyeIndex;
    public float targetMouthIndex;
    public float targetRightEyeIndex;
    public bool useUpdateTargetIndex;

    private bool SetFacePart(Renderer renderer, Texture2D[] textures, int targetIndex, ref int currentIndex)
    {
        if (targetIndex != currentIndex)
        {
            if (renderer == null)
            {
                Debug.LogError("[GalTouch] face renderer not set : " + base.gameObject.name);
                return false;
            }
            if ((targetIndex < 0) || (targetIndex >= textures.Length))
            {
                Debug.LogError(string.Format("[GalTouch] face frame index({0}) out of range : {1}", targetIndex.ToString(), base.gameObject.name));
                return false;
            }
            renderer.material.mainTexture = textures[targetIndex];
            currentIndex = targetIndex;
        }
        return true;
    }

    public void SetLeftEye(int index)
    {
        this.SetFacePart(this.leftEyeRenderer, this.leftEyeTextures, index, ref this._currentLeftEyeIndex);
    }

    public void SetMouth(int index)
    {
        this.SetFacePart(this.mouthRenderer, this.mouthTextures, index, ref this._currentMouthIndex);
    }

    public void SetRightEye(int index)
    {
        this.SetFacePart(this.rightEyeRenderer, this.rightEyeTextures, index, ref this._currentRightEyeIndex);
    }

    private void Start()
    {
        this.SetLeftEye(0);
        this.SetRightEye(0);
        this.SetMouth(0);
    }

    private void Update()
    {
        if (this.useUpdateTargetIndex)
        {
            this.SetLeftEye((int) this.targetLeftEyeIndex);
            this.SetRightEye((int) this.targetRightEyeIndex);
            this.SetMouth((int) this.targetMouthIndex);
        }
    }
}

