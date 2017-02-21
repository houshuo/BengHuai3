namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class CacheDataUtil
    {
        private Dictionary<ECacheData, DataWrapper> _cacheDataUtilDic = new Dictionary<ECacheData, DataWrapper>();

        public void CheckCacheValidAndGo<T>(ECacheData type, NotifyTypes notify) where T: class
        {
            foreach (KeyValuePair<ECacheData, DataWrapper> pair in this._cacheDataUtilDic)
            {
                if (((ECacheData) pair.Key) == type)
                {
                    CacheData<T> data = pair.Value._data as CacheData<T>;
                    if (data.CacheValid)
                    {
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(notify, null));
                        return;
                    }
                    pair.Value._notify = notify;
                    pair.Value._req();
                }
            }
        }

        public void CreateCacheUtil(ECacheData type, object data, Action req, ushort rspID)
        {
            DataWrapper wrapper = new DataWrapper {
                _data = data,
                _req = req,
                _rspID = rspID,
                _notify = NotifyTypes.None
            };
            this._cacheDataUtilDic.Add(type, wrapper);
        }

        public void OnGetRsp(ushort cmdID)
        {
            foreach (KeyValuePair<ECacheData, DataWrapper> pair in this._cacheDataUtilDic)
            {
                if (pair.Value._rspID == cmdID)
                {
                    if (pair.Value._notify != NotifyTypes.None)
                    {
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(pair.Value._notify, null));
                        pair.Value._notify = NotifyTypes.None;
                    }
                    return;
                }
            }
        }

        private class DataWrapper
        {
            public object _data;
            public NotifyTypes _notify;
            public Action _req;
            public ushort _rspID;
        }
    }
}

