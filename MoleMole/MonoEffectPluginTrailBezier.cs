namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginTrailBezier : MonoEffectPluginTrail
    {
        private int _pointReady;
        private Vector3[] _points;
        private const int CURVE_DEGREE = 4;
        public int InterpolationSteps = 7;
        public float SmoothValue = 1f;

        protected override void Awake()
        {
            base.Awake();
            this._points = new Vector3[4];
            this._pointReady = 0;
        }

        private void DrawBezierCurve()
        {
            Vector3 zero = Vector3.zero;
            Vector3 vector2 = Vector3.zero;
            Bezier.GetControlPoint(this._points[0], this._points[1], this._points[2], this._points[3], this.SmoothValue, ref zero, ref vector2);
            float num = 1f / ((float) this.InterpolationSteps);
            for (int i = 0; i < this.InterpolationSteps; i++)
            {
                base.TrailRendererTransform.position = Bezier.GetPoint(this._points[1], zero, vector2, this._points[2], i * num);
            }
        }

        public override void Setup()
        {
        }

        protected override void Update()
        {
            if ((base._curFrame < base.FramePosList.Length) && !this.IsToBeRemove())
            {
                if (this._pointReady < 4)
                {
                    this._pointReady++;
                }
                for (int i = 1; i < 4; i++)
                {
                    this._points[i - 1] = this._points[i];
                }
                base.AniAnchorTransform.localPosition = base.FramePosList[base._curFrame];
                this._points[this._points.Length - 1] = base.AniAnchorTransform.position;
                if (this._pointReady >= 4)
                {
                    this.DrawBezierCurve();
                }
                base._curFrame++;
            }
        }
    }
}

