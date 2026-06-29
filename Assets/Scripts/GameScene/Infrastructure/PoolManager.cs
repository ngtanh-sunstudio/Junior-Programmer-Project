using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolConfig
{
    public PoolType poolType;
    public GameObject prefab;
    public int initialSize;
    public bool canExpand;
}

public class PoolManager : Singleton<PoolManager>
{
    [SerializeField] private PoolConfig[] poolConfigs;

    private Dictionary<PoolType, Queue<GameObject>> pools;
    private HashSet<GameObject> availableObjects;
    // Tracks which instances are already queued so duplicate returns cannot
    // make an active object available to another request.
    private Dictionary<GameObject, PoolType> instanceToPoolType;
    // instanceToPoolType is used so that an object can return itself
    // without every caller remembering which pool it came from.
    private Dictionary<PoolType, PoolConfig> configByPoolType;
    // Cached configs avoid repeatedly scanning the serialized array
    // when expanding pools.
    private Dictionary<PoolType, Transform> parentByPoolType;
    // Separate parent transforms keep pooled objects grouped in the Hierarchy.

    protected override void Awake()
    {
        base.Awake();

        if (!IsSingletonInstance)
        {
            return;
        }

        InitializePools();
    }

    public GameObject GetObjectFromPool(PoolType poolType, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(poolType))
        {
            Debug.LogError($"{nameof(PoolManager)} has no pool configured for {poolType}.", this);
            return null;
        }

        GameObject pooledObject = GetAvailableObject(poolType);
        if (pooledObject == null)
        {
            return null;
        }

        pooledObject.transform.SetPositionAndRotation(position, rotation);
        pooledObject.SetActive(true);
        return pooledObject;
    }

    public void ReturnObjectToPool(PoolType poolType, GameObject instance)
    {
        if (!TryGetValidReturnPoolType(poolType, instance, out PoolType returnPoolType))
        {
            return;
        }

        EnqueueAvailableObject(returnPoolType, instance);
    }

    public void ReturnObjectToPool(GameObject instance)
    {
        if (!TryGetValidReturnPoolType(instance, out PoolType returnPoolType))
        {
            return;
        }

        EnqueueAvailableObject(returnPoolType, instance);
    }

    public Quaternion GetDefaultRotation(PoolType poolType)
    {
        if (!configByPoolType.TryGetValue(poolType, out PoolConfig config) || config.prefab == null)
        {
            Debug.LogError($"{nameof(PoolManager)} cannot get a default rotation because {poolType} is not configured.", this);
            return Quaternion.identity;
        }

        return config.prefab.transform.rotation;
    }

    private void InitializePools()
    {
        pools = new Dictionary<PoolType, Queue<GameObject>>();
        availableObjects = new HashSet<GameObject>();
        instanceToPoolType = new Dictionary<GameObject, PoolType>();
        configByPoolType = new Dictionary<PoolType, PoolConfig>();
        parentByPoolType = new Dictionary<PoolType, Transform>();

        if (poolConfigs == null || poolConfigs.Length == 0)
        {
            Debug.LogError($"{nameof(PoolManager)} has no pool configs.", this);
            enabled = false;
            return;
        }

        foreach (PoolConfig config in poolConfigs)
        {
            if (!ValidatePoolConfig(config))
            {
                continue;
            }

            pools.Add(config.poolType, new Queue<GameObject>());
            configByPoolType.Add(config.poolType, config);

            // A parent per pool type makes runtime debugging easier in the Hierarchy.
            Transform poolParent = new GameObject($"{config.poolType} Pool").transform;
            poolParent.SetParent(transform);
            parentByPoolType.Add(config.poolType, poolParent);

            int initialSize = Mathf.Max(0, config.initialSize);
            for (int i = 0; i < initialSize; i++)
            {
                GameObject pooledObject = CreatePooledObject(config.poolType);
                EnqueueAvailableObject(config.poolType, pooledObject);
            }
        }
    }
    
    private GameObject GetAvailableObject(PoolType poolType)
    {
        Queue<GameObject> pool = pools[poolType];
        while (pool.Count > 0)
        {
            GameObject pooledObject = pool.Dequeue();

            if (!availableObjects.Remove(pooledObject))
            {
                Debug.LogError($"{poolType} contained an object that was not marked as available.", this);
                continue;
            }

            if (pooledObject == null)
            {
                Debug.LogError($"{poolType} contained a destroyed object.", this);
                continue;
            }

            if (pooledObject.activeSelf)
            {
                Debug.LogError($"{poolType} contained the active object {pooledObject.name}.", pooledObject);
                continue;
            }

            return pooledObject;
        }

        PoolConfig config = configByPoolType[poolType];
        if (!config.canExpand)
        {
            Debug.LogWarning($"{poolType} pool is exhausted.", this);
            return null;
        }

        return CreatePooledObject(poolType);
    }

    private void EnqueueAvailableObject(PoolType poolType, GameObject instance)
    {
        if (!availableObjects.Add(instance))
        {
            Debug.LogWarning($"{instance.name} was already returned to {poolType}; the duplicate return was ignored.", instance);
            return;
        }

        instance.SetActive(false);
        instance.transform.SetParent(parentByPoolType[poolType]);
        pools[poolType].Enqueue(instance);
    }

    private GameObject CreatePooledObject(PoolType poolType)
    {
        PoolConfig config = configByPoolType[poolType];
        Transform poolParent = parentByPoolType[poolType];
        GameObject pooledObject = Instantiate(config.prefab, poolParent);

        pooledObject.SetActive(false);
        instanceToPoolType.Add(pooledObject, poolType);
        return pooledObject;
    }

    private bool ValidatePoolConfig(PoolConfig config)
    {
        if (config == null)
        {
            Debug.LogError($"{nameof(PoolManager)} has a missing pool config.", this);
            return false;
        }

        if (config.prefab == null)
        {
            Debug.LogError($"{nameof(PoolManager)} has no prefab assigned for {config.poolType}.", this);
            return false;
        }

        if (configByPoolType.ContainsKey(config.poolType))
        {
            Debug.LogError($"{nameof(PoolManager)} has duplicate configs for {config.poolType}.", this);
            return false;
        }

        return true;
    }

    private bool TryGetValidReturnPoolType(PoolType requestedPoolType, GameObject instance, out PoolType returnPoolType)
    {
        returnPoolType = requestedPoolType;

        if (!TryGetValidReturnPoolType(instance, out PoolType registeredPoolType))
        {
            return false;
        }

        if (!pools.ContainsKey(requestedPoolType))
        {
            Debug.LogError($"{nameof(PoolManager)} has no pool configured for {requestedPoolType}.", this);
            instance.SetActive(false);
            return false;
        }

        if (registeredPoolType != requestedPoolType)
        {
            Debug.LogWarning($"{nameof(PoolManager)} tried to return {instance.name} to {requestedPoolType}, but it belongs to {registeredPoolType}.", instance);
            // Return to the registered pool so the object does not get mixed into the wrong queue.
            returnPoolType = registeredPoolType;
        }

        return true;
    }

    private bool TryGetValidReturnPoolType(GameObject instance, out PoolType returnPoolType)
    {
        returnPoolType = default;

        if (instance == null)
        {
            Debug.LogWarning($"{nameof(PoolManager)} cannot return a null object.", this);
            return false;
        }

        if (!instanceToPoolType.TryGetValue(instance, out returnPoolType))
        {
            Debug.LogWarning($"{nameof(PoolManager)} received an object that was not created by a pool.", instance);
            instance.SetActive(false);
            return false;
        }

        if (!pools.ContainsKey(returnPoolType))
        {
            Debug.LogError($"{nameof(PoolManager)} has no pool configured for {returnPoolType}.", this);
            instance.SetActive(false);
            return false;
        }

        return true;
    }
}
