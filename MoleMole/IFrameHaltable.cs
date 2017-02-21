namespace MoleMole
{
    using System;
    using UnityEngine;

    public interface IFrameHaltable
    {
        void FrameHalt(int frameNum);
        uint GetRuntimeID();
        bool IsActive();
        bool IsToBeRemove();

        FixedStack<float> timeScaleStack { get; }

        Vector3 XZPosition { get; }
    }
}

