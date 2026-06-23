using UnityEngine;

public enum PowerupName
{
    None,
    SpeedBoost,
    Shield,
    Multifire
}

public abstract class PowerupEffect : ScriptableObject
{
    [SerializeField] private PowerupName powerUpName;
    [SerializeField] private GameObject powerupPrefab;
    [SerializeField] private float duration;

    public PowerupName PowerUpName => powerUpName; // public readonly access
    public GameObject PowerupPrefab => powerupPrefab;
    public float Duration => duration;

    public abstract void Apply(GameObject player);
    public abstract void Remove(GameObject player);
}
