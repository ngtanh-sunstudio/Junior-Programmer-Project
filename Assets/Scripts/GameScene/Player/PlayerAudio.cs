using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [SerializeField] private AudioClip playerDieSFX;
    [SerializeField] private AudioClip playerShieldSFX;
    [SerializeField] private AudioClip playerFireSFX;
    [SerializeField] private AudioClip playerSpeedSFX;
    [SerializeField] private AudioClip playerMultifireSFX;
    
    private PlayerHealth playerHealthScript;
    private PlayerWeapon playerWeaponScript;
    private PlayerMovement playerMovementScript;

    void Awake()
    {
        playerHealthScript = GetComponent<PlayerHealth>();
        playerWeaponScript = GetComponent<PlayerWeapon>();
        playerMovementScript = GetComponent<PlayerMovement>();
    }

    private void OnEnable()
    {
        playerHealthScript.Died += PlayDieSFX;
        playerHealthScript.Shielded += PlayShieldSFX;
        playerMovementScript.SpedUp += PlaySpeedSFX;
        playerWeaponScript.Fired += PlayFireSFX;
        playerWeaponScript.Multifire += PlayMultifireSFX;
    }

    private void OnDisable()
    {
        playerHealthScript.Died -= PlayDieSFX;
        playerHealthScript.Shielded -= PlayShieldSFX;
        playerMovementScript.SpedUp -= PlaySpeedSFX;
        playerWeaponScript.Fired -= PlayFireSFX;
        playerWeaponScript.Multifire -= PlayMultifireSFX;
    }

    private void PlayDieSFX()
    {
        AudioManager.Instance?.PlaySFX(playerDieSFX);
    }

    private void PlayShieldSFX()
    {
        AudioManager.Instance?.PlaySFX(playerShieldSFX);
    }
    
    private void PlaySpeedSFX()
    {
        AudioManager.Instance?.PlaySFX(playerSpeedSFX);
    }

    private void PlayFireSFX()
    {
        AudioManager.Instance?.PlaySFX(playerFireSFX);
    }

    private void PlayMultifireSFX()
    {
        AudioManager.Instance?.PlaySFX(playerMultifireSFX);
    }
}
