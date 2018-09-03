using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(EdgeCollider2D))]
public sealed class Enemy : MonoBehaviour
{
    [SerializeField] private Sprite SpriteForWhenIdle;
    [SerializeField] private Sprite SpriteForWhenMoving;
    [SerializeField] private Sprite SpriteForDeath1;
    [SerializeField] private Sprite SpriteForDeath2;
    [SerializeField] private float MinWaypointSpacing;
    [SerializeField] private float Speed;
    private static Transform PlayerTransform;
 
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private EdgeCollider2D ec;
    public Vector2 waypoint;
    private float idleTimeEndpoint;
    private int numSequentialWaypointHits;
    private bool hasFired;

    public delegate void DeathAction();
    public static event DeathAction OnDeath;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        ec = GetComponent<EdgeCollider2D>();
    }

    private void OnEnable()
    {
        waypoint = transform.position;
        hasFired = false;
        sr.sprite = SpriteForWhenMoving;

        if (PlayerTransform == null)
        {
            PlayerTransform = FindObjectOfType<Player>().transform;
        }
    }

    private void OnDestroy()
    {
        if (PlayerTransform != null)
        {
            PlayerTransform = null;
        }
    }

    private void FixedUpdate()
    {
        // Position.
        var newPosition = Vector2.MoveTowards(rb.position, waypoint, Speed * Time.deltaTime);
        rb.MovePosition(GameplayBounds.instance.ClosestPointEnemyBounds(newPosition));

        // Waypoints.
        
        if (Vector2.Distance(rb.position, waypoint) < 0.3f && Time.time > idleTimeEndpoint)
        {
            if (!hasFired)
            {
             
                    idleTimeEndpoint = Time.time + 1.0f;
                    sr.sprite = SpriteForWhenIdle;
                    BulletManager.instance.Spawn(transform.position);
                    hasFired = true;
                
            }

            if (++numSequentialWaypointHits > 2)
            {
                numSequentialWaypointHits = 0;
                idleTimeEndpoint = Time.time + 1.0f;
                sr.sprite = SpriteForWhenIdle;
                    BulletManager.instance.Spawn(transform.position);
                hasFired = true;
            }
            else
            {
                if (sr.sprite.GetInstanceID() != SpriteForWhenMoving.GetInstanceID())
                {
                    sr.sprite = SpriteForWhenMoving;
                }

                // Oof 
                var currentPosition = rb.position;
                var newWaypoint = Vector2.zero;
                while (true)
                {
                    newWaypoint = GameplayBounds.instance.RandomPointEnemyBounds();
                    if (Vector2.Distance(currentPosition, newWaypoint) > MinWaypointSpacing)
                    {
                        break;
                    }
                }

                waypoint = newWaypoint;
            }
        }

        // Rotation.
        var dir = PlayerTransform.position - transform.position;
        int angle = (int)(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        angle += 90;
        angle = Mathf.RoundToInt(angle / 45) * 45;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            
            if (ec.enabled)
            {
                ec.enabled = false;
                collision.gameObject.SetActive(false);
                if (OnDeath != null)
                {
                    OnDeath.Invoke();
                }
                StartCoroutine(DeathCoroutine());
            }
        }
    }

    private IEnumerator DeathCoroutine()
    {
        sr.sprite = SpriteForDeath1;

        Time.timeScale = 0.1f;
        float pauseEndTime = Time.realtimeSinceStartup + 0.05f;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        Time.timeScale = 1.0f;

        sr.sprite = SpriteForDeath2;

        Color c = sr.color;
        while (c.a > 0.25f)
        {
            c.a = Mathf.MoveTowards(c.a, 0, Time.deltaTime * 2f);
            sr.color = c;
            yield return 0;
        }

        gameObject.SetActive(false);
    }
}