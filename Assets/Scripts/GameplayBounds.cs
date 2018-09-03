using UnityEngine;

public sealed class GameplayBounds : MonoSingleton<GameplayBounds>
{
    [SerializeField] private Bounds playerRoamBounds;
    [SerializeField] private Bounds enemyRoamBounds;
    [SerializeField] private Bounds enemySpawnBounds;
    [SerializeField] private Bounds bulletRoamBounds;
    

    public Vector3 ClosestPointPlayerBounds(Vector3 position)
    {
        return playerRoamBounds.ClosestPoint(position);
    }

    public Vector3 ClosestPointEnemyBounds(Vector3 position)
    {
        return enemyRoamBounds.ClosestPoint(position);
    }

    public Vector3 ClosestPointBulletBounds(Vector3 position)
    {
        return bulletRoamBounds.ClosestPoint(position);
    }

    public Vector2 RandomPointEnemyBounds()
    {
        float x;
        float y;
        x = Random.Range(enemyRoamBounds.min.x, enemyRoamBounds.max.x);
        y = Random.Range(enemyRoamBounds.min.y, enemyRoamBounds.max.y);

        return new Vector2(x, y);
    }

    public Vector2 RandomPointEnemySpawnBounds()
    {
        float x;
        float y;
        x = Random.Range(enemySpawnBounds.min.x, enemySpawnBounds.max.x);
        y = Random.Range(enemySpawnBounds.min.y, enemySpawnBounds.max.y);

        return new Vector2(x, y);
    }

    private void Awake()
    {
      //  DontDestroyOnLoad(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(playerRoamBounds.center, playerRoamBounds.size);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(enemyRoamBounds.center, enemyRoamBounds.size);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bulletRoamBounds.center, bulletRoamBounds.size);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(enemySpawnBounds.center, enemySpawnBounds.size);
    }
}