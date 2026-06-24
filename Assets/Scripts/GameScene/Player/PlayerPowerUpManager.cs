using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUpManager : MonoBehaviour
{
    private Dictionary<PowerupEffect, Coroutine> activePowerups = 
        new Dictionary<PowerupEffect, Coroutine>();

    private PlayerHealth playerHealth;

    void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    void OnEnable()
    {
        playerHealth.ShieldConsumed += HandleShieldConsumed;
    }

    public void CollectPowerup(PowerupEffect effect)
    {
        if (activePowerups.ContainsKey(effect)) // Refunds the duration without reapplying
        {
            StopCoroutine(activePowerups[effect]);
            activePowerups[effect] = StartCoroutine(PowerupRoutine(effect));
        }
        else
        {
            effect.Apply(gameObject);
            activePowerups[effect] = StartCoroutine(PowerupRoutine(effect));
        }
    }

    private IEnumerator PowerupRoutine(PowerupEffect effect)
    {
        yield return new WaitForSeconds(effect.Duration);

        effect.Remove(gameObject);
        activePowerups.Remove(effect);
    }

    private void CancelPowerup(PowerupName powerupName)
    {
        PowerupEffect effect = null;

        foreach (PowerupEffect activeEffect in activePowerups.Keys)
        {
            if (activeEffect.PowerUpName == powerupName)
            {
                effect = activeEffect;
                break;
            }
        }

        if (effect == null)
        {
            return;
        }

        StopCoroutine(activePowerups[effect]);
        activePowerups.Remove(effect);
    }

    private void HandleShieldConsumed()
    {
        CancelPowerup(PowerupName.Shield);
    }

    void OnDisable()
    {
        playerHealth.ShieldConsumed -= HandleShieldConsumed;
    }
}
