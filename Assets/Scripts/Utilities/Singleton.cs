using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour 
    where T : Singleton<T>
{
    public static T Instance { get; private set; }

    // Derived managers use this after base.Awake() to avoid initializing duplicates.
    protected bool IsSingletonInstance => Instance == this;

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"A second {typeof(T).Name} instance was created. The duplicate will be destroyed.", this);
            Destroy(gameObject);
            return;
        }

        Instance = (T)this;
    }

    protected virtual void OnDestroy()
    {
        if (IsSingletonInstance)
        {
            Instance = null;
        }
    }

    // A manager can release an invalid instance without exposing the public setter.
    protected void ReleaseSingletonInstance()
    {
        if (IsSingletonInstance)
        {
            Instance = null;
        }
    }
}

public abstract class SingletonPersistent<T> : Singleton<T>
    where T : SingletonPersistent<T>
{
    protected override void Awake()
    {
        base.Awake();

        if (!IsSingletonInstance)
        {
            return;
        }

        DontDestroyOnLoad(gameObject);
    }
}
