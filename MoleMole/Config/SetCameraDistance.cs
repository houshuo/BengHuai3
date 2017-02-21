namespace MoleMole.Config
{
    using System;

    public class SetCameraDistance : ConfigAvatarCameraAction
    {
        public float CenterY;
        public float Elevation;
        public float FOVOffset;
        public string LerpCurve;
        public float LerpTime;
        public float RadiusRatio = 1f;
        public float Time;
    }
}

