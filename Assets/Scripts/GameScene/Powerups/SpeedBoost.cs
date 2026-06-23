using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoost", menuName = "PowerUps/Speed Boost")]
public class SpeedBoost : PowerupEffect
{
    [SerializeField] private float baseSpeedMultiplier = 1f;
    [SerializeField] private float speedMultiplier = 2f;

    public override void Apply(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetSpeedMultiplier(speedMultiplier);
        }
    }

    public override void Remove(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetSpeedMultiplier(baseSpeedMultiplier);
        }
    }
}
