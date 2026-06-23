using UnityEngine;

public class PowerupItem : MonoBehaviour
{
    [SerializeField] private PowerupEffect effect;

    private void Awake()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider powerupCollider in colliders)
        {
            powerupCollider.isTrigger = true;
        }
    }

    public void Initialize(PowerupEffect powerupEffect)
    {
        effect = powerupEffect;
    }

    void OnTriggerEnter(Collider other)
    {
        if (effect == null)
        {
            return;
        }

        PlayerPowerUpManager powerupManager =
            other.GetComponentInParent<PlayerPowerUpManager>();

        if (powerupManager == null)
        {
            return;
        }

        powerupManager.CollectPowerup(effect);
        Destroy(gameObject);
    }
}
