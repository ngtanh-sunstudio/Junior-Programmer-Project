using UnityEngine;

[CreateAssetMenu(fileName = "Shield", menuName = "PowerUps/Shield")]
public class Shield : PowerupEffect
{
    public override void Apply(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SetShielded(true);
        }
    }

    public override void Remove(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SetShielded(false);
        }
    }
}
