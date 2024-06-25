using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableUnit<T> : MonoBehaviour where T : Component
{
    protected ObjectPool<PoolableUnit<T>> objectPool;
    public void Init(ObjectPool<PoolableUnit<T>> pool)
    {
        objectPool = pool;
    }

    public void PushPool()
    {
        if (objectPool == null) 
            return;
        objectPool.PushPool(this);
    }
}
