namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoStoreKitEventListener : MonoBehaviour
    {
        public static Action<PayResult> IAP_PURCHASE_CALLBACK;
    }
}

