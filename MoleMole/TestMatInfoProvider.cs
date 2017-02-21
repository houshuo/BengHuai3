namespace MoleMole
{
    using System;
    using UnityEngine;

    public class TestMatInfoProvider : MonoBehaviour, IFaceMatInfoProvider
    {
        public Texture2D[] textures;

        public FaceMatInfo GetFaceMatInfo(int index)
        {
            FaceMatInfo info = new FaceMatInfo();
            if ((index >= 0) && (index < this.textures.Length))
            {
                info.texture = this.textures[index];
                info.tile = new Vector2(1f, 1f);
                info.offset = Vector2.zero;
            }
            return info;
        }

        public string[] GetMatInfoNames()
        {
            string[] strArray = new string[this.textures.Length];
            int index = 0;
            int length = this.textures.Length;
            while (index < length)
            {
                strArray[index] = this.textures[index].name;
                index++;
            }
            return strArray;
        }

        public int capacity
        {
            get
            {
                return this.textures.Length;
            }
        }
    }
}

