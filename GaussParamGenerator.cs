using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class GaussParamGenerator
{
    private Vector4[] _offsets;
    private float[] _weights;

    public GaussParamGenerator(float s, float imageWidth)
    {
        this.CalcParams(s, imageWidth);
    }

    private void CalcParams(float s, float imageWidth)
    {
        int num;
        float[] numArray = this.generateGaussianWeights(s, out num);
        int num2 = (2 * num) + 1;
        int num3 = Mathf.CeilToInt(((float) num2) / 2f);
        this._weights = new float[num3];
        this._offsets = new Vector4[Mathf.CeilToInt(((float) num3) / 2f)];
        for (int i = 0; i < this._offsets.Length; i++)
        {
            this._offsets[i] = Vector4.zero;
        }
        for (int j = 0; j < num3; j++)
        {
            float num7;
            float num6 = numArray[j * 2];
            if (((j * 2) + 1) > (num2 - 1))
            {
                num7 = 0f;
            }
            else
            {
                num7 = numArray[(j * 2) + 1];
            }
            this._weights[j] = num6 + num7;
            float num8 = num7 / (num6 + num7);
            float num9 = ((j * 2) - num) + num8;
            num9 /= imageWidth;
            this._offsets[j / 2][(j % 2) * 2] = num9;
            this._offsets[j / 2][((j % 2) * 2) + 1] = num9;
        }
    }

    private float gaussian(float x, float s)
    {
        return Mathf.Exp(-(s * x) * (s * x));
    }

    private float[] generateGaussianWeights(float s, out int width)
    {
        width = (int) (3f / s);
        int num = (width * 2) + 1;
        float[] numArray = new float[num];
        float num2 = 0f;
        for (int i = 0; i < num; i++)
        {
            numArray[i] = this.gaussian(i - ((float) width), s);
            num2 += numArray[i];
        }
        for (int j = 0; j < num; j++)
        {
            numArray[j] /= num2;
        }
        return numArray;
    }

    public Vector4[] Offsets
    {
        get
        {
            return this._offsets;
        }
    }

    public float[] Weights
    {
        get
        {
            return this._weights;
        }
    }
}

