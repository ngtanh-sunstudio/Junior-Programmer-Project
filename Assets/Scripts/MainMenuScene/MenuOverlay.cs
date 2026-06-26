using UnityEngine;
using UnityEngine.UI;

public class MenuOverlay : MonoBehaviour
{
    [SerializeField] private MenuController menuControllerScript;
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    void Awake()
    {
        if (!ValidateSerializedReferences())
        {
            enabled = false;
            return;
        }

        HandleStartGame();
        HandleOpenSettings();
        HandleQuitGame();
    }

    private void HandleStartGame()
    {
        playButton.onClick.AddListener(menuControllerScript.StartGame);
    }

    private void HandleOpenSettings()
    {
        settingsButton.onClick.AddListener(menuControllerScript.OpenSettings);
    }

    private void HandleQuitGame()
    {
        quitButton.onClick.AddListener(menuControllerScript.QuitGame);
    }

    private bool ValidateSerializedReferences()
    {
        bool hasReferences = true;

        if (menuControllerScript == null)
        {
            Debug.LogError($"{nameof(MenuOverlay)} is missing a menu controller reference.", this);
            hasReferences = false;
        }

        if (playButton == null)
        {
            Debug.LogError($"{nameof(MenuOverlay)} is missing a play button reference.", this);
            hasReferences = false;
        }

        if (settingsButton == null)
        {
            Debug.LogError($"{nameof(MenuOverlay)} is missing a settings button reference.", this);
            hasReferences = false;
        }

        if (quitButton == null)
        {
            Debug.LogError($"{nameof(MenuOverlay)} is missing a quit button reference.", this);
            hasReferences = false;
        }

        return hasReferences;
    }
}
