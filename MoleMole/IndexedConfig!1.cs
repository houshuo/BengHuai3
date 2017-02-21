namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public abstract class IndexedConfig<T> : IComparable<T> where T: IndexedConfig<T>
    {
        private static List<T> _allInstancesList;
        public static IntMapping<T> Mapping;

        static IndexedConfig()
        {
            IndexedConfig<T>._allInstancesList = new List<T>();
            IndexedConfig<T>.Mapping = new IntMapping<T>();
        }

        public IndexedConfig()
        {
            IndexedConfig<T>._allInstancesList.Add((T) this);
        }

        public abstract int CompareTo(T other);
        public abstract int ContentHash();
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() != typeof(T))
            {
                return false;
            }
            return (this.CompareTo((T) obj) == 0);
        }

        public override int GetHashCode()
        {
            return this.ContentHash();
        }

        public static void InitializeMapping()
        {
            HashSet<T> set = new HashSet<T>(IndexedConfig<T>._allInstancesList);
            for (int i = 1; i <= IndexedConfig<T>.Mapping.length; i++)
            {
                set.Add(IndexedConfig<T>.Mapping.Get(i));
            }
            T[] arr = new T[set.Count];
            set.CopyTo(arr);
            IndexedConfig<T>.Mapping = new IntMapping<T>(arr);
            IndexedConfig<T>._allInstancesList.Clear();
        }
    }
}

