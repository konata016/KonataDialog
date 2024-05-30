using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    
    protected abstract bool _isDontDestroy { get; }

    public static T I
    {
        get
        {
            if (instance != null)
            {
                return instance;
            }

            instance = (T)FindObjectOfType(typeof(T));

            if (instance == null)
            {
                Debug.LogError($"SingletonMonoBehaviour エラー");
            }

            return instance;
        }
    }

    private void Awake()
    {
        OnAwake();

        if (!_isDontDestroy)
        {
            return;
        }

        if (this != I)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    protected virtual void OnAwake() { }
}