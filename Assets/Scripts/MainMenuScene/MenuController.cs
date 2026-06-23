using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField] private int gameSceneIndex = 1;
    [SerializeField] private Animator menuAnimator;

    void Awake()
    {
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneIndex);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
        #else
            Application.Quit();
        #endif
    }

    public void OpenSettings()
    {
        menuAnimator.SetBool("SettingsOpen", true);
    }

    public void CloseSettings()
    {
        menuAnimator.SetBool("SettingsOpen", false);
    }
}
