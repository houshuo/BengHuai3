namespace MoleMole
{
    using CinemaDirector;
    using System;
    using UnityEngine;

    public interface ICinema
    {
        Transform GetCameraTransform();
        Cutscene GetCutscene();
        float GetInitCameraClipZNear();
        float GetInitCameraFOV();
        void Init(Transform target);
        bool IsShouldStop();
        void Play();
    }
}

