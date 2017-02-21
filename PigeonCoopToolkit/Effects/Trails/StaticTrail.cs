namespace PigeonCoopToolkit.Effects.Trails
{
    using PigeonCoopToolkit.Utillities;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class StaticTrail : TrailRenderer_Base
    {
        protected AnimationCurve _appearCurve;
        protected float _appearDuration;
        private CircularBuffer<ControlPoint> _controlPoints;
        protected float _timer;
        protected AnimationCurve _vanishCurve;
        protected float _vanishDuration;
        public int PointsBetweenControlPoints = 4;
        [NonSerialized]
        public float TimeScale = 1f;

        protected virtual void AddControlPoint(Vector3 position, bool isEndPoint = false)
        {
            if (!isEndPoint)
            {
                for (int i = 0; i < this.PointsBetweenControlPoints; i++)
                {
                    this.AddPoint(new PCTrailPoint(), position);
                }
            }
            this.AddPoint(new PCTrailPoint(), position);
            ControlPoint item = new ControlPoint {
                p = position
            };
            if (base.TrailData.UseForwardOverride)
            {
                item.forward = base.TrailData.ForwardOverride.normalized;
            }
            this._controlPoints.Add(item);
        }

        protected void AddPoint(PCTrailPoint newPoint, Vector3 pos)
        {
            if (base._activeTrail != null)
            {
                newPoint.Position = pos;
                newPoint.PointNumber = (base._activeTrail.Points.Count != 0) ? (base._activeTrail.Points[base._activeTrail.Points.Count - 1].PointNumber + 1) : 0;
                this.InitialiseNewPoint(newPoint);
                newPoint.SetDistanceFromStart((base._activeTrail.Points.Count != 0) ? (base._activeTrail.Points[base._activeTrail.Points.Count - 1].GetDistanceFromStart() + Vector3.Distance(base._activeTrail.Points[base._activeTrail.Points.Count - 1].Position, pos)) : 0f);
                if (base.TrailData.UseForwardOverride)
                {
                    newPoint.Forward = base.TrailData.ForwardOverride.normalized;
                }
                base._activeTrail.Points.Add(newPoint);
            }
        }

        protected override void Awake()
        {
            base._t = base.transform;
            base._emit = false;
            Material[] materials = base.TrailData.GetMaterialsContainer().materials;
            for (int i = 0; i < materials.Length; i++)
            {
                string tag = materials[i].GetTag("Distortion", false);
                materials[i] = new Material(materials[i]);
                materials[i].SetOverrideTag("Distortion", tag);
                Material material1 = materials[i];
                material1.name = material1.name + "(Instance)";
            }
        }

        private void GenerateMesh(PCTrail trail)
        {
            trail.Mesh.Clear(false);
            Vector3 rhs = (Camera.main == null) ? Vector3.forward : Camera.main.transform.forward;
            if (base.TrailData.UseForwardOverride)
            {
                rhs = base.TrailData.ForwardOverride.normalized;
            }
            trail.activePointCount = trail.Points.Count;
            if (trail.activePointCount >= 2)
            {
                int index = 0;
                for (int i = 0; i < trail.Points.Count; i++)
                {
                    PCTrailPoint point = trail.Points[i];
                    if (point.TimeActive() <= base.TrailData.Lifetime)
                    {
                        if (base.TrailData.UseForwardOverride && base.TrailData.ForwardOverrideRelative)
                        {
                            rhs = point.Forward;
                        }
                        Vector3 zero = Vector3.zero;
                        if (i < (trail.Points.Count - 1))
                        {
                            if (base.TrailData.UseForwardOverride && base.TrailData.ForwardOverrideRelative)
                            {
                                Vector3 vector5 = trail.Points[i + 1].Position - point.Position;
                                zero = Vector3.Cross(vector5.normalized, rhs).normalized;
                            }
                            else
                            {
                                Vector3 vector7 = trail.Points[i + 1].Position - point.Position;
                                zero = vector7.normalized;
                            }
                        }
                        else if (base.TrailData.UseForwardOverride && base.TrailData.ForwardOverrideRelative)
                        {
                            Vector3 vector8 = point.Position - trail.Points[i - 1].Position;
                            zero = Vector3.Cross(vector8.normalized, rhs).normalized;
                        }
                        else
                        {
                            Vector3 vector10 = point.Position - trail.Points[i - 1].Position;
                            zero = vector10.normalized;
                        }
                        trail.verticies[index] = point.Position;
                        trail.normals[index] = (Vector3) (zero * base.StretchUpRatio);
                        if (base.clockSize)
                        {
                            trail.uvs[index] = new Vector2(point.GetDistanceFromStart() / trail.Points[trail.Points.Count - 1].GetDistanceFromStart(), 1f);
                        }
                        else
                        {
                            trail.uvs[index] = new Vector2(point.GetDistanceFromStart() / trail.Points[trail.Points.Count - 1].GetDistanceFromStart(), 0f);
                        }
                        trail.colors[index] = Color.white;
                        index++;
                        trail.verticies[index] = point.Position;
                        trail.normals[index] = (Vector3) (-zero * base.StretchDownRatio);
                        if (base.clockSize)
                        {
                            trail.uvs[index] = new Vector2(point.GetDistanceFromStart() / trail.Points[trail.Points.Count - 1].GetDistanceFromStart(), 0f);
                        }
                        else
                        {
                            trail.uvs[index] = new Vector2(point.GetDistanceFromStart() / trail.Points[trail.Points.Count - 1].GetDistanceFromStart(), 1f);
                        }
                        trail.colors[index] = Color.white;
                        index++;
                    }
                }
                Vector2 vector3 = trail.verticies[index - 1];
                Vector3 vector4 = trail.normals[index - 1];
                for (int j = index; j < trail.verticies.Length; j++)
                {
                    trail.verticies[j] = (Vector3) vector3;
                    trail.normals[j] = vector4;
                }
                int num4 = 0;
                for (int k = 0; k < (2 * (trail.activePointCount - 1)); k++)
                {
                    if ((k % 2) == 0)
                    {
                        trail.indicies[num4] = k;
                        num4++;
                        trail.indicies[num4] = k + 1;
                        num4++;
                        trail.indicies[num4] = k + 2;
                    }
                    else
                    {
                        trail.indicies[num4] = k + 2;
                        num4++;
                        trail.indicies[num4] = k + 1;
                        num4++;
                        trail.indicies[num4] = k;
                    }
                    num4++;
                }
                int num6 = trail.indicies[num4 - 1];
                for (int m = num4; m < trail.indicies.Length; m++)
                {
                    trail.indicies[m] = num6;
                }
                trail.Mesh.vertices = trail.verticies;
                trail.Mesh.SetIndices(trail.indicies, MeshTopology.Triangles, 0);
                trail.Mesh.uv = trail.uvs;
                trail.Mesh.normals = trail.normals;
                trail.Mesh.colors = trail.colors;
            }
        }

        protected virtual void GenerateTrail(PCTrail trail)
        {
            int num = 0;
            for (int i = 0; i < this._controlPoints.Count; i++)
            {
                trail.Points[num].Position = this._controlPoints[i].p;
                if (base.TrailData.UseForwardOverride)
                {
                    trail.Points[num].Forward = this._controlPoints[i].forward;
                }
                num++;
                if (i < (this._controlPoints.Count - 1))
                {
                    Vector3 vector;
                    Vector3 vector2;
                    float num3 = Vector3.Distance(this._controlPoints[i].p, this._controlPoints[i + 1].p) / 2f;
                    if (i == 0)
                    {
                        Vector3 vector3 = this._controlPoints[i + 1].p - this._controlPoints[i].p;
                        vector = this._controlPoints[i].p + ((Vector3) (vector3.normalized * num3));
                    }
                    else
                    {
                        Vector3 vector4 = this._controlPoints[i + 1].p - this._controlPoints[i - 1].p;
                        vector = this._controlPoints[i].p + ((Vector3) (vector4.normalized * num3));
                    }
                    int num4 = i + 1;
                    if (num4 == (this._controlPoints.Count - 1))
                    {
                        Vector3 vector5 = this._controlPoints[num4 - 1].p - this._controlPoints[num4].p;
                        vector2 = this._controlPoints[num4].p + ((Vector3) (vector5.normalized * num3));
                    }
                    else
                    {
                        Vector3 vector6 = this._controlPoints[num4 - 1].p - this._controlPoints[num4 + 1].p;
                        vector2 = this._controlPoints[num4].p + ((Vector3) (vector6.normalized * num3));
                    }
                    PCTrailPoint point = trail.Points[num - 1];
                    PCTrailPoint point2 = trail.Points[((num - 1) + this.PointsBetweenControlPoints) + 1];
                    for (int k = 0; k < this.PointsBetweenControlPoints; k++)
                    {
                        float t = (k + 1f) / (this.PointsBetweenControlPoints + 1f);
                        trail.Points[num].Position = this.GetPointAlongCurve(this._controlPoints[i].p, vector, this._controlPoints[i + 1].p, vector2, t, 0.3f);
                        trail.Points[num].SetTimeActive(Mathf.Lerp(point.TimeActive(), point2.TimeActive(), t));
                        if (base.TrailData.UseForwardOverride)
                        {
                            trail.Points[num].Forward = Vector3.Lerp(point.Forward, point2.Forward, t);
                        }
                        num++;
                    }
                }
            }
            float distance = 0f;
            for (int j = 1; j < trail.Points.Count; j++)
            {
                distance += Vector3.Distance(trail.Points[j - 1].Position, trail.Points[j].Position);
                trail.Points[j].SetDistanceFromStart(distance);
            }
        }

        protected override int GetMaxNumberOfPoints()
        {
            throw new NotImplementedException();
        }

        public Vector3 GetPointAlongCurve(Vector3 curveStart, Vector3 curveStartHandle, Vector3 curveEnd, Vector3 curveEndHandle, float t, float crease)
        {
            float f = 1f - t;
            float num2 = Mathf.Pow(f, 3f);
            float num3 = Mathf.Pow(f, 2f);
            float num4 = 1f - crease;
            return (Vector3) ((((((num2 * curveStart) * num4) + ((((3f * num3) * t) * curveStartHandle) * crease)) + ((((3f * f) * Mathf.Pow(t, 2f)) * curveEndHandle) * crease)) + ((Mathf.Pow(t, 3f) * curveEnd) * num4)) / ((((num2 * num4) + (((3f * num3) * t) * crease)) + (((3f * f) * Mathf.Pow(t, 2f)) * crease)) + (Mathf.Pow(t, 3f) * num4)));
        }

        public Vector3 GetPointAlongTrail(float t)
        {
            CircularBuffer<PCTrailPoint> points = base._activeTrail.Points;
            if (points.Count == 0)
            {
                return Vector3.zero;
            }
            if (points.Count < 2)
            {
                return base._t.TransformPoint(points[0].Position);
            }
            t = Mathf.Clamp01(t);
            float num = points[points.Count - 1].GetDistanceFromStart() * t;
            Vector3 position = points[points.Count - 1].Position;
            for (int i = 1; i < points.Count; i++)
            {
                if (points[i].GetDistanceFromStart() > num)
                {
                    float num3 = (num - points[i - 1].GetDistanceFromStart()) / (points[i].GetDistanceFromStart() - points[i - 1].GetDistanceFromStart());
                    position = Vector3.Lerp(points[i - 1].Position, points[i].Position, num3);
                    break;
                }
            }
            return base._t.TransformPoint(position);
        }

        public virtual void Init(Vector3[] controlPoints)
        {
            if (base._activeTrail != null)
            {
                base._activeTrail.Dispose();
            }
            base._activeTrail = new PCTrail(controlPoints.Length + ((controlPoints.Length - 1) * this.PointsBetweenControlPoints));
            this._controlPoints = new CircularBuffer<ControlPoint>(controlPoints.Length);
            for (int i = 0; i < (controlPoints.Length - 1); i++)
            {
                this.AddControlPoint(controlPoints[i], false);
            }
            if (controlPoints.Length > 0)
            {
                this.AddControlPoint(controlPoints[controlPoints.Length - 1], true);
            }
            this.GenerateTrail(base._activeTrail);
            this.GenerateMesh(base._activeTrail);
        }

        protected override void LateUpdate()
        {
            if (base._activeTrail != null)
            {
                foreach (Material material in base.TrailData.GetMaterialsContainer().materials)
                {
                    Graphics.DrawMesh(base._activeTrail.Mesh, base._t.localToWorldMatrix, material, base.gameObject.layer);
                }
            }
        }

        protected override void OnDestroy()
        {
            Material[] materials = base.TrailData.GetMaterialsContainer().materials;
            for (int i = 0; i < materials.Length; i++)
            {
                if (Application.isEditor)
                {
                    UnityEngine.Object.DestroyImmediate(materials[i]);
                }
                else
                {
                    UnityEngine.Object.Destroy(materials[i]);
                }
                materials[i] = null;
            }
        }

        protected override void OnStartEmit()
        {
        }

        public virtual void PlayAnimation(float deltaTime)
        {
            if (this.IsActive)
            {
                this._timer += deltaTime;
                if (this._timer >= 0f)
                {
                    float time = this._timer / this._appearDuration;
                    float num2 = 0f;
                    if (time > 1f)
                    {
                        time = 1f;
                        num2 = (this._timer - this._appearDuration) / this._vanishDuration;
                        if (num2 > 1f)
                        {
                            this.IsActive = false;
                        }
                    }
                    time = this._appearCurve.Evaluate(time);
                    num2 = this._vanishCurve.Evaluate(num2);
                    float num3 = 0f;
                    Vector3 zero = Vector3.zero;
                    if (!base.TrailData.UseForwardOverride || !base.TrailData.ForwardOverrideRelative)
                    {
                        num3 = 1f;
                        zero = (Camera.main == null) ? Vector3.forward : Camera.main.transform.forward;
                        if (base.TrailData.UseForwardOverride)
                        {
                            zero = base.TrailData.ForwardOverride.normalized;
                        }
                        zero = base._t.InverseTransformDirection(zero);
                    }
                    foreach (Material material in base.TrailData.GetMaterialsContainer().materials)
                    {
                        material.SetVector("_CamForward", zero);
                        material.SetFloat("_IsUseCamForward", num3);
                        material.SetFloat("_AppearTime", time);
                        material.SetFloat("_VanishTime", num2);
                    }
                }
            }
        }

        public virtual void ResetAnimation(float appearDuration, AnimationCurve appearCurve, float vanishDuration, AnimationCurve vanishCurve)
        {
            this.IsActive = true;
            this._timer = 0f;
            this._appearDuration = appearDuration;
            this._appearCurve = appearCurve;
            this._vanishDuration = vanishDuration;
            this._vanishCurve = vanishCurve;
            Matrix4x4 matrix = new Matrix4x4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    matrix[i, j] = base.TrailData.SizeOverLife.Evaluate(((float) ((i * 4) + j)) / 15f);
                }
            }
            Vector4 zero = Vector4.zero;
            if (base.TrailData.ColorOverLife.alphaKeys.Length > 2)
            {
                zero.x = base.TrailData.ColorOverLife.alphaKeys[1].time;
                zero.y = base.TrailData.ColorOverLife.alphaKeys[2].time;
            }
            else
            {
                zero.x = 0f;
                zero.y = 1f;
            }
            foreach (Material material in base.TrailData.GetMaterialsContainer().materials)
            {
                material.SetFloat("_AppearTime", 0f);
                material.SetFloat("_VanishTime", 0f);
                material.SetMatrix("_SizeOverLife", matrix);
                material.SetVector("_AlphaOverLife", zero);
            }
        }

        protected override void Update()
        {
        }

        public bool IsActive { get; set; }

        private class ControlPoint
        {
            public Vector3 forward;
            public Vector3 p;
        }
    }
}

