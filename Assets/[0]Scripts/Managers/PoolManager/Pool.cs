using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class Pool
{

    private Transform parentPool;

    private Dictionary<int, Stack<GameObject>> cachedGO = new Dictionary<int, Stack<GameObject>>();
    private Dictionary<int, int> cachedIDs = new Dictionary<int, int>();

    public Pool PopulateWith(GameObject prefab, int amount, int amountPerTick, int tickSize = 1)
    {
        var key = prefab.GetInstanceID();
        var stack = new Stack<GameObject>(amount);
        cachedGO.Add(key, stack);

        Observable.IntervalFrame(tickSize, FrameCountType.EndOfFrame).Where(val => amount > 0).Subscribe(_loop =>
        {

            Observable.Range(0, amountPerTick).Where(check => amount > 0).Subscribe(_pop =>
            {

                var go = Populate(prefab, Vector3.zero, Quaternion.identity, parentPool);
                go.SetActive(false);
                cachedIDs.Add(go.GetInstanceID(), key);
                cachedGO[key].Push(go);
                amount--;

            });

        });

        return this;
    }

    public void SetParent(Transform parent)
    {
        this.parentPool = parent;
    }

    public GameObject Spawn(GameObject prefab, Vector3 position = default(Vector3),
        Quaternion rotation = default(Quaternion), Transform parent = null)
    {
        var key = prefab.GetInstanceID();
        var stack = new Stack<GameObject>();
        var isStackExist = cachedGO.TryGetValue(key, out stack);

        if (isStackExist && stack.Count > 0)
        {

            var transform = stack.Pop().transform;
            transform.parent = parent;
            transform.gameObject.SetActive(true);
            if ((object)parent != null)
            {
                transform.localRotation = rotation;
                transform.localPosition = position;
            }
            else
            {
                transform.rotation = rotation;
                transform.position = position;
            }
            var poolable = transform.GetComponent<IPoolable>();
            
            if (poolable != null)
            {
                poolable.OnSpawn();
            }
            return transform.gameObject;
        }

        if (!isStackExist)
        {
            cachedGO.Add(key, new Stack<GameObject>());
        }

        var newPrefab = Populate(prefab, position, rotation, parent);
        cachedIDs.Add(newPrefab.GetInstanceID(), key);       

        return newPrefab;
    }

    public void Despawn(GameObject go)
    {
        go.SetActive(false);
        cachedGO[cachedIDs[go.GetInstanceID()]].Push(go);
        var poolable = go.GetComponent<IPoolable>();
        if (poolable != null)
        {
            poolable.OnDespawn();
        }

        if (parentPool != null)
        {
            go.transform.parent = parentPool;
        }
    }

    public void Dispose()
    {
        parentPool = null;
        cachedGO.Clear();
        cachedIDs.Clear();
    }

    private GameObject Populate(GameObject prefab, Vector3 position = default(Vector3),
        Quaternion rotation = default(Quaternion), Transform parent = null)
    {
        var go = Object.Instantiate(prefab, position, rotation, parent).transform;

        if (parent == null)
        {
            go.rotation = rotation;
            go.position = position;
        }
        else
        {
            go.localRotation = rotation;
            go.localPosition = position;
        }

        var poolable = go.GetComponent<IPoolable>();

        if (poolable != null)
        {
            poolable.OnSpawn();
        }

        return go.gameObject;
    }

}
