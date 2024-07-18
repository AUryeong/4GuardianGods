using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PoolableUnit<T> : SerializedMonoBehaviour where T : Component
{
    protected ObjectPool<PoolableUnit<T>> objectPool;
    public void Init(ObjectPool<PoolableUnit<T>> pool)
    {
        objectPool = pool;
    }

    public void PushPool()
    {
        objectPool?.PushPool(this);
    }
}
