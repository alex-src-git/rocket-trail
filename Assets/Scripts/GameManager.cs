using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public sealed class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text gameOverScreenText;
    [SerializeField] private GameObject startText;

    private static bool firstTimeRunning = true;
    private int score;
    private float nextSampleTime;
    private bool isAlive;

    private void Start()
    {
        if (!firstTimeRunning)
        {
            startText.SetActive(false);
        }

        gameOverScreen.SetActive(false);
        isAlive = true;
        Enemy.OnDeath += OnEnemyDestroyed;
        Player.OnDeath += OnPlayerDestroyed;
    }

    private void OnDestroy()
    {
        Enemy.OnDeath -= OnEnemyDestroyed;
        Player.OnDeath -= OnPlayerDestroyed;
    }

    private void OnEnemyDestroyed()
    {
        score += 100;
    }

    private void OnPlayerDestroyed()
    {
        isAlive = false;
        scoreText.gameObject.SetActive(false);
        gameOverScreen.SetActive(true);
        gameOverScreenText.text = "Score: " + score.ToString();
    }

    private void Update()
    {
        if (!firstTimeRunning)
        {
            if (isAlive)
            {
                if (Time.time > nextSampleTime)
                {
                    score += 1;
                    nextSampleTime = Time.time + 1;

                    scoreText.text = score.ToString();
                }
            }
            else
            {
                if (Time.timeScale != 0.1f)
                {
                    Time.timeScale = 0.1f;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    Time.timeScale = 1.0f;
                    Time.fixedDeltaTime = 0.02f * Time.timeScale;

                    foreach (var mb in FindObjectsOfType<MonoBehaviour>())
                    {
                        mb.StopAllCoroutines();
                    }
                    SceneManager.LoadScene(0);
                }
            }
        }
        else
        {
            if (Time.timeScale != 0.1f)
            {
                Time.timeScale = 0.1f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }

            if (Input.anyKeyDown)
            {
                firstTimeRunning = false;
                startText.SetActive(false);
                Time.timeScale = 1.0f;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }

        }
    }
}