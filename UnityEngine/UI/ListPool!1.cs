namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine.Events;

    internal static class ListPool<T>
    {
        [CompilerGenerated]
        private static UnityAction<List<T>> <>f__am$cache1;
        private static readonly UnityEngine.UI.ObjectPool<List<T>> s_ListPool;

        static ListPool()
        {
            if (ListPool<T>.<>f__am$cache1 == null)
            {
                ListPool<T>.<>f__am$cache1 = new UnityAction<List<T>>(ListPool<T>.<s_ListPool>m__1BA);
            }
            ListPool<T>.s_ListPool = new UnityEngine.UI.ObjectPool<List<T>>(null, ListPool<T>.<>f__am$cache1);
        }

        [CompilerGenerated]
        private static void <s_ListPool>m__1BA(List<T> l)
        {
            l.Clear();
        }

        public static List<T> Get()
        {
            return ListPool<T>.s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            ListPool<T>.s_ListPool.Release(toRelease);
        }
    }
}

