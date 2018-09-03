using System.Collections;
using UnityEngine;

// Based on the YouTube tutorial: "How to make a Homing Missile in Unity" by Brackeys.
// https://www.youtube.com/watch?v=0v_H3oOR0aU

[RequireComponent(typeof(Rigidbody2D), typeof(BoxCollider2D), typeof(SpriteRenderer))]
public sealed class HomingProjectile : MonoBehaviour
{

    [SerializeField] private float Speed;
    [SerializeField] private float RotateSpeed;
    private BoxCollider2D bc;
    private Rigidbody2D rb;
    private static Transform playerTransform;

    private void OnEnable()
    {
        bc.enabled = false;
        StartCoroutine(NoDamageTime());
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();

        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<Player>().transform;
        }
    }

    private void OnDestroy()
    {
        if (playerTransform != null)
        {
            playerTransform = null;
        }
    }

    private void FixedUpdate()
    {
        Vector2 direction = playerTransform.position - transform.position;
        direction.Normalize();

        float rotateAmount = Vector3.Cross(direction, transform.up).z;
        rotateAmount += Random.Range(-0.75f, 0.75f);
        rb.angularVelocity = -rotateAmount * RotateSpeed;
        rb.velocity = transform.up * Speed;
    }

    private  IEnumerator NoDamageTime()
    {
        bc.enabled = false;
        float endofNoDamageTime = Time.time + 0.3333f;
        while (Time.time < endofNoDamageTime)
        {
            yield return 0;
        }

        bc.enabled = true;
    }
}