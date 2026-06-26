using UnityEngine;
using UnityEngine.UI;

public class WinOverlay : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (!ValidateSerializedReferences())
        {
            enabled = false;
            return;
        }

        HandleRestartGame();
        HandleQuitGame();
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
            Debug.LogError($"{nameof(WinOverlay)} is missing a game manager reference.", this);
            hasReferences = false;
        }

        if (restartButton == null)
        {
            Debug.LogError($"{nameof(WinOverlay)} is missing a restart button reference.", this);
            hasReferences = false;
        }

        if (quitButton == null)
        {
            Debug.LogError($"{nameof(WinOverlay)} is missing a quit button reference.", this);
            hasReferences = false;
        }

        return hasReferences;
    }
}
