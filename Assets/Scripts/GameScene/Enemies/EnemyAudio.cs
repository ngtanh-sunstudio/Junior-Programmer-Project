using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [SerializeField] private AudioClip enemyDieSFX;

    private EnemyController enemyControllerScript;

    void Awake()
    {
        enemyControllerScript = GetComponent<EnemyController>();        
    }

    void OnEnable()
    {
        enemyControllerScript.Died += PlayDieSFX;
    }

    private void PlayDieSFX()
    {
        AudioManager.Instance?.PlaySFX(enemyDieSFX);
    }

    void OnDisable()
    {
        enemyControllerScript.Died -= PlayDieSFX;
    }
}
