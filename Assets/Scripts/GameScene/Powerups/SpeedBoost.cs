using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoost", menuName = "PowerUps/Speed Boost")]
public class SpeedBoost : PowerupEffect
{
    [SerializeField] private float baseSpeedMultiplier = 1f;
    [SerializeField] private float speedMultiplier = 2f;

    public override void Apply(GameObject player)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.SetSpeedMultiplier(speedMultiplier);
        }
    }

    public override void Remove(GameObject player)
    {
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.SetSpeedMultiplier(baseSpeedMultiplier);
        }
    }
}
