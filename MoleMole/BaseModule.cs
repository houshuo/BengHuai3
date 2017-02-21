namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class BaseModule
    {
        protected List<int> ConvertList(List<uint> fromList)
        {
            List<int> list = new List<int>();
            foreach (uint num in fromList)
            {
                list.Add((int) num);
            }
            return list;
        }

        public virtual bool OnPacket(NetPacketV1 packet)
        {
            return false;
        }

        protected void UpdateField<T>(ref T field, T newValue, Action<T, T> callback = null)
        {
            T local = field;
            field = newValue;
            if (callback != null)
            {
                callback(local, newValue);
            }
        }

        protected void UpdateField<T>(bool isSpecified, ref T field, T newValue, Action<T, T> callback = null)
        {
            if (isSpecified)
            {
                this.UpdateField<T>(ref field, newValue, callback);
            }
        }
    }
}

