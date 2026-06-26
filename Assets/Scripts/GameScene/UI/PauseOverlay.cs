using UnityEngine;
using UnityEngine.UI;

public class PauseOverlay : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (!ValidateSerializedReferences())
        {
            enabled = false;
            return;
        }

        HandleResumeGame();
        HandleRestartGame();
        HandleQuitGame();
    }

    private void HandleResumeGame()
    {
        resumeButton.onClick.AddListener(gameManager.TogglePause);
    }

    private void HandleRestartGame()
    {
        restartButton.onClick.AddListener(gameManager.RestartGame);
    }

    private void HandleQuitGame()
    {
        quitButton.onClick.AddListener(gameManager.QuitGame);
    }

    private bool ValidateSerializedReferences()
    {
        bool hasReferences = true;

        if (gameManager == null)
        {
            Debug.LogError($"{nameof(PauseOverlay)} is missing a game manager reference.", this);
            hasReferences = false;
        }

        if (resumeButton == null)
        {
            Debug.LogError($"{nameof(PauseOverlay)} is missing a resume button reference.", this);
            hasReferences = false;
        }

        if (restartButton == null)
        {
            Debug.LogError($"{nameof(PauseOverlay)} is missing a restart button reference.", this);
            hasReferences = false;
        }

        if (quitButton == null)
        {
            Debug.LogError($"{nameof(PauseOverlay)} is missing a quit button reference.", this);
            hasReferences = false;
        }

        return hasReferences;
    }
}
