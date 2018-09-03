using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(EdgeCollider2D))]
public sealed class Player : MonoBehaviour
{
    [SerializeField] private Sprite SpriteForWhenIdle;
    [SerializeField] private Sprite SpriteForWhenMoving;
    [SerializeField] private Sprite SpriteForDeath1;
    [SerializeField] private Sprite SpriteForDeath2;
    [SerializeField] private float Speed;
    [SerializeField] private float SmoothTime;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 directionalInput;
    private Vector2 directionalInputPrior;
    private Vector2 velocity;
    private Vector2 velocitySmoothing;
    private Vector3 eulerRotation;

    public delegate void DeathAction();
    public static event DeathAction OnDeath;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Screen.SetResolution(360, 360, true);
        }

        directionalInputPrior = directionalInput;
        directionalInput = Vector2.zero;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            directionalInput.x -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            directionalInput.x += 1.0f;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            directionalInput.y += 1.0f;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            directionalInput.y -= 1.0f;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            directionalInput *= 0.333333333f;
        }
   
        if (directionalInput != directionalInputPrior)
        {
            sr.sprite = directionalInput != Vector2.zero 
                ? SpriteForWhenMoving : SpriteForWhenIdle;
        }
    }

    private void FixedUpdate()
    {
        velocity = Vector2.SmoothDamp(
            velocity, 
            directionalInput * (Speed * Time.deltaTime), 
            ref velocitySmoothing, 
            SmoothTime
        );

        rb.MovePosition(
            GameplayBounds.instance.ClosestPointPlayerBounds(
                rb.position + velocity
            )
        );

        HandleRotation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            collision.gameObject.SetActive(false);
         
            StartCoroutine(DeathCoroutine());
        }
    }

    private IEnumerator DeathCoroutine()
    {
        sr.sprite = SpriteForDeath1;

        Time.timeScale = 0.1f;
        float pauseEndTime = Time.realtimeSinceStartup + 0.5f;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }

        Time.timeScale = 1.0f;

        sr.sprite = SpriteForDeath2;

        Color c = sr.color;
        while (c.a > 0.25f)
        {
            c.a = Mathf.MoveTowards(c.a, 0, Time.deltaTime * 8.0f);
            sr.color = c;
            yield return 0;
        }
        
        gameObject.SetActive(false);

        if (OnDeath != null)
        {
            OnDeath.Invoke();
        }
    }

    private void HandleRotation()
    {
        // Uh oh....

        if (directionalInput.x < 0.0f && directionalInput.y == 0.0f)
        {
            // left
            eulerRotation.z = 90.0f;
        }
        else if (directionalInput.x < 0.0f && directionalInput.y < 0.0f)
        {
            // left down
            eulerRotation.z = 135.0f;
        }
        else if (directionalInput.x == 0.0f && directionalInput.y < 0.0f)
        {
            // down
            eulerRotation.z = 180.0f;
        }
        else if (directionalInput.x > 0.0f && directionalInput.y < 0.0f)
        {
            // right down
            eulerRotation.z = -135.0f;
        }
        else if (directionalInput.x > 0.0f && directionalInput.y == 0.0f)
        {
            // right
            eulerRotation.z = -90.0f;
        }
        else if (directionalInput.x > 0.0f && directionalInput.y > 0.0f)
        {
            // right up
            eulerRotation.z = -45.0f;
        }
        else if (directionalInput.x == 0.0f && directionalInput.y > 0.0f)
        {
            // up
            eulerRotation.z = 0.0f;
        }
        else if (directionalInput.x < 0.0f && directionalInput.y > 0.0f)
        {
            // left up
            eulerRotation.z = 45.0f;
        }

        transform.localEulerAngles = eulerRotation;
    }
}