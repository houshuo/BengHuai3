namespace MoleMole
{
    using MoleMole.Config;
    using System;

    public static class IndexedConfig
    {
        public static int Compare(DynamicFloat lhs, DynamicFloat rhs)
        {
            if ((lhs == null) && (rhs == null))
            {
                return 0;
            }
            if (lhs == null)
            {
                return -1;
            }
            if (rhs == null)
            {
                return 1;
            }
            int num = lhs.isDynamic.CompareTo(rhs.isDynamic);
            if (num == 0)
            {
                num = Compare(lhs.dynamicKey, rhs.dynamicKey);
                if (num != 0)
                {
                    return num;
                }
                num = lhs.fixedValue.CompareTo(rhs.fixedValue);
                if (num != 0)
                {
                    return num;
                }
            }
            return num;
        }

        public static int Compare(string lhs, string rhs)
        {
            if ((lhs == null) && (rhs == null))
            {
                return 0;
            }
            if (lhs == null)
            {
                return -1;
            }
            return lhs.CompareTo(rhs);
        }

        public static int Compare(string[] lhs, string[] rhs)
        {
            if ((lhs == null) && (rhs == null))
            {
                return 0;
            }
            if (lhs == null)
            {
                return -1;
            }
            if (rhs == null)
            {
                return 1;
            }
            int num = lhs.Length.CompareTo(rhs.Length);
            if (num == 0)
            {
                int length = lhs.Length;
                for (int i = 0; i < length; i++)
                {
                    num = Compare(lhs[i], rhs[i]);
                    if (num != 0)
                    {
                        return num;
                    }
                }
            }
            return num;
        }
    }
}

