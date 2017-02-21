namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct TechTreeNodeLockInfo
    {
        public TechTreeNodeLock _lockType;
        public int _needLevel;
    }
}

