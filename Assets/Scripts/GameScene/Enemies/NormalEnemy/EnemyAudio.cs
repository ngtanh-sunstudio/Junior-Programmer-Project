using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyAudio : MonoBehaviour
{
    [SerializeField] private AudioClip enemyDieSFX;

    private EnemyController enemyController;

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
    }

    private void OnEnable()
    {
        enemyController.Died += PlayDieSFX;
    }

    private void PlayDieSFX()
    {
        AudioManager.Instance?.PlaySFX(enemyDieSFX);
    }

    private void OnDisable()
    {
        if (enemyController != null)
        {
            enemyController.Died -= PlayDieSFX;
        }
    }
}
