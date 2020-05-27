using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleGames;

public class PoolManager : Singleton<PoolManager>
{
    private Dictionary<int, Pool> pools = new Dictionary<int, Pool>();


    public Pool AddPool(PoolType id, bool reparent = true)
    {
        Pool pool;

        if (pools.TryGetValue((int)id, out pool) == false)
        {
            pool = new Pool();
            pools.Add((int)id, pool);

            if (reparent)
            {
                var poolsFolder = GameObject.Find("[POOLS]") ?? new GameObject("[POOLS]");

                var poolFolder = new GameObject("pool:" + id);
                poolFolder.transform.parent = poolsFolder.transform;

                pool.SetParent(poolFolder.transform);
            }
        }
        return pool;
    }

    public GameObject Spawn(PoolType id, GameObject prefab, Vector3 position = default(Vector3),
       Quaternion rotation = default(Quaternion), Transform parent = null)
    {
        return pools[(int)id].Spawn(prefab, position, rotation, parent);
    }

    public T Spawn<T>(PoolType id, GameObject prefab, Vector3 position = default(Vector3),
       Quaternion rotation = default(Quaternion), Transform parent = null) where T: class
    {
        var go = pools[(int)id].Spawn(prefab, position, rotation, parent);
        return go.GetComponent<T>();
    }

    public void Despawn(PoolType id, GameObject go)
    {
        pools[(int)id].Despawn(go);
    }

    public void Dispose()
    {
        foreach (var poolValue in pools.Values)
        {
            poolValue.Dispose();
        }
        pools.Clear();
    }

}