namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public sealed class CirclePlatform : BasePlatform
    {
        public CirclePlatform(MonoBasePerpStage stageOwner, uint radius) : base(stageOwner)
        {
            this.Radius = radius;
        }

        public override Vector3 GetARandomPlace()
        {
            Vector2 vector = (Vector2) (UnityEngine.Random.insideUnitCircle * this.Radius);
            return new Vector3(vector.x, 0f, vector.y);
        }

        public uint Radius { get; private set; }
    }
}

