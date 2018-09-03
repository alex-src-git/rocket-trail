using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemySpawner : MonoBehaviour
{
    [SerializeField] private GameObject Prefab;
    [SerializeField] private int Capacity;
    private Queue<GameObject> pool;
    private int numKilledEnemies;

    private void Spawn(Vector2 position)
    {
        GameObject prefabClone = pool.Dequeue();
        prefabClone.transform.position = position;
        prefabClone.SetActive(true);

        prefabClone.GetComponent<Enemy>().waypoint = 
            GameplayBounds.instance.RandomPointEnemyBounds();

        pool.Enqueue(prefabClone);
    }

    private void Awake()
    {
        pool = new Queue<GameObject>(Capacity);

        for (int i = 0; i < Capacity; i++)
        {
            GameObject prefabClone = Instantiate(Prefab, transform);
            prefabClone.name = string.Format("Enemy[{0}]", i);
            prefabClone.SetActive(false);
            pool.Enqueue(prefabClone);
        }
        
        Spawn(GameplayBounds.instance.RandomPointEnemySpawnBounds());
        Spawn(GameplayBounds.instance.RandomPointEnemySpawnBounds());
        Spawn(GameplayBounds.instance.RandomPointEnemySpawnBounds());
        Spawn(GameplayBounds.instance.RandomPointEnemySpawnBounds());
        Spawn(GameplayBounds.instance.RandomPointEnemySpawnBounds());
        Spawn(GameplayBounds.instance.RandomPointEnemySpawnBounds());

        Enemy.OnDeath += OnEnemyDied;
    }

    private void OnDestroy()
    {
        Enemy.OnDeath -= OnEnemyDied;
    }

    private void OnEnemyDied()
    {
        if (numKilledEnemies < 1)
        {
            Spawn(GameplayBounds.instance.RandomPointEnemySpawnBounds());
        }
        if (numKilledEnemies > 1 && numKilledEnemies < 6)
        {
            int r = Random.Range(2, 4);
            for (int i = 0; i < r; i ++)
            {
                Spawn(GameplayBounds.instance.RandomPointEnemySpawnBounds());
            }
        }
        else if (numKilledEnemies > 5 && numKilledEnemies < 10)
        {
            int r = Random.Range(1, 5);
            for (int i = 0; i < r; i++)
            {
                Spawn(GameplayBounds.instance.RandomPointEnemySpawnBounds());
            }
        }
        else if (numKilledEnemies > 10)
        {
            int r = Random.Range(1,4);
            for (int i = 0; i < r; i++)
            {
                Spawn(GameplayBounds.instance.RandomPointEnemySpawnBounds());
            }
        }

        numKilledEnemies++;
    }
}