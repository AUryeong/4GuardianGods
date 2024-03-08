using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SingletonBehavior<T> : SerializedMonoBehaviour where T : SerializedMonoBehaviour
{
    private static T instance;
    protected virtual bool IsDontDestroying => false;
    public static T Instance
    {
        get
        {
            if (instance != null) return instance;

            instance = FindObjectOfType(typeof(T)) as T;
            if (instance != null) return instance;

            var temp = new GameObject(typeof(T).Name);
            instance = temp.AddComponent<T>();
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
            instance = this as T;

        if (!Instance.Equals(this))
        {
            Destroy(gameObject);
            return;
        }

        OnCreated();
        if (!IsDontDestroying) return;

        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnReset();
    }

    protected virtual void OnReset()
    {
    }

    protected virtual void OnCreated()
    {
    }
}