using System;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool<T> : ObjectPool<GameObject>
{
    private GameObject holdPrefab;
    public readonly Transform toolParent;
    public Dictionary<GameObject, T> dataDic = new Dictionary<GameObject, T>();
    public GameObjectPool(GameObject prefab, Transform parent, int maxNuber, bool isCanExceed) : base(maxNuber, isCanExceed)
    {
        holdPrefab = prefab;
        toolParent = parent;

        base.CreateEvent += CreateAction;
        base.PutInEvent += PutInAction;
        base.TakeOutEvent += TakeOutAction;
        if (typeof(T).IsClass)
            canGet = typeof(T).IsSubclassOf(typeof(Behaviour));
        else if (typeof(T).IsInterface)
            canGet = true;
        else
            canGet = false;
    }
    bool canGet;
    protected override GameObject ClonePoolData()
    {
        GameObject go = null;
        if (holdPrefab != null)
            go = GameObject.Instantiate(holdPrefab);
        if (canGet)
            dataDic.Add(go, go.GetComponent<T>());
        else
            dataDic.Add(go, (T)Activator.CreateInstance(typeof(T)));
        base.CreateEventInvoke(go);
        return go;
    }

    public override void RemoveData(GameObject item)
    {
        base.RemoveData(item);

        dataDic.Remove(item);
        GameObject.Destroy(item);
    }

    public T TakeOutT()
    {
        GameObject go = base.TakeOut();
        return dataDic[go];
    }
    public GameObject TakeOutG()
    {
        GameObject go = base.TakeOut();
        return go;
    }

    public override void Clear()
    {
        base.Clear();
        dataDic.Clear();
    }

    public T ForceTakeOutT()
    {
        GameObject go = base.ForceTakeOut();
        return dataDic[go];
    }

    public event Action<GameObject, T> CreateEventT;
    public event Action<GameObject, T> PutInEventT;
    public event Action<GameObject, T> TakeOutEventT;

    private void CreateAction(GameObject go)
    {
        go.transform.SetParent(toolParent);
        CreateEventT?.Invoke(go, dataDic[go]);
    }

    private void PutInAction(GameObject go)
    {
        go.transform.SetParent(toolParent);
        go.SetActive(false);
        PutInEventT?.Invoke(go, dataDic[go]);
    }

    private void TakeOutAction(GameObject go)
    {
        go.SetActive(true);
        TakeOutEventT?.Invoke(go, dataDic[go]);
    }
}
public interface IPoolData
{
    GameObject gameObject { get; set; }
    Transform transform { get; set; }
    void Init(Transform transform) { this.transform = transform; this.gameObject = transform.gameObject; }
}
