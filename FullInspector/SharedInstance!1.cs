namespace FullInspector
{
    using System;

    public abstract class SharedInstance<T> : SharedInstance<T, FullSerializerSerializer>
    {
        protected SharedInstance()
        {
        }
    }
}

