namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable]
    public class Notify
    {
        public object body;
        public NotifyTypes type;

        public Notify(NotifyTypes type, object body = null)
        {
            this.type = type;
            this.body = body;
        }
    }
}

