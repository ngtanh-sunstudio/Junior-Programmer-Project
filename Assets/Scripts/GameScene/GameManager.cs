using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private SpawnManager spawnManager;
    [SerializeField] private Animator gameAnimator;
    [SerializeField] private float resumeAnimationDuration = 0.25f;
    private bool isPaused;
    private bool isPauseTransitioning;
    private Coroutine resumeCoroutine;
    public bool IsPaused => isPaused;
    [SerializeField] private int menuSceneIndex = 0;

    [Header("Game Over Screen")]
    [SerializeField] private GameObject gameOverOverlay;
    [SerializeField] private ScoreKeeper scoreKeeper;
    [SerializeField] private TextMeshProUGUI finalScoreText;

    [Header("Win Screen")]
    [SerializeField] private GameObject winOverlay;
    [SerializeField] private TextMeshProUGUI winScoreText;


    public bool IsGameActive { get; private set; } = true;

    private void Start()
    {
        if (gameOverOverlay != null)
        {
            gameOverOverlay.SetActive(false);
        }
        if (gameAnimator != null)
        {
            gameAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;
            gameAnimator.SetBool("IsPaused", false);
        }
        if (winOverlay != null)
        {
            winOverlay.SetActive(false);
        }
    }

    public void GameOver()
    {
        if (!IsGameActive) return;

        IsGameActive = false;

        if (isPaused)
        {
            StopResumeCoroutine();
            isPaused = false;
            isPauseTransitioning = false;
            Time.timeScale = 1f;

            if (gameAnimator != null)
            {
                gameAnimator.SetBool("IsPaused", false);
            }
        }
        
        if (spawnManager != null)
        {
            spawnManager.StopSpawning();
        }

        if (finalScoreText != null && scoreKeeper != null)
        {
            finalScoreText.text = $"Final Score: {scoreKeeper.Score}";
        }

        if (gameOverOverlay != null)
        {
            gameOverOverlay.SetActive(true);
        }
    }

    public void WinGame()
    {
        if (!IsGameActive) return;

        IsGameActive = false;
        Time.timeScale = 0f;

        if (isPaused)
        {
            StopResumeCoroutine();
            isPaused = false;
            isPauseTransitioning = false;

            if (gameAnimator != null)
            {
                gameAnimator.SetBool("IsPaused", false);
            }
        }

        if (spawnManager != null)
        {
            spawnManager.StopSpawning();
        }

        if (winScoreText != null && scoreKeeper != null)
        {
            winScoreText.text = $"Final Score: {scoreKeeper.Score}";
        }

        if (winOverlay != null)
        {
            winOverlay.SetActive(true);
        }
    }

    public void TogglePause()
    {
        if (!IsGameActive) return;
        if (isPauseTransitioning) return;

        if (isPaused)
        {
            resumeCoroutine = StartCoroutine(ResumeAfterAnimation());
            return;
        }

        PauseGame();
    }

    private void PauseGame()
    {
        isPaused = true;

        if (gameAnimator != null)
        {
            gameAnimator.SetBool("IsPaused", true);
        }

        Time.timeScale = 0f;
    }

    private IEnumerator ResumeAfterAnimation()
    {
        isPauseTransitioning = true;

        if (gameAnimator != null)
        {
            gameAnimator.SetBool("IsPaused", false);
        }

        yield return new WaitForSecondsRealtime(resumeAnimationDuration);

        Time.timeScale = 1f;
        isPaused = false;
        isPauseTransitioning = false;
        resumeCoroutine = null;
    }

    private void StopResumeCoroutine()
    {
        if (resumeCoroutine == null) return;

        StopCoroutine(resumeCoroutine);
        resumeCoroutine = null;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneIndex);
    }
}
