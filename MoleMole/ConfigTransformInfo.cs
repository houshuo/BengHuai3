namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ConfigTransformInfo
    {
        public List<float> Euler;
        public List<float> Pos;

        public Vector3 EulerAngle
        {
            get
            {
                return new Vector3(this.Euler[0], this.Euler[1], this.Euler[2]);
            }
        }

        public Vector3 Position
        {
            get
            {
                return new Vector3(this.Pos[0], this.Pos[1], this.Pos[2]);
            }
        }
    }
}

