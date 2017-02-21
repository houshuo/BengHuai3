namespace MoleMole
{
    using System;

    public interface IFaceMatInfoProvider
    {
        FaceMatInfo GetFaceMatInfo(int index);
        string[] GetMatInfoNames();

        int capacity { get; }
    }
}

