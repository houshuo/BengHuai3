namespace MoleMole
{
    using System;
    using UnityEngine;

    public class AsyncAssetRequst
    {
        public AsyncOperation operation;

        public AsyncAssetRequst(AsyncOperation operation)
        {
            this.operation = operation;
        }

        public object asset
        {
            get
            {
                if (this.operation is ResourceRequest)
                {
                    return ((ResourceRequest) this.operation).asset;
                }
                if (this.operation is AssetBundleRequest)
                {
                    return ((AssetBundleRequest) this.operation).asset;
                }
                return null;
            }
        }
    }
}

