using UnityEngine;

public class BossAudio : MonoBehaviour
{
    [SerializeField] private AudioClip bossDieSFX;
    [SerializeField] private AudioClip bossFireSFX;
    
    private BossHealth bossHealthScript;
    private BossWeapon bossWeaponScript;

    void Awake()
    {
        bossHealthScript = GetComponent<BossHealth>();
        bossWeaponScript = GetComponent<BossWeapon>();
    }

    private void OnEnable()
    {
        bossHealthScript.Died += PlayDieSFX;
        bossWeaponScript.Fired += PlayFireSFX;
    }

    private void OnDisable()
    {
        bossHealthScript.Died -= PlayDieSFX;
        bossWeaponScript.Fired -= PlayFireSFX;
    }

    private void PlayDieSFX()
    {
        AudioManager.Instance?.PlaySFX(bossDieSFX);
    }

    private void PlayFireSFX()
    {
        AudioManager.Instance?.PlaySFX(bossFireSFX);
    }
}
