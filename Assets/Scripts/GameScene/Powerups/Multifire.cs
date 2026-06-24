using UnityEngine;

[CreateAssetMenu(fileName = "Multifire", menuName = "PowerUps/Multifire")]
public class Multifire : PowerupEffect
{
    public override void Apply(GameObject player)
    {
        PlayerWeapon playerWeapon = player.GetComponent<PlayerWeapon>();
        if (playerWeapon != null)
        {
            playerWeapon.SetMultifiring(true);
        }
    }

    public override void Remove(GameObject player)
    {
        PlayerWeapon playerWeapon = player.GetComponent<PlayerWeapon>();
        if (playerWeapon != null)
        {
            playerWeapon.SetMultifiring(false);
        }
    }
}
