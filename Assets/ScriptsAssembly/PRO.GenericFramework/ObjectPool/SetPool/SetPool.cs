using System.Collections.Generic;

namespace PRO.Tool
{
    public class SetPool
    {
        private static ObjectPool<List<object>> pool_List = new ObjectPool<List<object>>();
        public static void PutIn<T>(ListPackage<T> package) where T : class
        {
            package.Clear();
            pool_List.PutIn(package.set);
            package.set = null;
        }
        public static ListPackage<T> TakeOut_List<T>() where T : class
        {
            var package = new ListPackage<T>();
            package.set = pool_List.TakeOut();
            return package;
        }

        private static ObjectPool<Dictionary<object, object>> pool_Dic = new ObjectPool<Dictionary<object, object>>();
        public static void PutIn<K, V>(DictionaryPackage<K, V> package) where K : class where V : class
        {
            package.Clear();
            pool_Dic.PutIn(package.set);
            package.set = null;
        }
        public static DictionaryPackage<K, V> TakeOut_DIc<K, V>() where K : class where V : class
        {
            var package = new DictionaryPackage<K, V>();
            package.set = pool_Dic.TakeOut();
            return package;
        }

        private static ObjectPool<HashSet<object>> pool_HashSet = new ObjectPool<HashSet<object>>();
        public static void PutIn<T>(HashSetPackage<T> package) where T : class
        {
            package.Clear();
            pool_HashSet.PutIn(package.set);
            package.set = null;
        }
        public static HashSetPackage<T> TakeOut_HashSet<T>() where T : class
        {
            var package = new HashSetPackage<T>();
            package.set = pool_HashSet.TakeOut();
            return package;
        }
    }
}
