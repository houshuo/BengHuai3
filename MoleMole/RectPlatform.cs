namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class RectPlatform : BasePlatform
    {
        public RectPlatform(MonoBasePerpStage stageOwner, uint width, uint height) : base(stageOwner)
        {
            this.Width = width;
            this.Height = height;
        }

        public override Vector3 GetARandomPlace()
        {
            return new Vector3((UnityEngine.Random.value - 0.5f) * this.Width, 0f, (UnityEngine.Random.value - 0.5f) * this.Height);
        }

        public uint Height { get; private set; }

        public uint Width { get; private set; }
    }
}

