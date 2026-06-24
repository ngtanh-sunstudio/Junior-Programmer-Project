using UnityEngine;

[RequireComponent(typeof(ProjectileController))]
public class ProjectileAudio : MonoBehaviour
{
    [SerializeField] private AudioClip projectileHitSFX;

    private ProjectileController projectileControllerScript;

    void Awake()
    {
        projectileControllerScript = GetComponent<ProjectileController>();
    }

    private void OnEnable()
    {
        projectileControllerScript.Hit += PlayHitSFX;
    }

    private void OnDisable()
    {
        projectileControllerScript.Hit -= PlayHitSFX;
    }

    private void PlayHitSFX()
    {
        AudioManager.Instance?.PlaySFX(projectileHitSFX);
    }
}
