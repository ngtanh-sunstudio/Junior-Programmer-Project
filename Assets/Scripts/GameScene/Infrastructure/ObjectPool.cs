using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject objectToPool;
    [SerializeField] private int numObjectsToPool = 75;

    private List<GameObject> pool = new List<GameObject>();
    
    public Quaternion DefaultRotation =>
      objectToPool != null
          ? objectToPool.transform.rotation
          : Quaternion.identity;

    private void Awake()
    {
        if (objectToPool == null)
        {
            Debug.LogError("No prefabs assigned to pool");
            this.enabled = false;
            return;
        }

        for (int i = 0; i < numObjectsToPool; i++)
        {
            GameObject pooledObject = Instantiate(objectToPool, transform);
            pooledObject.SetActive(false);
            pool.Add(pooledObject);
        }
    }

    public GameObject GetObjectFromPool(Vector3 position, Quaternion rotation)
    {
        foreach (GameObject pooledObject in pool)
        {
            if (!pooledObject.activeSelf)
            {
                pooledObject.transform.SetPositionAndRotation(position, rotation);
                pooledObject.SetActive(true);
                return pooledObject;
            }
        }

        return null;
    }

    public void ReturnObjectToPool(GameObject pooledObject)
    {
        if (pooledObject == null || !pool.Contains(pooledObject))
        {
            Debug.LogWarning("Attempt to return an invalid object to pool");
            return;
        }

        pooledObject.SetActive(false);
        pooledObject.transform.SetParent(transform);
    }
}
