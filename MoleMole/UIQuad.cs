namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class UIQuad
    {
        public Vector2 max;
        public static readonly int MAX_UV_COUNT = 4;
        public Vector2 min;
        public Vector2[] uvMax;
        public Vector2[] uvMin;

        public UIQuad()
        {
            Vector2 vector = -Vector2.one;
            this.Set(Vector2.zero, Vector2.zero, vector, vector, vector, vector, vector, vector, vector, vector);
        }

        public UIQuad(UIQuad quad)
        {
            this.Set(quad.min, quad.max, quad.uvMin[0], quad.uvMax[0], quad.uvMin[1], quad.uvMax[1], quad.uvMin[2], quad.uvMax[2], quad.uvMin[3], quad.uvMax[3]);
        }

        public UIQuad(Vector2 min, Vector2 max, Vector2 uvMin0, Vector2 uvMax0, Vector2 uvMin1, Vector2 uvMax1)
        {
            Vector2 vector = -Vector2.one;
            this.Set(min, max, uvMin0, uvMax0, uvMin1, uvMax1, vector, vector, vector, vector);
        }

        public UIQuad(Vector2 min, Vector2 max, Vector2 uvMin0, Vector2 uvMax0, Vector2 uvMin1, Vector2 uvMax1, Vector2 uvMin2, Vector2 uvMax2, Vector2 uvMin3, Vector2 uvMax3)
        {
            this.Set(min, max, uvMin0, uvMax0, uvMin1, uvMax1, uvMin2, uvMax2, uvMin3, uvMax3);
        }

        private UIQuad GetHigherPart(float splitPoint, int dir, int anotherUIid, bool revertUIid)
        {
            splitPoint = Mathf.Max(this.min[dir], splitPoint);
            UIQuad quad = new UIQuad(this);
            quad.min[dir] = splitPoint;
            float t = (splitPoint - this.min[dir]) / (this.max[dir] - this.min[dir]);
            if (!revertUIid)
            {
                for (int j = 0; j < MAX_UV_COUNT; j++)
                {
                    if (j != anotherUIid)
                    {
                        quad.uvMin[j][dir] = Mathf.Lerp(this.uvMin[j][dir], this.uvMax[j][dir], t);
                    }
                    else
                    {
                        quad.uvMin[j][dir] = -1f;
                    }
                }
                return quad;
            }
            for (int i = 0; i < MAX_UV_COUNT; i++)
            {
                if (i != anotherUIid)
                {
                    quad.uvMin[i][dir] = -1f;
                }
                else
                {
                    quad.uvMin[i][dir] = Mathf.Lerp(this.uvMin[i][dir], this.uvMax[i][dir], t);
                }
            }
            return quad;
        }

        private UIQuad GetLowerPart(float splitPoint, int dir, int anotherUIid, bool revertUIid)
        {
            splitPoint = Mathf.Min(this.max[dir], splitPoint);
            UIQuad quad = new UIQuad(this);
            quad.max[dir] = splitPoint;
            float t = (splitPoint - this.min[dir]) / (this.max[dir] - this.min[dir]);
            if (!revertUIid)
            {
                for (int j = 0; j < MAX_UV_COUNT; j++)
                {
                    if (j != anotherUIid)
                    {
                        quad.uvMax[j][dir] = Mathf.Lerp(this.uvMin[j][dir], this.uvMax[j][dir], t);
                    }
                    else
                    {
                        quad.uvMax[j][dir] = -1f;
                    }
                }
                return quad;
            }
            for (int i = 0; i < MAX_UV_COUNT; i++)
            {
                if (i != anotherUIid)
                {
                    quad.uvMax[i][dir] = -1f;
                }
                else
                {
                    quad.uvMax[i][dir] = Mathf.Lerp(this.uvMin[i][dir], this.uvMax[i][dir], t);
                }
            }
            return quad;
        }

        private UIQuad GetMiddlePart(float splitPoint1, float splitPoint2, int anotherUIid, int dir)
        {
            UIQuad quad = new UIQuad(this);
            quad.min[dir] = splitPoint1;
            quad.max[dir] = splitPoint2;
            float t = (splitPoint1 - this.min[dir]) / (this.max[dir] - this.min[dir]);
            float num2 = (splitPoint2 - this.min[dir]) / (this.max[dir] - this.min[dir]);
            for (int i = 0; i < MAX_UV_COUNT; i++)
            {
                quad.uvMin[i][dir] = Mathf.Lerp(this.uvMin[i][dir], this.uvMax[i][dir], t);
                quad.uvMax[i][dir] = Mathf.Lerp(this.uvMin[i][dir], this.uvMax[i][dir], num2);
            }
            return quad;
        }

        public void Set(Vector2 min, Vector2 max, Vector2 uvMin0, Vector2 uvMax0, Vector2 uvMin1, Vector2 uvMax1, Vector2 uvMin2, Vector2 uvMax2, Vector2 uvMin3, Vector2 uvMax3)
        {
            this.min = min;
            this.max = max;
            this.uvMin = new Vector2[] { uvMin0, uvMin1, uvMin2, uvMin3 };
            this.uvMax = new Vector2[] { uvMax0, uvMax1, uvMax2, uvMax3 };
        }

        public UIQuad Split(UIQuad another, int anotherUIid = 1, List<UIQuad> unOverlappedList = null, bool revertUVid = false)
        {
            UIQuad quad = null;
            UIQuad quad2 = this.Split(another, anotherUIid, unOverlappedList, 0, revertUVid);
            if (quad2 != null)
            {
                quad = quad2.Split(another, anotherUIid, unOverlappedList, 1, revertUVid);
            }
            return quad;
        }

        private UIQuad Split(UIQuad another, int anotherUIid, List<UIQuad> unOverlappedList, int dir, bool revertUVid)
        {
            float splitPoint = Mathf.Max(this.min[dir], another.min[dir]);
            float num2 = Mathf.Min(this.max[dir], another.max[dir]);
            if ((this.min[dir] < splitPoint) && (unOverlappedList != null))
            {
                unOverlappedList.Add(this.GetLowerPart(splitPoint, dir, anotherUIid, revertUVid));
            }
            if ((num2 < this.max[dir]) && (unOverlappedList != null))
            {
                unOverlappedList.Add(this.GetHigherPart(num2, dir, anotherUIid, revertUVid));
            }
            UIQuad quad = null;
            if (splitPoint < num2)
            {
                quad = this.GetMiddlePart(splitPoint, num2, anotherUIid, dir);
                if (!revertUVid)
                {
                    quad.uvMin[anotherUIid][dir] = Mathf.Lerp(another.uvMin[anotherUIid][dir], another.uvMax[anotherUIid][dir], (splitPoint - another.min[dir]) / (another.max[dir] - another.min[dir]));
                    quad.uvMax[anotherUIid][dir] = Mathf.Lerp(another.uvMin[anotherUIid][dir], another.uvMax[anotherUIid][dir], (num2 - another.min[dir]) / (another.max[dir] - another.min[dir]));
                }
            }
            return quad;
        }

        public UIVertex[] ToUIQuad(UIVertex template)
        {
            UIVertex[] vertexArray = new UIVertex[] { template, template, template, template };
            vertexArray[0].position = new Vector3(this.min.x, this.min.y, 0f);
            vertexArray[1].position = new Vector3(this.min.x, this.max.y, 0f);
            vertexArray[2].position = new Vector3(this.max.x, this.max.y, 0f);
            vertexArray[3].position = new Vector3(this.max.x, this.min.y, 0f);
            vertexArray[0].uv0 = new Vector3(this.uvMin[0].x, this.uvMin[0].y);
            vertexArray[1].uv0 = new Vector3(this.uvMin[0].x, this.uvMax[0].y);
            vertexArray[2].uv0 = new Vector3(this.uvMax[0].x, this.uvMax[0].y);
            vertexArray[3].uv0 = new Vector3(this.uvMax[0].x, this.uvMin[0].y);
            vertexArray[0].uv1 = new Vector3(this.uvMin[1].x, this.uvMin[1].y);
            vertexArray[1].uv1 = new Vector3(this.uvMin[1].x, this.uvMax[1].y);
            vertexArray[2].uv1 = new Vector3(this.uvMax[1].x, this.uvMax[1].y);
            vertexArray[3].uv1 = new Vector3(this.uvMax[1].x, this.uvMin[1].y);
            vertexArray[0].normal = new Vector3(this.uvMin[2].x, 0f, this.uvMin[2].y);
            vertexArray[1].normal = new Vector3(this.uvMin[2].x, 0f, this.uvMax[2].y);
            vertexArray[2].normal = new Vector3(this.uvMax[2].x, 0f, this.uvMax[2].y);
            vertexArray[3].normal = new Vector3(this.uvMax[2].x, 0f, this.uvMin[2].y);
            vertexArray[0].tangent = new Vector3(this.uvMin[3].x, 0f, this.uvMin[3].y);
            vertexArray[1].tangent = new Vector3(this.uvMin[3].x, 0f, this.uvMax[3].y);
            vertexArray[2].tangent = new Vector3(this.uvMax[3].x, 0f, this.uvMax[3].y);
            vertexArray[3].tangent = new Vector3(this.uvMax[3].x, 0f, this.uvMin[3].y);
            return vertexArray;
        }
    }
}

