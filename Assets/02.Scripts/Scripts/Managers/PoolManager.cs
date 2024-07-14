using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : SingletonBehavior<PoolManager>
{
    public Dictionary<string, ParticleSystem> particlePoolList;
    private Dictionary<string, ListableObjectPool<ParticleSystem>> particlePool;

    protected override void OnCreated()
    {
        base.OnCreated();
        particlePool = new Dictionary<string, ListableObjectPool<ParticleSystem>>(particlePoolList.Count);
        foreach(var particle in particlePoolList)
        {
            particlePool.Add(particle.Key, new ListableObjectPool<ParticleSystem>(particle.Value));
        }
    }
}
