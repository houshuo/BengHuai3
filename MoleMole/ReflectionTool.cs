namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public static class ReflectionTool
    {
        private static Vector4 _blurScale = new Vector4();
        private static Vector4[] _dofHexagon;
        private static string[] _dofScatterPropertyNames = new string[] { "dofScatter1", "dofScatter2", "dofScatter3", "dofScatter4", "dofScatter5", "dofScatter6" };
        private static Vector4[] _dofVector = new Vector4[6];

        public static void ApplyHexBlur(Material hexBlurMat, RenderTexture source, RenderTexture destination, float blurFactor, float bokehIntensity, float tapScale, int pass = 0)
        {
            GenerateDOFHexagon();
            float num = 1f / ((float) source.width);
            float num2 = 1f / ((float) source.height);
            _blurScale.Set(blurFactor * num, blurFactor * num2, 0f, bokehIntensity);
            hexBlurMat.SetVector("blurScale", _blurScale);
            int num3 = 0;
            int num4 = 1;
            for (int i = 1; i < 7; i++)
            {
                _dofVector[i - 1].Set((num4 * _dofHexagon[i].x) - (num3 * _dofHexagon[i].y), (num3 * _dofHexagon[i].x) + (num4 * _dofHexagon[i].y), 0f, 0f);
                _dofVector[i - 1] = (Vector4) (_dofVector[i - 1] * tapScale);
            }
            for (int j = 0; j < 6; j++)
            {
                hexBlurMat.SetVector(_dofScatterPropertyNames[j], _dofVector[j]);
            }
            hexBlurMat.mainTexture = source;
            destination.DiscardContents();
            Graphics.Blit(source, destination, hexBlurMat, pass);
        }

        public static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projMatrix, float horizObl, float vertObl)
        {
            Matrix4x4 matrixx = projMatrix;
            matrixx[0, 2] = horizObl;
            matrixx[1, 2] = vertObl;
            return matrixx;
        }

        public static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane, float sideSign)
        {
            Vector4 b = (Vector4) (projection.inverse * new Vector4(sgn(clipPlane.x), sgn(clipPlane.y), 1f, 1f));
            Vector4 vector2 = (Vector4) (clipPlane * (2f / Vector4.Dot(clipPlane, b)));
            projection[2] = vector2.x + (Mathf.Sign(sideSign) * projection[3]);
            projection[6] = vector2.y + (Mathf.Sign(sideSign) * projection[7]);
            projection[10] = vector2.z + (Mathf.Sign(sideSign) * projection[11]);
            projection[14] = vector2.w + (Mathf.Sign(sideSign) * projection[15]);
            return projection;
        }

        public static Matrix4x4 CalculateReflectionMatrix(Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = 1f - ((2f * plane[0]) * plane[0]);
            reflectionMat.m01 = (-2f * plane[0]) * plane[1];
            reflectionMat.m02 = (-2f * plane[0]) * plane[2];
            reflectionMat.m03 = (-2f * plane[3]) * plane[0];
            reflectionMat.m10 = (-2f * plane[1]) * plane[0];
            reflectionMat.m11 = 1f - ((2f * plane[1]) * plane[1]);
            reflectionMat.m12 = (-2f * plane[1]) * plane[2];
            reflectionMat.m13 = (-2f * plane[3]) * plane[1];
            reflectionMat.m20 = (-2f * plane[2]) * plane[0];
            reflectionMat.m21 = (-2f * plane[2]) * plane[1];
            reflectionMat.m22 = 1f - ((2f * plane[2]) * plane[2]);
            reflectionMat.m23 = (-2f * plane[3]) * plane[2];
            reflectionMat.m30 = 0f;
            reflectionMat.m31 = 0f;
            reflectionMat.m32 = 0f;
            reflectionMat.m33 = 1f;
            return reflectionMat;
        }

        public static Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign, float clipPlaneOffset)
        {
            Vector3 v = pos + ((Vector3) (normal * clipPlaneOffset));
            Matrix4x4 worldToCameraMatrix = cam.worldToCameraMatrix;
            Vector3 lhs = worldToCameraMatrix.MultiplyPoint(v);
            Vector3 rhs = (Vector3) (worldToCameraMatrix.MultiplyVector(normal).normalized * sideSign);
            return new Vector4(rhs.x, rhs.y, rhs.z, -Vector3.Dot(lhs, rhs));
        }

        private static void GenerateDOFHexagon()
        {
            if (_dofHexagon == null)
            {
                _dofHexagon = new Vector4[7];
                _dofHexagon[0] = new Vector4(0f, 0f, 0f, 0f);
                for (int i = 0; i < 6; i++)
                {
                    float num2 = Mathf.Sin(1.047198f * i);
                    float num3 = Mathf.Cos(1.047198f * i);
                    float num4 = 0f;
                    float num5 = 1f;
                    float x = (num3 * num4) - (num2 * num5);
                    float y = (num2 * num4) + (num3 * num5);
                    _dofHexagon[i + 1] = new Vector4(x, y, 0f, 0f);
                }
            }
        }

        private static float sgn(float a)
        {
            if (a > 0f)
            {
                return 1f;
            }
            if (a < 0f)
            {
                return -1f;
            }
            return 0f;
        }

        public static Matrix4x4 UV_Tex2DProj2Tex2D(Transform transform, Camera cam)
        {
            Matrix4x4 matrixx = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
            Vector3 lossyScale = transform.lossyScale;
            Matrix4x4 matrixx2 = transform.localToWorldMatrix * Matrix4x4.Scale(new Vector3(1f / lossyScale.x, 1f / lossyScale.y, 1f / lossyScale.z));
            return (((matrixx * cam.projectionMatrix) * cam.worldToCameraMatrix) * matrixx2);
        }
    }
}

