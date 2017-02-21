namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class WaitingEventHelper
    {
        private Dictionary<string, bool> _eventFinishMap = new Dictionary<string, bool>();

        public WaitingEventHelper(string[] eventNameArray = null)
        {
            if (eventNameArray != null)
            {
                foreach (string str in eventNameArray)
                {
                    this._eventFinishMap.Add(str, false);
                }
            }
        }

        public void Add(string eventName)
        {
            this._eventFinishMap.Add(eventName, false);
        }

        public bool isFinish(string eventName)
        {
            return this._eventFinishMap[eventName];
        }

        public bool isFinishAll()
        {
            foreach (string str in this._eventFinishMap.Keys)
            {
                if (!this._eventFinishMap[str])
                {
                    return false;
                }
            }
            return true;
        }

        public void Remove(string eventName)
        {
            this._eventFinishMap.Remove(eventName);
        }

        public void SetFinish(string eventName)
        {
            this._eventFinishMap[eventName] = true;
        }

        public void SetFinishAll()
        {
            foreach (string str in this._eventFinishMap.Keys)
            {
                this._eventFinishMap[str] = true;
            }
        }
    }
}

