using UnityEngine;
namespace PRO.Tool
{
    public class GameObjectPool<T> : ObjectPoolBase<T> where T : Component
    {
        private T holdPrefab;
        public readonly Transform toolParent;

        public GameObjectPool(T prefab, Transform parent)
        {
            prefab.gameObject.SetActive(false);
            if (prefab.gameObject.scene.IsValid())
                prefab.transform.parent = parent;
            holdPrefab = prefab;
            toolParent = parent;
        }
        protected override T NewObject()
        {
            GameObject go = GameObject.Instantiate(holdPrefab.gameObject);
            return go.GetComponent<T>();
        }
        public override T TakeOut()
        {
            var ret = base.TakeOut();
            ret.gameObject.SetActive(true);
            return ret;
        }
        public override void PutIn(T item)
        {
            base.PutIn(item);
            item.gameObject.SetActive(false);
            item.transform.parent = toolParent;
        }
    }

    public class GameObjectPool_NoMono<T> : ObjectPoolBase<T> where T : class, IGameObjectPool_NoMono, new()
    {
        private GameObject holdPrefab;
        public readonly Transform toolParent;

        public GameObjectPool_NoMono(GameObject prefab, Transform parent)
        {
            prefab.SetActive(false);
            prefab.transform.parent = parent;
            holdPrefab = prefab;
            toolParent = parent;
        }
        protected override T NewObject()
        {
            GameObject go = GameObject.Instantiate(holdPrefab.gameObject);
            var t = new T();
            t.gameObject = go;
            t.transform = go.transform;
            t.transform.parent = toolParent;
            return t;
        }
        public override T TakeOut()
        {
            var ret = base.TakeOut();
            ret.gameObject.SetActive(true);
            return ret;
        }
        public override void PutIn(T item)
        {
            base.PutIn(item);
            item.gameObject.SetActive(false);
            item.transform.parent = toolParent;
        }
    }

    public interface IGameObjectPool_NoMono
    {
        public Transform transform { get; set; }
        public GameObject gameObject { get; set; }
    }
}