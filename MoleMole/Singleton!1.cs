namespace MoleMole
{
    using System;

    public static class Singleton<T> where T: class
    {
        private static T _instance;

        public static void Create()
        {
            Singleton<T>._instance = (T) Activator.CreateInstance(typeof(T), true);
        }

        public static void CreateByInstance(T instance)
        {
            Singleton<T>._instance = instance;
        }

        public static void Destroy()
        {
            Singleton<T>._instance = null;
        }

        public static T GetInstance()
        {
            return Singleton<T>._instance;
        }

        public static T Instance
        {
            get
            {
                return Singleton<T>._instance;
            }
        }
    }
}

