namespace MoleMole
{
    using System;
    using UnityEngine;

    public static class MeshGenerator
    {
        public static Mesh BillboardQuad()
        {
            Vector3[] vectorArray = new Vector3[4];
            Vector2[] vectorArray2 = new Vector2[4];
            int[] numArray = new int[6];
            for (int i = 0; i < 4; i++)
            {
                vectorArray[i] = Vector3.zero;
            }
            vectorArray2[0] = new Vector2(0f, 0f);
            vectorArray2[1] = new Vector2(1f, 0f);
            vectorArray2[2] = new Vector2(0f, 1f);
            vectorArray2[3] = new Vector2(1f, 1f);
            numArray[0] = 0;
            numArray[1] = 1;
            numArray[2] = 2;
            numArray[3] = 1;
            numArray[4] = 3;
            numArray[5] = 2;
            return new Mesh { name = "BillboardQuad", vertices = vectorArray, uv = vectorArray2, triangles = numArray };
        }

        public static Mesh Quad()
        {
            Vector3[] vectorArray = new Vector3[4];
            Vector2[] vectorArray2 = new Vector2[4];
            int[] numArray = new int[6];
            vectorArray[0] = new Vector3(-0.5f, -0.5f, 0f);
            vectorArray[1] = new Vector3(0.5f, -0.5f, 0f);
            vectorArray[2] = new Vector3(-0.5f, 0.5f, 0f);
            vectorArray[3] = new Vector3(0.5f, 0.5f, 0f);
            vectorArray2[0] = new Vector2(0f, 0f);
            vectorArray2[1] = new Vector2(1f, 0f);
            vectorArray2[2] = new Vector2(0f, 1f);
            vectorArray2[3] = new Vector2(1f, 1f);
            numArray[0] = 0;
            numArray[1] = 1;
            numArray[2] = 2;
            numArray[3] = 1;
            numArray[4] = 3;
            numArray[5] = 2;
            return new Mesh { name = "BillboardQuad", vertices = vectorArray, uv = vectorArray2, triangles = numArray };
        }

        public static Mesh QuadFaceUp()
        {
            Vector3[] vectorArray = new Vector3[4];
            Vector2[] vectorArray2 = new Vector2[4];
            int[] numArray = new int[6];
            vectorArray[0] = new Vector3(-0.5f, 0f, -0.5f);
            vectorArray[1] = new Vector3(0.5f, 0f, -0.5f);
            vectorArray[2] = new Vector3(-0.5f, 0f, 0.5f);
            vectorArray[3] = new Vector3(0.5f, 0f, 0.5f);
            vectorArray2[0] = new Vector2(0f, 0f);
            vectorArray2[1] = new Vector2(1f, 0f);
            vectorArray2[2] = new Vector2(0f, 1f);
            vectorArray2[3] = new Vector2(1f, 1f);
            numArray[0] = 0;
            numArray[1] = 1;
            numArray[2] = 2;
            numArray[3] = 1;
            numArray[4] = 3;
            numArray[5] = 2;
            return new Mesh { name = "BillboardQuad", vertices = vectorArray, uv = vectorArray2, triangles = numArray };
        }
    }
}

