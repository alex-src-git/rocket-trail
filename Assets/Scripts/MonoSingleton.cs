using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly object sLock = new object();
    private static T sInstance;
    
    public static T instance
    {
        get
        {
            lock (sLock)
            {
                if (sInstance == null)
                {
                    sInstance = FindObjectOfType<T>();
                    if (sInstance == null)
                    {
                        GameObject newGameObject = new GameObject(typeof(T).ToString());
                        sInstance = newGameObject.AddComponent<T>();
                        DontDestroyOnLoad(newGameObject);
                    }
                }
            }

            return sInstance;
        }
    }
}