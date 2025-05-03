using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PRO.Tool
{
    public class SetPool
    {
        private static ObjectPool<List<object>> pool_List = new ObjectPool<List<object>>();
        public static void PutIn<T>(ListPackage<T> package) where T : class
        {
            package.Clear();
            lock (pool_List) pool_List.PutIn(package.set);
            package.set = null;
        }
        public static ListPackage<T> TakeOut_List<T>() where T : class
        {
            var package = new ListPackage<T>();
            lock (pool_List) package.set = pool_List.TakeOut();
            return package;
        }



        private static ObjectPool<Dictionary<object, object>> pool_Dic = new ObjectPool<Dictionary<object, object>>();
        public static void PutIn<K, V>(DictionaryPackage<K, V> package) where K : class where V : class
        {
            package.Clear();
            lock (pool_Dic) pool_Dic.PutIn(package.set);
            package.set = null;
        }
        public static DictionaryPackage<K, V> TakeOut_DIc<K, V>() where K : class where V : class
        {
            var package = new DictionaryPackage<K, V>();
            lock (pool_Dic) package.set = pool_Dic.TakeOut();
            return package;
        }



        private static ObjectPool<HashSet<object>> pool_HashSet = new ObjectPool<HashSet<object>>();
        public static void PutIn<T>(HashSetPackage<T> package) where T : class
        {
            package.Clear();
            lock (pool_HashSet) pool_HashSet.PutIn(package.set);
            package.set = null;
        }
        public static HashSetPackage<T> TakeOut_HashSet<T>() where T : class
        {
            var package = new HashSetPackage<T>();
            lock(pool_HashSet) package.set = pool_HashSet.TakeOut();
            return package;
        }



        private static ObjectPool<List<Vector2Int>> pool_List_Vector2Int = new ObjectPool<List<Vector2Int>>();
        public static List<Vector2Int> TakeOut_List_Vector2Int()
        {
            lock (pool_List_Vector2Int) return pool_List_Vector2Int.TakeOut();
        }
        public static void PutIn(List<Vector2Int> list)
        {
            list.Clear();
            lock(pool_List_Vector2Int) pool_List_Vector2Int.PutIn(list);
        }


        private static ObjectPool<StringBuilder> pool_sb = new ObjectPool<StringBuilder>();
        public static StringBuilder TakeOut_SB()
        {
            lock (pool_sb) return pool_sb.TakeOut();
        }
        public static void PutIn(StringBuilder sb)
        {
            sb.Clear(); lock (pool_sb) pool_sb.PutIn(sb);
        }
    }
}
