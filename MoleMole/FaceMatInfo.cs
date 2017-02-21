namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct FaceMatInfo
    {
        public Texture2D texture;
        public Vector2 tile;
        public Vector2 offset;
        public bool valid
        {
            get
            {
                return (this.texture != null);
            }
        }
    }
}

