using System.Collections.Generic;
using UnityEngine;

public sealed class BulletManager : MonoSingleton<BulletManager>
{
    [SerializeField] private GameObject Prefab;
    [SerializeField] private int Capacity;
    private Queue<GameObject> pool;

    public void Spawn(Vector2 position)
    {
        GameObject prefabClone = pool.Dequeue();
        prefabClone.transform.position = position;
        prefabClone.SetActive(true);
        pool.Enqueue(prefabClone);
    }
    
    private void Awake()
    {
      //  DontDestroyOnLoad(gameObject);

        pool = new Queue<GameObject>(Capacity);

        for (int i = 0; i < Capacity; i++)
        {
            GameObject prefabClone = Instantiate(Prefab, transform);
            prefabClone.name = string.Format("Bullet[{0}]", i);
            prefabClone.SetActive(false);
            pool.Enqueue(prefabClone);
        }
    }    
}