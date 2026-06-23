using UnityEngine;

[CreateAssetMenu(fileName = "Multifire", menuName = "PowerUps/Multifire")]
public class Multifire : PowerupEffect
{
    public override void Apply(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetMultifiring(true);
        }
    }

    public override void Remove(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.SetMultifiring(false);
        }
    }
}
