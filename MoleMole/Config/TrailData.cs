namespace MoleMole.Config
{
    using System;
    using UnityEngine;

    public class TrailData
    {
        public bool clockwise;
        public float liftY;
        public int points;
        public Vector3 position;
        public float radiusX;
        public float radiusY;
        public Vector3 rotate;
        public float startPolarOffset;
        public float totalPolar;

        public TrailData()
        {
            this.clockwise = true;
            this.points = 5;
            this.totalPolar = 6.283185f;
            this.radiusX = 1f;
            this.radiusY = 1f;
            this.liftY = 1f;
            this.rotate = Vector3.zero;
            this.position = Vector3.zero;
            this.clockwise = true;
            this.points = 5;
            this.totalPolar = 6.283185f;
            this.startPolarOffset = 0f;
            this.radiusX = 1f;
            this.radiusY = 1f;
            this.liftY = 1f;
            this.rotate = Vector3.zero;
            this.position = Vector3.zero;
        }

        public TrailData(bool clockwise, int points, float totalPolar, float startPolarOffset, float radiusX, float radiusY, float liftY, Vector3 rotate, Vector3 position)
        {
            this.clockwise = true;
            this.points = 5;
            this.totalPolar = 6.283185f;
            this.radiusX = 1f;
            this.radiusY = 1f;
            this.liftY = 1f;
            this.rotate = Vector3.zero;
            this.position = Vector3.zero;
            this.clockwise = clockwise;
            this.points = points;
            this.totalPolar = totalPolar;
            this.startPolarOffset = startPolarOffset;
            this.radiusX = radiusX;
            this.radiusY = radiusY;
            this.liftY = liftY;
            this.rotate = rotate;
            this.position = position;
        }
    }
}

