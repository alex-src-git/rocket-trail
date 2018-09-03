using System.Collections;
using System.Collections.Generic;
using UnityEngine;

sealed class EnemySpawner2 : MonoBehaviour
{
    public GameObject m_Prefab;
    const int k_Capacity = 200;
    const int k_MaxEnimiesOnScreenAtOneTime = 15;
    const int k_MinEnimiesOnScreenAtOneTime = 4;
    Queue<GameObject> m_Pool;
    int m_NumberOfEnemiesKilled;
    int m_NumberOfKillsRequiredToInvokeNextWave;
    int m_NumberOfEnemiesOnScreen;
    float m_NextSpawnTime;

    void Spawn()
    {
        GameObject gameObject = m_Pool.Dequeue();
        m_Pool.Enqueue(gameObject);
        gameObject.SetActive(true);

        Vector2 startPosition = GameplayBounds.instance.RandomPointEnemySpawnBounds();
        gameObject.transform.position = startPosition;

        Enemy enemy = gameObject.GetComponent<Enemy>();
        Vector2 waypointPosition = GameplayBounds.instance.RandomPointEnemyBounds();
        enemy.waypoint = waypointPosition;

        m_NumberOfEnemiesOnScreen++;
    }

    void SpawnWave(int waveSize)
    {
        waveSize = Mathf.Clamp(waveSize, 0, k_Capacity);
        for (var i = 0; i < waveSize; i++)
        {
            Spawn();
        }
    }

    void OnEnable()
    {
        Enemy.OnDeath += AccountForEnemyDeath;
    }

    void OnDisable()
    {
        Enemy.OnDeath -= AccountForEnemyDeath;
    }

    void Awake()
    {
        m_Pool = new Queue<GameObject>(k_Capacity);
        for (var i = 0; i < k_Capacity; i++)
        {
            GameObject clone = Instantiate(m_Prefab, transform);
            clone.SetActive(false);
            m_Pool.Enqueue(clone);

            #if DEBUG
            clone.name = string.Format("Enemy[{0}]", i);
            #endif
        }
    }

    void Update()
    {
         
        if (m_NumberOfEnemiesOnScreen == 0 || 
            m_NumberOfEnemiesOnScreen < k_MinEnimiesOnScreenAtOneTime ||
            (m_NumberOfEnemiesOnScreen < k_MaxEnimiesOnScreenAtOneTime &&
            m_NumberOfEnemiesKilled > (m_NumberOfKillsRequiredToInvokeNextWave - 1)
            && Time.time > m_NextSpawnTime))
        {
            m_NextSpawnTime = CalculateNextSpawnTime();
            int waveSize = CalculateNextWaveSize();
            int n = CalculateNumberOfKillsRequiredToInvokeNextWave(waveSize);
            m_NumberOfKillsRequiredToInvokeNextWave = n;
            SpawnWave(waveSize);
        }
    }

    void AccountForEnemyDeath()
    {
        m_NumberOfEnemiesKilled++;
        m_NumberOfEnemiesOnScreen--;

        if (Random.Range(0, 100) == 1)
        {
            Spawn();
        }
    }

    float CalculateNextSpawnTime()
    {
        const int k_MinWaveSpawnInterval = 8;
        const int k_MaxWaveSpawnInterval = 15;

        float t = Time.time;
        if (m_NumberOfEnemiesKilled > 5)
        {
            t += Random.Range(k_MinWaveSpawnInterval, k_MaxWaveSpawnInterval);
        }
        else
        {
            t += k_MaxWaveSpawnInterval;
        }

        return t;
    }

    int CalculateNextWaveSize()
    {
        const int k_MinWaveSize = 2;
        const int k_MaxWaveSize = 6;

        int waveSize = m_NumberOfEnemiesKilled;
        waveSize = Mathf.Clamp(waveSize, k_MinWaveSize, k_MaxWaveSize);
        if (waveSize + m_NumberOfEnemiesOnScreen > k_MaxEnimiesOnScreenAtOneTime)
        {
            waveSize = k_MaxEnimiesOnScreenAtOneTime - m_NumberOfEnemiesOnScreen;
        }

        return waveSize;
    }

    int CalculateNumberOfKillsRequiredToInvokeNextWave(int waveSize)
    {
        return m_NumberOfEnemiesKilled + (waveSize / 2);
    }
}