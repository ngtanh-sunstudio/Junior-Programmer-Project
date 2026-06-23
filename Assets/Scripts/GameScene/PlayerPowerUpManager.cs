using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPowerUpManager : MonoBehaviour
{
    private Dictionary<PowerupEffect, Coroutine> activePowerups = 
        new Dictionary<PowerupEffect, Coroutine>();

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
}
